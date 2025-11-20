namespace CziCheck.TestHelper;

/// <summary>
/// Represents the result of a CZI check operation.
/// </summary>
public class CziCheckResult
{
    public string? ErrorOutput { get; init; }

    public string? OverallResult { get; init; }

    public List<CheckerResult> CheckerResults { get; init; } = [];

    public IEnumerable<Finding> Findings => CheckerResults.SelectMany(cr => cr.Findings);

    public string? Version { get; init; }
}
