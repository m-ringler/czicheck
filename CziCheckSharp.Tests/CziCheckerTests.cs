namespace CziCheckSharp.Tests;

using AwesomeAssertions;

/// <summary>
/// Tests for <see cref="CziChecker"/>.
/// </summary>
public class CziCheckerTests
{
    [Fact]
    public void GetVersion_ReturnsExpected()
    {
        _ = CziChecker.GetVersion().Should().Be("0.6.5");
    }
}
