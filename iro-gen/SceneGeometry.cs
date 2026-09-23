using System.Windows;

namespace IroGen;

/// <summary>Resolved synthetic geometry; distances are model assumptions, not camera measurements.</summary>
public sealed record SpatialSceneInfo(double RotationDegrees, double SideViewDegrees,
    double VerticalViewDegrees, double WallGapCentimeters, double CenterX, double CenterY);

/// <summary>Legacy strip projection followed by a pinhole plane projection in camera axes.</summary>
public sealed class SceneGeometry
{
    private readonly double width, length, k, cos, sin, cx, cy;
    private readonly double a, b, c, d, e, f, g, h, i;
    private readonly bool spatial;
    public SpatialSceneInfo Info { get; }

    public SceneGeometry(GeneratorOptions o)
    {
        o.Validate();
        var random = new SeedRandom(MixSeed(o.Seed));
        // Independent stream keeps palettes, textures and older scenes unchanged.
        double rotation = o.RandomPlacement ? random.Next() * 360 - 180 : o.RotationDegrees;
        double yaw = Degrees(o.SideView) * (random.Next() < .5 ? -1 : 1);
        double pitch = Degrees(o.VerticalView) * (o.VerticalDirection switch
        {
            VerticalViewDirection.FromAbove => 1,
            VerticalViewDirection.FromBelow => -1,
            _ => random.Next() < .5 ? -1 : 1
        });
        double gap = o.WallGap switch { WallSeparation.Small => .05, WallSeparation.Greater => .15, WallSeparation.Large => .30, _ => 0 };
        bool vertical = o.Orientation == StripOrientation.Vertical;
        double distanceScale = o.Distance switch { CameraDistance.TooClose => 3.4, CameraDistance.TooFar => .18, CameraDistance.Near => 1.35, CameraDistance.Nearer => 2, CameraDistance.Far => .65, CameraDistance.Farther => .35, _ => 1 };
        width = (vertical ? o.Width : o.Height) * o.StripWidthPercent / 100 * distanceScale;
        length = (vertical ? o.Height : o.Width) * o.StripLengthPercent / 100 * distanceScale;
        k = o.Perspective switch { Severity.Light => .2, Severity.Medium => .85, Severity.Strong => 3, _ => 0 };
        width *= o.Perspective switch { Severity.Light => .9, Severity.Medium => .55, Severity.Strong => .18, _ => 1 };
        double angle = (rotation + (vertical ? 0 : -90)) * Math.PI / 180;
        cos = Math.Cos(angle); sin = Math.Sin(angle);

        // Artificial camera: focal length 4 image long sides; wall distance 1.5 m.
        // No calibrated size/defocus or physical illumination claim is made.
        double focal = 4 * Math.Max(o.Width, o.Height), z = -gap / 1.5 * focal;
        double sy = Math.Sin(yaw * Math.PI / 180), cyaw = Math.Cos(yaw * Math.PI / 180);
        double sp = Math.Sin(pitch * Math.PI / 180), cp = Math.Cos(pitch * Math.PI / 180);
        a = cyaw; b = 0; c = sy * z;
        d = sp * sy; e = cp; f = -sp * cyaw * z;
        g = -cp * sy / focal; h = sp / focal; i = 1 + cp * cyaw * z / focal;
        spatial = yaw != 0 || pitch != 0 || gap != 0;
        double boxW = Math.Abs(width * cos) + Math.Abs(length * sin);
        double boxH = Math.Abs(width * sin) + Math.Abs(length * cos);
        cx = o.Width / 2d; cy = o.Height / 2d;
        if (o.Distance is not (CameraDistance.TooClose or CameraDistance.Near or CameraDistance.Nearer))
        {
            if (o.Position == StripPosition.Left) cx = o.Width * o.MarginPercent / 100 + boxW / 2;
            if (o.Position == StripPosition.Right) cx = o.Width * (1 - o.MarginPercent / 100) - boxW / 2;
            if (o.Position == StripPosition.Top) cy = o.Height * o.MarginPercent / 100 + boxH / 2;
            if (o.Position == StripPosition.Bottom) cy = o.Height * (1 - o.MarginPercent / 100) - boxH / 2;
        }
        if (spatial && !o.RandomPlacement)
        {
            // Aim the camera at the selected image position after projection. Otherwise a
            // large separation/pitch would merely push the whole strip out of the frame.
            Point[] projected = [Project(0, 0), Project(1, 0), Project(1, 1), Project(0, 1)];
            double minX = projected.Min(p => p.X) - cx, maxX = projected.Max(p => p.X) - cx;
            double minY = projected.Min(p => p.Y) - cy, maxY = projected.Max(p => p.Y) - cy;
            cx = o.Width / 2d - (minX + maxX) / 2;
            cy = o.Height / 2d - (minY + maxY) / 2;
            if (o.Distance is not (CameraDistance.TooClose or CameraDistance.Near or CameraDistance.Nearer))
            {
                if (o.Position == StripPosition.Left) cx = o.Width * o.MarginPercent / 100 - minX;
                if (o.Position == StripPosition.Right) cx = o.Width * (1 - o.MarginPercent / 100) - maxX;
                if (o.Position == StripPosition.Top) cy = o.Height * o.MarginPercent / 100 - minY;
                if (o.Position == StripPosition.Bottom) cy = o.Height * (1 - o.MarginPercent / 100) - maxY;
            }
        }
        if (o.RandomPlacement)
        {
            // Pick a translation that fits where possible; oversized strips can remain cropped.
            Point[] corners = [Project(0, 0), Project(1, 0), Project(1, 1), Project(0, 1)];
            double minX = corners.Min(p => p.X) - cx, maxX = corners.Max(p => p.X) - cx;
            double minY = corners.Min(p => p.Y) - cy, maxY = corners.Max(p => p.Y) - cy;
            cx = Place(o.Width, minX, maxX, random.Next());
            cy = Place(o.Height, minY, maxY, random.Next());
        }
        Info = new(rotation, yaw, pitch, gap * 100, cx, cy);
    }

    private static int MixSeed(int seed)
    {
        // Avalanche adjacent seeds before the LCG's first draw.
        unchecked
        {
            uint value = (uint)seed ^ 0x53B19D27u;
            value = (value ^ (value >> 16)) * 0x7FEB352Du;
            value = (value ^ (value >> 15)) * 0x846CA68Bu;
            return (int)(value ^ (value >> 16));
        }
    }

    private static double Degrees(ViewStrength strength) => strength switch
    { ViewStrength.Light => 15, ViewStrength.Strong => 45, ViewStrength.VeryStrong => 70, _ => 0 };

    private static double Place(double size, double min, double max, double random) =>
        max - min <= size ? -min + random * (size - max + min) : size * (.25 + .5 * random) - (min + max) / 2;

    public Point Project(double u, double v)
    {
        double x = (u - .5) * width / (1 + k * v), y = (v - .5) * length / (1 + k * v);
        double px = x * cos - y * sin, py = x * sin + y * cos;
        if (!spatial) return new(cx + x * cos - y * sin, cy + x * sin + y * cos);
        double denominator = g * px + h * py + i;
        return new(cx + (a * px + b * py + c) / denominator, cy + (d * px + e * py + f) / denominator);
    }

    public Point Unproject(double x, double y)
    {
        double px = x - cx, py = y - cy;
        if (spatial)
        {
            // Solve the two linear equations of the inverse homography.
            double aa = a - px * g, bb = b - px * h, cc = px * i - c;
            double dd = d - py * g, ee = e - py * h, ff = py * i - f;
            double determinant = aa * ee - bb * dd;
            if (Math.Abs(determinant) < 1e-12) return new(-1, -1);
            px = (cc * ee - bb * ff) / determinant;
            py = (aa * ff - cc * dd) / determinant;
        }
        double lx = px * cos + py * sin, ly = -px * sin + py * cos;
        double denominator = length - k * ly;
        double v = denominator > 0 ? (ly + .5 * length) / denominator : -1;
        return new(.5 + lx * (1 + k * v) / width, v);
    }

    public double WallShadow(double x, double y, int imageWidth, int imageHeight)
    {
        if (Info.WallGapCentimeters == 0) return 1;
        double gap = Info.WallGapCentimeters / 100;
        var uv = Unproject(x - gap * imageWidth * .25, y - gap * imageHeight * .15);
        double feather = .01 + gap * .12;
        double coverage = Math.Clamp(Math.Min(uv.X, 1 - uv.X) / feather + .5, 0, 1)
            * Math.Clamp(Math.Min(uv.Y, 1 - uv.Y) / feather + .5, 0, 1);
        return 1 - coverage * .28 / (1 + 2 * gap);
    }
}
