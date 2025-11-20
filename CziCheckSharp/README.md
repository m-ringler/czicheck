# CziCheckSharp

A .NET wrapper for CZICheck, providing validation and checking capabilities for CZI (Carl Zeiss Image) files through a native P/Invoke interface.

## Overview

CziCheckSharp provides a type-safe C# API for validating CZI files using the native CZICheck library. It wraps the C API (`libczicheckc`) and provides:

- Structured validation results with detailed findings
- Configurable validation checks
- JSON output parsing
- Proper resource management through `IDisposable`

## Installation

```bash
dotnet add package m-ringler.CziCheckSharp
```

**Important:** This package requires the native `libczicheckc` library (DLL on Windows, SO on Linux) to be available at runtime. See [Native Library Requirements](#native-library-requirements) below.

## Quick Start

```csharp
using CziCheckSharp;

// Create a checker with default configuration
var configuration = new Configuration();
using var checker = new CziChecker(configuration);

// Validate a CZI file
var result = checker.Check("sample.czi");

// Check validation status
if (result.OverallResult == "✔")
{
    Console.WriteLine("Validation passed!");
}
else
{
    Console.WriteLine($"Validation issues found:");
    foreach (var checkerResult in result.CheckerResults)
    {
        Console.WriteLine($"  {checkerResult.CheckName}: {checkerResult.Summary}");
    }
}
```

## Configuration

Customize which checks to run and how they behave:

```csharp
var configuration = new Configuration
{
    Checks = Checks.Default,           // Or Checks.All, or specific flags
    MaxFindings = 100,                 // Limit findings per check (-1 for unlimited)
    LaxParsing = false,                // Enable tolerant CZI parsing
    IgnoreSizeM = false                // Ignore M dimension for pyramid subblocks
};

using var checker = new CziChecker(configuration);
var result = checker.Check("file.czi");
```

### Available Checks

```csharp
// Individual checks (can be combined with | operator)
Checks.HasValidSubBlockPositions
Checks.HasValidSubBlockSegments
Checks.HasConsistentSubBlockDimensions
Checks.HasNoDuplicateSubBlockCoordinates
Checks.DoesNotUseBIndex
Checks.HasOnlyOnePixelTypePerChannel
Checks.HasPlaneIndicesStartingAtZero
Checks.HasConsecutivePlaneIndices
Checks.AllSubblocksHaveMIndex
Checks.HasBasicallyValidMetadata
Checks.HasXmlSchemaValidMetadata          // Opt-in (requires XercesC)
Checks.HasNoOverlappingScenesAtScale1
Checks.HasValidSubBlockBitmaps            // Opt-in (expensive)
Checks.HasValidApplianceMetadataTopography

// Convenience flags
Checks.Default    // All checks except opt-in
Checks.All        // All available checks
Checks.OptIn      // Only the opt-in checks
```

## Results

The `CziCheckResult` class provides:

```csharp
public class CziCheckResult
{
    public string? OverallResult { get; set; }           // "✔" or "❌"
    public List<CheckerResult> CheckerResults { get; set; }
    public string? Version { get; set; }
    public string? ErrorOutput { get; set; }
}

public class CheckerResult
{
    public string? CheckName { get; set; }
    public string? Summary { get; set; }
    public List<Finding>? Findings { get; set; }
}

public class Finding
{
    public string? Severity { get; set; }
    public string? Information { get; set; }
}
```

## Native Library Requirements

This package requires the native `libczicheckc` library at runtime:

- **Windows**: `libczicheckc.dll`
- **Linux**: `libczicheckc.so`

### Deployment Options

1. **Include in your application**: Place the native library in your application's output directory
2. **System-wide installation**: Install the library in a system directory (e.g., `/usr/lib` on Linux)
3. **Custom path**: Use `NativeLibrary.SetDllImportResolver` to specify a custom location

Example of custom resolver:

```csharp
using System.Runtime.InteropServices;

NativeLibrary.SetDllImportResolver(typeof(CziChecker).Assembly, (libraryName, assembly, searchPath) =>
{
    if (libraryName == "libczicheckc")
    {
        return NativeLibrary.Load("path/to/libczicheckc.dll");
    }
    return IntPtr.Zero;
});
```

### Building the Native Library

If you need to build the native library yourself:

```bash
# Clone the repository
git clone https://github.com/ZEISS/czicheck.git
cd czicheck

# Build with CMake (example for Windows)
cmake --preset x64-Debug
cmake --build out/build/x64-Debug

# The library will be in: out/build/x64-Debug/CZICheck/capi/
```

See the [CZICheck building documentation](https://github.com/ZEISS/czicheck/blob/main/documentation/building.md) for more details.

## API Reference

### CziChecker Class

```csharp
public class CziChecker : IDisposable
{
    // Constructor
    public CziChecker(Configuration configuration);
    
    // Methods
    public CziCheckResult Check(string cziFilePath);
    public static string GetVersion();
    
    // Properties
    public Configuration Configuration { get; }
    public bool IsDisposed { get; }
}
```

### Configuration Class

```csharp
public class Configuration
{
    public Checks Checks { get; init; } = Checks.Default;
    public int MaxFindings { get; init; } = -1;
    public bool LaxParsing { get; init; } = false;
    public bool IgnoreSizeM { get; init; } = false;
}
```

## Error Handling

The library handles errors through the `ErrorOutput` property of `CziCheckResult`:

```csharp
var result = checker.Check("file.czi");

if (!string.IsNullOrEmpty(result.ErrorOutput))
{
    Console.WriteLine($"Error: {result.ErrorOutput}");
}
```

Common error scenarios:
- **File not found**: Check the file path
- **DllNotFoundException**: Native library not found (see [Native Library Requirements](#native-library-requirements))
- **Unavailable checks**: Some checks (e.g., `HasXmlSchemaValidMetadata`) may not be available if the native library wasn't compiled with required dependencies

## Version Information

Get the version of the underlying CZICheck library:

```csharp
string version = CziChecker.GetVersion();
Console.WriteLine($"CZICheck version: {version}");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/ZEISS/czicheck/blob/main/LICENSE) file for details.

## Links

- [CZICheck Repository](https://github.com/ZEISS/czicheck)
- [CZICheck Documentation](https://github.com/ZEISS/czicheck/tree/main/documentation)
- [Description of Checkers](https://github.com/ZEISS/czicheck/blob/main/documentation/description_of_checkers.md)
- [Version History](https://github.com/ZEISS/czicheck/blob/main/documentation/version-history.md)
