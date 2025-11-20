// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp;

/// <summary>
/// Configuration options for <see cref="CziChecker"/>.
/// </summary>
public sealed record class Configuration
{
    /// <summary>
    /// Gets a reusable default configuration.
    /// </summary>
    public static Configuration Default { get; } = new();

    /// <summary>
    /// Specifies which checks to run. Default is <see cref="Checks.Default"/>.
    /// </summary>
    public Checks Checks { get; init; } = Checks.Default;

    /// <summary>
    /// Specifies how many findings are to be reported (for every check).
    /// A negative number means 'no limit'. Default is -1.
    /// </summary>
    public int MaxFindings { get; init; } = -1;

    /// <summary>
    /// Specifies whether lax parsing for file opening is enabled.
    /// This option allows operation on some malformed CZIs which would
    /// otherwise not be analyzable at all. Default is false.
    /// </summary>
    public bool LaxParsing { get; init; } = false;

    /// <summary>
    /// Specifies whether to ignore the 'SizeM' field for pyramid subblocks.
    /// This option allows operation on some malformed CZIs which would
    /// otherwise not be analyzable at all. Default is false.
    /// </summary>
    public bool IgnoreSizeM { get; init; } = false;
}