namespace CziCheck.TestHelper;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the output version information.
/// </summary>
internal record class OutputVersion
{
    [JsonPropertyName("command")]
    public string? Command { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
