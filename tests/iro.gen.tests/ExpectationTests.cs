using Iro.Analysis;
using Iro.Core.Analysis;
using IroGen;
using System.Text.Json;

namespace IroGenTests;

public class ExpectationTests
{
    [Fact]
    public void V1PlansStayReadableAndV1CannotSmuggleNewExpectations()
    {
        var legacy = TestPlan.Parse("""{"formatVersion":1,"kind":"irogen-test-plan","name":"Alt","defaults":{},"cases":[{"name":"Alt","count":1,"coverRange":false,"options":{}}]}""");
        Assert.Null(legacy.Expand()[0].Options.TestExpectation);
        var text = """{"formatVersion":2,"kind":"irogen-test-plan","name":"Neu","defaults":{},"cases":[{"name":"Neu","count":1,"coverRange":false,"options":{},"expected":{"formatVersion":1,"verification":"verified","basis":"Geometrische Kontrolle","measurementAllowed":true,"releasedFieldCount":3}}]}""";
        Assert.NotNull(TestPlan.Parse(text).Expand()[0].Options.TestExpectation);
        Assert.Throws<ArgumentException>(() => (TestPlan.Parse(text) with { FormatVersion=1 }).Expand());
    }
    private static BehaviorExpectation Reject() => new() { Verification = "verified", Basis = "Unabhängig konstruierte Geometrie",
        MeasurementAllowed = false, ReleasedFieldCount = 0, RequiredHint = AnalysisHintCode.PerspectiveTaper };
    private static ImageAnalysis Actual(int count, AnalysisHintCode? code)
    {
        var region = new RegionMeasurement(new(10,10,100,100), true, null, new(50,0,0), 100,0,0,0);
        var fields = Enumerable.Range(0,count).Select(i => new FieldAnalysis("f"+i,region.Bounds,region.Bounds,
            region,region,2,true,null,i==0)).ToArray();
        return new(ImageAnalyzer.Version,new(),800,600,count > 0 ? AnalysisStatus.Measured : AnalysisStatus.UnsuitableGeometry,
            "Beliebig anders formulierter UI-Text",fields,[]) { HintCode = code };
    }
    [Fact]
    public void HintWordingIsIrrelevantButWrongReasonFails()
    {
        Assert.Equal(ExpectationStatus.Passed, ExpectationEvaluator.Evaluate(Reject(), Actual(0,AnalysisHintCode.PerspectiveTaper)).Status);
        var result = ExpectationEvaluator.Evaluate(Reject(),Actual(0,AnalysisHintCode.Other));
        Assert.Equal(ExpectationStatus.Failed,result.Status);
        Assert.Contains("Erforderlicher Hinweis fehlt",result.Details);
    }
    [Fact]
    public void UnexpectedReleaseAndWrongFieldCountAreDetected()
    {
        var result=ExpectationEvaluator.Evaluate(Reject(),Actual(2,AnalysisHintCode.None));
        Assert.Equal(ExpectationStatus.Failed,result.Status);
        Assert.Contains("tatsächlich 2",result.Details);
        var measured = new BehaviorExpectation { Verification="verified",Basis="Kontrolle",MeasurementAllowed=true,
            ReleasedFieldCount=3,ForbiddenHint=AnalysisHintCode.PerspectiveTaper };
        Assert.Equal(ExpectationStatus.Passed,ExpectationEvaluator.Evaluate(measured,Actual(3,AnalysisHintCode.None)).Status);
        Assert.Equal(ExpectationStatus.Failed,ExpectationEvaluator.Evaluate(measured,Actual(2,AnalysisHintCode.None)).Status);
        Assert.Equal(ExpectationStatus.Failed,ExpectationEvaluator.Evaluate(measured,Actual(3,AnalysisHintCode.PerspectiveTaper)).Status);
        Assert.Equal(ExpectationStatus.Failed,ExpectationEvaluator.Evaluate(measured,Actual(0,AnalysisHintCode.Other)).Status);
    }
    [Fact]
    public void MissingProposedAndOldReasonDataNeverBecomePassed()
    {
        Assert.Equal(ExpectationStatus.NotEvaluated,ExpectationEvaluator.Evaluate(null,Actual(0,null)).Status);
        Assert.Equal(ExpectationStatus.NotEvaluated,ExpectationEvaluator.Evaluate(Reject() with {Verification="proposed"},Actual(0,null)).Status);
        Assert.Equal(ExpectationStatus.NotEvaluated,ExpectationEvaluator.Evaluate(Reject(),Actual(0,null)).Status);
        Assert.Equal(ExpectationStatus.Error,ExpectationEvaluator.Evaluate(Reject(),null).Status);
        Assert.Equal(ExpectationStatus.Error,ExpectationEvaluator.Evaluate(Reject(),Actual(0,AnalysisHintCode.PerspectiveTaper),"PNG unlesbar").Status);
    }
    [Fact]
    public void InvalidAndContradictoryExpectationsAreRejected()
    {
        foreach(var invalid in new[] { Reject() with {Basis=null}, Reject() with {FormatVersion=7},
            Reject() with {ReleasedFieldCount=3}, Reject() with {ReleasedFieldCount=-1},
            Reject() with {RequiredHint=(AnalysisHintCode)99}, Reject() with {ForbiddenHint=AnalysisHintCode.PerspectiveTaper} })
            Assert.Throws<ArgumentException>(()=>invalid.Validate());
        Assert.Throws<JsonException>(()=>JsonSerializer.Deserialize<BehaviorExpectation>(
            """{"formatVersion":1,"verification":"verified","basis":"X","measurementAllowed":false,"typo":true}""",
            AnalysisRunner.JsonOptions));
    }
    [Fact]
    public void PassedRejectionIsNotAProblemButFailedNominalControlIs()
    {
        var row=new TestRunRow("r","q",null,"s","",ReviewVerdict.Abgewiesen,"","",3,0,null,0,"")
        { Expectation=Reject(), Evaluation=ExpectationEvaluator.Evaluate(Reject(),Actual(0,AnalysisHintCode.PerspectiveTaper)) };
        Assert.False(row.IsProblem);
        var failed=row with {Verdict=ReviewVerdict.NominalUnauffaellig,Evaluation=new(ExpectationStatus.Failed,"Falsche Freigabe")};
        Assert.True(failed.IsProblem);
        string report=TestReviewExport.Create([row,failed],1,true);
        Assert.Contains("1 erfüllt; 1 nicht erfüllt",report);
        Assert.Contains("Falsche Freigabe",report);
        Assert.Contains("Unabhängig konstruierte Geometrie",report);
        Assert.Contains("Vollständige Sperre",report);
    }
}