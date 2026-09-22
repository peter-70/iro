namespace Iro.Core.Analysis;

public readonly record struct PixelRect(int X, int Y, int Width, int Height)
{
    public int Right => X + Width;
    public int Bottom => Y + Height;
    public long Area => (long)Width * Height;
    public bool Intersects(PixelRect other) => X < other.Right && Right > other.X && Y < other.Bottom && Bottom > other.Y;
    public PixelRect Inset(double fraction)
    {
        int dx = (int)Math.Ceiling(Width * fraction), dy = (int)Math.Ceiling(Height * fraction);
        return new(X + dx, Y + dy, Math.Max(0, Width - 2 * dx), Math.Max(0, Height - 2 * dy));
    }
}

/// <summary>Opaque, encoded sRGB RGB24. Coordinates are original image pixels, never preview pixels.</summary>
public sealed class RgbFrame
{
    public int Width { get; }
    public int Height { get; }
    public int Stride { get; }
    public ReadOnlyMemory<byte> Pixels { get; }
    public RgbFrame(int width, int height, int stride, ReadOnlyMemory<byte> pixels)
    {
        if (width < 1 || height < 1 || (long)width * height > 12_000_000 || stride < (long)width * 3 || (long)stride * height > pixels.Length)
            throw new ArgumentException("Ungültiger RGB-Puffer: Bildgröße, Stride oder Datenlänge.");
        Width = width; Height = height; Stride = stride; Pixels = pixels;
    }
    public RgbColor GetPixel(int x, int y)
    {
        if ((uint)x >= Width || (uint)y >= Height) throw new ArgumentOutOfRangeException(nameof(x));
        var data = Pixels.Span; int offset = y * Stride + x * 3;
        return new(data[offset], data[offset + 1], data[offset + 2]);
    }
}

public readonly record struct RgbColor(byte R, byte G, byte B);
public readonly record struct LabColor(double L, double A, double B);
public enum ReferenceMode { SharedAutomaticTrial, AdjacentPerFieldTrial }
public enum AnalysisStatus { Measured, PartiallyMeasured, NoPattern, AmbiguousPattern, UnsuitableGeometry, InvalidReference, InvalidFields }

/// <summary>Explicit experimental profile; these thresholds are not a product/device accuracy promise.</summary>
public sealed record AnalysisOptions
{
    public string ProfileVersion { get; init; } = "synthetic-trial-1";
    public ReferenceMode ReferenceMode { get; init; } = ReferenceMode.SharedAutomaticTrial;
    public int DetectionLongestSide { get; init; } = 720;
    public double RegionTolerance { get; init; } = 14;
    public double MinimumFillRatio { get; init; } = .77;
    public int MinimumFieldSide { get; init; } = 40;
    public int MinimumFieldArea { get; init; } = 2500;
    public int MinimumSamples { get; init; } = 600;
    public double InnerMargin { get; init; } = .18;
    public double MaximumOutlierFraction { get; init; } = .18;
    public double MaximumChannelMad { get; init; } = 12;
    public double SurfaceMargin { get; init; } = .05;
    public double MaximumSpatialDeltaE { get; init; } = 2;
    public double MaximumCrossEdgeDeviation { get; init; } = .06;
    public double MinimumEdgeConcentration { get; init; } = .18;
    public void Validate()
    {
        if (ProfileVersion != "synthetic-trial-1" || !Enum.IsDefined(ReferenceMode) || DetectionLongestSide is < 200 or > 1600 ||
            !double.IsFinite(RegionTolerance) || RegionTolerance is < 1 or > 40 ||
            !double.IsFinite(MinimumFillRatio) || MinimumFillRatio is < .5 or > 1 ||
            MinimumFieldSide is < 8 or > 1000 || MinimumFieldArea is < 64 or > 1_000_000 || MinimumSamples is < 16 or > 40000 ||
            !double.IsFinite(InnerMargin) || InnerMargin is < .05 or > .4 ||
            !double.IsFinite(MaximumOutlierFraction) || MaximumOutlierFraction is < 0 or > .4 ||
            !double.IsFinite(MaximumChannelMad) || MaximumChannelMad is < 1 or > 40 ||
            !double.IsFinite(SurfaceMargin) || SurfaceMargin < 0 || SurfaceMargin > InnerMargin ||
            !double.IsFinite(MaximumSpatialDeltaE) || MaximumSpatialDeltaE is <= 0 or > 200 ||
            !double.IsFinite(MaximumCrossEdgeDeviation) || MaximumCrossEdgeDeviation is < .01 or > .2 ||
            !double.IsFinite(MinimumEdgeConcentration) || MinimumEdgeConcentration is < .01 or > 1)
            throw new ArgumentException("Ungültige Analyseparameter oder unbekannte Profilversion.");
    }
}

public sealed record RegionMeasurement(PixelRect Bounds, bool IsUsable, string? Reason, LabColor? Lab,
    int SampleCount, double RejectedFraction, double ChannelMad, double NearLimitFraction)
{
    public double? SpatialDeltaE { get; init; }
}
public sealed record FieldAnalysis(string FieldId, PixelRect Bounds, PixelRect InnerBounds, RegionMeasurement Measurement,
    RegionMeasurement? Reference, double? DeltaE00, bool MeasurementAllowed, string? Hint, bool IsNearest)
{
    public double? SurfaceSpatialDeltaE { get; init; }
}
public sealed record ImageAnalysis(string AnalyzerVersion, AnalysisOptions Options, int Width, int Height,
    AnalysisStatus Status, string Hint, IReadOnlyList<FieldAnalysis> Fields, IReadOnlyList<string> Diagnostics);

public interface IImageAnalyzer
{
    ImageAnalysis Analyze(RgbFrame image, AnalysisOptions options, CancellationToken cancellationToken = default);
}
