namespace Iro.Core.Analysis;

/// <summary>Iro calculation path, independently reference-tested; never calls the generator.</summary>
public static class ColorMath
{
    public static double Decode(byte value)
    {
        double encoded = value / 255d;
        return encoded <= .04045 ? encoded / 12.92 : Math.Pow((encoded + .055) / 1.055, 2.4);
    }
    public static LabColor ToLab(RgbColor rgb) => LinearToLab(Decode(rgb.R), Decode(rgb.G), Decode(rgb.B));
    public static LabColor LinearToLab(double r, double g, double b)
    {
        static double Transform(double t) => t > Math.Pow(6d / 29, 3) ? Math.Cbrt(t) : t / (3 * Math.Pow(6d / 29, 2)) + 4d / 29;
        double x = Transform((r * .4124564 + g * .3575761 + b * .1804375) / .95047);
        double y = Transform(r * .2126729 + g * .7151522 + b * .0721750);
        double z = Transform((r * .0193339 + g * .1191920 + b * .9503041) / 1.08883);
        return new(116 * y - 16, 500 * (x - y), 200 * (y - z));
    }

    public static double DeltaE00(LabColor a, LabColor b)
    {
        const double radians = Math.PI / 180;
        static double Hypot(double x, double y) => Math.Sqrt(x * x + y * y);
        static double SeventhRatio(double value) { double p = Math.Pow(value, 7); return p / (p + 6103515625d); }
        static double Hue(double y, double x)
        {
            double value = Math.Atan2(y, x);
            return value < 0 ? value + 2 * Math.PI : value;
        }
        double averageChroma = (Hypot(a.A, a.B) + Hypot(b.A, b.B)) / 2;
        double factor = 1 + (1 - Math.Sqrt(SeventhRatio(averageChroma))) / 2;
        double ca = Hypot(a.A * factor, a.B), cb = Hypot(b.A * factor, b.B);
        double ha = Hue(a.B, a.A * factor), hb = Hue(b.B, b.A * factor);
        double hueDifference = hb - ha;
        if (ca * cb == 0) hueDifference = 0;
        else if (hueDifference > Math.PI) hueDifference -= 2 * Math.PI;
        else if (hueDifference < -Math.PI) hueDifference += 2 * Math.PI;
        double meanHue;
        if (ca * cb == 0) meanHue = ha + hb;
        else if (Math.Abs(ha - hb) <= Math.PI) meanHue = (ha + hb) / 2;
        else meanHue = (ha + hb + (ha + hb < 2 * Math.PI ? 2 * Math.PI : -2 * Math.PI)) / 2;
        double meanL = (a.L + b.L) / 2, meanC = (ca + cb) / 2;
        double weightL = 1 + .015 * Math.Pow(meanL - 50, 2) / Math.Sqrt(20 + Math.Pow(meanL - 50, 2));
        double t = 1 - .17 * Math.Cos(meanHue - 30 * radians) + .24 * Math.Cos(2 * meanHue)
            + .32 * Math.Cos(3 * meanHue + 6 * radians) - .20 * Math.Cos(4 * meanHue - 63 * radians);
        double dl = (b.L - a.L) / weightL, dc = (cb - ca) / (1 + .045 * meanC);
        double dh = 2 * Math.Sqrt(ca * cb) * Math.Sin(hueDifference / 2) / (1 + .015 * meanC * t);
        double rotation = -2 * Math.Sqrt(SeventhRatio(meanC)) * Math.Sin(60 * radians * Math.Exp(-Math.Pow((meanHue / radians - 275) / 25, 2)));
        return Math.Sqrt(Math.Max(0, dl * dl + dc * dc + dh * dh + rotation * dc * dh));
    }
}
