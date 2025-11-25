# CziCheckSharp.Tests

This project contains unit tests for the CziCheckSharp library, which provides a .NET wrapper around the native CZICheck C API.

## Overview

The tests verify that the C# wrapper correctly calls the native `libczicheckc` library and properly handles validation results. The test suite includes:

- **CziCheckerTests.cs** - Unit tests for the CziChecker class functionality
- **SampleCziTests.cs** - Integration tests that validate actual CZI files against expected results

## Prerequisites

The test project requires the native CZICheck C API library to be built before running tests.

### Native Library Location

The project is configured to copy native libraries from the path specified by the `CziCheckNativeLibraryPath` MSBuild property in `CziCheckSharp.Tests.csproj`:

**On Windows:**
```xml
<CziCheckNativeLibraryPath>..\.\out\build\x64-Debug\CZICheck\capi</CziCheckNativeLibraryPath>
```

**On other OS (Linux):**
```xml
<CziCheckNativeLibraryPath>../build/CZICheck/capi</CziCheckNativeLibraryPath>
```

This corresponds to the CMake build output directory when using the `x64-Debug` configuration.

The following files are copied to the test output directory:
- `*.dll` - Native library (Windows)
- `*.so` - Native library (Linux)
- `*.pdb` - Debug symbols (Windows)

### Building the Native Library

Follow the instructions in [the CZICheck build documentation](../documentation/building.md), using `Debug` instead of `Release`.

If you use a different CMake configuration or build directory, update the `<CziCheckNativeLibraryPath>` property in `CziCheckSharp.Tests.csproj` to point to your build output directory.

Alternatively, you can override it on the command line without modifying the `.csproj` file:

```bash
dotnet test -p:CziCheckNativeLibraryPath="..\..\out\build\x64-Release\CZICheck\capi"
```

Or when building:

```bash
dotnet build -p:CziCheckNativeLibraryPath="path\to\your\native\libs"
```

## Running Tests

```bash
dotnet test
```

## Troubleshooting

**DllNotFoundException**: If you see this error, the native library wasn't found. Ensure:
1. The native CZICheck library has been built
2. The `CziCheckNativeLibraryPath` matches your CMake build directory
3. The native library files were copied to the test output directory

**Memory Access Violations**: Ensure you're using debug symbols (`.pdb`) and that the native library version matches the C# wrapper's expectations.

**HTTP Errors**: The tests download sample CZI files over the internet. Make
sure that you have internet access and allow these requests to pass through
your firewall.