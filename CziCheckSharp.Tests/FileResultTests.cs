// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp.Tests;

using System.Collections.Immutable;
using System.Security.Cryptography;

/// <summary>
/// Tests for <see cref="FileResult"/>.
/// </summary>
public class FileResultTests
{
    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        var file = "test.czi";
        var fileStatus = CheckStatus.Ok;
        var checkResults = ImmutableArray<CheckResult>.Empty;

        // Act
        var result = new FileResult(file, fileStatus, checkResults);

        // Assert
        _ = result.File.Should().Be(file);
        _ = result.FileStatus.Should().Be(fileStatus);
        _ = result.CheckResults.Equals(checkResults).Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithCheckResults_StoresCheckResults()
    {
        // Arrange
        var checkResults = ImmutableArray.Create(
            new CheckResult(Checks.HasValidSubBlockPositions, "Check 1", CheckStatus.Ok, ImmutableArray<Finding>.Empty),
            new CheckResult(Checks.HasValidSubBlockSegments, "Check 2", CheckStatus.Warn, ImmutableArray<Finding>.Empty));

        // Act
        var result = new FileResult(
            "sample.czi",
            CheckStatus.Warn,
            checkResults);

        // Assert
        _ = result.CheckResults.Equals(checkResults).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithSameInstance_ReturnsTrue()
    {
        // Arrange
        var checkResults = ImmutableArray.Create(
            new CheckResult(Checks.HasValidSubBlockPositions, "Check", CheckStatus.Ok, ImmutableArray<Finding>.Empty));

        var result1 = new FileResult(
            "test.czi",
            CheckStatus.Ok,
            checkResults);

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
        var checkResults = ImmutableArray.Create(
            new CheckResult(Checks.HasValidSubBlockPositions, "Check", CheckStatus.Ok, ImmutableArray<Finding>.Empty));

        var result1 = new FileResult(
            "test.czi",
            CheckStatus.Ok,
            checkResults);

        var result2 = result1 with { CheckResults = [.. checkResults] };

        // Act & Assert
        _ = result1.Should().Be(result2);
        _ = result1.Equals(result2).Should().BeTrue();
        _ = (result1 == result2).Should().BeTrue();
        _ = result1.GetHashCode().Should().Be(result2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentFile_ReturnsFalse()
    {
        // Arrange
        var checkResults = ImmutableArray<CheckResult>.Empty;

        var result1 = new FileResult(
            "test1.czi",
            CheckStatus.Ok,
            checkResults);

        var result2 = new FileResult(
            "test2.czi",
            CheckStatus.Ok,
            checkResults);

        // Act & Assert
        _ = result1.Should().NotBe(result2);
        _ = result1.Equals(result2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentFileStatus_ReturnsFalse()
    {
        // Arrange
        var checkResults = ImmutableArray<CheckResult>.Empty;

        var result1 = new FileResult(
            "test.czi",
            CheckStatus.Ok,
            checkResults);

        var result2 = new FileResult(
            "test.czi",
            CheckStatus.Fail,
            checkResults);

        // Act & Assert
        _ = result1.Should().NotBe(result2);
    }

    [Fact]
    public void Equals_WithDifferentCheckResults_ReturnsFalse()
    {
        // Arrange
        var checkResults1 = ImmutableArray.Create(
            new CheckResult(Checks.HasValidSubBlockPositions, "Check", CheckStatus.Ok, ImmutableArray<Finding>.Empty));

        var checkResults2 = ImmutableArray.Create(
            new CheckResult(Checks.HasValidSubBlockSegments, "Check", CheckStatus.Ok, ImmutableArray<Finding>.Empty));

        var result1 = new FileResult(
            "test.czi",
            CheckStatus.Ok,
            checkResults1);

        var result2 = new FileResult(
            "test.czi",
            CheckStatus.Ok,
            checkResults2);

        // Act & Assert
        _ = result1.Should().NotBe(result2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var result = new FileResult(
            "test.czi",
            CheckStatus.Ok,
            ImmutableArray<CheckResult>.Empty);

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
        var result = new FileResult(
            "test.czi",
            status,
            ImmutableArray<CheckResult>.Empty);

        // Assert
        _ = result.FileStatus.Should().Be(status);
    }

    [Fact]
    public void Constructor_WithEmptyFile_CreatesInstance()
    {
        // Arrange & Act
        var result = new FileResult(
            string.Empty,
            CheckStatus.Ok,
            ImmutableArray<CheckResult>.Empty);

        // Assert
        _ = result.File.Should().BeEmpty();
    }

    [Fact]
    public void Indexer_ReturnsCorrectCheckResults()
    {
        // Arrange
        var checkResults = ImmutableArray.Create(
            new CheckResult(Checks.HasValidSubBlockPositions, "Check 1", CheckStatus.Ok, ImmutableArray<Finding>.Empty),
            new CheckResult(Checks.HasValidSubBlockSegments, "Check 2", CheckStatus.Warn, ImmutableArray<Finding>.Empty),
            new CheckResult(Checks.HasConsistentSubBlockDimensions, "Check 3", CheckStatus.Ok, ImmutableArray<Finding>.Empty),
            new CheckResult(Checks.HasConsistentSubBlockDimensions, "Check 4", CheckStatus.Fail, ImmutableArray<Finding>.Empty));

        var result = new FileResult(
            "test.czi",
            CheckStatus.Warn,
            checkResults);

        // Act
        var okResults = result[CheckStatus.Ok].ToList();
        var warnResults = result[CheckStatus.Warn].ToList();
        var failResults = result[CheckStatus.Fail].ToList();

        // Assert
        _ = okResults.Should().Equal([checkResults[0], checkResults[2]]);
        _ = warnResults.Should().Equal([checkResults[1]]);
        _ = failResults.Should().Equal([checkResults[3]]);
    }

    [Fact]
    public void Indexer_WithNoMatchingStatus_ReturnsEmpty()
    {
        // Arrange
        var result = new FileResult(
            "test.czi",
            CheckStatus.Ok,
            []);

        // Act
        var failResults = result[CheckStatus.Fail].ToList();

        // Assert
        _ = failResults.Should().BeEmpty();
    }

    [Fact]
    public void ParseFromJson_ReturnsExpected()
    {
        // Arrange
        var testDataQuery =
            from triple in SampleCziTests.EnumerateSampleCziTestData()
            where triple.cziFilePath.EndsWith("edf-superfluous-missing-channel-subblock.czi")
            select triple;
        var testData = testDataQuery.First();

        // Act
        FileResult actual = FileResultDto
            .FromJson(testData.json)
            .ToResultFor(testData.cziFilePath);

        // Assert
        AssertionConfiguration.Current.Formatting.MaxLines = 10000;
        FileResult expected = new FileResult(testData.cziFilePath, CheckStatus.Fail,
        [
            new CheckResult(
                Checks.HasValidSubBlockPositions,
                "SubBlock-Segment in SubBlockDirectory within file",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.HasValidSubBlockSegments,
                "SubBlock-Segments in SubBlockDirectory are valid",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.HasConsistentSubBlockDimensions,
                "Check subblock's coordinates for 'consistent dimensions'",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.HasNoDuplicateSubBlockCoordinates,
                "Check subblock's coordinates being unique",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.DoesNotUseBIndex,
                "Check whether the document uses the deprecated 'B-index'",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.HasOnlyOnePixelTypePerChannel,
                "Check that the subblocks of a channel have the same pixeltype",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.HasPlaneIndicesStartingAtZero,
                "Check that planes indices start at 0",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.HasConsecutivePlaneIndices,
                "Check that planes have consecutive indices",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.AllSubblocksHaveMIndex,
                "Check if all subblocks have the M index",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.HasBasicallyValidMetadata,
                "Basic semantic checks of the XML-metadata",
                CheckStatus.Warn,
            [
                new Finding(
                    FindingSeverity.Warning,
                    "document statistics gives 1 channels, whereas in XML-metadata 2 channels are found.",
                    ""),
                new Finding(
                    FindingSeverity.Info,
                    "No sub block-information found for channel index 1, metadata pixelType: gray32float",
                    ""),
                new Finding(
                    FindingSeverity.Warning,
                    "No valid ComponentBitCount information found in metadata for channel #1.",
                    "")
            ]),
            new CheckResult(
                Checks.HasXmlSchemaValidMetadata,
                "Validate the XML-metadata against XSD-schema",
                CheckStatus.Fail,
            [
                new Finding(
                    FindingSeverity.Error,
                    "(120,22): no declaration found for element 'RotationCenter'",
                    ""),
                new Finding(
                    FindingSeverity.Error,
                    "(126,15): element 'RotationCenter' is not allowed for content model 'All(SessionMatrix?,HolderZeissName?,HolderZeissId?,HolderCwsId?,SessionCount?,SessionRotationAtStart?,CustomAttributes?)'",
                    "")
            ]),
            new CheckResult(
                Checks.HasNoOverlappingScenesAtScale1,
                "Check if subblocks at pyramid-layer 0 of different scenes are overlapping",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.HasValidSubBlockBitmaps,
                "SubBlock-Segments in SubBlockDirectory are valid and valid content",
                CheckStatus.Ok,
                []),
            new CheckResult(
                Checks.HasValidApplianceMetadataTopography,
                "Basic semantic checks for TopographyDataItems",
                CheckStatus.Fail,
            [
                new Finding(
                    FindingSeverity.Warning,
                    "There are superfluous dimensions specified in the TopographyDataItems. This might yield errors.",
                    ""),
                new Finding(
                    FindingSeverity.Error,
                    "The Topography metadata specifies channels for the texture or heightmap subblocks, that are not present in the Subblock Collection of the image.",
                    "")
            ])
        ]);
    
        _ = actual.Should().BeEquivalentTo(
            expected,
            cfg => cfg.WithStrictOrdering());
        _ = actual.Should().Be(expected);
    }
}