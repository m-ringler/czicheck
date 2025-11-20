// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp.Tests;

/// <summary>
/// Tests for <see cref="Configuration"/>.
/// </summary>
public class ConfigurationTests
{
    [Fact]
    public void Default_ReturnsConfigurationWithDefaultValues()
    {
        // Act
        var config = Configuration.Default;

        // Assert
        _ = config.Checks.Should().Be(Checks.Default);
        _ = config.MaxFindings.Should().Be(-1);
        _ = config.LaxParsing.Should().BeFalse();
        _ = config.IgnoreSizeM.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNoParameters_UsesDefaultValues()
    {
        // Act
        var config = new Configuration();

        // Assert
        _ = config.Should().Be(Configuration.Default);
    }

    [Fact]
    public void WithInitializer_CanSetMultipleProperties()
    {
        // Act
        var config = new Configuration
        {
            Checks = Checks.OptIn,
            MaxFindings = 50,
            LaxParsing = true,
            IgnoreSizeM = true
        };

        // Assert
        _ = config.Checks.Should().Be(Checks.OptIn);
        _ = config.MaxFindings.Should().Be(50);
        _ = config.LaxParsing.Should().BeTrue();
        _ = config.IgnoreSizeM.Should().BeTrue();
    }

    [Fact]
    public void Default_ReturnsSameInstance()
    {
        // Act
        var default1 = Configuration.Default;
        var default2 = Configuration.Default;

        // Assert
        _ = ReferenceEquals(default1, default2).Should().BeTrue();
    }
}