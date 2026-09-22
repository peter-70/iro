using System.Globalization;
using Iro.Core.Analysis;

namespace Iro.Core.Tests;

public class AnalysisTests
{
    public static IEnumerable<object[]> PublishedPairs() => File.ReadLines(Path.Combine(AppContext.BaseDirectory,"ciede2000testdata.txt"))
        .Where(l=>!string.IsNullOrWhiteSpace(l)).Select(l=>l.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries).Select(v=>(object)double.Parse(v,CultureInfo.InvariantCulture)).ToArray());
    [Theory, MemberData(nameof(PublishedPairs))]
    public void CoreMatchesAllPublishedCiede2000Pairs(double l1,double a1,double b1,double l2,double a2,double b2,double expected)
    {
        var a = new LabColor(l1,a1,b1); var b = new LabColor(l2,a2,b2);
        Assert.InRange(Math.Abs(ColorMath.DeltaE00(a,b)-expected),0,.0001);
        Assert.InRange(Math.Abs(ColorMath.DeltaE00(b,a)-expected),0,.0001);
        Assert.Equal(0,ColorMath.DeltaE00(a,a));
    }
    [Theory]
    [InlineData(255,255,255,100,0,0)]
    [InlineData(0,0,0,0,0,0)]
    [InlineData(255,0,0,53.2408,80.0925,67.2032)]
    [InlineData(0,255,0,87.7347,-86.1827,83.1793)]
    [InlineData(0,0,255,32.2970,79.1875,-107.8602)]
    public void D65ReferenceColors(byte r,byte g,byte b,double l,double a,double labB)
    {
        var actual=ColorMath.ToLab(new(r,g,b));
        Assert.InRange(Math.Abs(actual.L-l),0,.001); Assert.InRange(Math.Abs(actual.A-a),0,.001); Assert.InRange(Math.Abs(actual.B-labB),0,.001);
    }
    [Fact]
    public void RegionMeanIsComputedBeforeLabAndInLinearLight()
    {
        byte[] bytes=new byte[40*40*3];
        for(int i=0;i<bytes.Length;i++)bytes[i]=(byte)((i/3)%2==0?100:120);
        var region=RegionSampler.Measure(new(40,40,120,bytes),new(0,0,40,40),new(){MaximumChannelMad=25});
        Assert.True(region.IsUsable);
        double linear=(ColorMath.Decode(100)+ColorMath.Decode(120))/2;
        Assert.Equal(ColorMath.LinearToLab(linear,linear,linear).L,region.Lab!.Value.L,8);
        Assert.True(Math.Abs(region.Lab.Value.L-ColorMath.ToLab(new(110,110,110)).L)>.1);
    }
    [Fact]
    public void ConstantRegionAndSmallTextOutliersAreHandledWithoutZeroMadFailure()
    {
        var data=Enumerable.Repeat((byte)120,60*60*3).ToArray();
        Array.Fill(data,(byte)0,0,100*3);
        var region=RegionSampler.Measure(new(60,60,180,data),new(0,0,60,60),new());
        Assert.True(region.IsUsable);Assert.Equal(0,region.ChannelMad);
        Assert.Equal(ColorMath.ToLab(new(120,120,120)).L,region.Lab!.Value.L,8);
    }
    [Fact]
    public void InhomogeneousRegionDoesNotReturnAnInventedColor()
    {
        var data=Enumerable.Repeat((byte)120,60*60*3).ToArray();Array.Fill(data,(byte)240,0,1500*3);
        var region=RegionSampler.Measure(new(60,60,180,data),new(0,0,60,60),new());
        Assert.False(region.IsUsable);Assert.Null(region.Lab);
    }
    [Fact]
    public void EmptyWallDoesNotBecomeASwatch()
    {
        var result=new ImageAnalyzer().Analyze(new(100,100,300,Enumerable.Repeat((byte)150,30000).ToArray()),new());
        Assert.Equal(AnalysisStatus.NoPattern,result.Status);Assert.Empty(result.Fields);
    }
    [Fact]
    public void StrideAndBufferBoundsAreValidated()
    {
        byte[] buffer=new byte[16];buffer[8]=123;
        var frame=new RgbFrame(2,2,8,buffer); Assert.Equal(123,frame.GetPixel(0,1).R);
        Assert.Throws<ArgumentException>(()=>new RgbFrame(2,2,5,buffer));
        Assert.Throws<ArgumentException>(()=>new RgbFrame(2,2,8,new byte[15]));
        Assert.Throws<ArgumentOutOfRangeException>(()=>frame.GetPixel(-1,0));
    }
    [Fact]
    public void CancellationAndInvalidProfilesAreRejected()
    {
        var frame=new RgbFrame(10,10,30,new byte[300]);
        Assert.Throws<OperationCanceledException>(()=>new ImageAnalyzer().Analyze(frame,new(),new CancellationToken(true)));
        Assert.Throws<ArgumentException>(()=>new ImageAnalyzer().Analyze(frame,new(){InnerMargin=double.NaN}));
    }
}

