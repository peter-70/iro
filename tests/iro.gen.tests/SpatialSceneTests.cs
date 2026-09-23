using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;
using IroGen;
using Xunit.Abstractions;

namespace IroGenTests;

public class SpatialSceneTests(ITestOutputHelper output)
{
    private static T Sta<T>(Func<T> action)
    {
        T result = default!; Exception? error = null;
        var thread = new Thread(() => { try { result = action(); } catch (Exception e) { error = e; } });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        return result;
    }

    [Fact]
    public void ProjectionAndInverseAgreeAcrossCombinedExtremeOptions()
    {
        foreach (var orientation in Enum.GetValues<StripOrientation>())
        foreach (var distance in Enum.GetValues<CameraDistance>())
        foreach (var strength in Enum.GetValues<ViewStrength>())
        foreach (var direction in Enum.GetValues<VerticalViewDirection>())
        {
            var geometry = new SceneGeometry(new() { Orientation = orientation, Distance = distance,
                SideView = strength, VerticalView = strength, VerticalDirection = direction,
                WallGap = WallSeparation.Large, RandomPlacement = true, Perspective = Severity.Medium });
            foreach (double u in new[] { 0, .23, .5, 1 })
            foreach (double v in new[] { 0, .31, .75, 1 })
            {
                var p = geometry.Project(u, v);
                Assert.True(double.IsFinite(p.X) && double.IsFinite(p.Y));
                var inverse = geometry.Unproject(p.X, p.Y);
                Assert.InRange(Math.Abs(inverse.X - u), 0, 1e-10);
                Assert.InRange(Math.Abs(inverse.Y - v), 0, 1e-10);
            }
        }
    }

    [Fact]
    public void CameraAxesForeshortenTheRequestedDirection()
    {
        static double Width(SceneGeometry g) => (g.Project(1, .5) - g.Project(0, .5)).Length;
        static double Height(SceneGeometry g) => (g.Project(.5, 1) - g.Project(.5, 0)).Length;
        var normal = new SceneGeometry(new());
        double lastWidth = Width(normal), lastHeight = Height(normal);
        foreach (var strength in new[] { ViewStrength.Light, ViewStrength.Strong, ViewStrength.VeryStrong })
        {
            double width = Width(new(new() { SideView = strength }));
            double height = Height(new(new() { VerticalView = strength }));
            Assert.True(width < lastWidth); Assert.True(height < lastHeight);
            lastWidth = width; lastHeight = height;
        }
        var above = new SceneGeometry(new() { VerticalView = ViewStrength.Strong, VerticalDirection = VerticalViewDirection.FromAbove });
        var below = new SceneGeometry(new() { VerticalView = ViewStrength.Strong, VerticalDirection = VerticalViewDirection.FromBelow });
        Assert.Equal(-above.Info.VerticalViewDegrees, below.Info.VerticalViewDegrees);
        Assert.True((above.Project(1, 0) - above.Project(0, 0)).Length > (above.Project(1, 1) - above.Project(0, 1)).Length);
        Assert.True((below.Project(1, 0) - below.Project(0, 0)).Length < (below.Project(1, 1) - below.Project(0, 1)).Length);
    }

    [Fact]
    public void RandomPlacementIsRepeatableAndCoversDirections()
    {
        var options = new GeneratorOptions { RandomPlacement = true, VerticalView = ViewStrength.Strong, SideView = ViewStrength.Light, StripLengthPercent = 55 };
        Assert.Equal(new SceneGeometry(options).Info, new SceneGeometry(options).Info);
        var infos = Enumerable.Range(1, 32).Select(seed => new SceneGeometry(options with { Seed = seed })).ToArray();
        Assert.Equal(32, infos.Select(g => g.Info).Distinct().Count());
        Assert.Contains(infos, g => g.Info.VerticalViewDegrees > 0);
        Assert.Contains(infos, g => g.Info.VerticalViewDegrees < 0);
        Assert.True(infos.Max(g => g.Info.RotationDegrees) - infos.Min(g => g.Info.RotationDegrees) > 300);
        foreach (var geometry in infos)
        foreach (var p in new[] { geometry.Project(0,0), geometry.Project(1,0), geometry.Project(1,1), geometry.Project(0,1) })
        { Assert.InRange(p.X, -1e-8, options.Width + 1e-8); Assert.InRange(p.Y, -1e-8, options.Height + 1e-8); }
    }

    [Theory]
    [InlineData(WallSeparation.Small, 5)]
    [InlineData(WallSeparation.Greater, 15)]
    [InlineData(WallSeparation.Large, 30)]
    public void WallGapScalesStripAndCastsOnlyWallShadow(WallSeparation gap, double centimeters) => Sta(() =>
    {
        var options = new GeneratorOptions { Width = 800, Height = 600, Position = StripPosition.Center,
            WallGap = gap, Labels = false, RoundedTop = false, StripLengthPercent = 55 };
        var scene = SceneGenerator.Generate(options);
        var geometry = new SceneGeometry(options);
        Assert.Equal(centimeters, scene.SpatialGeometry!.WallGapCentimeters, 8);
        var plain = new SceneGeometry(options with { WallGap = WallSeparation.None });
        Assert.True((geometry.Project(1,.5)-geometry.Project(0,.5)).Length > (plain.Project(1,.5)-plain.Project(0,.5)).Length);
        var pixels = new byte[options.Width * options.Height * 4]; scene.Image.CopyPixels(pixels, options.Width * 4, 0);
        Assert.Contains(Enumerable.Range(0, options.Width * options.Height), index =>
        {
            var uv = geometry.Unproject(index % options.Width + .5, index / options.Width + .5);
            return (uv.X < 0 || uv.X >= 1 || uv.Y < 0 || uv.Y >= 1)
                && pixels[4 * index + 2] < scene.Wall.R;
        });
        foreach (var field in scene.Fields)
        {
            int x = (int)field.Polygon.Average(p => p.X), y = (int)field.Polygon.Average(p => p.Y);
            int offset = (y * options.Width + x) * 4;
            Assert.Equal(field.Color.R, pixels[offset + 2]);
            Assert.Equal(field.Color.G, pixels[offset + 1]);
            Assert.Equal(field.Color.B, pixels[offset]);
        }
        return true;
    });

    [Theory]
    [InlineData(StripOrientation.Vertical)]
    [InlineData(StripOrientation.Horizontal)]
    public void ProjectedFieldInteriorsMatchRasterAndExport(StripOrientation orientation) => Sta(() =>
    {
        var options = new GeneratorOptions { Width = 800, Height = 800, Orientation = orientation,
            RandomPlacement = true, SideView = ViewStrength.Strong, VerticalView = ViewStrength.Strong,
            WallGap = WallSeparation.Greater, Labels = false, RoundedTop = false, StripLengthPercent = 55 };
        var first = SceneGenerator.Generate(options);
        byte[] Pixels(GeneratedScene s) { var p = new byte[800 * 800 * 4]; s.Image.CopyPixels(p, 3200, 0); return p; }
        var pixels = Pixels(first);
        Assert.Equal(pixels, Pixels(SceneGenerator.Generate(options)));
        foreach (var field in first.Fields)
        {
            int x = (int)field.Polygon.Average(p => p.X), y = (int)field.Polygon.Average(p => p.Y);
            int index = (y * 800 + x) * 4;
            Assert.Equal(field.Color.R, pixels[index + 2]); Assert.Equal(field.Color.G, pixels[index + 1]); Assert.Equal(field.Color.B, pixels[index]);
        }
        using var metadata = JsonDocument.Parse(JsonSerializer.Serialize(SceneExport.CreateMetadata(first, "spatial"), SceneExport.JsonOptions));
        var parameters = metadata.RootElement.GetProperty("conditions").GetProperty("generator").GetProperty("parameters");
        Assert.Equal(first.SpatialGeometry!.RotationDegrees, parameters.GetProperty("spatialGeometry").GetProperty("rotationDegrees").GetDouble());
        return true;
    });

    [Fact]
    public async Task SpatialPlanRunsThroughAnalysisAndCompactExport()
    {
        string root = Path.Combine(Path.GetTempPath(), "iro-spatial-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            File.WriteAllText(Path.Combine(root, "iro.slnx"), "<Solution/>");
            File.WriteAllText(Path.Combine(root, "IRO-KONSOLIDIERTER-PLAN.md"), "Isolated spatial test");
            var plan = TestPlan.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "testplans", "raumlage-perspektive-und-wandabstand.json")));
            Assert.Equal(108, plan.Expand().Sum(c => c.Case.Count));
            using var batch = Sta(() => plan.Generate(root));
            var request = IroTestHandoff.Submit(batch, root);
            var prior = await new Iro.Analysis.AnalysisRunner().RunAsync(request.Folder, Path.Combine(root, "tests", "runs"), new() { Straighten = false });
            var before = TestRunReview.Load(root, 1, runId: prior.Report.RunId);
            var run = await new Iro.Analysis.AnalysisRunner().RunAsync(request.Folder, Path.Combine(root, "tests", "runs"), new());
            var rows = TestRunReview.Load(root, 1, runId: run.Report.RunId);
            Assert.Equal(108, rows.Count);
            // Assert behavior of identified cases, not historical totals across unrelated safety policies.

            var oldRandom = before.Where(r => r.CaseName!.StartsWith("Zufällige Lage")).ToArray();
            Assert.True(oldRandom.Sum(r => r.Measured) < rows.Where(r => r.CaseName!.StartsWith("Zufällige Lage")).Sum(r => r.Measured));

            var random = rows.Where(r => r.CaseName!.StartsWith("Zufällige Lage")).ToArray();
            Assert.Equal(9, random.Count(r => r.Verdict == ReviewVerdict.NominalUnauffaellig));
            Assert.Equal(3, random.Count(r => r.Verdict == ReviewVerdict.Abgewiesen)); // Three generated horizontal strips are actually cropped.
            Assert.All(random.Where(r => r.Measured > 0), r => Assert.InRange(r.MaxDeviation!.Value, 0, .15));
            foreach (var old in before.Where(r => r.Verdict == ReviewVerdict.NominalUnauffaellig))
                Assert.Equal(ReviewVerdict.NominalUnauffaellig, rows.Single(r => r.CaptureId == old.CaptureId).Verdict);
            Assert.DoesNotContain(rows, r => r.Verdict == ReviewVerdict.Fehler);
            Assert.All(rows.Where(r => r.CaseName is "Kontrolle senkrecht" or "Kontrolle waagerecht"),
                r => Assert.Equal(ReviewVerdict.NominalUnauffaellig, r.Verdict));
            Assert.Contains(rows, r => r.Disturbances.Contains("Zufällige Position") && r.Scene.Contains("zufällig positioniert und gedreht"));
            Assert.Contains(rows, r => r.Disturbances.Contains("von unten"));
            Assert.Contains(rows, r => r.Disturbances.Contains("> 20 cm"));
            Assert.All(rows.Where(r => r.CaseName is "Kontrolle senkrecht" or "Kontrolle waagerecht"),
                r => Assert.Equal("keine Störung", r.Disturbances));
            foreach (var group in rows.GroupBy(r => r.Verdict)) output.WriteLine($"{group.Key}: {group.Count()}");
            var report = TestReviewExport.Create(rows, 1, false);
            Assert.Contains("108", report);
            Assert.Contains("Simulierte Lage:", report);
            Assert.NotNull(batch.Items[0].Load().SpatialGeometry);
            string? artifacts = Environment.GetEnvironmentVariable("IROGEN_SPATIAL_OUTPUT");
            if (artifacts != null)
            {
                Directory.CreateDirectory(artifacts);
                File.WriteAllText(Path.Combine(artifacts, "bericht.md"), report);
                File.WriteAllText(Path.Combine(artifacts, "vorher.md"), TestReviewExport.Create(before, 1, false));
                Sta(() =>
                {
                    foreach (var (name, options) in new (string, GeneratorOptions)[] {
                        ("kontrolle", new()), ("zufaellig", new() {RandomPlacement=true}),
                        ("seite", new() {SideView=ViewStrength.VeryStrong}),
                        ("unten", new() {VerticalView=ViewStrength.Strong,VerticalDirection=VerticalViewDirection.FromBelow}),
                        ("wandabstand", new() {WallGap=WallSeparation.Large}),
                        ("hochhalten", new() {VerticalView=ViewStrength.Strong,VerticalDirection=VerticalViewDirection.FromBelow,WallGap=WallSeparation.Greater,MotionBlur=Severity.Medium,ExposureStops=-2,Noise=Severity.Medium}) })
                    {
                        var scene = SceneGenerator.Generate(options with { Width=800,Height=600,Position=StripPosition.Center });
                        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(scene.Image));
                        using var stream = File.Create(Path.Combine(artifacts, name + ".png")); encoder.Save(stream);
                    }
                    return true;
                });
            }
        }
        finally { Directory.Delete(root, true); }
    }
}
