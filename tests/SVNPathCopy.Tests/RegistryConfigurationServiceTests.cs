using NUnit.Framework;
using Shouldly;
using SVNPathCopy.Core.Models;
using SVNPathCopy.Core.Services;

namespace SVNPathCopy.Tests;

[TestFixture]
public class RegistryConfigurationServiceTests
{

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
