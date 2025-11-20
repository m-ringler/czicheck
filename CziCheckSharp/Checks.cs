namespace CziCheck.TestHelper;

/// <summary>
/// Flags enumeration specifying which CZI checks to run.
/// Maps to the CZICHECK_* bitmask constants in the native C API.
/// </summary>
/// <seealso href="https://github.com/ZEISS/czicheck/blob/main/documentation/description_of_checks.md"/>
[Flags]
public enum Checks : ulong
{
    /// <summary>
    /// No checks selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// SubBlock-Segment positions within file range (SubBlockDirectoryPositionsWithinRange).
    /// Part of default set.
    /// </summary>
    SubBlockDirPositions = 0x0001UL,

    /// <summary>
    /// SubBlock-Segments in SubBlockDirectory are valid (SubBlockDirectorySegmentValid).
    /// Part of default set.
    /// </summary>
    SubBlockSegmentValid = 0x0002UL,

    /// <summary>
    /// Check subblock's coordinates for 'consistent dimensions' (ConsistentSubBlockCoordinates).
    /// Part of default set.
    /// </summary>
    ConsistentSubBlockCoordinates = 0x0004UL,

    /// <summary>
    /// Check subblock's coordinates being unique (DuplicateSubBlockCoordinates).
    /// Part of default set.
    /// </summary>
    DuplicateSubBlockCoordinates = 0x0008UL,

    /// <summary>
    /// Check whether the document uses the deprecated 'B-index' (BenabledDocument).
    /// Part of default set.
    /// </summary>
    BenabledDocument = 0x0010UL,

    /// <summary>
    /// Check that the subblocks of a channel have the same pixel type (SamePixeltypePerChannel).
    /// Part of default set.
    /// </summary>
    SamePixeltypePerChannel = 0x0020UL,

    /// <summary>
    /// Check that planes indices start at 0 (PlanesIndicesStartAtZero).
    /// Part of default set.
    /// </summary>
    PlanesIndicesStartAtZero = 0x0040UL,

    /// <summary>
    /// Check that planes have consecutive indices (PlaneIndicesAreConsecutive).
    /// Part of default set.
    /// </summary>
    PlaneIndicesAreConsecutive = 0x0080UL,

    /// <summary>
    /// Check if all subblocks have the M index (SubblocksHaveMindex).
    /// Part of default set.
    /// </summary>
    SubblocksHaveMindex = 0x0100UL,

    /// <summary>
    /// Basic semantic checks of the XML-metadata (BasicMetadataValidation).
    /// Part of default set.
    /// </summary>
    BasicMetadataValidation = 0x0200UL,

    /// <summary>
    /// Validate the XML-metadata against XSD-schema (XmlMetadataSchemaValidation).
    /// Opt-in check (expensive).
    /// </summary>
    XmlMetadataSchemaValidation = 0x0400UL,

    /// <summary>
    /// Check if subblocks at pyramid-layer 0 of different scenes are overlapping (CCheckOverlappingScenesOnLayer0).
    /// Part of default set.
    /// </summary>
    OverlappingScenesLayer0 = 0x0800UL,

    /// <summary>
    /// SubBlock bitmap content validation (CheckSubBlockBitmapValid).
    /// Opt-in check (expensive).
    /// </summary>
    SubBlockBitmapValid = 0x1000UL,

    /// <summary>
    /// Check for consistent M-Index usage (ConsistentMIndex).
    /// Part of default set.
    /// </summary>
    ConsistentMIndex = 0x2000UL,

    /// <summary>
    /// Attachment directory positions within file range (AttachmentDirectoryPositionsWithinRange).
    /// Part of default set.
    /// </summary>
    AttachmentDirPositions = 0x4000UL,

    /// <summary>
    /// Basic semantic checks for TopographyDataItems (ApplianceMetadataTopographyItemValid).
    /// Part of default set.
    /// </summary>
    ApplianceMetadataTopographyValid = 0x8000UL,

    /// <summary>
    /// The checks that are disabled by default (expensive operations).
    /// </summary>
    OptIn = XmlMetadataSchemaValidation | SubBlockBitmapValid,

    /// <summary>
    /// All available checks.
    /// </summary>
    All = 0xFFFFUL,

    /// <summary>
    /// Default set of checks (all checks that are not flagged as opt-in).
    /// </summary>
    Default = All & (~OptIn),
}
