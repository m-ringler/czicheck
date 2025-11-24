// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp.Tests;

using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

/// <summary>
/// This test class runs the same tests as
/// $(repo)/CZICheck/test/CZICheckRunTests.py,
/// using the C# wrapper of czi-check.
/// It also uses the same CZI sample files, downloading them if necessary.
/// </summary>
public class SampleCziTests
{
    // From $(repo)/CZICheck/CMakeLists.txt
    private static readonly string[] TestDataRepos =
    [
        "https://libczirwtestdata.z13.web.core.windows.net/CZICheckSamples/MD5/",
        "https://github.com/ptahmose/libCZI_testdata/raw/main/MD5/",
    ];

    /// <summary>
    /// Tests that we have at least one test case.
    /// </summary>
    /// <remarks>
    /// This test is useful for debugging the generation of the theory data.
    /// </remarks>
    [Fact]
    public void TheoryDataIsNotEmpty()
    {
        var testData = GetSampleCziTestData();
        _ = testData.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that the code snippets in the README compile and run without errors.
    /// </summary>
    [Fact]
    public void ReadmeCodeHasNoErrors()
    {
        var testData = EnumerateSampleCziTestData();
        
        string file = testData.First().cziFilePath;
        
        var act1 = () => ReadmeExamples.CheckAndPrintResult(file);
        _ = act1.Should().NotThrow();

        var act2 = () => ReadmeExamples.ConfigurationExample(file);
        _ = act2.Should().NotThrow();

        var act3 = ReadmeExamples.GetVersion;
        _ = act3.Should().NotThrow();
    }

    [Theory]
    [MemberData(nameof(GetSampleCziTestData))]
    public async Task RunAll(
        string cziFilePath,
        string md5Content,
        string expectedJsonContent)
    {
        // ARRANGE
        _ = expectedJsonContent;
        var md5 = md5Content.Trim();
        await Ensure(cziFilePath, md5);
        _ = GetFileMd5(cziFilePath).Should().Be(md5);

        FileResult actual;
        var config = new Configuration
        {
            LaxParsing = true,
            Checks = Checks.All,
        };

        // ACT
        using (var sut = new CziChecker(config))
        {
            actual = sut.Check(cziFilePath);
        }

        // ASSERT
        var expected = FileResultDto
            .FromJson(expectedJsonContent)
            .ToResultFor(cziFilePath);

        try
        {
            _ = actual.Should().BeEquivalentTo(expected);
        }
        catch
        {
            Console.WriteLine("==ACTUAL==");
            Console.WriteLine(JsonSerializer.Serialize(actual));
            Console.WriteLine("==EXPECTED==");
            Console.WriteLine(JsonSerializer.Serialize(expected));
            throw;
        }

        _ = actual.CheckResults
            .Where(x => x.Check == Checks.None)
            .Should().BeEmpty();
    }

    public static TheoryData<string, string, string> GetSampleCziTestData()
    {
        var data = new TheoryData<string, string, string>();
        var testData = EnumerateSampleCziTestData();
        foreach (var (cziFilePath, md5, json) in testData)
        {
            data.Add(cziFilePath, md5, json);
        }

        return data;
    }
    
    
    internal static IEnumerable<(string cziFilePath, string md5, string json)>
        EnumerateSampleCziTestData()
    {
        return GetSampleCziTestDataCore();
    }

    private static IEnumerable<(string cziFilePath, string md5, string json)>
        GetSampleCziTestDataCore(
            [CallerFilePath] string? sourceFilePath = null)
    {
        var cziCheckSamplesPath = GetTestDataFolder(sourceFilePath);

        if (!Directory.Exists(cziCheckSamplesPath))
        {
            yield break;
        }

        // Find all .czi.md5 files
        var md5Files = Directory.GetFiles(cziCheckSamplesPath, "*.czi.md5");

        foreach (var md5File in md5Files.OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase))
        {
            // Get the base name (without .czi.md5 extension)
            var baseName = Path.GetFullPath(md5File[..^".czi.md5".Length]);
            var jsonFile = baseName + ".txt.json";

            // Only add if both files exist
            if (File.Exists(jsonFile))
            {
                var cziFile = baseName + ".czi";
                var md5Content = File.ReadAllText(md5File);
                var jsonContent = File.ReadAllText(jsonFile);
                yield return(cziFile, md5Content, jsonContent);
            }
        }
    }

    private static string GetTestDataFolder(string? sourceFilePath)
    {
        var sourceDirectory = Path.GetDirectoryName(sourceFilePath) ?? string.Empty;

        // Construct the test data folder path
        var repoRoot = Path.GetFullPath(
            Path.Combine(sourceDirectory, ".."));

        var cziCheckSamplesPath =
            Path.Combine(repoRoot, "Test", "CZICheckSamples");
        return cziCheckSamplesPath;
    }

    private static async Task Ensure(string cziFilePath, string md5)
    {
        if (File.Exists(cziFilePath))
        {
            return;
        }

        // Try each base URL until one succeeds
        Exception? lastException = null;

        foreach (var baseUrl in TestDataRepos)
        {
            try
            {
                var url = $"{baseUrl}{md5}";
                using var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(1)
                };

                using var response = await httpClient.GetAsync(url);
                _ = response.EnsureSuccessStatusCode();

                // Stream directly to file
                using (var fileStream = new FileStream(
                    cziFilePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }

                return; // Success
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
                // Continue to next URL
            }
            catch (TaskCanceledException ex)
            {
                lastException = ex;
                // Continue to next URL (timeout or cancellation)
            }
        }

        // If we get here, all URLs failed
        throw new InvalidOperationException(
            $"Failed to download file from any base URL for MD5: {md5}",
            lastException);
    }

    private static string GetFileMd5(string filePath)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }
}