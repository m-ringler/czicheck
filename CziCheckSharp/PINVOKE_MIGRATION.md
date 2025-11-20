# CziCheckSharp P/Invoke Migration

This document describes the migration of CziCheckSharp from a Process-based wrapper around the CZICheck executable to a direct P/Invoke wrapper around the `libczicheckc.dll` native library.

## Summary of Changes

### 1. New Files

- **NativeMethods.cs**: Contains P/Invoke declarations for the native C API using `LibraryImportAttribute` (modern .NET 7+ approach).

### 2. Modified Files

#### CziChecker.cs
- **Before**: Launched `CZICheck.exe` as a separate process, parsed command-line arguments, and captured standard output/error.
- **After**: Directly calls native C API functions via P/Invoke, eliminating process overhead and improving performance.

Key changes:
- Constructor no longer requires `executablePath` - now only takes `Configuration`
- `Check()` method added (synchronous version)
- `CheckAsync()` runs the native call on a background thread
- Direct memory management using `Marshal.AllocHGlobal` and `Marshal.FreeHGlobal`
- JSON results are retrieved directly from the native library

#### Checks.cs
- **Before**: Enum with `long` base type and bit-shifted values
- **After**: Enum with `ulong` base type and explicit hex values matching the C API constants

The mapping:
```csharp
// C API constant                                  → C# Enum Value
CZICHECK_SUBBLOCK_DIR_POSITIONS (0x0001)         → SubBlockDirPositions
CZICHECK_SUBBLOCK_SEGMENT_VALID (0x0002)         → SubBlockSegmentValid
CZICHECK_CONSISTENT_SUBBLOCK_COORDINATES (0x0004)→ ConsistentSubBlockCoordinates
CZICHECK_DUPLICATE_SUBBLOCK_COORDINATES (0x0008) → DuplicateSubBlockCoordinates
CZICHECK_BENABLED_DOCUMENT (0x0010)              → BenabledDocument
CZICHECK_SAME_PIXELTYPE_PER_CHANNEL (0x0020)    → SamePixeltypePerChannel
CZICHECK_PLANES_START_AT_ZERO (0x0040)          → PlanesIndicesStartAtZero
CZICHECK_PLANES_CONSECUTIVE (0x0080)            → PlaneIndicesAreConsecutive
CZICHECK_SUBBLOCKS_HAVE_MINDEX (0x0100)         → SubblocksHaveMindex
CZICHECK_BASIC_METADATA_VALIDATION (0x0200)     → BasicMetadataValidation
CZICHECK_XML_METADATA_SCHEMA_VALIDATION (0x0400)→ XmlMetadataSchemaValidation
CZICHECK_OVERLAPPING_SCENES_LAYER0 (0x0800)     → OverlappingScenesLayer0
CZICHECK_SUBBLOCK_BITMAP_VALID (0x1000)         → SubBlockBitmapValid
CZICHECK_CONSISTENT_MINDEX (0x2000)             → ConsistentMIndex
CZICHECK_ATTACHMENT_DIR_POSITIONS (0x4000)      → AttachmentDirPositions
CZICHECK_APPLIANCE_METADATA_TOPOGRAPHY_VALID (0x8000) → ApplianceMetadataTopographyValid
```

#### Configuration.cs
- Added note that `TimeoutMilliseconds` is not directly applicable to synchronous native calls
- All other properties map directly to C API parameters

#### CziCheckSharp.csproj
- Added `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>` to support P/Invoke with `nint` types

## Usage Changes

### Before (Process-based)
```csharp
var checker = new CziChecker(
    executablePath: @"C:\path\to\CZICheck.exe",
    configuration: new Configuration 
    {
        Checks = Checks.Default,
        MaxFindings = -1
    });

var result = await checker.CheckAsync("test.czi");
```

### After (P/Invoke-based)
```csharp
var checker = new CziChecker(
    configuration: new Configuration 
    {
        Checks = Checks.Default,
        MaxFindings = -1
    });

// Synchronous
var result = checker.Check("test.czi");

// Or asynchronous
var result = await checker.CheckAsync("test.czi");
```

## Benefits

1. **Performance**: No process creation overhead, direct native calls
2. **Simplicity**: No need to distribute or locate the CZICheck executable
3. **Memory efficiency**: Direct memory management without intermediate buffers for stdout/stderr
4. **Type safety**: Strongly-typed enum values that match the C API exactly
5. **Modern .NET**: Uses `LibraryImportAttribute` (source-generated P/Invoke)

## Requirements

1. **Native Library**: The `libczicheckc.dll` (or `.so`/`.dylib` on other platforms) must be available in the application directory or in the system PATH.
2. **.NET Version**: Requires .NET 7 or later for `LibraryImportAttribute` support.
3. **Unsafe Code**: The project now uses unsafe code blocks for P/Invoke memory management.

## Native Library Location

The P/Invoke runtime will search for `libczicheckc.dll` in the following order:
1. Application directory
2. Current working directory
3. System directories (Windows: System32, etc.)
4. Directories in PATH environment variable

You can also set a custom library search path using `NativeLibrary.SetDllImportResolver` if needed.

## Breaking Changes

1. **Constructor**: `CziChecker` constructor no longer takes `executablePath` parameter
2. **Enum Names**: Some `Checks` enum values have been renamed to match C API constants exactly
3. **Dependencies**: Now requires `libczicheckc.dll` instead of `CZICheck.exe`

## Migration Guide

To migrate existing code:

1. Remove `executablePath` parameter from `CziChecker` constructor
2. Update any `Checks` enum references to new names (see mapping above)
3. Ensure `libczicheckc.dll` is deployed with your application
4. Consider using synchronous `Check()` method for better performance if threading is not required

## API Reference

### NativeMethods

Internal P/Invoke declarations:
- `CreateValidator(ulong checksBitmask, int maxFindings, bool laxParsing, bool ignoreSizem)` → `nint`
- `ValidateFile(nint validator, string inputPath, nint jsonBuffer, ref ulong jsonBufferSize, nint errorMessage, ref nuint errorMessageLength)` → `int`
- `DestroyValidator(nint validator)` → `void`
- `GetLibVersion(out int major, out int minor, out int patch)` → `void`
- `GetLibVersionString(nint buffer, ref ulong size)` → `bool`

### Error Codes (ValidateFile)

- `0`: Success (validation completed, results in JSON buffer)
- `1`: JSON buffer size too small (required size written to `jsonBufferSize`)
- `2`: File access error (error details in error message)
- `3`: Invalid validator pointer
