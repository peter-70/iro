using Iro.Analysis;
using Iro.Core.Analysis;
using System.Text.Json;

namespace IroGenTests;

public class RankingExpectationTests
{
    private static BehaviorExpectation Expected(IReadOnlyList<IReadOnlyList<string>>? groups=null,double tolerance=0) => new()
    {
        FormatVersion=3,Verification="verified",Basis="Unabhängige Auswerter-Gegenprobe",
        Fields=[new(){FieldId="one",MeasurementAllowed=true},new(){FieldId="two",MeasurementAllowed=true},new(){FieldId="three",MeasurementAllowed=true}],
        Ranking=new(){Groups=groups ?? [["one"],["two"],["three"]],TieTolerance=tolerance}
    };
    private static ImageAnalysis Actual(double a=2,double b=4,double c=8)
    {
        var region=new RegionMeasurement(new(10,10,100,100),true,null,new(50,0,0),100,0,0,0);
        double[] values=[a,b,c];double min=values.Min();
        var fields=values.Select((v,i)=>new FieldAnalysis("actual-"+i,region.Bounds,region.Bounds,region,region,v,true,null,v==min)).ToArray();
        return new(ImageAnalyzer.Version,new(),800,600,AnalysisStatus.Measured,"Kontrolle",fields,[]);
    }
    private static FieldComparison[] Mappings()=>new[]{"one","two","three"}.Select((id,i)=>new FieldComparison(id,"actual-"+i,1,999,999,999,"measured")).ToArray();
    private static ExpectationEvaluation Evaluate(BehaviorExpectation expected,ImageAnalysis actual)=>ExpectationEvaluator.Evaluate(expected,actual,comparisons:Mappings());
    [Fact]
    public void OrderUsesMappedValuesNotArrayOrderOrNominalMetadata()
    {
        var actual=Actual() with {Fields=Actual().Fields.Reverse().ToArray()};
        var result=Evaluate(Expected(),actual);
        Assert.Equal(ExpectationStatus.Passed,result.Status);
        Assert.Equal(3,result.Ranking!.Pairs.Count);
        Assert.All(result.Ranking.Pairs,p=>Assert.True(p.Passed));
        Assert.Equal(ExpectationStatus.Failed,Evaluate(Expected(),Actual(4,2,8)).Status);
        Assert.Equal(ExpectationStatus.Failed,Evaluate(Expected(),Actual(2,8,4)).Status);
        var read=JsonSerializer.Deserialize<ExpectationEvaluation>(JsonSerializer.Serialize(result,AnalysisRunner.JsonOptions),AnalysisRunner.JsonOptions)!;
        Assert.Equal(result.Ranking,read.Ranking! with {Pairs=result.Ranking.Pairs});
        Assert.Equal(result.Ranking.Pairs,read.Ranking.Pairs);
    }
    [Theory]
    [InlineData(2,0)]
    [InlineData(2.125,.125)]
    [InlineData(1.875,.125)]
    public void ExactAndNearTiesIncludeBoundaryInEitherDirection(double second,double tolerance)
    {
        var result=Evaluate(Expected([["one","two"],["three"]],tolerance),Actual(2,second,8));
        Assert.Equal(ExpectationStatus.Passed,result.Status);
        Assert.Equal("tied",result.Ranking!.Pairs[0].ActualRelation);
        Assert.True(result.Ranking.NearestFlagsCorrect);
    }
    [Fact]
    public void TiesOutsideToleranceAndAccidentalTiesFail()
    {
        Assert.Equal(ExpectationStatus.Failed,Evaluate(Expected([["one","two"],["three"]],.125),Actual(2,2.125001,8)).Status);
        Assert.Equal(ExpectationStatus.Failed,Evaluate(Expected(tolerance:.125),Actual(2,2.125,8)).Status);
        Assert.Equal(ExpectationStatus.Failed,Evaluate(Expected(),Actual(2,2,8)).Status);
    }
    [Fact]
    public void ChainedNearValuesDoNotBecomeAnInventedThreeWayTie()
    {
        var actual=Actual(2,2.125,2.25);
        Assert.Equal(ExpectationStatus.Failed,Evaluate(Expected([["one","two","three"]],.125),actual).Status);
        Assert.Equal(ExpectationStatus.Failed,Evaluate(Expected([["one","two"],["three"]],.125),actual).Status);
    }
    [Fact]
    public void WrongMissingOrIncompleteNearestFlagsFailEvenWithCorrectValues()
    {
        foreach(var actual in new[]{
            Actual() with {Fields=Actual().Fields.Select(f=>f with {IsNearest=false}).ToArray()},
            Actual() with {Fields=Actual().Fields.Select(f=>f with {IsNearest=f.FieldId=="actual-1"}).ToArray()},
            Actual() with {Fields=Actual().Fields.Select(f=>f with {IsNearest=true}).ToArray()} })
        {
            var result=Evaluate(Expected(),actual);
            Assert.Equal(ExpectationStatus.Failed,result.Status);
            Assert.False(result.Ranking!.NearestFlagsCorrect);
        }
        var tied=Actual(2,2,8);
        var incomplete=tied with {Fields=tied.Fields.Select(f=>f with {IsNearest=f.FieldId=="actual-0"}).ToArray()};
        Assert.Equal(ExpectationStatus.Failed,Evaluate(Expected([["one","two"],["three"]]),incomplete).Status);
        // A near tie is accepted as a test group, but a false exact-minimum flag is not.
        var near=Actual(2,2.125,8);
        var wrongNear=near with {Fields=near.Fields.Select(f=>f with {IsNearest=f.FieldId=="actual-1"}).ToArray()};
        Assert.Equal(ExpectationStatus.Failed,Evaluate(Expected([["one","two"],["three"]],.125),wrongNear).Status);
    }
    [Fact]
    public void MissingInvalidAndRejectedValuesCannotPassRanking()
    {
        foreach(double? value in new double?[]{null,double.NaN,double.PositiveInfinity,-1})
        {
            var actual=Actual() with {Fields=[Actual().Fields[0] with {DeltaE00=value},..Actual().Fields.Skip(1)]};
            var result=Evaluate(Expected(),actual);
            Assert.NotEqual(ExpectationStatus.Passed,result.Status);
            Assert.Equal(ExpectationStatus.NotEvaluated,result.Ranking!.Status);
        }
        var blocked=Actual() with {Fields=Actual().Fields.Select(f=>f with {MeasurementAllowed=false,DeltaE00=null,IsNearest=false}).ToArray()};
        Assert.Equal(ExpectationStatus.Failed,Evaluate(Expected(),blocked).Status);
        Assert.Equal(ExpectationStatus.NotEvaluated,ExpectationEvaluator.Evaluate(Expected(),Actual()).Status);
        Assert.Equal(ExpectationStatus.NotEvaluated,Evaluate(Expected() with {Verification="proposed"},Actual()).Status);
    }
    [Fact]
    public void PartialComparisonRanksOnlyReleasedFieldsAndRejectsStaleFlags()
    {
        var expected=Expected([["one"],["two"]]) with {Fields=[Expected().Fields![0],Expected().Fields![1],new(){FieldId="three",MeasurementAllowed=false}]};
        var partial=Actual() with {Status=AnalysisStatus.PartiallyMeasured,Fields=[..Actual().Fields.Take(2),Actual().Fields[2] with {MeasurementAllowed=false,DeltaE00=null,IsNearest=false}]};
        Assert.Equal(ExpectationStatus.Passed,Evaluate(expected,partial).Status);
        var stale=partial with {Fields=[..partial.Fields.Take(2),partial.Fields[2] with {IsNearest=true}]};
        Assert.Equal(ExpectationStatus.Failed,Evaluate(expected,stale).Status);
    }
    [Fact]
    public void InvalidRankingContractsAreRejectedAndOldContractsRemainReadable()
    {
        foreach(var invalid in new[]{Expected() with {FormatVersion=2},Expected() with {Ranking=null},Expected([]),Expected([["one"],["two"]]),Expected([["one","one"],["two","three"]]),Expected([["one"],["two"],["unknown"]]),Expected(tolerance:-1),Expected(tolerance:double.NaN),Expected(tolerance:double.PositiveInfinity)})
            Assert.Throws<ArgumentException>(()=>invalid.Validate());
        Assert.Throws<JsonException>(()=>JsonSerializer.Deserialize<RankingExpectation>("""{"groups":[["one"]]}""",AnalysisRunner.JsonOptions));
        (Expected() with {FormatVersion=2,Ranking=null}).Validate();
        new BehaviorExpectation{MeasurementAllowed=true}.Validate();
    }
}
