using NUnit.Framework;
using Shouldly;
using SVNPathCopy.Core.Models;

namespace SVNPathCopy.Tests;

[TestFixture]
public class SvnItemInfoTests
{
    [Test]
    public void SvnItemInfo_Properties_CanBeSetAndRetrieved()
    {
        // Arrange & Act
        var info = new SvnItemInfo
        {
            Uri = "https://svn.example.com/repo/file.txt",
            LastChangeRevision = 123,
            LocalPath = @"C:\Projects\repo\file.txt",
            RepositoryRoot = "https://svn.example.com/repo"
        };

        // Assert
        info.Uri.ShouldBe("https://svn.example.com/repo/file.txt");
        info.LastChangeRevision.ShouldBe(123);
        info.LocalPath.ShouldBe(@"C:\Projects\repo\file.txt");
        info.RepositoryRoot.ShouldBe("https://svn.example.com/repo");
    }

    [Test]
    public void SvnItemStatus_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var status = new SvnItemStatus();

        // Assert
        status.IsVersioned.ShouldBeFalse();
        status.HasLocalModifications.ShouldBeFalse();
        status.IsScheduledForAddition.ShouldBeFalse();
        status.ExistsInRepository.ShouldBeFalse();
        status.LocalPath.ShouldBeNull();
    }
}
