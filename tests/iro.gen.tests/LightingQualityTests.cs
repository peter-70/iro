using IroGen;
using Iro.Core.Analysis;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xunit.Abstractions;

namespace IroGenTests;

public class LightingQualityTests(ITestOutputHelper output)
{
    private static GeneratedScene Generate(GeneratorOptions options)
    {
        GeneratedScene result = null!;
        Exception? error = null;
        var thread = new Thread(() => { try { result = SceneGenerator.Generate(options); } catch (Exception e) { error = e; } });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (error != null) throw error;
        return result;
    }

    [Fact]
    public void CompareSpatialQualityAgainstPriorRules()
    {
        foreach (int seed in new[] { 12345, -1640519182, 1013916587 })
        foreach (var (name, options) in new (string, GeneratorOptions)[] {
            ("control", new()), ("shadow", new() { Shadows = Severity.Strong }),
            ("vignette-light", new() { Vignette = Severity.Light }),
            ("vignette-medium", new() { Vignette = Severity.Medium }),
            ("noise", new() { Noise = Severity.Medium }) })
        {
            var scene = Generate(options with { Seed = seed });
            var bitmap = new FormatConvertedBitmap(scene.Image, PixelFormats.Rgb24, null, 0);
            byte[] data = new byte[scene.Options.Width * scene.Options.Height * 3];
            bitmap.CopyPixels(data, scene.Options.Width * 3, 0);
            var frame = new RgbFrame(scene.Options.Width, scene.Options.Height, scene.Options.Width * 3, data);
            var before = new ImageAnalyzer().Analyze(frame, new() { MaximumSpatialDeltaE = 200 });
            var after = new ImageAnalyzer().Analyze(frame, new());
            output.WriteLine($"{name} seed={seed}: allowed {before.Fields.Count(f=>f.MeasurementAllowed)} -> {after.Fields.Count(f=>f.MeasurementAllowed)}; refSpatial={after.Fields.FirstOrDefault()?.Reference?.SpatialDeltaE:F2}; fieldSpatial={string.Join(",", after.Fields.Select(f=>f.SurfaceSpatialDeltaE?.ToString("F2")))}");
            double MaxError(ImageAnalysis result)
            {
                var errors = new List<double>();
                foreach (var field in result.Fields.Where(f => f.MeasurementAllowed))
                {
                    var target = scene.Fields.OrderByDescending(f =>
                    {
                        var b = f.Bounds!;
                        double intersection = Math.Max(0, Math.Min(b.X + b.Width, field.Bounds.Right) - Math.Max(b.X, field.Bounds.X))
                            * (double)Math.Max(0, Math.Min(b.Y + b.Height, field.Bounds.Bottom) - Math.Max(b.Y, field.Bounds.Y));
                        return intersection / (b.Width * (double)b.Height + field.Bounds.Area - intersection);
                    }).First();
                    errors.Add(Math.Abs(field.DeltaE00!.Value - target.DeltaE00));
                }
                return errors.DefaultIfEmpty(0).Max();
            }
            output.WriteLine($"maxError: {MaxError(before):F2} -> {MaxError(after):F2}");
            if (name is "control" or "noise") Assert.Equal(7, after.Fields.Count(f=>f.MeasurementAllowed));
            else Assert.DoesNotContain(after.Fields, f => f.MeasurementAllowed);
        }
    }
    [Fact]
    public async Task WholePlanKeepsControlsAndReducesNominalOutliers()
    {
        string root = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "iro-lighting-" + Guid.NewGuid().ToString("N"));
        System.IO.Directory.CreateDirectory(root);
        try
        {
            System.IO.File.WriteAllText(System.IO.Path.Combine(root, "iro.slnx"), "<Solution/>");
            System.IO.File.WriteAllText(System.IO.Path.Combine(root, "IRO-KONSOLIDIERTER-PLAN.md"), "Isolated comparison");
            var plan = TestPlan.Parse(System.IO.File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "testplans", "bildqualitaet-und-abstand.json")));
            GeneratedBatch batch = null!; Exception? failure = null;
            var thread = new Thread(() => { try { batch = plan.Generate(root); } catch (Exception error) { failure = error; } });
            thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
            if (failure != null) throw failure;
            using (batch)
            {
                var request = IroTestHandoff.Submit(batch, root);
                var runner = new Iro.Analysis.AnalysisRunner();
                var before = await runner.RunAsync(request.Folder, System.IO.Path.Combine(root, "tests", "runs"), new() { MaximumSpatialDeltaE = 200 });
                var after = await runner.RunAsync(request.Folder, System.IO.Path.Combine(root, "tests", "runs"), new());
                var beforeRows = TestRunReview.Load(root, 1, runId: before.Report.RunId);
                var afterRows = TestRunReview.Load(root, 1, runId: after.Report.RunId);
                Assert.Equal(113, afterRows.Count);
                Assert.DoesNotContain(afterRows, r => r.Verdict == ReviewVerdict.Fehler);
                Assert.Equal(17, afterRows.Count(r => r.CaseName!.StartsWith("Kontrolle") && r.Verdict == ReviewVerdict.Erreicht));
                Assert.Equal(20, beforeRows.Count(r => r.Verdict == ReviewVerdict.FalscherMesswert));
                Assert.True(afterRows.Count(r => r.Verdict == ReviewVerdict.FalscherMesswert) < 20);
                foreach (var group in afterRows.GroupBy(r => r.CaseName))
                {
                    var prior = beforeRows.Where(r => r.CaseName == group.Key).ToArray();
                    output.WriteLine($"{group.Key}: full {prior.Count(r=>r.Verdict==ReviewVerdict.Erreicht)} -> {group.Count(r=>r.Verdict==ReviewVerdict.Erreicht)}, wrong {prior.Count(r=>r.Verdict==ReviewVerdict.FalscherMesswert)} -> {group.Count(r=>r.Verdict==ReviewVerdict.FalscherMesswert)}, fields {prior.Sum(r=>r.Measured)} -> {group.Sum(r=>r.Measured)}");
                }
                output.WriteLine($"TOTAL: {string.Join(", ", Enum.GetValues<ReviewVerdict>().Select(v=>$"{v}={afterRows.Count(r=>r.Verdict==v)}"))}");
                string? artifact = Environment.GetEnvironmentVariable("IRO_LIGHTING_REPORT");
                if (artifact != null) System.IO.File.WriteAllText(artifact, TestReviewExport.Create(afterRows, 1, false));
            }
        }
        finally { System.IO.Directory.Delete(root, true); }
    }    [Fact]
    public void AdditionalSeedsAndVariableGeometryDoNotLosePreviouslyUsableControls()
    {
        foreach (int seed in new[] { 7, 2146, 46883, 9517, 82721, 9876543 })
        foreach (bool horizontal in new[] { false, true })
        {
            var scene = Generate(new() { Seed = seed, VariableFieldHeights = true, Noise = Severity.Medium, Texture = Severity.Medium,
                Orientation = horizontal ? StripOrientation.Horizontal : StripOrientation.Vertical,
                Position = horizontal ? StripPosition.Bottom : StripPosition.Left });
            var bitmap = new FormatConvertedBitmap(scene.Image, PixelFormats.Rgb24, null, 0);
            var data = new byte[scene.Options.Width * scene.Options.Height * 3];
            bitmap.CopyPixels(data, scene.Options.Width * 3, 0);
            var frame = new RgbFrame(scene.Options.Width, scene.Options.Height, scene.Options.Width * 3, data);
            var before = new ImageAnalyzer().Analyze(frame, new() { MaximumSpatialDeltaE = 200 });
            var after = new ImageAnalyzer().Analyze(frame, new());
            Assert.True(before.Fields.Any(f => f.MeasurementAllowed), $"Unusable control seed={seed}");
            Assert.Equal(before.Fields.Where(f => f.MeasurementAllowed).Select(f => f.FieldId),
                after.Fields.Where(f => f.MeasurementAllowed).Select(f => f.FieldId));
            foreach (var field in after.Fields.Where(f => f.MeasurementAllowed))
                Assert.Equal(before.Fields.Single(f => f.FieldId == field.FieldId).DeltaE00, field.DeltaE00);
        }
    }}
