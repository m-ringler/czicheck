// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp.Tests;

/// <summary>
/// Tests for <see cref="CziChecker"/>.
/// </summary>
public class CziCheckerTests
{
    [Fact]
    public void GetVersion_ReturnsExpected()
    {
        _ = CziChecker.GetCziCheckVersion().Should().Be("0.6.5");
    }

    [Fact]
    public void Dispose_SetsIsDisposedToTrue()
    {
        // Arrange
        var checker = new CziChecker(Configuration.Default);

        // Act
        checker.Dispose();

        // Assert
        _ = checker.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var checker = new CziChecker(Configuration.Default);

        // Act
        var act = () =>
        {
            checker.Dispose();
            checker.Dispose();
            checker.Dispose();
        };

        // Assert
        _ = act.Should().NotThrow();
    }

    [Fact]
    public void Check_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var checker = new CziChecker(Configuration.Default);
        checker.Dispose();

        // Act
        var act = () => checker.Check("somefile.czi");

        // Assert
        _ = act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void IsDisposed_BeforeDispose_ReturnsFalse()
    {
        // Arrange
        using var checker = new CziChecker(Configuration.Default);

        // Assert
        _ = checker.IsDisposed.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNullConfiguration_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new CziChecker(null!);

        // Assert
        _ = act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Configuration_ReturnsProvidedConfiguration()
    {
        // Arrange
        var config = new Configuration
        {
            Checks = Checks.All,
            MaxFindings = 100
        };

        // Act
        using var checker = new CziChecker(config);

        // Assert
        _ = checker.Configuration.Should().Be(config);
    }
}