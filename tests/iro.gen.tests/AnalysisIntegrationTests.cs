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


