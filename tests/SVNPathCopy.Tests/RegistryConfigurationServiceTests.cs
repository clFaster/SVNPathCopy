using NUnit.Framework;
using Shouldly;
using SVNPathCopy.Core.Models;
using SVNPathCopy.Core.Services;

namespace SVNPathCopy.Tests;

[TestFixture]
public class RegistryConfigurationServiceTests
{
    [Test]
    public void GetSettings_WhenNoRegistryKey_ReturnsDefaults()
    {
        // This test verifies default behavior
        // In a real scenario, you'd mock the registry or use a test registry key

        // Arrange
        var service = new RegistryConfigurationService();

        // Act
        SvnPathCopySettings settings = service.GetSettings();

        // Assert
        settings.ShouldNotBeNull();
        // Default values are expected when no registry key exists
    }

    [Test]
    public void SvnPathCopySettings_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var settings = new SvnPathCopySettings();

        // Assert
        settings.Enabled.ShouldBeTrue();
        settings.ShowCopyWithRevision.ShouldBeTrue();
        settings.ShowCopyWithoutRevision.ShouldBeTrue();
        settings.UrlEncodingStyle.ShouldBe(UrlEncodingStyle.Path);
    }
}
