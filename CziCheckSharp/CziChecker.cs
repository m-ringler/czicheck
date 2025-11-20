namespace CziCheck.TestHelper;

using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

/// <summary>
/// Wrapper class for the CZICheck native library (libczicheckc).
/// Provides an object-oriented interface for checking CZI file integrity via P/Invoke.
/// </summary>
public class CziChecker
{
    private readonly Configuration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="CziChecker"/> class.
    /// </summary>
    /// <param name="configuration">Configuration options for CZI validation.</param>
    public CziChecker(Configuration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Gets the version information of the CZICheck library.
    /// </summary>
    /// <returns>Version string from CZICheck.</returns>
    public string GetVersion()
    {
        try
        {
            // Try to get the version string
            ulong size = 0;
            NativeMethods.GetLibVersionString(nint.Zero, ref size);
            
            if (size > 0)
            {
                nint buffer = Marshal.AllocHGlobal((int)size);
                try
                {
                    if (NativeMethods.GetLibVersionString(buffer, ref size))
                    {
                        return Marshal.PtrToStringUTF8(buffer) ?? "Unknown version";
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }

            // Fallback to numeric version
            NativeMethods.GetLibVersion(out int major, out int minor, out int patch);
            return $"{major}.{minor}.{patch}";
        }
        catch
        {
            return "Unknown version";
        }
    }

    /// <summary>
    /// Gets the version information of the CZICheck library asynchronously.
    /// </summary>
    /// <returns>Version string from CZICheck.</returns>
    public Task<string> GetVersionAsync()
    {
        return Task.FromResult(GetVersion());
    }

    /// <summary>
    /// Checks a CZI file with the specified options.
    /// </summary>
    /// <param name="cziFilePath">Path to the CZI file to check.</param>
    /// <param name="cancellationToken">Cancellation token (not used in native implementation).</param>
    /// <returns>Result of the check operation.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="cziFilePath"/> is null or empty.
    /// </exception>
    public CziCheckResult Check(string cziFilePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cziFilePath))
        {
            throw new ArgumentException(
                "CZI file path must be specified.",
                nameof(cziFilePath));
        }

        // Create validator
        nint validator = NativeMethods.CreateValidator(
            (ulong)_configuration.Checks,
            _configuration.MaxFindings,
            _configuration.LaxParsing,
            _configuration.IgnoreSizeM);

        if (validator == nint.Zero)
        {
            throw new InvalidOperationException("Failed to create validator. Invalid configuration parameters.");
        }

        try
        {
            // First call to get required buffer size
            ulong jsonBufferSize = 0;
            nuint errorMessageLength = 0;
            int result = NativeMethods.ValidateFile(
                validator,
                cziFilePath,
                nint.Zero,
                ref jsonBufferSize,
                nint.Zero,
                ref errorMessageLength);

            if (result == 3)
            {
                throw new InvalidOperationException("Invalid validator pointer.");
            }

            // Allocate buffers
            nint jsonBuffer = Marshal.AllocHGlobal((int)jsonBufferSize);
            nint errorBuffer = errorMessageLength > 0 
                ? Marshal.AllocHGlobal((int)errorMessageLength) 
                : nint.Zero;

            try
            {
                // Second call with allocated buffers
                result = NativeMethods.ValidateFile(
                    validator,
                    cziFilePath,
                    jsonBuffer,
                    ref jsonBufferSize,
                    errorBuffer,
                    ref errorMessageLength);

                string? jsonOutput = null;
                string? errorOutput = null;

                if (jsonBufferSize > 0)
                {
                    jsonOutput = Marshal.PtrToStringUTF8(jsonBuffer);
                }

                if (errorMessageLength > 0 && errorBuffer != nint.Zero)
                {
                    errorOutput = Marshal.PtrToStringUTF8(errorBuffer);
                }

                // Handle different result codes
                return result switch
                {
                    0 => ParseJsonOutput(jsonOutput, errorOutput),
                    2 => new CziCheckResult 
                    { 
                        ErrorOutput = errorOutput ?? "File access error: Could not open or read the CZI file." 
                    },
                    _ => new CziCheckResult 
                    { 
                        ErrorOutput = $"Validation failed with error code {result}. {errorOutput}" 
                    }
                };
            }
            finally
            {
                if (jsonBuffer != nint.Zero)
                    Marshal.FreeHGlobal(jsonBuffer);
                if (errorBuffer != nint.Zero)
                    Marshal.FreeHGlobal(errorBuffer);
            }
        }
        finally
        {
            NativeMethods.DestroyValidator(validator);
        }
    }

    /// <summary>
    /// Checks a CZI file with the specified options asynchronously.
    /// </summary>
    /// <param name="cziFilePath">Path to the CZI file to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the check operation.</returns>
    public Task<CziCheckResult> CheckAsync(
        string cziFilePath,
        CancellationToken cancellationToken = default)
    {
        // Run the check operation on a background thread to avoid blocking
        return Task.Run(() => Check(cziFilePath, cancellationToken), cancellationToken);
    }

    private static CziCheckResult ParseJsonOutput(
        string? jsonOutput,
        string? errorOutput)
    {
        if (string.IsNullOrWhiteSpace(jsonOutput))
        {
            return new CziCheckResult { ErrorOutput = errorOutput ?? "No output received from validation." };
        }

        try
        {
            var output = JsonSerializer.Deserialize<CziCheckJsonOutput>(jsonOutput);
            return output == null
                ? new CziCheckResult { ErrorOutput = errorOutput }
                : new CziCheckResult
                {
                    OverallResult = output.OverallResult,
                    CheckerResults = output.Tests ?? [],
                    Version = output.OutputVersion?.Version,
                    ErrorOutput = errorOutput
                };
        }
        catch (JsonException ex)
        {
            return new CziCheckResult 
            { 
                ErrorOutput = $"Failed to parse JSON output: {ex.Message}\nRaw output: {jsonOutput}" 
            };
        }
    }
}
