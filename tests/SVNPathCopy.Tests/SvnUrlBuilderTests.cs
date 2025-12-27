using NUnit.Framework;
using Shouldly;
using SVNPathCopy.Core.Models;
using SVNPathCopy.Core.Services;

namespace SVNPathCopy.Tests;

[TestFixture]
public class SvnUrlBuilderTests
{
    [Test]
    public void BuildUrl_WithoutRevision_ReturnsUrlOnly()
    {
        // Arrange
        var info = new SvnItemInfo
        {
            Uri = "https://svn.example.com/repo/file.txt",
            LastChangeRevision = 42,
        };

        // Act
        string result = SvnUrlBuilder.BuildUrl(info, false, UrlEncodingStyle.None);

        // Assert
        result.ShouldBe("https://svn.example.com/repo/file.txt");
    }

    [Test]
    public void BuildUrl_WithRevision_IncludesRevisionParameter()
    {
        // Arrange
        var info = new SvnItemInfo
        {
            Uri = "https://svn.example.com/repo/file.txt",
            LastChangeRevision = 42,
        };

        // Act
        string result = SvnUrlBuilder.BuildUrl(info, true, UrlEncodingStyle.None);

        // Assert
        result.ShouldBe("https://svn.example.com/repo/file.txt?p=42");
    }

    [Test]
    public void BuildUrl_WithRevisionZero_DoesNotIncludeRevision()
    {
        // Arrange
        var info = new SvnItemInfo
        {
            Uri = "https://svn.example.com/repo/file.txt",
            LastChangeRevision = 0,
        };

        // Act
        string result = SvnUrlBuilder.BuildUrl(info, true, UrlEncodingStyle.None);

        // Assert
        result.ShouldBe("https://svn.example.com/repo/file.txt");
    }

    [Test]
    public void BuildUrl_WithPathEncoding_EncodesSpaces()
    {
        // Arrange
        var info = new SvnItemInfo
        {
            Uri = "https://svn.example.com/repo/my file.txt",
            LastChangeRevision = 42,
        };

        // Act
        string result = SvnUrlBuilder.BuildUrl(info, false, UrlEncodingStyle.Path);

        // Assert
        result.ShouldContain("%20");
    }

    [Test]
    public void BuildUrl_NullUri_ThrowsArgumentException()
    {
        // Arrange
        var info = new SvnItemInfo { Uri = null, LastChangeRevision = 42 };

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            SvnUrlBuilder.BuildUrl(info, false, UrlEncodingStyle.None)
        );
    }

    [Test]
    public void BuildUrl_NullInfo_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            SvnUrlBuilder.BuildUrl(null!, false, UrlEncodingStyle.None)
        );
    }
}
