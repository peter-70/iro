using Iro.Core.Analysis;
namespace Iro.TestFixtures;
public static class ProjectiveFixture
{
    public const int Size = 900;
    public static RgbFrame Scene(double strength, bool horizontal, int noise, double angle = 0, bool reverse = false)
    {
        var data = new byte[Size * Size * 3];
        var random = new Random(92741);
        double c = Math.Cos(angle * Math.PI / 180), s = Math.Sin(angle * Math.PI / 180);
        for (int y = 0; y < Size; y++) for (int x = 0; x < Size; x++)
        {
            double rx = c * (x + .5 - 450) + s * (y + .5 - 450) + 450;
            double ry = -s * (x + .5 - 450) + c * (y + .5 - 450) + 450;
            if (horizontal) (rx, ry) = (ry, Size - rx);
            if (reverse) rx = Size - rx;
            // Inverse of a projective map of the entire planar scene:
            // X = 510 + u/(1+k*u/180), Y = 450 + v/(1+k*u/180).
            double divisor = 1 - strength * (rx - 510) / 180;
            double sx = divisor > 0 ? 510 + (rx - 510) / divisor : -1;
            double sy = divisor > 0 ? 450 + (ry - 450) / divisor : -1;
            int field = -1;
            for (int i = 0; i < 3; i++) if (sx >= 510 && sx < 690 && sy >= 180 + i * 150 && sy < 300 + i * 150) field = i;
            for (int channel = 0; channel < 3; channel++)
                data[(y * Size + x) * 3 + channel] = (byte)Math.Clamp((field < 0 ? 140 : 60 + field * 30 + channel * 15) + (noise == 0 ? 0 : random.Next(-noise, noise + 1)), 0, 255);
        }
        return new(Size, Size, Size * 3, data);
    }
}
