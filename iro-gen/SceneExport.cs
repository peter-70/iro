using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace IroGen;

public static class SceneExport
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters = { new JsonStringEnumConverter() }
    };

    public static string Save(GeneratedScene scene, string parentDirectory)
    {
        string id = "irogen-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N");
        string folder = Path.Combine(parentDirectory, id);
        Directory.CreateDirectory(folder);
        try
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(scene.Image));
            using (var stream = new FileStream(Path.Combine(folder, id + ".png"), FileMode.CreateNew)) encoder.Save(stream);
            object metadata = CreateMetadata(scene, id);
            File.WriteAllText(Path.Combine(folder, id + ".json"), JsonSerializer.Serialize(metadata, JsonOptions));
            return folder;
        }
        catch
        {
            // Only this newly created, uniquely named package is removed on an incomplete export.
            foreach (string file in Directory.EnumerateFiles(folder)) File.Delete(file);
            Directory.Delete(folder);
            throw;
        }
    }

    public static object CreateMetadata(GeneratedScene scene, string id) => new
    {
        formatVersion = 1,
        captureId = id,
        description = "Synthetische Wand mit Farbmusterstreifen; aufgedruckte Sollwerte sind nominale ΔE00 vor Störeffekten.",
        imageFile = id + ".png",
        source = "generator",
        width = scene.Options.Width,
        height = scene.Options.Height,
        colorSpace = "sRGB",
        createdAtUtc = DateTime.UtcNow.ToString("O"),
        conditions = new
        {
            generator = new
            {
                name = "IroGen", version = SceneGenerator.Version, seed = scene.Options.Seed,
                parameters = new
                {
                    options = scene.Options,
                    spatialGeometry = scene.SpatialGeometry,
                    referenceRgb = scene.Wall.Channels,
                    nominalColorSpace = "8-bit sRGB → linear sRGB → XYZ D65 → Lab D65; CIEDE2000 kL=kC=kH=1",
                    nominalValues = scene.Fields.Select(f => new
                    {
                        f.FieldId, rgb = f.Color.Channels, f.DeltaE00,
                        projectedCorners = f.Polygon.Select(p => new { x = p.X, y = p.Y }),
                        f.Bounds
                    }),
                    geometryNote = "Eckpunkte beschreiben das volle projizierte Rechteck vor Bildzuschnitt und abgerundetem Abschluss; bounds ist dessen zugeschnittene Umhüllung, keine Messmaske.",
                    ranking = scene.Fields.OrderBy(f => f.DeltaE00).Select(f => f.FieldId).ToArray(),
                    effectsNote = "Nominale Werte sind keine rekonstruierten Materialmesswerte aus dem gestörten Bild. Störstärken sind Generatorparameter, keine Iro-Freigabegrenzen."
                }
            },
            camera = (object?)null
        },
        expected = new
        {
            verification = "proposed",
            basis = "Numerische Sollfarben und nominale Abstände stehen unter conditions.generator.parameters.nominalValues. Der App-Sollbefund muss am erzeugten Bild geprüft werden. Deshalb hier keine ungeprüften Bildfarbabstände oder Freigaben.",
            referenceBounds = (object?)null,
            fields = scene.Fields.Select(f => new
            {
                f.FieldId, f.Bounds,
                deltaE00 = (double?)null,
                measurementAllowed = (bool?)null,
                requiredHint = (string?)null
            }),
            measurementAllowed = (bool?)null,
            requiredHint = (string?)null
        }
    };
}
