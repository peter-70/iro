using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;
using IroGen;

namespace IroGenTests;

public class GeneratorTests
{
    private static T Sta<T>(Func<T> action)
    {
        T? result = default;
        Exception? failure = null;
        var thread = new Thread(() => { try { result = action(); } catch (Exception e) { failure = e; } });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (failure != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
        return result!;
    }
    private static byte[] Pixels(BitmapSource bitmap)
    {
        var pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
        bitmap.CopyPixels(pixels, bitmap.PixelWidth * 4, 0); return pixels;
    }
    public static IEnumerable<object[]> SharmaPairs() => File.ReadLines(Path.Combine(AppContext.BaseDirectory, "ciede2000testdata.txt"))
        .Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Select(v => (object)double.Parse(v, CultureInfo.InvariantCulture)).ToArray());

    [Theory]
    [MemberData(nameof(SharmaPairs))]
    public void PublishedCiede2000Pairs(double l1, double a1, double b1, double l2, double a2, double b2, double expected)
    {
        var first = new Lab(l1, a1, b1); var second = new Lab(l2, a2, b2);
        Assert.InRange(Math.Abs(ColorScience.DeltaE00(first, second) - expected), 0, .0001);
        Assert.InRange(Math.Abs(ColorScience.DeltaE00(second, first) - expected), 0, .0001);
        Assert.Equal(0, ColorScience.DeltaE00(first, first));
    }

    [Theory]
    [InlineData(255,255,255,100,0,0)]
    [InlineData(0,0,0,0,0,0)]
    [InlineData(255,0,0,53.2408,80.0925,67.2032)]
    [InlineData(0,255,0,87.7347,-86.1827,83.1793)]
    [InlineData(0,0,255,32.2970,79.1875,-107.8602)]
    public void SrgbD65ReferenceColors(byte r, byte g, byte b, double l, double a, double labB)
    {
        var actual = ColorScience.ToLab(new(r,g,b));
        Assert.InRange(Math.Abs(actual.L-l),0,.001); Assert.InRange(Math.Abs(actual.A-a),0,.001); Assert.InRange(Math.Abs(actual.B-labB),0,.001);
    }

    [Fact]
    public void SameSeedReproducesPixelsAndDifferentSeedChangesThem() => Sta(() =>
    {
        var options = new GeneratorOptions { Width = 640, Height = 480, Dirt = Severity.Light, Noise = Severity.Light };
        var first = SceneGenerator.Generate(options);
        Assert.Equal(Pixels(first.Image), Pixels(SceneGenerator.Generate(options).Image));
        Assert.NotEqual(first.Wall, SceneGenerator.Generate(options with { Seed = 6789 }).Wall);
        return true;
    });

    [Theory]
    [InlineData(StripOrientation.Vertical)]
    [InlineData(StripOrientation.Horizontal)]
    public void RenderedFieldInteriorsAndWallMatchExactSourceBytes(StripOrientation orientation) => Sta(() =>
    {
        var scene = SceneGenerator.Generate(new() { Width = 800, Height = 800, Orientation = orientation, Labels = true });
        var pixels = Pixels(scene.Image);
        void AssertPixel(int x, int y, Rgb expected)
        {
            int i = (y * 800 + x) * 4;
            Assert.Equal(expected.B, pixels[i]); Assert.Equal(expected.G, pixels[i+1]); Assert.Equal(expected.R, pixels[i+2]);
        }
        AssertPixel(5,5,scene.Wall);
        foreach (var field in scene.Fields)
        {
            int x = (int)field.Polygon.Average(p=>p.X), y = (int)field.Polygon.Average(p=>p.Y);
            AssertPixel(x,y,field.Color);
        }
        Assert.Equal(0, scene.Fields[2].DeltaE00);
        return true;
    });

    [Theory]
    [InlineData(WallDifference.Light,1)]
    [InlineData(WallDifference.Medium,4)]
    [InlineData(WallDifference.Strong,12)]
    public void WallDifferenceTracksNominalTarget(WallDifference mode, double target) => Sta(() =>
    {
        foreach (int seed in new[] { 0, 12, 54321 })
        {
            var scene = SceneGenerator.Generate(new() { Width=320,Height=320,Seed=seed,WallDifference=mode });
            Assert.InRange(scene.Fields[2].DeltaE00,target-.6,target+.6);
        }
        return true;
    });

    public static IEnumerable<object[]> Effects() => typeof(GeneratorOptions).GetProperties().Where(p=>p.PropertyType==typeof(Severity)).Select(p=>new object[]{p.Name});
    [Theory]
    [MemberData(nameof(Effects))]
    public void EveryEffectChangesPixelsWithoutChangingNominalTargets(string effect) => Sta(() =>
    {
        var options = new GeneratorOptions { Width=480,Height=480 };
        var baseline = SceneGenerator.Generate(options);
        foreach (var strength in new[]{Severity.Light,Severity.Medium,Severity.Strong})
        {
            var changed = options with { };
            typeof(GeneratorOptions).GetProperty(effect)!.SetValue(changed,strength);
            var scene = SceneGenerator.Generate(changed);
            Assert.False(Pixels(baseline.Image).SequenceEqual(Pixels(scene.Image)));
            Assert.Equal(baseline.Fields.Select(f=>f.DeltaE00),scene.Fields.Select(f=>f.DeltaE00));
            Assert.All(Pixels(scene.Image).Where((_,i)=>i%4==3),alpha=>Assert.Equal(255,alpha));
        }
        return true;
    });

    [Fact]
    public void ExportRoundTripsImageOptionsAndDoesNotInventAppExpectations() => Sta(() =>
    {
        string folder = Path.Combine(Path.GetTempPath(),"irogen-test-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            var scene = SceneGenerator.Generate(new() { Width=480,Height=320,Blur=Severity.Strong,Position=StripPosition.Center });
            string output = SceneExport.Save(scene,folder);
            using var json = JsonDocument.Parse(File.ReadAllText(Directory.GetFiles(output,"*.json").Single()));
            var root = json.RootElement;
            Assert.Equal(JsonValueKind.Null, root.GetProperty("expected").GetProperty("measurementAllowed").ValueKind);
            var options = root.GetProperty("conditions").GetProperty("generator").GetProperty("parameters").GetProperty("options").Deserialize<GeneratorOptions>(SceneExport.JsonOptions);
            Assert.Equal(scene.Options,options);
            using var stream = File.OpenRead(Path.Combine(output,root.GetProperty("imageFile").GetString()!));
            var decoded = BitmapDecoder.Create(stream,BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad).Frames[0];
            Assert.Equal(Pixels(scene.Image),Pixels(decoded));
            Assert.Equal(Pixels(scene.Image),Pixels(SceneGenerator.Generate(options!).Image));
            Assert.NotEqual(output,SceneExport.Save(scene,folder));
        }
        finally { Directory.Delete(folder,true); }
        return true;
    });

    [Fact]
    public void GeometrySupportsAllPositionsAndDirectionsAndClipping() => Sta(() =>
    {
        foreach (var orientation in Enum.GetValues<StripOrientation>())
        foreach (var position in Enum.GetValues<StripPosition>())
        foreach (var distance in Enum.GetValues<CameraDistance>())
        {
            var scene=SceneGenerator.Generate(new(){Width=320,Height=400,Orientation=orientation,Position=position,Distance=distance,Perspective=Severity.Medium,RotationDegrees=12,VariableFieldHeights=true});
            Assert.Equal(7,scene.Fields.Count);
            foreach(var field in scene.Fields.Where(f=>f.Bounds!=null))
            {
                var b=field.Bounds!;
                Assert.InRange(b.X,0,319); Assert.InRange(b.Y,0,399);
                Assert.InRange(b.X+b.Width,1,320); Assert.InRange(b.Y+b.Height,1,400);
            }
        }
        return true;
    });

    [Fact]
    public void InvalidDimensionsAndMissingReferenceFieldAreRejected()
    {
        Assert.Throws<ArgumentException>(() => (new GeneratorOptions { Width = int.MaxValue }).Validate());
        Assert.Throws<ArgumentException>(() => (new GeneratorOptions { FieldCount = 2, MatchingField = 3 }).Validate());
        Assert.Throws<ArgumentException>(() => (new GeneratorOptions { RotationDegrees = double.NaN }).Validate());
        Assert.Throws<ArgumentException>(() => (new GeneratorOptions { Glare = (Severity)99 }).Validate());
    }
}
