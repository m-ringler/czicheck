// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp;

/// <summary>
/// Represents the result of a CZI check operation.
/// </summary>
public class CziCheckResult
{
    public string? ErrorOutput { get; init; }

    public string? OverallResult { get; init; }

    public IReadOnlyList<CheckerResult> CheckerResults { get; init; } = [];

    public IEnumerable<Finding> Findings => this.CheckerResults.SelectMany(cr => cr.Findings);

    public string? Version { get; init; }
}
