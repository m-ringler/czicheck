namespace CziCheckSharp.Tests;

/// <summary>
/// Tests for <see cref="CziChecker"/>.
/// </summary>
public class CziCheckerTests
{
    [Fact]
    public void Foo()
    {
    }

    [Fact]
    public void GetVersion_ReturnsExpected()
    {
        Assert.Equal("0.6.5", CziChecker.GetVersion());
    }
}
