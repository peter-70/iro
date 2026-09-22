using System.IO;
using System.Text.Json;

namespace IroGen;

public sealed record TestHandoff(string RequestId, string Folder, int ImageCount)
{
    public string Status => $"{ImageCount} Bilder als Iro-Testauftrag bereitgestellt. Bereit für die lokale Bildanalyse. Auftrag: {Folder}";
}

public static class IroTestHandoff
{
    public static string? FindProjectRoot(string start)
    {
        for (var directory = new DirectoryInfo(start); directory != null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "iro.slnx")) && File.Exists(Path.Combine(directory.FullName, "IRO-KONSOLIDIERTER-PLAN.md")))
                return directory.FullName;
        return null;
    }

    public static TestHandoff Submit(GeneratedBatch batch, string projectRoot, CancellationToken token = default)
    {
        string root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(projectRoot));
        if (FindProjectRoot(root) != root) throw new ArgumentException("Bitte den Iro-Projektstamm mit iro.slnx auswählen.");
        string requestId = "iro-request-" + Guid.NewGuid().ToString("N");
        string requests = Path.Combine(root, "tests", "requests");
        string staging = Path.Combine(requests, requestId + ".incomplete");
        string destination = Path.Combine(requests, requestId);
        Directory.CreateDirectory(staging);
        try
        {
            string dataset = BatchGenerator.SaveAll(batch, Path.Combine(staging, "inputs"), token);
            var request = new
            {
                formatVersion = 2, kind = "iro-test-request", requestId,
                createdAtUtc = DateTime.UtcNow.ToString("O"), source = "IroGen", generatorVersion = SceneGenerator.Version,
                batchId = batch.Id, imageCount = batch.Items.Count,
                seriesFile = Path.GetRelativePath(staging, Path.Combine(dataset, "series.json")).Replace('\\', '/'),
                status = "pending-analysis", analysisAvailable = true,
                note = "Vollständige Eingaben bereitgestellt. Lokale Iro-Analyse verfügbar; Ergebnisse werden getrennt unter tests/runs gespeichert."
            };
            File.WriteAllText(Path.Combine(staging, "request.json"), JsonSerializer.Serialize(request, SceneExport.JsonOptions));
            token.ThrowIfCancellationRequested();
            Directory.Move(staging, destination);
            return new(requestId, destination, batch.Items.Count);
        }
        catch { if (Directory.Exists(staging)) Directory.Delete(staging, true); throw; }
    }
}


