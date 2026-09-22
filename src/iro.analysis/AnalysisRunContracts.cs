using Iro.Core.Analysis;

namespace Iro.Analysis;

public sealed record FieldComparison(string ExpectedFieldId, string? DetectedFieldId, double? IntersectionOverUnion,
    double? NominalDeltaE00, double? ActualDeltaE00, double? DifferenceFromNominal, string Status);
public sealed record CaptureAnalysis(string CaptureId, string ImageSha256, string? Error, ImageAnalysis? Analysis,
    IReadOnlyList<FieldComparison> Comparisons, int UnexpectedDetectedFields);
public sealed record AnalysisRun(int FormatVersion, string Kind, string RunId, string RequestId, string AnalyzerVersion,
    string CreatedAtUtc, AnalysisOptions Options, string Status, int RequestedCount, int ProcessedCount,
    int ImagesWithMeasurements, int MeasuredFields, string ComparisonBasis, IReadOnlyList<CaptureAnalysis> Captures);
public sealed record AnalysisRunProgress(int Completed, int Total, string CaptureId);
public sealed record SavedAnalysisRun(AnalysisRun Report, string ResultFile);
