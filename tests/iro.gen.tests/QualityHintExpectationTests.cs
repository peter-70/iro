using Iro.Analysis;
using Iro.Core.Analysis;
using IroGen;
using System.Text.Json;

namespace IroGenTests;

public class QualityHintExpectationTests
{
    [Theory]
    [InlineData(AnalysisHintCode.UnusableBlur, "Unschärfe")]
    [InlineData(AnalysisHintCode.ChannelLimit, "Farbkanal")]
    [InlineData(AnalysisHintCode.UnevenSurface, "räumlich ungleichmäßig")]
    public void ExactReasonsSurviveSerializationAndRejectEveryOtherReason(AnalysisHintCode code, string text)
    {
        var expectation=new BehaviorExpectation {Verification="verified",Basis="Gezielte Hinweis-Vertragsprüfung",MeasurementAllowed=false,ReleasedFieldCount=0,RequiredHint=code};
        expectation=JsonSerializer.Deserialize<BehaviorExpectation>(JsonSerializer.Serialize(expectation,AnalysisRunner.JsonOptions),AnalysisRunner.JsonOptions)!;
        foreach(var actualCode in Enum.GetValues<AnalysisHintCode>())
        {
            var actual=new ImageAnalysis(ImageAnalyzer.Version,new(),800,600,AnalysisStatus.InvalidFields,"Absichtlich anders formulierter Text",[],[]){HintCode=actualCode};
            var evaluation=ExpectationEvaluator.Evaluate(expectation,actual);
            Assert.Equal(code==actualCode ? ExpectationStatus.Passed : ExpectationStatus.Failed,evaluation.Status);
            var row=new TestRunRow("run","request",null,"synthetisch","",ReviewVerdict.Abgewiesen,"","",3,0,null,0,""){Expectation=expectation,Evaluation=evaluation};
            string export=TestReviewExport.Create([row],1,false);
            Assert.Contains(text,export);
            var forbidden=expectation with {RequiredHint=null,ForbiddenHint=code};
            Assert.Equal(code==actualCode ? ExpectationStatus.Failed : ExpectationStatus.Passed,ExpectationEvaluator.Evaluate(forbidden,actual).Status);
        }
    }
    [Fact]
    public void AnUnknownImageDoesNotAcquireAnInventedPhysicalCause()
    {
        var pixels=Enumerable.Repeat((byte)140,320*320*3).ToArray();
        var result=new ImageAnalyzer().Analyze(new(320,320,960,pixels),new());
        Assert.Equal(AnalysisHintCode.Other,result.HintCode);
        Assert.Empty(result.Fields);
        var old=result with {HintCode=null};
        var expected=new BehaviorExpectation{Verification="verified",Basis="Hinweisvertrag",RequiredHint=AnalysisHintCode.UnusableBlur};
        Assert.Equal(ExpectationStatus.NotEvaluated,ExpectationEvaluator.Evaluate(expected,old).Status);
    }
}
