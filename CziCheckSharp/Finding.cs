namespace CziCheck.TestHelper;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a finding from a CZI check.
/// </summary>
public record class Finding
{
    [JsonPropertyName("severity")]
    public string? Severity { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("details")]
    public string? Details { get; set; }
}
