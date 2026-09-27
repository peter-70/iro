using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Iro.Analysis;
using Iro.Core.Analysis;
using static Iro.TestFixtures.ReflectionFixture;

namespace IroGenTests;

public class ReflectionPngTests
{
    internal static byte[] Png(RgbFrame image)
    {
        byte[] pixels = new byte[image.Width * image.Height * 3];
        for (int y = 0; y < image.Height; y++) for (int x = 0; x < image.Width; x++)
        {
            var p = image.GetPixel(x, y); int offset = (y * image.Width + x) * 3;
            pixels[offset] = p.R; pixels[offset + 1] = p.G; pixels[offset + 2] = p.B;
        }
        byte[]? png = null; Exception? failure = null;
        var thread = new Thread(() => { try {
            var bitmap = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Rgb24, null, pixels, image.Width * 3);
            var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using var stream = new MemoryStream(); encoder.Save(stream); png = stream.ToArray();
        } catch (Exception ex) { failure = ex; } });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (failure != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
        return png!;
    }
    [Theory]
    [InlineData(false, 0)] [InlineData(false, 2)] [InlineData(true, 0)] [InlineData(true, 2)]
    public async Task EveryReflectionFixtureSurvivesPngWithIdenticalSafetyAndValues(bool horizontal, int noise)
    {
        string root = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(root, "iro.slnx"))) root = Directory.GetParent(root)!.FullName;
        string folder = Path.Combine(root, "tests", "adjustments", "reflexe-20260924", "bilder");
        Directory.CreateDirectory(folder);
        foreach (string shape in Shapes) foreach (string target in new[] { "Feld", "Wand", "Beide" })
        {
            var frame = Make(shape, target, horizontal, noise);
            var direct = new ImageAnalyzer().Analyze(frame, new());
            var png = Png(frame);
            var actual = await new PngAnalysisApi().AnalyzeAsync(png, new());
            Assert.Equal(direct.Status, actual.Status);
            Assert.Equal(direct.Hint, actual.Hint);
            Assert.Equal(direct.Fields.Count, actual.Fields.Count);
            for (int i = 0; i < direct.Fields.Count; i++)
            {
                Assert.Equal(direct.Fields[i].Bounds, actual.Fields[i].Bounds);
                Assert.Equal(direct.Fields[i].MeasurementAllowed, actual.Fields[i].MeasurementAllowed);
                Assert.Equal(direct.Fields[i].DeltaE00, actual.Fields[i].DeltaE00);
                Assert.Equal(direct.Fields[i].IsNearest, actual.Fields[i].IsNearest);
            }
            if (shape == "Lichtband" && target == "Feld")
            {
                var display = new AnalysisPresentation(); display.Present(actual);
                Assert.DoesNotContain(actual.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
                Assert.All(display.Fields, f => Assert.Equal("–", f.Value));
                Assert.False(string.IsNullOrWhiteSpace(display.Message));
            }
            string name = $"{shape}-{target}-{(horizontal ? "waagerecht" : "senkrecht")}-rauschen-{noise}";
            File.WriteAllBytes(Path.Combine(folder, name + ".png"), png);
        }
    }
}
