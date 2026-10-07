using System.ComponentModel;
using System.Globalization;

namespace Iro.Core.Analysis;

public enum MarkerKind { Measured, Unavailable, Uncertain }
public sealed record AnalysisMarker(string RegionId, PixelRect Bounds, IReadOnlyList<PixelPoint>? Polygon,
    string Label, string Value, MarkerKind Kind, bool IsNearest);

public sealed record MeasurementRow(string FieldId, string Label, string Value, string Hint);

/// <summary>Shared result-to-UI connection. Call from the UI context; pixels are processed in the background.</summary>
public sealed class AnalysisPresentation(IImageAnalyzer? analyzer = null) : INotifyPropertyChanged
{
    private readonly IImageAnalyzer engine = analyzer ?? new ImageAnalyzer();
    private CancellationTokenSource? pending;
    private int revision;
    public string Message { get; private set; } = "Ein Testbild auswählen, um die Analyse zu prüfen.";
    public string Heading { get; private set; } = "Bereit";
    public bool IsBusy { get; private set; }
    public IReadOnlyList<MeasurementRow> Fields { get; private set; } = [];
    public IReadOnlyList<AnalysisMarker> Markers { get; private set; } = [];
    public int ImageWidth { get; private set; }
    public int ImageHeight { get; private set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task AnalyzeAsync(Func<CancellationToken, Task<RgbFrame>> load)
    {
        pending?.Cancel();
        using var cancellation = new CancellationTokenSource();
        pending = cancellation;
        int request = ++revision;
        Fields = []; Markers = []; ImageWidth = ImageHeight = 0; IsBusy = true; Heading = "Analyse läuft"; Message = "Bild wird geprüft …"; Changed();
        try
        {
            var result = await Task.Run(async () =>
                engine.Analyze(await load(cancellation.Token).ConfigureAwait(false), new(), cancellation.Token), cancellation.Token);
            if (request == revision) Present(result);
        }
        catch (OperationCanceledException)
        {
            if (request == revision) Reset("Analyse abgebrochen", "Ein Testbild auswählen, um erneut zu prüfen.");
        }
        catch (Exception error)
        {
            System.Diagnostics.Trace.WriteLine(error);
            if (request == revision) Reset("Bild nicht auswertbar", "Das Bild konnte nicht verarbeitet werden. Bitte ein anderes Bild versuchen.");
        }
        finally
        {
            if (request == revision) { pending = null; IsBusy = false; Changed(); }
        }
    }

    public void Present(ImageAnalysis result)
    {
        // Also invalidate work if a newer result is delivered directly by a future frame source.
        ++revision; pending?.Cancel(); pending = null; IsBusy = false;
        bool acceptsValues = result.Status is AnalysisStatus.Measured or AnalysisStatus.PartiallyMeasured;
        ImageWidth = result.Width; ImageHeight = result.Height;
        var markers = new List<AnalysisMarker>();
        Fields = result.Fields.Select((field, index) =>
        {
            bool valid = acceptsValues && field.MeasurementAllowed && field.Measurement.IsUsable
                && field.Reference?.IsUsable == true && field.DeltaE00 is >= 0 && double.IsFinite(field.DeltaE00.Value);
            string label = $"Feld {index + 1}";
            string value = valid ? field.DeltaE00!.Value.ToString("F1", CultureInfo.GetCultureInfo("de-DE")) : "–";
            markers.Add(new(field.FieldId, field.Bounds, field.Polygon, label, value,
                valid ? MarkerKind.Measured : MarkerKind.Unavailable, valid && field.IsNearest));
            return new MeasurementRow(field.FieldId, label, value,
                valid ? (field.IsNearest ? "Nächstliegendes ausgewertetes Feld · ΔE00" : "ΔE00 · kleiner = ähnlicher")
                    : field.Hint ?? field.Measurement.Reason ?? result.Hint);
        }).Concat(result.UncertainRegions.Select(region =>
        {
            const string label = "Unsicherer Randbereich";
            markers.Add(new(region.RegionId, region.Bounds, null, label, "–", MarkerKind.Uncertain, false));
            return new MeasurementRow(region.RegionId, label, "–", region.Reason);
        })).ToArray();
        Markers = markers.ToArray();
        Heading = result.Status switch
        {
            AnalysisStatus.Measured => "Farbvergleich",
            AnalysisStatus.PartiallyMeasured => "Teilweise auswertbar",
            AnalysisStatus.NoPattern => "Kein Muster erkannt",
            AnalysisStatus.AmbiguousPattern => "Muster nicht eindeutig",
            AnalysisStatus.UnsuitableGeometry => "Muster nicht ausreichend erkennbar",
            AnalysisStatus.InvalidReference => "Wandreferenz nicht auswertbar",
            _ => "Keine zuverlässige Messung"
        };
        Message = result.Hint;
        Changed();
    }

    public void Cancel()
    {
        ++revision; pending?.Cancel(); pending = null;
        Reset("Analyse angehalten", "Ein Testbild auswählen, um erneut zu prüfen.");
    }

    private void Reset(string heading, string message)
    {
        IsBusy = false; Fields = []; Markers = []; ImageWidth = ImageHeight = 0; Heading = heading; Message = message; Changed();
    }
    private void Changed() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
}

