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
    private readonly ReadOnlyMemory<byte> pixels;
    public ReadOnlyMemory<byte> Pixels => Source == null ? pixels : throw new InvalidOperationException("Gedrehte Analysesicht besitzt keinen interpolierten Farbpuffer.");
    internal RgbFrame? Source { get; }
    internal ImageRotation? Rotation { get; }
    internal RgbFrame(RgbFrame source, ImageRotation rotation)
    {
        Source = source; Rotation = rotation; Width = rotation.Width; Height = rotation.Height; Stride = checked(Width * 3);
    }
    internal bool IsValidPixel(int x, int y)
    {
        if ((uint)x >= Width || (uint)y >= Height) return false;
        if (Rotation == null) return true;
        var p = Rotation.ToSource(x + .5, y + .5);
        return p.X >= 0 && p.Y >= 0 && p.X < Source!.Width && p.Y < Source.Height;
    }
    internal bool ContainsArea(PixelRect bounds) => bounds.X >= 0 && bounds.Y >= 0 && bounds.Right <= Width && bounds.Bottom <= Height
        && (Rotation == null || Rotation.ToSource(bounds).All(p => p.X >= 0 && p.Y >= 0 && p.X <= Source!.Width && p.Y <= Source.Height));
    internal IEnumerable<(RgbColor Pixel, double X, double Y)> Sample(PixelRect bounds, int step, CancellationToken token)
    {
        if (Rotation == null)
        {
            for (int y = bounds.Y; y < bounds.Bottom; y += step)
            {
                token.ThrowIfCancellationRequested();
                for (int x = bounds.X; x < bounds.Right; x += step) yield return (GetPixel(x, y), x, y);
            }
        }
        else
        {
            var original = Rotation.SourceBounds(bounds);
            step = Math.Max(step, (int)Math.Ceiling(Math.Sqrt(original.Area / 160000d)));
            for (int y = original.Y; y < original.Bottom; y += step)
            {
                token.ThrowIfCancellationRequested();
                for (int x = original.X; x < original.Right; x += step)
                {
                    var p = Rotation.ToAligned(x + .5, y + .5);
                    if (p.X >= bounds.X && p.Y >= bounds.Y && p.X < bounds.Right && p.Y < bounds.Bottom)
                        yield return (Source!.GetPixel(x, y), p.X, p.Y);
                }
            }
        }
    }
    public RgbFrame(int width, int height, int stride, ReadOnlyMemory<byte> pixels)
    {
        if (width < 1 || height < 1 || (long)width * height > 12_000_000 || stride < (long)width * 3 || (long)stride * height > pixels.Length)
            throw new ArgumentException("Ungültiger RGB-Puffer: Bildgröße, Stride oder Datenlänge.");
        Width = width; Height = height; Stride = stride; this.pixels = pixels;
    }
    public RgbColor GetPixel(int x, int y)
    {
        if ((uint)x >= Width || (uint)y >= Height) throw new ArgumentOutOfRangeException(nameof(x));
        if (Rotation != null)
        {
            var p = Rotation.ToSource(x + .5, y + .5);
            return IsValidPixel(x, y) ? Source!.GetPixel((int)p.X, (int)p.Y) : default;
        }
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
    public bool Straighten { get; init; } = true;
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
    public IReadOnlyList<PixelPoint>? Polygon { get; init; }
    public double? SpatialDeltaE { get; init; }
    internal bool HasUnresolvedChannels { get; init; }
}
public sealed record FieldAnalysis(string FieldId, PixelRect Bounds, PixelRect InnerBounds, RegionMeasurement Measurement,
    RegionMeasurement? Reference, double? DeltaE00, bool MeasurementAllowed, string? Hint, bool IsNearest)
{
    public IReadOnlyList<PixelPoint>? Polygon { get; init; }
    public IReadOnlyList<PixelPoint>? InnerPolygon { get; init; }
    public double? SurfaceSpatialDeltaE { get; init; }
}
public sealed record ImageAnalysis(string AnalyzerVersion, AnalysisOptions Options, int Width, int Height,
    AnalysisStatus Status, string Hint, IReadOnlyList<FieldAnalysis> Fields, IReadOnlyList<string> Diagnostics)
{ public double StraighteningDegrees { get; init; } }

public interface IImageAnalyzer
{
    ImageAnalysis Analyze(RgbFrame image, AnalysisOptions options, CancellationToken cancellationToken = default);
}
