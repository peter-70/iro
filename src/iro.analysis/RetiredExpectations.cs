namespace Iro.Analysis;

// Explicitly withdrawn by the user's MASTERPLAN 6.5 cleanup decision.
// Identity-based, not inferred from a date, format version or self-declared verification.
// Original images and expectations remain evidence; this list grants no new approvals.
internal static class RetiredExpectations
{
    private static readonly HashSet<string> CaptureIds = new(StringComparer.Ordinal)
    {
        "irogen-20260923-113626-4ac69da28d474ab99f58e6b1ebe9920c",
        "irogen-20260923-113626-590160c542564604893fdcb78b46fea2",
        "irogen-20260923-113626-74247a216eab4a478aae61b10d74a6b9",
        "irogen-20260923-113626-9dc7cf1a4f284f3da3493ee8eaef55c5",
        "irogen-20260923-113626-e5764357a4864c1b8bb6a83e1f9a07a1",
        "irogen-20260923-113627-1f7a753f3bb6496183628388c53639fa",
        "irogen-20260923-113627-213490e0856947558f85e9bed5917b5c",
        "irogen-20260923-113627-50389934999c4eff87698f654cc4a0fc",
        "irogen-20260923-113627-c048084a628c483ab92e54930866cd71",
        "irogen-20260923-113627-dd3cec33db584652a3296f974af760cf",
        "irogen-20260924-091205-1332715a65b443cc80a150ad119f14b6",
        "irogen-20260924-091205-643393e38b90487681570a93623dd240",
        "irogen-20260924-091205-9cc19d0e690744329637011fe6caf27c",
        "irogen-20260924-091206-3ea9e0fbc17047c896c4b74cff1a1200",
        "irogen-20260924-091206-782b420e3a4c4ac8b58457bfc7327cff",
        "irogen-20260924-091206-9bdd7cd5c3fc4dc489c581b97f925e01",
        "irogen-20260924-091206-fecd2ab26d404cbdae664b7de1e6be46",
        "irogen-20260924-091207-a83cf77aae8e428f831e0c2d98c03ac3",
    };

    internal static bool Contains(string? captureId) => captureId != null && CaptureIds.Contains(captureId);
    internal const string Reason = "Nicht bewertet: Erwartung dieser identifizierten Aufnahme stammt aus der Zeit vor dem Reset; "
        + "ihre frühere verified-Kennzeichnung ist keine aktuelle fachliche Bestätigung (MASTERPLAN 6.5).";
}