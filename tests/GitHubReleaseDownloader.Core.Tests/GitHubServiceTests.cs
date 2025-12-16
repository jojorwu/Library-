using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using GitHubReleaseDownloader.Core;
using System.Collections.Generic;
using System;

namespace GitHubReleaseDownloader.Core.Tests
{
    [TestFixture]
    public class GitHubServiceTests
    {
        private GitHubService _githubService;
        private Mock<IGitHubClient> _githubClientMock;
        private Mock<IReleaseClient> _releasesClientMock;

        [SetUp]
        public void Setup()
        {
            _githubClientMock = new Mock<IGitHubClient>();
            _releasesClientMock = new Mock<IReleaseClient>();
            _githubClientMock.Setup(c => c.Release).Returns(_releasesClientMock.Object);
            _githubService = new GitHubService(_githubClientMock.Object);
        }

        [Test]
        public async Task GetReleaseAssetsAsync_ReleaseFound_ReturnsAssets()
        {
            // Arrange
            var assetMock1 = new Mock<IReleaseAsset>();
            var assetMock2 = new Mock<IReleaseAsset>();
            var releaseMock = new Mock<IRelease>();
            releaseMock.Setup(r => r.Assets).Returns(new List<IReleaseAsset> { assetMock1.Object, assetMock2.Object });
            _releasesClientMock.Setup(r => r.GetAll("owner", "repo")).ReturnsAsync(new List<IRelease> { releaseMock.Object });

            // Act
            var assets = await _githubService.GetReleaseAssetsAsync("https://github.com/owner/repo");

            // Assert
            Assert.That(assets.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetReleaseAssetsAsync_NoReleases_ThrowsException()
        {
            // Arrange
            _releasesClientMock.Setup(r => r.GetAll("owner", "repo")).ReturnsAsync(new List<IRelease>());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _githubService.GetReleaseAssetsAsync("https://github.com/owner/repo"));
        }
    }
}
