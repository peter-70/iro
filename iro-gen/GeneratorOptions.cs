namespace IroGen;

public enum StripPosition { Right, Left, Bottom, Top, Center }
public enum StripOrientation { Vertical, Horizontal }
public enum Severity { None, Light, Medium, Strong }
public enum ViewStrength { None, Light, Strong, VeryStrong }
public enum VerticalViewDirection { Random, FromAbove, FromBelow }
public enum WallSeparation { None, Small, Greater, Large }
public enum WallDifference { Exact, Light, Medium, Strong, Opposite }
public enum CameraDistance { Normal, TooClose, TooFar, Near, Nearer, Far, Farther }

public sealed record GeneratorOptions
{
    public string? TestCaseName { get; init; }
    public int Seed { get; init; } = 12345;
    public SceneColors? Colors { get; init; }
    public int Width { get; init; } = 1600;
    public int Height { get; init; } = 1200;
    public StripPosition Position { get; init; } = StripPosition.Right;
    public StripOrientation Orientation { get; init; }
    public int FieldCount { get; init; } = 7;
    public double StripWidthPercent { get; init; } = 23;
    public double StripLengthPercent { get; init; } = 82;
    public double GapPercent { get; init; } = 3;
    public double MarginPercent { get; init; } = 6;
    public double ShadeStep { get; init; } = 5;
    public int MatchingField { get; init; } = 3;
    public WallDifference WallDifference { get; init; }
    public bool RoundedTop { get; init; } = true;
    public bool VariableFieldHeights { get; init; }
    public double[]? FieldWidthFactors { get; init; }
    public bool Labels { get; init; } = true;
    public double RotationDegrees { get; init; }
    public CameraDistance Distance { get; init; }
    public Severity Perspective { get; init; }
    public bool RandomPlacement { get; init; }
    public ViewStrength SideView { get; init; }
    public ViewStrength VerticalView { get; init; }
    public VerticalViewDirection VerticalDirection { get; init; }
    public WallSeparation WallGap { get; init; }
    public Severity Glare { get; init; }
    public Severity Dirt { get; init; }
    public Severity Blur { get; init; }
    public Severity MotionBlur { get; init; }
    public Severity Shadows { get; init; }
    public Severity Texture { get; init; }
    public Severity Noise { get; init; }
    public Severity Vignette { get; init; }
    public Severity Occlusion { get; init; }
    public Severity Haze { get; init; }
    public double ExposureStops { get; init; }

    public void Validate()
    {
        static void Range(double value, double min, double max, string name)
        {
            if (!double.IsFinite(value) || value < min || value > max)
                throw new ArgumentException($"{name}: Bitte einen Wert von {min} bis {max} eingeben.");
        }
        Range(Width, 320, 4096, "Bildbreite"); Range(Height, 320, 4096, "Bildhöhe");
        if ((long)Width * Height > 12_000_000) throw new ArgumentException("Maximal 12 Millionen Bildpixel verwenden.");
        Range(FieldCount, 1, 20, "Feldanzahl"); Range(MatchingField, 1, FieldCount, "Bezugsfeld");
        Range(StripWidthPercent, 5, 95, "Streifenbreite"); Range(StripLengthPercent, 10, 98, "Streifenlänge");
        Range(GapPercent, 0, 15, "Feldabstand"); Range(MarginPercent, 0, 25, "Randabstand");
        Range(ShadeStep, 0.5, 10, "Helligkeitsabstufung"); Range(RotationDegrees, -80, 80, "Drehung");
        Range(ExposureStops, -3, 3, "Belichtung");
        if (FieldWidthFactors is { } widths)
        {
            if (widths.Length != FieldCount)
                throw new ArgumentException("Feldbreiten: Ein Faktor je Farbfeld erforderlich.");
            foreach (double factor in widths) Range(factor, .3, 1, "Feldbreitenfaktor");
        }
        if (Colors is { } colors)
        {
            if (colors.Palette == null || colors.Palette.Length != FieldCount)
                throw new ArgumentException("Die gespeicherte Farbpalette passt nicht zur Feldanzahl.");
            Range(colors.AnchorField, 1, FieldCount, "Paletten-Bezugsfeld");
            Range(colors.TargetDeltaE, 0, 200, "Zielabstand");
            if (string.IsNullOrWhiteSpace(colors.CoverageBand)) throw new ArgumentException("Abstandsbereich fehlt.");
        }
        foreach (var property in GetType().GetProperties().Where(p => p.PropertyType.IsEnum))
            if (!Enum.IsDefined(property.PropertyType, property.GetValue(this)!))
                throw new ArgumentException($"Ungültige Auswahl: {property.Name}.");
    }
}

/// <summary>Fixed algorithm; does not depend on the runtime's System.Random implementation.</summary>
public sealed class SeedRandom(int seed)
{
    private uint state = unchecked((uint)seed) ^ 0xA341316Cu;
    public double Next()
    {
        state = unchecked(state * 1664525u + 1013904223u);
        return state / 4294967296.0;
    }
}

