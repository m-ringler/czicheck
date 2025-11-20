# CziCheck .NET Wrapper

A C# wrapper library for the [ZEISS CZICheck](https://github.com/ZEISS/czicheck) command-line utility, providing an object-oriented interface for checking CZI file integrity.

## Overview

This library wraps the CZICheck tool (version 0.6.3+) and provides a type-safe, async-friendly API for validating CZI files. It supports all available checkers and provides structured access to validation results.

## Quick Start

### Basic Usage

```csharp
using CziCheckSharp;

// Default checker instance (with zscaler-safe download)
var checker = await CziChecker.GetCachedOrDownloadAsync(cancellationToken);

// Run validation with default checkers
var result = await checker.CheckAsync("sample.czi", new(), cancellationToken);

// Check if validation passed
result.Findings.Should().BeEmpty();
```
