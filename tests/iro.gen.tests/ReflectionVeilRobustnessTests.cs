using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
using Iro.Analysis;
using Iro.Core.Analysis;
using IroGen;

namespace IroGenTests;

public class ReflectionVeilRobustnessTests
{
    private sealed record Sample(int Level, ColorProfile Profile, int Seed, byte[] Png, PixelRect Expected,
        double Mean, double BrightFraction, double Saturation, double Contrast);
    private sealed record Outcome(Sample Sample, bool FullyCorrect, int FalseHits, int MissedHits, string? Error);
    private sealed record Candidate(string Name, Func<Sample, double> Value, bool HighIsWarning);

    [Fact]
    public void ScaleIsReproducibleWholeImageAndRejectsInvalidLevels() => Sta(() =>
    {
        var baseline = SceneGenerator.Generate(BaseOptions(0, ColorProfile.General, 44321));
        var repeated = SceneGenerator.Generate(BaseOptions(0, ColorProfile.General, 44321));
        var strongest = SceneGenerator.Generate(BaseOptions(10, ColorProfile.General, 44321));
        byte[] original = Pixels(baseline.Image), same = Pixels(repeated.Image), veiled = Pixels(strongest.Image);
        Assert.Equal(original, same);
        for (int i = 0; i < original.Length; i++)
        {
            if (i % 4 == 3) Assert.Equal((byte)255, veiled[i]);
            else Assert.Equal(ColorScience.Byte(original[i] + (255 - original[i]) * .4), veiled[i]);
        }
        Assert.Throws<ArgumentException>(() => (BaseOptions(-1, ColorProfile.General, 1)).Validate());
        Assert.Throws<ArgumentException>(() => (BaseOptions(11, ColorProfile.General, 1)).Validate());
        return true;
    });

    [Fact]
    public async Task MeasuresMatchingFieldRobustnessAcrossElevenVeilLevels()
    {
        const int perProfile = 20;
        var samples = Sta(() =>
        {
            var result = new List<Sample>();
            foreach (int level in Enumerable.Range(0, 11))
            foreach (ColorProfile profile in Enum.GetValues<ColorProfile>())
            for (int index = 0; index < perProfile; index++)
            {
                int seed = unchecked(173205 + level * 100003 + (int)profile * 1009 + index * 7919);
                var scene = SceneGenerator.Generate(BaseOptions(level, profile, seed));
                var bounds = scene.Fields[2].Bounds ?? throw new InvalidOperationException("Bezugsfeld liegt außerhalb des Bildes.");
                byte[] pixels = Pixels(scene.Image);
                var metrics = Metrics(pixels);
                result.Add(new(level, profile, seed, Png(scene.Image),
                    new(bounds.X, bounds.Y, bounds.Width, bounds.Height), metrics.Mean, metrics.BrightFraction, metrics.Saturation, metrics.Contrast));
            }
            return result;
        });

        Assert.Equal(880, samples.Count);
        var api = new PngAnalysisApi();
        var outcomes = new List<Outcome>(samples.Count);
        foreach (var sample in samples)
        {
            try
            {
                var analysis = await api.AnalyzeAsync(sample.Png, new());
                var nearest = analysis.Fields.Where(field => field.MeasurementAllowed && field.IsNearest).ToArray();
                var expected = analysis.Fields
                    .Select(field => (Field: field, Overlap: Iou(field.Bounds, sample.Expected)))
                    .Where(match => match.Overlap >= .45)
                    .OrderByDescending(match => match.Overlap)
                    .Select(match => match.Field)
                    .FirstOrDefault();
                int missed = expected == null || !nearest.Any(field => field.FieldId == expected.FieldId) ? 1 : 0;
                int falseHits = nearest.Count(field => expected == null || field.FieldId != expected.FieldId);
                outcomes.Add(new(sample, missed == 0 && falseHits == 0, falseHits, missed, null));
            }
            catch (Exception error)
            {
                outcomes.Add(new(sample, false, 0, 1, error.Message));
            }
        }

        Assert.Equal(880, outcomes.Count);
        Assert.All(Enumerable.Range(0, 11), level => Assert.Equal(80, outcomes.Count(item => item.Sample.Level == level)));
        WriteReport(outcomes);
        var failures = outcomes.Where(item => !item.FullyCorrect).ToArray();
        Assert.True(failures.Length == 0,
            $"Der relative Wand-/Farbfeldvergleich war in {failures.Length} von {outcomes.Count} Bildern falsch. " +
            $"Falsche Treffer: {failures.Sum(item => item.FalseHits)}, " +
            $"übersehene Treffer: {failures.Sum(item => item.MissedHits)}, " +
            $"Verarbeitungsfehler: {failures.Count(item => item.Error != null)}. " +
            "Einzelheiten stehen im erzeugten Robustheitsbericht.");
    }

    private static GeneratorOptions BaseOptions(int level, ColorProfile profile, int seed) => new()
    {
        Width = 640, Height = 480, FieldCount = 5, MatchingField = 3,
        StripWidthPercent = 25, StripLengthPercent = 82, GapPercent = 3,
        MarginPercent = 6, ShadeStep = 4, RoundedTop = false, Labels = false,
        Position = StripPosition.Right, Orientation = StripOrientation.Vertical,
        ColorProfile = profile, ReflectionVeilLevel = level, Seed = seed
    };

    private static void WriteReport(IReadOnlyList<Outcome> outcomes)
    {
        var report = new StringBuilder("# Robustheit bei gleichmäßigem Reflexschleier\n\n");
        report.AppendLine($"Stand 27. September 2026. IroGen {SceneGenerator.Version}, Analyse {ImageAnalyzer.Version}. 11 Stufen × 4 Farbprofile × 20 reproduzierbare Farben = {outcomes.Count} PNG-Einzelbilder.");
        report.AppendLine("Stufe 0 enthält keine Überlagerung. Jede weitere Stufe mischt bildweit weitere 4 Prozentpunkte Weiß in das kodierte sRGB-Bild: Stufe 1 = 4 %, 2 = 8 %, 3 = 12 %, …, 10 = 40 %. Dies ist eine Generator-Testskala, kein Kameramodell und kein Produktgrenzwert. Iro erhält ausschließlich PNG-Pixel; Stufe, Profil, Seed, Sollfeld und Farben werden erst außerhalb der Analyse zur Auswertung benutzt.\n");
        report.AppendLine("Richtige Antwort: Genau das zur Wand identische dritte Feld ist als ähnlichster freigegebener Treffer markiert. Jeder andere markierte Treffer zählt einzeln als falscher Treffer; fehlt die Markierung des dritten Feldes, zählt dies als ein übersehener Treffer. Vollständig richtig verlangt beides: kein falscher und kein übersehener Treffer.\n");
        report.AppendLine("| Stufe | Weißmischung | Bilder | vollständig richtig | Anteil | falsche Treffer | übersehene Treffer | 97-%-Ziel |");
        report.AppendLine("|---:|---:|---:|---:|---:|---:|---:|---|");
        foreach (int level in Enumerable.Range(0, 11))
        {
            var rows = outcomes.Where(item => item.Sample.Level == level).ToArray();
            int correct = rows.Count(item => item.FullyCorrect);
            report.AppendLine($"| {level} | {level * 4} % | {rows.Length} | {correct} | {(100d * correct / rows.Length).ToString("F2", CultureInfo.InvariantCulture)} % | {rows.Sum(item => item.FalseHits)} | {rows.Sum(item => item.MissedHits)} | {(correct >= Math.Ceiling(rows.Length * .97) ? "erreicht" : "verfehlt")} |");
        }

        report.AppendLine("\n## Farbprofile\n");
        report.AppendLine("Allgemein verwendet die bisherige mittlere Farbverteilung. Hell hält sämtliche Felder im hellen Bereich. Blass kombiniert geringe Sättigung mit heller Grundfarbe. Gering gesättigt verwendet nahezu neutrale Farben über einen mittleren Helligkeitsbereich. Diese Profile sind Testdatenklassen, keine Materialklassifikation durch Iro.\n");
        report.AppendLine("| Profil | Stufe | vollständig richtig / 20 | falsche Treffer | übersehene Treffer |");
        report.AppendLine("|---|---:|---:|---:|---:|");
        foreach (ColorProfile profile in Enum.GetValues<ColorProfile>())
        foreach (int level in Enumerable.Range(0, 11))
        {
            var rows = outcomes.Where(item => item.Sample.Profile == profile && item.Sample.Level == level).ToArray();
            report.AppendLine($"| {Profile(profile)} | {level} | {rows.Count(item => item.FullyCorrect)} / {rows.Length} | {rows.Sum(item => item.FalseHits)} | {rows.Sum(item => item.MissedHits)} |");
        }

        var errors = outcomes.Where(item => !item.FullyCorrect).ToArray();
        report.AppendLine("\n## Einzelbildmerkmale bei fehlerhaften Antworten\n");
        if (errors.Length == 0)
        {
            report.AppendLine("In dieser Matrix trat keine fehlerhafte Antwort auf; daher war keine Warnmerkmal-Untersuchung erforderlich. Dies beweist keine Fehlerfreiheit außerhalb der Matrix.");
        }
        else
        {
            var controls = outcomes.Where(item => item.Sample.Level == 0 && item.FullyCorrect).Select(item => item.Sample).ToArray();
            int allowedFalseWarnings = (int)Math.Floor(controls.Length * .03);
            var candidates = new Candidate[]
            {
                new("hohe mittlere sRGB-Helligkeit", sample => sample.Mean, true),
                new("hoher Anteil sehr heller Pixel", sample => sample.BrightFraction, true),
                new("geringe mittlere RGB-Sättigung", sample => sample.Saturation, false),
                new("geringer globaler Helligkeitskontrast", sample => sample.Contrast, false)
            };
            report.AppendLine($"Untersuchungskriterium ausschließlich für diesen Datensatz: mindestens 97 % der {errors.Length} fehlerhaften Antworten erfassen und höchstens 3 % der {controls.Length} vollständig richtigen reflexfreien hellen/blassen/gering gesättigten Kontrollen warnen. Dies ist kein Produktgrenzwert.\n");
            report.AppendLine("| untersuchtes Pixelmerkmal | beste Erfassung der Fehler bei höchstens 3 % falschen Kontrollwarnungen | falsche Kontrollwarnungen | 97/3-Kriterium |");
            report.AppendLine("|---|---:|---:|---|");
            foreach (var candidate in candidates)
            {
                var result = BestThreshold(candidate, controls, errors.Select(item => item.Sample).ToArray(), allowedFalseWarnings);
                report.AppendLine($"| {candidate.Name} | {result.Caught} / {errors.Length} ({(100d * result.Caught / errors.Length).ToString("F2", CultureInfo.InvariantCulture)} %) | {result.FalseWarnings} / {controls.Length} | {(result.Caught >= Math.Ceiling(errors.Length * .97) ? "erreicht" : "verfehlt")} |");
            }
            report.AppendLine("\nAuch ein in dieser endlichen Matrix auffälliges Merkmal könnte die bereits bytegenau belegten sauberen Gegenbilder nicht von den Schleierbildern unterscheiden. Ohne unabhängige reale Validierung wird daraus keine Sperre, Warnung oder Produktlogik abgeleitet.");
        }
        int processingErrors = outcomes.Count(item => item.Error != null);
        report.AppendLine($"\nVerarbeitungsfehler: {processingErrors}. Keine Generatorparameter oder Sollwerte an Iro übergeben. Keine Produktlogik geändert.");

        string root = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(root, "iro.slnx"))) root = Directory.GetParent(root)!.FullName;
        string folder = Path.Combine(root, "tests", "adjustments", "reflexschleier-20260927");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, $"robustheit-{ImageAnalyzer.Version}.md"), report.ToString());
    }

    private static (int Caught, int FalseWarnings) BestThreshold(Candidate candidate, Sample[] controls, Sample[] errors, int maximumFalseWarnings)
    {
        double[] values = controls.Concat(errors).Select(candidate.Value).Distinct().OrderBy(value => value).ToArray();
        var thresholds = values.SelectMany(value => new[] { value, Math.BitIncrement(value), Math.BitDecrement(value) });
        int bestCaught = -1, bestFalse = int.MaxValue;
        foreach (double threshold in thresholds)
        {
            bool Warn(Sample sample) => candidate.HighIsWarning ? candidate.Value(sample) >= threshold : candidate.Value(sample) <= threshold;
            int falseWarnings = controls.Count(Warn);
            if (falseWarnings > maximumFalseWarnings) continue;
            int caught = errors.Count(Warn);
            if (caught > bestCaught || caught == bestCaught && falseWarnings < bestFalse)
                (bestCaught, bestFalse) = (caught, falseWarnings);
        }
        return (Math.Max(0, bestCaught), bestFalse == int.MaxValue ? 0 : bestFalse);
    }

    private static (double Mean, double BrightFraction, double Saturation, double Contrast) Metrics(byte[] pixels)
    {
        double sum = 0, sumSquares = 0, saturation = 0;
        int bright = 0, count = pixels.Length / 4;
        for (int i = 0; i < pixels.Length; i += 4)
        {
            double b = pixels[i] / 255d, g = pixels[i + 1] / 255d, r = pixels[i + 2] / 255d;
            double y = .2126 * r + .7152 * g + .0722 * b;
            sum += y; sumSquares += y * y;
            if (y >= .9) bright++;
            double maximum = Math.Max(r, Math.Max(g, b)), minimum = Math.Min(r, Math.Min(g, b));
            saturation += maximum == 0 ? 0 : (maximum - minimum) / maximum;
        }
        double mean = sum / count;
        return (mean, (double)bright / count, saturation / count, Math.Sqrt(Math.Max(0, sumSquares / count - mean * mean)));
    }

    private static double Iou(PixelRect a, PixelRect b)
    {
        double overlap = Math.Max(0, Math.Min(a.Right, b.Right) - Math.Max(a.X, b.X))
            * Math.Max(0, Math.Min(a.Bottom, b.Bottom) - Math.Max(a.Y, b.Y));
        return overlap / (a.Area + b.Area - overlap);
    }

    private static string Profile(ColorProfile profile) => profile switch
    {
        ColorProfile.Bright => "hell", ColorProfile.Pale => "blass",
        ColorProfile.LowSaturation => "gering gesättigt", _ => "allgemein"
    };

    private static byte[] Pixels(BitmapSource bitmap)
    {
        var pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
        bitmap.CopyPixels(pixels, bitmap.PixelWidth * 4, 0);
        return pixels;
    }

    private static byte[] Png(BitmapSource bitmap)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = new MemoryStream();
        encoder.Save(stream);
        return stream.ToArray();
    }

    private static T Sta<T>(Func<T> action)
    {
        T result = default!; Exception? error = null;
        var thread = new Thread(() => { try { result = action(); } catch (Exception exception) { error = exception; } });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        return result;
    }
}
