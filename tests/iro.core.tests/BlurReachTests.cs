using System.IO.Compression;
using System.Reflection;
using Iro.Core.Analysis;
using Xunit;

namespace Iro.Core.Tests;

// Bounded regressions from the 84-run Gaussian-input diagnosis (2026-10-01).
// Fixtures are finished RGB inputs, not analyzer parameters. Scene truth is used
// only below, after analysis, to audit physical identity and selected original pixels.
// These cases do not establish general optical accuracy or new quality thresholds.
public sealed class BlurReachTests
{
    [Theory]
    [InlineData(false, false)] [InlineData(true, false)]
    [InlineData(false, true)] [InlineData(true, true)]
    public void IsolatedEdgeDoesNotInvalidateAnyUnchangedComparison(bool horizontal, bool local)
    {
        var options = Options(local);
        var baseline = Analyze(Load(horizontal, "straight-baseline-0"), options);
        var image = Load(horizontal, "straight-one-4");
        var result = Analyze(image, options);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Contains(result.Diagnostics, d => d.Contains("Breiter Farbübergang"));
        Assert.Equal(3, result.Fields.Count);
        for (int i = 0; i < 3; i++)
        {
            Assert.Equal(baseline.Fields[i].Bounds, result.Fields[i].Bounds);
            Assert.Equal(baseline.Fields[i].Measurement, result.Fields[i].Measurement);
            Assert.Equal(baseline.Fields[i].Reference, result.Fields[i].Reference);
            Assert.Equal(baseline.Fields[i].DeltaE00, result.Fields[i].DeltaE00);
            Assert.Equal(baseline.Fields[i].IsNearest, result.Fields[i].IsNearest);
        }
        Audit(image, result, "straight", horizontal);
    }

    [Theory]
    [InlineData(false, false, "rotated", 4)] [InlineData(true, false, "rotated", 4)]
    [InlineData(false, true, "rotated", 4)] [InlineData(true, true, "rotated", 4)]
    [InlineData(false, false, "small", 6)] [InlineData(true, false, "small", 6)]
    [InlineData(false, true, "small", 6)] [InlineData(true, true, "small", 6)]
    public void CrossFieldEffectsAreCheckedThroughActualGeometryAndPixels(bool horizontal, bool local, string scene, int sigma)
    {
        var options = Options(local);
        var baselineImage = Load(horizontal, scene + "-baseline-0");
        var baseline = Analyze(baselineImage, options);
        var image = Load(horizontal, $"{scene}-multi-{sigma}");
        var result = Analyze(image, options);
        Assert.NotEqual(baseline.StraighteningDegrees, result.StraighteningDegrees);
        Assert.Equal(scene == "small" ? AnalysisStatus.PartiallyMeasured : AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        if (scene == "small")
        {
            Assert.False(result.Fields[0].MeasurementAllowed);
            Assert.Null(result.Fields[0].DeltaE00);
            Assert.True(result.Fields[0].SurfaceSpatialDeltaE > options.MaximumSpatialDeltaE);
        }
        var before = Audit(baselineImage, baseline, scene, horizontal);
        var after = Audit(image, result, scene, horizontal);
        // Same physical neighbors, but their original pixel sets really changed.
        foreach (int id in new[] { 2, 3 })
        {
            Assert.False(before[id].SetEquals(after[id]));
            Assert.True(result.Fields[id-1].MeasurementAllowed);
        }
        Assert.Equal(baseline.Fields.Where(f => f.IsNearest).Select(f => f.FieldId),
            result.Fields.Where(f => f.IsNearest).Select(f => f.FieldId));
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void GlobalBlurWithBadSharedReferenceStillHasNoDependentComparisons(bool horizontal)
    {
        var result = Analyze(Load(horizontal, "straight-global-12"), new());
        Assert.Equal(AnalysisStatus.InvalidReference, result.Status);
        Assert.NotEmpty(result.Fields);
        Assert.All(result.Fields, f =>
        {
            Assert.NotNull(f.Reference);
            Assert.False(f.Reference!.IsUsable);
            Assert.True(f.Reference.SpatialDeltaE > result.Options.MaximumSpatialDeltaE);
            Assert.False(f.MeasurementAllowed); Assert.Null(f.DeltaE00); Assert.False(f.IsNearest);
        });
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void InvalidSharedReferenceAlsoBlocksOtherwiseIsolatedEdgeCase(bool horizontal)
    {
        var original = Load(horizontal, "straight-one-4");
        var first = Analyze(original, new());
        var reference = first.Fields[0].Reference!.Bounds;
        var pixels = original.Pixels.ToArray();
        for (int y = reference.Y; y < reference.Bottom; y++)
        for (int x = reference.X; x < reference.Right; x++)
        {
            int offset = (y*original.Width+x)*3;
            int shift = x < reference.X+reference.Width/2 ? -6 : 6;
            for (int c = 0; c < 3; c++) pixels[offset+c] = (byte)(pixels[offset+c]+shift);
        }
        var result = Analyze(new(original.Width, original.Height, original.Stride, pixels), new());
        Assert.Equal(AnalysisStatus.InvalidReference, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.All(result.Fields, f => { Assert.False(f.Reference!.IsUsable); Assert.Null(f.DeltaE00); Assert.False(f.IsNearest); });
    }

    private static AnalysisOptions Options(bool local) => new() { ReferenceMode = local ? ReferenceMode.AdjacentPerFieldTrial : ReferenceMode.SharedAutomaticTrial };
    private static ImageAnalysis Analyze(RgbFrame image, AnalysisOptions options)
    {
        var before = image.Pixels.ToArray();
        var result = new ImageAnalyzer().Analyze(image, options);
        Assert.Equal(before, image.Pixels.ToArray());
        return result;
    }
    private static RgbFrame Load(bool horizontal, string name)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Blur", $"{(horizontal ? "H" : "V")}-{name}.rgb.gz");
        using var input = File.OpenRead(path); using var zip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream(); zip.CopyTo(output);
        int width = horizontal ? 960 : 1440, height = horizontal ? 1440 : 960;
        Assert.Equal(width*height*3, output.Length);
        return new(width,height,width*3,output.ToArray());
    }

    private static Dictionary<int,HashSet<int>> Audit(RgbFrame source, ImageAnalysis result, string scene, bool horizontal)
    {
        var options = result.Options;
        ImageRotation? rotation = result.StraighteningDegrees == 0 ? null : new(source.Width, source.Height, -result.StraighteningDegrees);
        var view = rotation == null ? source : new RgbFrame(source, rotation);
        var detection = StripDetector.Detect(view, options, default);
        var exclusions = detection.Fields.Concat(detection.BorderRegions ?? []).Concat(detection.WeakBorderRegions).ToArray();
        var combined = new PixelRect(detection.Fields.Min(b=>b.X), detection.Fields.Min(b=>b.Y),
            detection.Fields.Max(b=>b.Right)-detection.Fields.Min(b=>b.X), detection.Fields.Max(b=>b.Bottom)-detection.Fields.Min(b=>b.Y));
        var find = typeof(ImageAnalyzer).GetMethod("FindReference", BindingFlags.NonPublic|BindingFlags.Static)!;
        var ids = new Dictionary<int,HashSet<int>>();
        foreach (var field in result.Fields.Where(f=>f.MeasurementAllowed))
        {
            int id = Physical(field.Bounds.X + field.Bounds.Width/2d, field.Bounds.Y + field.Bounds.Height/2d, scene, horizontal);
            Assert.InRange(id,1,3);
            var bounds = Assert.Single(detection.Fields, b => (rotation?.SourceBounds(b) ?? b) == field.Bounds);
            detection.SurfacePolygons.TryGetValue(bounds, out var contour);
            var target = options.ReferenceMode == ReferenceMode.SharedAutomaticTrial ? combined : bounds;
            var reference = (PixelRect)find.Invoke(null,[view,target,exclusions,detection.Orientation])!;
            Assert.Equal(rotation?.SourceBounds(reference) ?? reference,field.Reference!.Bounds);
            var innerPixels = Pixels(bounds.Inset(options.InnerMargin),contour);
            var refPixels = Pixels(reference,null);
            Assert.All(innerPixels, p=>Assert.Equal(id,Physical(p.X+.5,p.Y+.5,scene,horizontal)));
            Assert.All(refPixels, p=>Assert.Equal(0,Physical(p.X+.5,p.Y+.5,scene,horizontal)));
            Assert.Equal(0,field.Measurement.RejectedFraction);
            Assert.Equal(0,field.Reference.RejectedFraction);
            Assert.Equal(field.Measurement.SampleCount,innerPixels.Count);
            Assert.Equal(field.Reference.SampleCount,refPixels.Count);
            var lab = Mean(innerPixels); var wall = Mean(refPixels);
            AssertLab(lab,field.Measurement.Lab!.Value); AssertLab(wall,field.Reference.Lab!.Value);
            Assert.Equal(ColorMath.DeltaE00(lab,wall),field.DeltaE00!.Value,9);
            ids.Add(id,innerPixels.Select(p=>p.Y*source.Width+p.X).ToHashSet());
        }
        return ids;

        List<(int X,int Y,RgbColor Color)> Pixels(PixelRect bounds, IReadOnlyList<PixelPoint>? mask)
        {
            int step = Math.Max(1,(int)Math.Ceiling(Math.Sqrt(bounds.Area/40000d)));
            var selected = new List<(int,int,RgbColor)>();
            foreach(var sample in view.Sample(bounds,step,default))
            {
                if(mask != null && !ContourMask.Contains(mask,sample.X+(rotation==null?.5:0),sample.Y+(rotation==null?.5:0))) continue;
                var p = rotation?.ToSource(sample.X,sample.Y) ?? new PixelPoint(sample.X,sample.Y);
                int x=(int)Math.Floor(p.X),y=(int)Math.Floor(p.Y);
                Assert.Equal(source.GetPixel(x,y),sample.Pixel);
                selected.Add((x,y,sample.Pixel));
            }
            return selected;
        }
    }
    private static LabColor Mean(List<(int X,int Y,RgbColor Color)> pixels) => ColorMath.LinearToLab(
        pixels.Average(p=>ColorMath.Decode(p.Color.R)),pixels.Average(p=>ColorMath.Decode(p.Color.G)),pixels.Average(p=>ColorMath.Decode(p.Color.B)));
    private static void AssertLab(LabColor expected, LabColor actual)
    { Assert.Equal(expected.L,actual.L,9); Assert.Equal(expected.A,actual.A,9); Assert.Equal(expected.B,actual.B,9); }
    private static int Physical(double x,double y,string scene,bool horizontal)
    {
        if(horizontal)(x,y)=(y,x);
        double a=(scene=="rotated"?7:0)*Math.PI/180,dx=x-720,dy=y-480;
        x=720+Math.Cos(a)*dx+Math.Sin(a)*dy; y=480-Math.Sin(a)*dx+Math.Cos(a)*dy;
        bool small=scene=="small";
        for(int i=0;i<3;i++)
        {
            int top=230+i*(small?110:150),right=650+(small?100:180),bottom=top+(small?70:110);
            if(x>=650 && x<right && y>=top && y<bottom)return i+1;
            if(x>=643 && x<right+7 && y>=top-7 && y<bottom+7)return -1;
        }
        return 0;
    }
}
