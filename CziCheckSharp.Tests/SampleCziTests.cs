namespace CziCheckSharp.Tests;

using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

public class SampleCziTests
{
    private static readonly string[] baseUrls = 
    [
        "https://libczirwtestdata.z13.web.core.windows.net/CZICheckSamples/MD5/",
        "https://github.com/ptahmose/libCZI_testdata/raw/main/MD5/",
    ];

    [Fact]
    public void DebugTheoryDataGen()
    {
        var testData = GetSampleCziTestData();
        Assert.NotEmpty(testData);
    }

    [Theory]
    [MemberData(nameof(GetSampleCziTestData))]
    public async Task RunAll(string cziFilePath, string md5Content, string expectedJsonContent)
    {
        _ = expectedJsonContent;
        await Ensure(cziFilePath, md5Content.Trim());

        CziCheckResult actual;
        using (var checker = new CziChecker(new Configuration { LaxParsing = true }))
        {
            actual = checker.Check(cziFilePath);
        }

        Console.WriteLine(JsonSerializer.Serialize(actual));
        var expected = CziCheckResult.FromJson(expectedJsonContent, actual.ErrorOutput);

        Assert.Equal(
            expected,
            actual);
    }

    public static TheoryData<string, string, string> GetSampleCziTestData()
    {
        return GetSampleCziTestDataCore();
    }

    private static TheoryData<string, string, string> GetSampleCziTestDataCore(
        [CallerFilePath] string? sourceFilePath = null)
    {
        var sourceDirectory = Path.GetDirectoryName(sourceFilePath) ?? string.Empty;
        
        // Navigate from CziCheckSharp.Tests to Test/CZICheckSamples
        var repoRoot = Path.GetFullPath(Path.Combine(sourceDirectory, ".."));
        var cziCheckSamplesPath = Path.Combine(repoRoot, "Test", "CZICheckSamples");

        var testData = new TheoryData<string, string, string>();
        
        if (!Directory.Exists(cziCheckSamplesPath))
        {
            return testData;
        }

        // Find all .czi.md5 files
        var md5Files = Directory.GetFiles(cziCheckSamplesPath, "*.czi.md5");
        
        foreach (var md5File in md5Files)
        {
            // Get the base name (without .czi.md5 extension)
            var baseName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(md5File));
            var jsonFile = Path.Combine(cziCheckSamplesPath, $"{baseName}.txt.json");
            
            // Only add if both files exist
            if (File.Exists(jsonFile))
            {
                var cziFile = Path.GetFullPath(
                    md5File.Substring(0, md5File.Length - ".md5".Length));
                var md5Content = File.ReadAllText(md5File);
                var jsonContent = File.ReadAllText(jsonFile);
                testData.Add(cziFile, md5Content, jsonContent);
            }
        }
        
        return testData;
    }

    private static async Task Ensure(string cziFilePath, string md5)
    {
        if (File.Exists(cziFilePath))
        {
            return;
        }

        // Try each base URL until one succeeds
        Exception? lastException = null;
        
        foreach (var baseUrl in baseUrls)
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
}
