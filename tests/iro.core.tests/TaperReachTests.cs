using System.Reflection;
using Iro.Core.Analysis;
using Xunit;
using Xunit.Abstractions;

namespace Iro.Core.Tests;

// Bounded synthetic regressions, not a general perspective/optical contract.
public sealed class TaperReachTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData(false, 0)] [InlineData(true, 0)]
    [InlineData(false, 7)] [InlineData(true, 7)]
    public void TaperPreservesCorrectOriginalPixelComparisons(bool horizontal, double angle)
    {
        var scene = new Scene(horizontal, angle, .2);
        var image = scene.Image();
        var control = Run(new Scene(horizontal, angle, 0).Image());
        var result = Run(image);
        var detection = Detect(image, result);
        Assert.True(MeasurementSafety.HasStrongCoherentTaper(detection));
        Assert.NotEmpty(detection.SurfacePolygons);
        if (angle != 0) Assert.NotEqual(0, result.StraighteningDegrees);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Audit(image, result, scene);
        Assert.Equal(AnalysisStatus.Measured, control.Status);
        foreach (var field in result.Fields)
        {
            int id = scene.Label(field.Bounds.X+field.Bounds.Width/2d,field.Bounds.Y+field.Bounds.Height/2d);
            Assert.Equal(control.Fields[id-1].DeltaE00!.Value,field.DeltaE00!.Value,9);
        }
        Assert.True(result.Fields[2].IsNearest);
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void UnevenFieldRemainsLocalWithTaper(bool horizontal)
    {
        var scene = new Scene(horizontal, 0, .2);
        var image = scene.Image(unevenField:true);
        var result = Run(image);
        Assert.True(MeasurementSafety.HasStrongCoherentTaper(Detect(image,result)));
        Assert.Equal(AnalysisStatus.PartiallyMeasured,result.Status);
        Assert.False(result.Fields[0].MeasurementAllowed);
        Assert.Null(result.Fields[0].DeltaE00);
        Assert.True(result.Fields[0].Measurement.SpatialDeltaE > result.Options.MaximumSpatialDeltaE);
        Assert.Equal(2,result.Fields.Count(f=>f.MeasurementAllowed));
        Audit(image,result,scene);
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void BadSharedReferenceStillInvalidatesDependentComparisons(bool horizontal)
    {
        var scene = new Scene(horizontal,0,.2);
        var baseline = Run(scene.Image());
        Assert.Equal(AnalysisStatus.Measured,baseline.Status);
        var bounds = baseline.Fields[0].Reference!.Bounds;
        var image = scene.Image(badReference:bounds);
        var result = Run(image);
        Assert.True(MeasurementSafety.HasStrongCoherentTaper(Detect(image,result)));
        Assert.Equal(AnalysisStatus.InvalidReference,result.Status);
        Assert.Equal(3,result.Fields.Count);
        Assert.All(result.Fields,f=>
        {
            Assert.True(f.Measurement.IsUsable);
            Assert.False(f.Reference!.IsUsable);
            Assert.Null(f.DeltaE00); Assert.False(f.IsNearest);
        });
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void CompetingGeometryIsNotFreedByTaperChange(bool horizontal)
    {
        var scene = new Scene(horizontal,0,.2);
        var image = scene.Image(competing:true);
        var result = Run(image);
        Assert.Equal(AnalysisStatus.AmbiguousPattern,result.Status);
        Assert.Empty(result.Fields);
    }

    private ImageAnalysis Run(RgbFrame image)
    {
        var pixels = image.Pixels.ToArray();
        var result = new ImageAnalyzer().Analyze(image,new());
        Assert.Equal(pixels,image.Pixels.ToArray());
        output.WriteLine($"{result.Status}: angle={result.StraighteningDegrees}, fields={result.Fields.Count}, valid={result.Fields.Count(f=>f.MeasurementAllowed)}");
        return result;
    }
    private static Detection Detect(RgbFrame source,ImageAnalysis result)
    {
        var rotation = result.StraighteningDegrees==0?null:new ImageRotation(source.Width,source.Height,-result.StraighteningDegrees);
        return StripDetector.Detect(rotation==null?source:new RgbFrame(source,rotation),result.Options,default);
    }
    private static void Audit(RgbFrame source,ImageAnalysis result,Scene scene)
    {
        var rotation=result.StraighteningDegrees==0?null:new ImageRotation(source.Width,source.Height,-result.StraighteningDegrees);
        var view=rotation==null?source:new RgbFrame(source,rotation);
        var detection=StripDetector.Detect(view,result.Options,default);
        var exclusions=detection.Fields.Concat(detection.BorderRegions??[]).Concat(detection.WeakBorderRegions).ToArray();
        var union=new PixelRect(detection.Fields.Min(b=>b.X),detection.Fields.Min(b=>b.Y),
            detection.Fields.Max(b=>b.Right)-detection.Fields.Min(b=>b.X),detection.Fields.Max(b=>b.Bottom)-detection.Fields.Min(b=>b.Y));
        var find=typeof(ImageAnalyzer).GetMethod("FindReference",BindingFlags.NonPublic|BindingFlags.Static)!;
        var reference=(PixelRect)find.Invoke(null,[view,union,exclusions,detection.Orientation])!;
        foreach(var field in result.Fields.Where(f=>f.MeasurementAllowed))
        {
            var bounds=Assert.Single(detection.Fields,b=>(rotation?.SourceBounds(b)??b)==field.Bounds);
            int id=scene.Label(field.Bounds.X+field.Bounds.Width/2d,field.Bounds.Y+field.Bounds.Height/2d);
            Assert.InRange(id,1,3);
            detection.SurfacePolygons.TryGetValue(bounds,out var contour);
            Assert.Equal(rotation?.SourceBounds(reference)??reference,field.Reference!.Bounds);
            CheckPixels(bounds.Inset(result.Options.InnerMargin),contour,id,field.Measurement.SampleCount);
            CheckPixels(reference,null,0,field.Reference.SampleCount);
            var expected=Lab((byte)(40+20*(id-1)));var wall=Lab(160);
            AssertLab(expected,field.Measurement.Lab!.Value);AssertLab(wall,field.Reference.Lab!.Value);
            Assert.Equal(ColorMath.DeltaE00(expected,wall),field.DeltaE00!.Value,9);
        }
        void CheckPixels(PixelRect bounds,IReadOnlyList<PixelPoint>? mask,int expected,int count)
        {
            int step=Math.Max(1,(int)Math.Ceiling(Math.Sqrt(bounds.Area/40000d))),found=0;
            foreach(var sample in view.Sample(bounds,step,default))
            {
                if(mask!=null&&!ContourMask.Contains(mask,sample.X+(rotation==null?.5:0),sample.Y+(rotation==null?.5:0)))continue;
                var p=rotation?.ToSource(sample.X,sample.Y)??new PixelPoint(sample.X,sample.Y);
                int x=(int)Math.Floor(p.X),y=(int)Math.Floor(p.Y);
                Assert.Equal(expected,scene.Label(x+.5,y+.5));
                Assert.Equal(source.GetPixel(x,y),sample.Pixel);found++;
            }
            Assert.Equal(count,found);
        }
    }
    private static LabColor Lab(byte value)=>ColorMath.LinearToLab(ColorMath.Decode(value),ColorMath.Decode(value),ColorMath.Decode(value));
    private static void AssertLab(LabColor expected,LabColor actual)
    { Assert.Equal(expected.L,actual.L,9);Assert.Equal(expected.A,actual.A,9);Assert.Equal(expected.B,actual.B,9); }

    private sealed record Scene(bool Horizontal,double Angle,double Slope)
    {
        private (double X,double Y) Canonical(double x,double y)
        {
            if(Horizontal)(x,y)=(y,x);
            double a=Angle*Math.PI/180,dx=x-720,dy=y-480;
            return (720+Math.Cos(a)*dx+Math.Sin(a)*dy,480-Math.Sin(a)*dx+Math.Cos(a)*dy);
        }
        public int Label(double x,double y)
        {
            (x,y)=Canonical(x,y);
            for(int i=0;i<3;i++)
                if(y>=200+160*i&&y<320+160*i&&Math.Abs(x-800)<(280-Slope*y)/2)return i+1;
            return 0;
        }
        public RgbFrame Image(bool unevenField=false,PixelRect? badReference=null,bool competing=false)
        {
            int width=Horizontal?960:1440,height=Horizontal?1440:960;
            var pixels=new byte[width*height*3];
            for(int y=0;y<height;y++)for(int x=0;x<width;x++)
            {
                var (cx,cy)=Canonical(x+.5,y+.5);int id=Label(x+.5,y+.5);
                byte value=id==0?(byte)160:(byte)(40+20*(id-1));
                if(unevenField&&id==1)value=(byte)(cx<800?34:46);
                if(badReference is {} r&&r.Contains(x,y))value=(byte)(x<r.X+r.Width/2?154:166);
                if(competing&&cy>=650&&cy<830&&((cx>=150&&cx<260)||(cx>=300&&cx<410)))value=65;
                int o=(y*width+x)*3;pixels[o]=pixels[o+1]=pixels[o+2]=value;
            }
            return new(width,height,width*3,pixels);
        }
    }
}
