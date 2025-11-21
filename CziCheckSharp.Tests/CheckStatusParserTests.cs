// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp.Tests;

/// <summary>
/// Tests for <see cref="CheckStatusParser"/>.
/// </summary>
public class CheckStatusParserTests
{
    [Theory]
    [InlineData("Ok", CheckStatus.Ok)]
    [InlineData("Warn", CheckStatus.Warn)]
    [InlineData("Fail", CheckStatus.Fail)]
    public void Parse_WithValidStatus_ReturnsExpected(string input, CheckStatus expected)
    {
        // Act
        var result = CheckStatusParser.Parse(input);

        // Assert
        _ = result.Should().Be(expected);
    }

    [Theory]
    [InlineData("ok", CheckStatus.Ok)]
    [InlineData("OK", CheckStatus.Ok)]
    [InlineData("warn", CheckStatus.Warn)]
    [InlineData("WARN", CheckStatus.Warn)]
    [InlineData("fail", CheckStatus.Fail)]
    [InlineData("FAIL", CheckStatus.Fail)]
    public void Parse_WithDifferentCasing_ReturnsExpected(string input, CheckStatus expected)
    {
        // Act
        var result = CheckStatusParser.Parse(input);

        // Assert
        _ = result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Invalid")]
    [InlineData("Unknown")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Parse_WithInvalidInput_ReturnsFail(string? input)
    {
        // Act
        var result = CheckStatusParser.Parse(input);

        // Assert
        _ = result.Should().Be(CheckStatus.Fail);
    }
}