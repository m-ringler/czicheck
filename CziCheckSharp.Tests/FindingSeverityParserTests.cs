// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp.Tests;

/// <summary>
/// Tests for <see cref="FindingSeverityParser"/>.
/// </summary>
public class FindingSeverityParserTests
{
    [Theory]
    [InlineData("INFO", FindingSeverity.Info)]
    [InlineData("Info", FindingSeverity.Info)]
    [InlineData("WARNING", FindingSeverity.Warning)]
    [InlineData("Warning", FindingSeverity.Warning)]
    [InlineData("warning", FindingSeverity.Warning)]
    [InlineData("warnIng", FindingSeverity.Warning)]
    [InlineData("FATaL", FindingSeverity.Error)]
    [InlineData("fatal", FindingSeverity.Error)]
    public void Parse_WithDifferentCasing_ReturnsExpected(string input, FindingSeverity expected)
    {
        // Act
        var result = FindingSeverityParser.Parse(input);

        // Assert
        _ = result.Should().Be(expected);
    }

    [Theory]
    [InlineData("error")]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData(null)]
    public void Parse_WithInvalidInput_ReturnsError(string? input)
    {
        // Act
        var result = FindingSeverityParser.Parse(input);

        // Assert
        _ = result.Should().Be(FindingSeverity.Error);
    }
}
