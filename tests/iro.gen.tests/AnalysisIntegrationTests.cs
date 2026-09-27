using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;
using Iro.Analysis;
using Iro.Core.Analysis;
using IroGen;

namespace IroGenTests;

public class AnalysisIntegrationTests
{
    private static T Sta<T>(Func<T> action)
    {
        T? value=default;Exception? failure=null;
        var thread=new Thread(()=>{try{value=action();}catch(Exception e){failure=e;}});
        thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();
        if(failure!=null)System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
        return value!;
    }
    private static byte[] Encode(GeneratedScene scene) => Sta(()=>
    {
        var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(scene.Image));
        using var stream=new MemoryStream();encoder.Save(stream);return stream.ToArray();
    });
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task CropQualityAndPartialScopeSurvivePng(bool horizontal, bool clean)
    {
        const int w = 800, h = 600;
        var pixels = Enumerable.Repeat((byte)140, w * h * 3).ToArray();
        for (int field = 0; field < 3; field++)
        {
            int top = field == 0 ? 0 : (clean ? 78 : 34) + (field - 1) * 128;
            for (int y = top; y < top + (field == 0 ? (clean ? 60 : 16) : 110); y++)
            for (int x = 540; x < 700; x++)
            for (int c = 0; c < 3; c++)
                pixels[(y * w + x) * 3 + c] = (byte)(60 + field * 30 + c * 15 + (field == 0 && !clean ? (x - 540) * 35 / 159 : 0));
        }
        int width = w, height = h;
        if (horizontal)
        {
            var turned = new byte[pixels.Length];
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                Array.Copy(pixels, (y * w + x) * 3, turned, (x * h + h - 1 - y) * 3, 3);
            pixels = turned; width = h; height = w;
        }
        var png = Sta(() => {
            var bitmap = BitmapSource.Create(width, height, 96, 96, System.Windows.Media.PixelFormats.Rgb24, null, pixels, width * 3);
            var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using var stream = new MemoryStream(); encoder.Save(stream); return stream.ToArray();
        });
        var result = await new PngAnalysisApi().AnalyzeAsync(png, new());
        var display = new AnalysisPresentation(); display.Present(result);
        if (clean)
        {
            Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
            Assert.Equal(3, result.Fields.Count(f => f.MeasurementAllowed));
            Assert.Contains("kein vollständiger Streifenvergleich", result.Hint);
            Assert.Equal("Teilweise auswertbar", display.Heading);
            Assert.All(display.Fields, field => Assert.NotEqual("–", field.Value));
        }
        else
        {
            Assert.Equal(AnalysisStatus.UnsuitableGeometry, result.Status);
            Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
            Assert.All(display.Fields, field => Assert.Equal("–", field.Value));
            Assert.Contains("sichere Auswertung", display.Message);
        }
        string root = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(root, "iro.slnx"))) root = Directory.GetParent(root)!.FullName;
        string folder = Path.Combine(root, "tests", "adjustments", "endbeschnitt-20260924");
        Directory.CreateDirectory(folder);
        string name = (clean ? "geeignet-" : "gestoert-") + (horizontal ? "waagerecht" : "senkrecht");
        File.WriteAllBytes(Path.Combine(folder, name + ".png"), png);
        File.WriteAllText(Path.Combine(folder, name + ".md"), $"# PNG-Gegenprobe {name}\n\nAnalyse {result.AnalyzerVersion}: {result.Status}. Freigegebene sichtbare Felder: {result.Fields.Count(f => f.MeasurementAllowed)}.\n\n{result.Hint}\n");
    }
    [Theory]
    [InlineData(StripOrientation.Vertical,StripPosition.Right)]
    [InlineData(StripOrientation.Vertical,StripPosition.Left)]
    [InlineData(StripOrientation.Vertical,StripPosition.Center)]
    [InlineData(StripOrientation.Horizontal,StripPosition.Top)]
    [InlineData(StripOrientation.Horizontal,StripPosition.Bottom)]
    public async Task UnstressedImagesAreDetectedFromPixelsAndMatchNominalValues(StripOrientation orientation,StripPosition position)
    {
        var scene=Sta(()=>SceneGenerator.Generate(new(){Orientation=orientation,Position=position}));
        var actual=await new PngAnalysisApi().AnalyzeAsync(Encode(scene),new());
        Assert.True(actual.Fields.Count==7,$"{actual.Status}: {actual.Hint}; fields={actual.Fields.Count}");
        Assert.Equal(AnalysisStatus.Measured,actual.Status);
        for(int i=0;i<7;i++)Assert.InRange(Math.Abs(actual.Fields[i].DeltaE00!.Value-scene.Fields[i].DeltaE00),0,.15);
        Assert.True(actual.Fields[2].IsNearest);
        Assert.All(actual.Fields,f=>Assert.Equal(actual.Fields[0].Reference!.Bounds,f.Reference!.Bounds));
    }
    [Theory]
    [InlineData(CameraDistance.TooFar)]
    [InlineData(CameraDistance.TooClose)]
    public async Task UnsuitableDistancesDoNotProduceAllNominalMeasurements(CameraDistance distance)
    {
        var scene=Sta(()=>SceneGenerator.Generate(new(){Distance=distance}));
        var actual=await new PngAnalysisApi().AnalyzeAsync(Encode(scene),new());
        Assert.True(actual.Status != AnalysisStatus.Measured, JsonSerializer.Serialize(actual, AnalysisRunner.JsonOptions));
    }
    [Fact]
    public async Task StrongBlurDoesNotBecomeAValidFullStrip()
    {
        var scene=Sta(()=>SceneGenerator.Generate(new(){Blur=Severity.Strong}));
        var actual=await new PngAnalysisApi().AnalyzeAsync(Encode(scene),new());
        Assert.True(actual.Status != AnalysisStatus.Measured, JsonSerializer.Serialize(actual, AnalysisRunner.JsonOptions));
    }
    [Fact]
    public async Task PreparedRequestRunsThroughActualApiAndDoesNotOverwriteInputs()
    {
        string root=Path.Combine(Path.GetTempPath(),"iro-analysis-test-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
        try
        {
            File.WriteAllText(Path.Combine(root,"iro.slnx"),"<Solution/>");File.WriteAllText(Path.Combine(root,"IRO-KONSOLIDIERTER-PLAN.md"),"Test");
            using var batch=Sta(()=>BatchGenerator.Generate(new(),2,false,root));
            var request=IroTestHandoff.Submit(batch,root);
            string requestText=File.ReadAllText(Path.Combine(request.Folder,"request.json"));
            var run=await new AnalysisRunner().RunAsync(request.Folder,Path.Combine(root,"runs"),new());
            Assert.Equal(2,run.Report.ProcessedCount);Assert.Equal(2,run.Report.ImagesWithMeasurements);
            Assert.All(run.Report.Captures,c=>{Assert.Null(c.Error);Assert.Equal(7,c.Comparisons.Count);Assert.All(c.Comparisons,p=>Assert.Equal("measured",p.Status));});
            Assert.Equal(requestText,File.ReadAllText(Path.Combine(request.Folder,"request.json")));
            Assert.True(File.Exists(run.ResultFile));
        }
        finally{Directory.Delete(root,true);}
    }
    [Fact]
    public void InputPathsCannotEscapeTestPackage()
    {
        Assert.Throws<ArgumentException>(()=>AnalysisRunner.SafePath("D:\\test","../secret.json"));
        Assert.Throws<ArgumentException>(()=>AnalysisRunner.SafePath("D:\\test","C:\\secret.json"));
        Assert.Throws<ArgumentException>(()=>AnalysisRunner.SafePath("D:\\test","image.png:stream"));
    }
}


