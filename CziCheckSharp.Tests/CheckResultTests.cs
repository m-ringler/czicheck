// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp.Tests;

using System.Collections.Immutable;

/// <summary>
/// Tests for <see cref="CheckResult"/>.
/// </summary>
public class CheckResultTests
{
    [Fact]
    public void Constructor_WithValidSingleCheck_CreatesInstance()
    {
        // Arrange
        var check = Checks.HasValidSubBlockPositions;
        var description = "Test check";
        var status = CheckStatus.Ok;
        var findings = ImmutableArray<Finding>.Empty;

        // Act
        var result = new CheckResult(check, description, status, findings);

        // Assert
        _ = result.Check.Should().Be(check);
        _ = result.CheckDescription.Should().Be(description);
        _ = result.Status.Should().Be(status);
        _ = result.Findings.Equals(findings).Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithChecksNone_CreatesInstance()
    {
        // Arrange
        var check = Checks.None;
        var description = "No check";
        var status = CheckStatus.Ok;
        var findings = ImmutableArray<Finding>.Empty;

        // Act
        var result = new CheckResult(check, description, status, findings);

        // Assert
        _ = result.Check.Should().Be(Checks.None);
    }

    [Fact]
    public void Constructor_WithMultipleChecksFlags_ThrowsArgumentException()
    {
        // Arrange
        var check = Checks.HasValidSubBlockPositions | Checks.HasValidSubBlockSegments;
        var description = "Test check";
        var status = CheckStatus.Ok;
        var findings = ImmutableArray<Finding>.Empty;

        // Act
        var act = () => new CheckResult(check, description, status, findings);

        // Assert
        _ = act.Should().Throw<ArgumentException>()
            .WithMessage("check must be None or a single check, but HasValidSubBlockPositions, HasValidSubBlockSegments is more than one check.*");
    }

    [Fact]
    public void Constructor_WithFindings_StoresFindings()
    {
        // Arrange
        var findings = ImmutableArray.Create(
            new Finding(FindingSeverity.Warning, "Warning 1", "Details 1"),
            new Finding(FindingSeverity.Error, "Error 1", "Details 2"));

        // Act
        var result = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Warn,
            findings);

        // Assert
        _ = result.Findings.Equals(findings).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithSameInstance_ReturnsTrue()
    {
        // Arrange
        var findings = ImmutableArray.Create(
            new Finding(FindingSeverity.Info, "Info", "Details"));

        var result1 = new CheckResult(
            Checks.AllSubblocksHaveMIndex,
            "Test check",
            CheckStatus.Ok,
            findings);

        var result2 = result1;

        // Act & Assert
        _ = result1.Should().Be(result2);
        _ = result1.Equals(result2).Should().BeTrue();
        _ = (result1 == result2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithSameValues_ReturnsTrue()
    {
        // Arrange
        var findings = ImmutableArray.Create(
            new Finding(FindingSeverity.Info, "Info", "Details"));

        var result1 = new CheckResult(
            Checks.HasValidApplianceMetadataTopography,
            "Test check",
            CheckStatus.Warn,
            findings);

        var result2 = result1 with { Findings = [..findings] };

        // Act & Assert
        _ = result1.Should().Be(result2);
        _ = result1.Equals(result2).Should().BeTrue();
        _ = (result1 == result2).Should().BeTrue();
        _ = result1.GetHashCode().Should().Be(result2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentCheck_ReturnsFalse()
    {
        // Arrange
        var findings = ImmutableArray<Finding>.Empty;

        var result1 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Ok,
            findings);

        var result2 = new CheckResult(
            Checks.HasValidSubBlockSegments,
            "Test check",
            CheckStatus.Ok,
            findings);

        // Act & Assert
        _ = result1.Should().NotBe(result2);
        _ = result1.Equals(result2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentDescription_ReturnsFalse()
    {
        // Arrange
        var findings = ImmutableArray<Finding>.Empty;

        var result1 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check 1",
            CheckStatus.Ok,
            findings);

        var result2 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check 2",
            CheckStatus.Ok,
            findings);

        // Act & Assert
        _ = result1.Should().NotBe(result2);
    }

    [Fact]
    public void Equals_WithDifferentStatus_ReturnsFalse()
    {
        // Arrange
        var findings = ImmutableArray<Finding>.Empty;

        var result1 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Ok,
            findings);

        var result2 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Fail,
            findings);

        // Act & Assert
        _ = result1.Should().NotBe(result2);
    }

    [Fact]
    public void Equals_WithDifferentFindings_ReturnsFalse()
    {
        // Arrange
        var findings1 = ImmutableArray.Create(
            new Finding(FindingSeverity.Info, "Info", "Details"));

        var findings2 = ImmutableArray.Create(
            new Finding(FindingSeverity.Warning, "Warning", "Details"));

        var result1 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Ok,
            findings1);

        var result2 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Ok,
            findings2);

        // Act & Assert
        _ = result1.Should().NotBe(result2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var result = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Ok,
            ImmutableArray<Finding>.Empty);

        // Act & Assert
        _ = result.Equals(null).Should().BeFalse();
    }

    [Theory]
    [InlineData(CheckStatus.Ok)]
    [InlineData(CheckStatus.Warn)]
    [InlineData(CheckStatus.Fail)]
    public void Constructor_WithAllCheckStatuses_CreatesInstance(CheckStatus status)
    {
        // Arrange & Act
        var result = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            status,
            ImmutableArray<Finding>.Empty);

        // Assert
        _ = result.Status.Should().Be(status);
    }

    [Fact]
    public void Constructor_WithEmptyDescription_CreatesInstance()
    {
        // Arrange & Act
        var result = new CheckResult(
            Checks.HasValidSubBlockPositions,
            string.Empty,
            CheckStatus.Ok,
            ImmutableArray<Finding>.Empty);

        // Assert
        _ = result.CheckDescription.Should().BeEmpty();
    }

    [Fact]
    public void Equals_WithDifferentFindingsOrder_ReturnsFalse()
    {
        // Arrange
        var findings1 = ImmutableArray.Create(
            new Finding(FindingSeverity.Info, "Info 1", "Details 1"),
            new Finding(FindingSeverity.Warning, "Warning", "Details 2"));

        var findings2 = ImmutableArray.Create(
            new Finding(FindingSeverity.Warning, "Warning", "Details 2"),
            new Finding(FindingSeverity.Info, "Info 1", "Details 1"));

        var result1 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Ok,
            findings1);

        var result2 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Ok,
            findings2);

        // Act & Assert
        _ = result1.Should().NotBe(result2);
    }

    [Fact]
    public void Equals_WithDifferentFindingsCount_ReturnsFalse()
    {
        // Arrange
        var findings1 = ImmutableArray.Create(
            new Finding(FindingSeverity.Info, "Info", "Details"));

        var findings2 = ImmutableArray.Create(
            new Finding(FindingSeverity.Info, "Info", "Details"),
            new Finding(FindingSeverity.Warning, "Warning", "Details 2"));

        var result1 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Ok,
            findings1);

        var result2 = new CheckResult(
            Checks.HasValidSubBlockPositions,
            "Test check",
            CheckStatus.Ok,
            findings2);

        // Act & Assert
        _ = result1.Should().NotBe(result2);
    }
}