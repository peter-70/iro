namespace IroGen;

public readonly record struct Rgb(byte R, byte G, byte B)
{
    public string Hex => $"#{R:X2}{G:X2}{B:X2}";
    public byte[] Channels => [R, G, B];
}
public readonly record struct Lab(double L, double A, double B);

/// <summary>Independent generator reference implementation. sRGB, D65, CIEDE2000 kL=kC=kH=1.</summary>
public static class ColorScience
{
    public static double Linear(double value) => value <= .04045 ? value / 12.92 : Math.Pow((value + .055) / 1.055, 2.4);
    public static byte Encode(double value) => Byte(255 * (value <= .0031308 ? 12.92 * value : 1.055 * Math.Pow(value, 1 / 2.4) - .055));
    public static byte Byte(double value) => (byte)Math.Clamp(Math.Round(value), 0, 255);
    public static Lab ToLab(Rgb rgb)
    {
        double r = Linear(rgb.R / 255.0), g = Linear(rgb.G / 255.0), b = Linear(rgb.B / 255.0);
        static double F(double t) => t > 216.0 / 24389 ? Math.Cbrt(t) : (24389.0 / 27 * t + 16) / 116;
        double x = F((.4124564 * r + .3575761 * g + .1804375 * b) / .95047);
        double y = F(.2126729 * r + .7151522 * g + .0721750 * b);
        double z = F((.0193339 * r + .1191920 * g + .9503041 * b) / 1.08883);
        return new(116 * y - 16, 500 * (x - y), 200 * (y - z));
    }

    public static Rgb Hsl(double hue, double saturation, double lightness)
    {
        hue = (hue % 360 + 360) % 360;
        double c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
        double x = c * (1 - Math.Abs(hue / 60 % 2 - 1)), m = lightness - c / 2;
        var (r, g, b) = hue switch
        {
            < 60 => (c, x, 0.0), < 120 => (x, c, 0.0), < 180 => (0.0, c, x),
            < 240 => (0.0, x, c), < 300 => (x, 0.0, c), _ => (c, 0.0, x)
        };
        return new(Byte((r + m) * 255), Byte((g + m) * 255), Byte((b + m) * 255));
    }

    public static double DeltaE00(Lab first, Lab second)
    {
        static double Sin(double x) => Math.Sin(x * Math.PI / 180);
        static double Cos(double x) => Math.Cos(x * Math.PI / 180);
        static double Hue(double a, double b) => (Math.Atan2(b, a) * 180 / Math.PI + 360) % 360;
        static double Ratio(double c) => Math.Pow(c, 7) / (Math.Pow(c, 7) + Math.Pow(25, 7));
        double c1 = Math.Sqrt(first.A * first.A + first.B * first.B);
        double c2 = Math.Sqrt(second.A * second.A + second.B * second.B);
        double g = .5 * (1 - Math.Sqrt(Ratio((c1 + c2) / 2)));
        double a1 = (1 + g) * first.A, a2 = (1 + g) * second.A;
        c1 = Math.Sqrt(a1 * a1 + first.B * first.B); c2 = Math.Sqrt(a2 * a2 + second.B * second.B);
        double h1 = Hue(a1, first.B), h2 = Hue(a2, second.B);
        double dl = second.L - first.L, dc = c2 - c1, dh = h2 - h1;
        if (c1 * c2 == 0) dh = 0;
        else if (dh > 180) dh -= 360;
        else if (dh < -180) dh += 360;
        double dH = 2 * Math.Sqrt(c1 * c2) * Sin(dh / 2);
        double l = (first.L + second.L) / 2, c = (c1 + c2) / 2;
        double h = c1 * c2 == 0 ? h1 + h2 : Math.Abs(h1 - h2) <= 180 ? (h1 + h2) / 2
            : h1 + h2 < 360 ? (h1 + h2 + 360) / 2 : (h1 + h2 - 360) / 2;
        double t = 1 - .17 * Cos(h - 30) + .24 * Cos(2 * h) + .32 * Cos(3 * h + 6) - .20 * Cos(4 * h - 63);
        double sl = 1 + .015 * (l - 50) * (l - 50) / Math.Sqrt(20 + (l - 50) * (l - 50));
        double sc = 1 + .045 * c, sh = 1 + .015 * c * t;
        double rt = -2 * Math.Sqrt(Ratio(c)) * Sin(60 * Math.Exp(-Math.Pow((h - 275) / 25, 2)));
        return Math.Sqrt(Math.Pow(dl / sl, 2) + Math.Pow(dc / sc, 2) + Math.Pow(dH / sh, 2) + rt * dc / sc * dH / sh);
    }
}
