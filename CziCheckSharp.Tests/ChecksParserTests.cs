// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp.Tests;

/// <summary>
/// Tests for <see cref="ChecksParser"/>.
/// </summary>
public class ChecksParserTests
{
    [Theory]
    [InlineData("HasValidSubBlockPositions", Checks.HasValidSubBlockPositions)]
    [InlineData("HasValidSubBlockSegments", Checks.HasValidSubBlockSegments)]
    [InlineData("HasConsistentSubBlockDimensions", Checks.HasConsistentSubBlockDimensions)]
    [InlineData("HasNoDuplicateSubBlockCoordinates", Checks.HasNoDuplicateSubBlockCoordinates)]
    [InlineData("DoesNotUseBIndex", Checks.DoesNotUseBIndex)]
    [InlineData("HasOnlyOnePixelTypePerChannel", Checks.HasOnlyOnePixelTypePerChannel)]
    [InlineData("HasPlaneIndicesStartingAtZero", Checks.HasPlaneIndicesStartingAtZero)]
    [InlineData("HasConsecutivePlaneIndices", Checks.HasConsecutivePlaneIndices)]
    [InlineData("AllSubblocksHaveMIndex", Checks.AllSubblocksHaveMIndex)]
    [InlineData("allSubblocksHaveMIndex", Checks.AllSubblocksHaveMIndex)]
    [InlineData("HasBasicallyValidMetadata", Checks.HasBasicallyValidMetadata)]
    [InlineData("HasXmlSchemaValidMetadata", Checks.HasXmlSchemaValidMetadata)]
    [InlineData("HasNoOverlappingScenesAtScale1", Checks.HasNoOverlappingScenesAtScale1)]
    [InlineData("HasValidSubBlockBitmaps", Checks.HasValidSubBlockBitmaps)]
    [InlineData("HasValidApplianceMetadataTopography", Checks.HasValidApplianceMetadataTopography)]
    [InlineData("Default", Checks.Default)]
    [InlineData("All", Checks.All)]
    [InlineData("OptIn", Checks.OptIn)]
    public void TryParse_WithEnumName_ReturnsTrue(string input, Checks expected)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(expected);
    }

    [Theory]
    [InlineData("hasvalidsubblockpositions", Checks.HasValidSubBlockPositions)]
    [InlineData("HASVALIDSUBBLOCKPOSITIONS", Checks.HasValidSubBlockPositions)]
    [InlineData("default", Checks.Default)]
    [InlineData("DEFAULT", Checks.Default)]
    [InlineData("all", Checks.All)]
    [InlineData("ALL", Checks.All)]
    public void TryParse_WithEnumNameDifferentCasing_ReturnsTrue(string input, Checks expected)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(expected);
    }

    [Theory]
    [InlineData("subblksegmentsinfile", Checks.HasValidSubBlockPositions)]
    [InlineData("subblksegmentsvalid", Checks.HasValidSubBlockSegments)]
    [InlineData("subblkdimconsistent", Checks.HasConsistentSubBlockDimensions)]
    [InlineData("subblkcoordsunique", Checks.HasNoDuplicateSubBlockCoordinates)]
    [InlineData("benabled", Checks.DoesNotUseBIndex)]
    [InlineData("samepixeltypeperchannel", Checks.HasOnlyOnePixelTypePerChannel)]
    [InlineData("planesstartindex", Checks.HasPlaneIndicesStartingAtZero)]
    [InlineData("consecutiveplaneindices", Checks.HasConsecutivePlaneIndices)]
    [InlineData("minallsubblks", Checks.AllSubblocksHaveMIndex)]
    [InlineData("basicxmlmetadata", Checks.HasBasicallyValidMetadata)]
    [InlineData("xmlmetadataschema", Checks.HasXmlSchemaValidMetadata)]
    [InlineData("overlappingscenes", Checks.HasNoOverlappingScenesAtScale1)]
    [InlineData("subblkbitmapvalid", Checks.HasValidSubBlockBitmaps)]
    [InlineData("topographymetadata", Checks.HasValidApplianceMetadataTopography)]
    public void TryParse_WithShortName_ReturnsTrue(string input, Checks expected)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(expected);
    }

    [Theory]
    [InlineData("SUBBLKSEGMENTSINFILE", Checks.HasValidSubBlockPositions)]
    [InlineData("SubBlkSegmentsInFile", Checks.HasValidSubBlockPositions)]
    [InlineData("BASICXMLMETADATA", Checks.HasBasicallyValidMetadata)]
    public void TryParse_WithShortNameDifferentCasing_ReturnsTrue(string input, Checks expected)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(expected);
    }

    [Theory]
    [InlineData("SubBlockDirectoryPositionsWithinRange", Checks.HasValidSubBlockPositions)]
    [InlineData("SubBlockDirectorySegmentValid", Checks.HasValidSubBlockSegments)]
    [InlineData("ConsistentSubBlockCoordinates", Checks.HasConsistentSubBlockDimensions)]
    [InlineData("DuplicateSubBlockCoordinates", Checks.HasNoDuplicateSubBlockCoordinates)]
    [InlineData("BenabledDocument", Checks.DoesNotUseBIndex)]
    [InlineData("PlanesIndicesStartZero", Checks.HasPlaneIndicesStartingAtZero)]
    [InlineData("PlaneIndicesAreConsecutive", Checks.HasConsecutivePlaneIndices)]
    [InlineData("SubblocksHaveMindex", Checks.AllSubblocksHaveMIndex)]
    [InlineData("BasicMetadataValidation", Checks.HasBasicallyValidMetadata)]
    [InlineData("XmlMetadataSchemaValidation", Checks.HasXmlSchemaValidMetadata)]
    [InlineData("CCheckOverlappingScenesOnLayer0", Checks.HasNoOverlappingScenesAtScale1)]
    [InlineData("CheckSubBlockBitmapValid", Checks.HasValidSubBlockBitmaps)]
    [InlineData("ApplianceMetadataTopographyItemValid", Checks.HasValidApplianceMetadataTopography)]
    public void TryParse_WithCEnumName_ReturnsTrue(string input, Checks expected)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(expected);
    }

    [Theory]
    [InlineData("subblockdirectorypositionswithinrange", Checks.HasValidSubBlockPositions)]
    [InlineData("SUBBLOCKDIRECTORYSEGMENTVALID", Checks.HasValidSubBlockSegments)]
    [InlineData("BasicMetadataValidation", Checks.HasBasicallyValidMetadata)]
    public void TryParse_WithCEnumNameDifferentCasing_ReturnsTrue(string input, Checks expected)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(expected);
    }

    [Theory]
    [InlineData("HasValidSubBlockPositions,HasValidSubBlockSegments", 
                Checks.HasValidSubBlockPositions | Checks.HasValidSubBlockSegments)]
    [InlineData("subblksegmentsinfile,subblksegmentsvalid", 
                Checks.HasValidSubBlockPositions | Checks.HasValidSubBlockSegments)]
    [InlineData("HasValidSubBlockPositions,subblksegmentsvalid,BasicMetadataValidation", 
                Checks.HasValidSubBlockPositions | Checks.HasValidSubBlockSegments | Checks.HasBasicallyValidMetadata)]
    [InlineData("all,HasValidSubBlockBitmaps", Checks.All | Checks.HasValidSubBlockBitmaps)]
    public void TryParse_WithMultipleChecks_CombinesFlags(string input, Checks expected)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(expected);
    }

    [Theory]
    [InlineData("HasValidSubBlockPositions, HasValidSubBlockSegments")]
    [InlineData("subblksegmentsinfile , subblksegmentsvalid")]
    [InlineData(" HasValidSubBlockPositions,HasValidSubBlockSegments ")]
    [InlineData("  HasValidSubBlockPositions  ,  HasValidSubBlockSegments  ")]
    public void TryParse_WithWhitespace_TrimsAndParses(string input)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(Checks.HasValidSubBlockPositions | Checks.HasValidSubBlockSegments);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void TryParse_WithNullOrWhitespace_ReturnsFalse(string? input)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeFalse();
        _ = checks.Should().Be(Checks.None);
    }

    [Theory]
    [InlineData("InvalidCheckName")]
    [InlineData("NotACheck")]
    [InlineData("Random123")]
    [InlineData("None")]
    public void TryParse_WithInvalidCheckName_ReturnsFalse(string input)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeFalse();
        _ = checks.Should().Be(Checks.None);
    }

    [Theory]
    [InlineData("HasValidSubBlockPositions,InvalidCheck")]
    [InlineData("InvalidCheck,HasValidSubBlockSegments")]
    [InlineData("all,InvalidCheck,default")]
    public void TryParse_WithOneInvalidCheckInList_ReturnsFalse(string input)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeFalse();
        _ = checks.Should().Be(Checks.None);
    }

    [Fact]
    public void TryParse_WithTrailingComma_ReturnsTrue()
    {
        // Arrange
        var input = "HasValidSubBlockPositions,";

        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(Checks.HasValidSubBlockPositions);
    }

    [Fact]
    public void TryParse_WithLeadingComma_ReturnsTrue()
    {
        // Arrange
        var input = ",HasValidSubBlockPositions";

        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(Checks.HasValidSubBlockPositions);
    }

    [Fact]
    public void TryParse_WithMultipleCommas_ReturnsTrue()
    {
        // Arrange
        var input = "HasValidSubBlockPositions,,HasValidSubBlockSegments";

        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(Checks.HasValidSubBlockPositions | Checks.HasValidSubBlockSegments);
    }

    [Theory]
    [InlineData("HasValidSubBlockPositions,HasValidSubBlockPositions", 
                Checks.HasValidSubBlockPositions)]
    [InlineData("all,all", Checks.All)]
    [InlineData("subblksegmentsinfile,subblksegmentsinfile,subblksegmentsinfile", 
                Checks.HasValidSubBlockPositions)]
    public void TryParse_WithDuplicateChecks_ReturnsExpectedFlags(string input, Checks expected)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(expected);
    }

    [Theory]
    [InlineData("HasValidSubBlockPositions,subblksegmentsinfile", 
                Checks.HasValidSubBlockPositions | Checks.HasValidSubBlockPositions)]
    [InlineData("HasValidSubBlockSegments,SubBlockDirectorySegmentValid", 
                Checks.HasValidSubBlockSegments)]
    public void TryParse_WithDifferentNamesForSameCheck_CombinesCorrectly(string input, Checks expected)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().Be(expected);
    }

    [Theory]
    [InlineData("default,HasValidSubBlockBitmaps")]
    [InlineData("Default,xmlmetadataschema,subblkbitmapvalid")]
    [InlineData("all")]
    public void TryParse_WithComplexCombinations_ReturnsExpectedFlags(string input)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().NotBe(Checks.None);
    }

    [Fact]
    public void TryParse_WithOnlyComma_ReturnsFalse()
    {
        // Arrange
        var input = ",";

        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeFalse();
        _ = checks.Should().Be(Checks.None);
    }

    [Fact]
    public void TryParse_WithOnlyCommas_ReturnsFalse()
    {
        // Arrange
        var input = ",,,";

        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeFalse();
        _ = checks.Should().Be(Checks.None);
    }

    [Theory]
    [InlineData("HasValidSubBlockPositions\nHasValidSubBlockSegments")]
    [InlineData("HasValidSubBlockPositions HasValidSubBlockSegments")]
    [InlineData("HasValidSubBlockPositions;HasValidSubBlockSegments")]
    public void TryParse_WithNonCommaSeparator_ReturnsFalse(string input)
    {
        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeFalse();
    }

    [Fact]
    public void TryParse_WithLongValidString_ReturnsTrue()
    {
        // Arrange
        var input = string.Join(",", 
            "HasValidSubBlockPositions",
            "HasValidSubBlockSegments",
            "HasConsistentSubBlockDimensions",
            "HasNoDuplicateSubBlockCoordinates",
            "DoesNotUseBIndex",
            "HasOnlyOnePixelTypePerChannel",
            "HasPlaneIndicesStartingAtZero",
            "HasConsecutivePlaneIndices",
            "AllSubblocksHaveMIndex",
            "HasBasicallyValidMetadata");

        // Act
        var result = ChecksParser.TryParse(input, out var checks);

        // Assert
        _ = result.Should().BeTrue();
        _ = checks.Should().NotBe(Checks.None);
        _ = ((checks & Checks.HasValidSubBlockPositions) != 0).Should().BeTrue();
        _ = ((checks & Checks.HasBasicallyValidMetadata) != 0).Should().BeTrue();
    }
}
