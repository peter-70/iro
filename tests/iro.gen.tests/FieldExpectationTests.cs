using Iro.Analysis;
using Iro.Core.Analysis;
using IroGen;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace IroGenTests;

public class FieldExpectationTests
{
    private static BehaviorExpectation Expected() => new() { FormatVersion=2, Verification="verified", Basis="Gezielte Auswerter-Gegenprobe, keine Pixelprüfung",
        Fields=[new() {FieldId="field-1",MeasurementAllowed=true,DeltaE00=2,AbsoluteTolerance=.1},new() {FieldId="field-2",MeasurementAllowed=true,DeltaE00=5,AbsoluteTolerance=.1}] };
    private static ImageAnalysis Actual()
    {
        var region=new RegionMeasurement(new(10,10,100,100),true,null,new(50,0,0),100,0,0,0);
        return new(ImageAnalyzer.Version,new(),800,600,AnalysisStatus.Measured,"Kontrolle",
            [new("a",region.Bounds,region.Bounds,region,region,2,true,null,true),new("b",region.Bounds,region.Bounds,region,region,5,true,null,false)],[]);
    }
    private static FieldComparison[] Mappings() => [new("field-1","a",.99,999,999,999,"measured"),new("field-2","b",.99,999,999,999,"measured")];
    [Fact]
    public void ExplicitValuesUseActualAnalysisNeverNominalOrCachedComparisonValues()
    {
        var result=ExpectationEvaluator.Evaluate(Expected(),Actual(),comparisons:Mappings());
        Assert.Equal(ExpectationStatus.Passed,result.Status);
        Assert.Contains("Ist 2",result.Details); Assert.Contains("Ist 5",result.Details);
        var wrong=Actual() with { Fields=Actual().Fields.Select(f=>f with {DeltaE00=f.DeltaE00==2 ? 5 : 2}).ToArray() };
        Assert.Equal(ExpectationStatus.Failed,ExpectationEvaluator.Evaluate(Expected(),wrong,comparisons:Mappings()).Status);
    }
    [Fact]
    public void WrongDuplicateAmbiguousAndWeakAssignmentsNeverPass()
    {
        foreach(var mappings in new[] {
            new[] {Mappings()[0] with {DetectedFieldId="b"},Mappings()[1] with {DetectedFieldId="a"}},
            new[] {Mappings()[0],Mappings()[1] with {DetectedFieldId="a"}},
            new[] {Mappings()[0] with {Status="ambiguous-assignment",DetectedFieldId=null},Mappings()[1]},
            new[] {Mappings()[0] with {IntersectionOverUnion=.89},Mappings()[1]},
            new[] {Mappings()[0] with {DetectedFieldId=null,Status="not-detected"},Mappings()[1]} })
            Assert.Equal(ExpectationStatus.Failed,ExpectationEvaluator.Evaluate(Expected(),Actual(),comparisons:mappings).Status);
    }
    [Fact]
    public void MissingUnverifiedAndMalformedFieldContractsNeverPass()
    {
        Assert.Equal(ExpectationStatus.NotEvaluated,ExpectationEvaluator.Evaluate(Expected(),Actual()).Status);
        Assert.Equal(ExpectationStatus.NotEvaluated,ExpectationEvaluator.Evaluate(Expected() with {Verification="proposed"},Actual(),comparisons:Mappings()).Status);
        foreach(var mappings in new[] {Array.Empty<FieldComparison>(),new[]{Mappings()[0],Mappings()[0]},new[]{Mappings()[0] with {ExpectedFieldId="other"},Mappings()[1]}})
            Assert.Equal(ExpectationStatus.Error,ExpectationEvaluator.Evaluate(Expected(),Actual(),comparisons:mappings).Status);
        foreach(var invalid in new[] {Expected() with {FormatVersion=1},Expected() with {Fields=[]},Expected() with {Fields=[Expected().Fields![0],Expected().Fields![0]]},Expected() with {ReleasedFieldCount=1},Expected() with {Fields=[Expected().Fields![0] with {AbsoluteTolerance=null}]},Expected() with {Fields=[Expected().Fields![0] with {DeltaE00=double.NaN}]},Expected() with {Fields=[Expected().Fields![0] with {MeasurementAllowed=false}]}})
            Assert.Throws<ArgumentException>(()=>invalid.Validate());
        Assert.Throws<JsonException>(()=>JsonSerializer.Deserialize<FieldExpectation>("""{"fieldId":"f"}""",AnalysisRunner.JsonOptions));
    }
    [Fact]
    public void RejectedFieldsAndUnexpectedReleasesAreCheckedIndividually()
    {
        var expected=Expected() with {Fields=[Expected().Fields![0],new(){FieldId="field-2",MeasurementAllowed=false}]};
        Assert.Equal(ExpectationStatus.Failed,ExpectationEvaluator.Evaluate(expected,Actual(),comparisons:Mappings()).Status);
        var blocked=Actual() with {Fields=Actual().Fields.Select(f=>f.FieldId=="b" ? f with {MeasurementAllowed=false,DeltaE00=null} : f).ToArray()};
        Assert.Equal(ExpectationStatus.Passed,ExpectationEvaluator.Evaluate(expected,blocked,comparisons:Mappings()).Status);
        var missing=Actual() with {Fields=[Actual().Fields[0]]};
        var mappings=new[]{Mappings()[0],Mappings()[1] with {DetectedFieldId=null,Status="not-detected"}};
        Assert.Equal(ExpectationStatus.Passed,ExpectationEvaluator.Evaluate(expected,missing,comparisons:mappings).Status);
        var extra=missing with {Fields=[..missing.Fields,Actual().Fields[1] with {FieldId="foreign"}]};
        Assert.Contains("Unerwartete freigegebene Fläche",ExpectationEvaluator.Evaluate(expected,extra,comparisons:mappings).Details);
    }
    [Theory]
    [InlineData(double.NaN)] [InlineData(double.PositiveInfinity)] [InlineData(-1)] [InlineData(2.101)]
    public void InvalidOrOutOfToleranceActualValuesFail(double value)
    {
        var actual=Actual() with {Fields=[Actual().Fields[0] with {DeltaE00=value},Actual().Fields[1]]};
        Assert.Equal(ExpectationStatus.Failed,ExpectationEvaluator.Evaluate(Expected(),actual,comparisons:Mappings()).Status);
    }
    [Theory]
    [InlineData("feldzuordnung-und-messwerte.json")]
    [InlineData("rangfolge-und-gleichstaende.json")]
    public async Task PlanRunsThroughPngPersistenceReviewAndExportWithoutLeakingExpectationsToAnalysis(string planFile)
    {
        string project=AppContext.BaseDirectory;
        while(!File.Exists(Path.Combine(project,"iro.slnx"))) project=Directory.GetParent(project)!.FullName;
        var plan=TestPlan.Parse(File.ReadAllText(Path.Combine(project,"iro-gen/testplans",planFile)));
        string root=Path.Combine(Path.GetTempPath(),"iro-field-expectations-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            File.WriteAllText(Path.Combine(root,"iro.slnx"),"<Solution/>");
            File.WriteAllText(Path.Combine(root,"IRO-KONSOLIDIERTER-PLAN.md"),"Isolierter Test");
            GeneratedBatch? batch=null; Exception? error=null;
            var thread=new Thread(()=>{try{batch=plan.Generate(root);}catch(Exception e){error=e;}});
            thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();
            if(error!=null) throw error;
            using var owned=batch!;
            var request=IroTestHandoff.Submit(owned,root);
            var runner=new AnalysisRunner();
            var saved=await runner.RunAsync(request.Folder,Path.Combine(root,"tests/runs"),new());
            int count=plan.Cases.Count;
            Assert.Equal(count,saved.Report.ProcessedCount);
            Assert.All(saved.Report.Captures,c=>{Assert.Null(c.Error);Assert.True(c.Evaluation!.Status==ExpectationStatus.Passed,c.Evaluation.Details);});
            // Mutating numerical expectations and nominal values changes only review, not analysis.
            foreach(string file in Directory.GetFiles(request.Folder,"irogen-*.json",SearchOption.AllDirectories))
            {
                var metadata=JsonNode.Parse(File.ReadAllText(file))!;
                var parameters=metadata["conditions"]!["generator"]!["parameters"]!;
                foreach(var field in parameters["behaviorExpectation"]!["fields"]!.AsArray()) field!["deltaE00"]=999;
                if(parameters["behaviorExpectation"]!["ranking"] is JsonObject ranking)
                {
                    var reversed=ranking["groups"]!.AsArray().Reverse().Select(g=>g!.DeepClone()).ToArray();
                    ranking["groups"]=new JsonArray(reversed);
                }
                foreach(var field in parameters["nominalValues"]!.AsArray()) field!["deltaE00"]=999;
                File.WriteAllText(file,metadata.ToJsonString());
            }
            var changed=await runner.RunAsync(request.Folder,Path.Combine(root,"tests/runs"),new());
            for(int i=0;i<count;i++)
            {
                Assert.Equal(JsonSerializer.Serialize(saved.Report.Captures[i].Analysis),JsonSerializer.Serialize(changed.Report.Captures[i].Analysis));
                Assert.Equal(ExpectationStatus.Failed,changed.Report.Captures[i].Evaluation!.Status);
            }
            Directory.Move(request.Folder,request.Folder+"-removed");
            // Remove metadata from review lookup without touching the immutable result.
            Directory.Move(Path.Combine(root,"tests/requests"),Path.Combine(root,"old-requests"));
            var rows=TestRunReview.Load(root,1);
            Assert.Equal(count,rows.Count(r=>r.CheckStatus==ExpectationStatus.Passed));
            Assert.Equal(count,rows.Count(r=>r.CheckStatus==ExpectationStatus.Failed));
            string report=TestReviewExport.Create(rows,1,false);
            Assert.Contains("geprüftes Soll ΔE00",report);Assert.Contains("field-1",report);Assert.Contains("NICHT erfüllt",report);
            if(planFile.StartsWith("rangfolge"))
            {
                Assert.All(saved.Report.Captures,c=>Assert.Equal(ExpectationStatus.Passed,c.Evaluation!.Ranking!.Status));
                Assert.All(changed.Report.Captures,c=>Assert.Equal(ExpectationStatus.Failed,c.Evaluation!.Ranking!.Status));
                Assert.Contains("Rangfolge",report); Assert.Contains("Gleichstand",report);
            }
            string evidence=Path.Combine(project,planFile.StartsWith("rangfolge") ? "tests/adjustments/rangfolge-20260927" : "tests/adjustments/felderwartungen-20260927",saved.Report.RunId);
            Directory.CreateDirectory(evidence);
            File.Copy(saved.ResultFile,Path.Combine(evidence,"results.json"));
            File.WriteAllText(Path.Combine(evidence,"bericht.md"),report);
        }
        finally { Directory.Delete(root,true); }
    }
}
