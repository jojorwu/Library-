using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using GitHubReleaseDownloader.Core;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using Moq.Protected;
using System.Threading;
using System.Collections.Generic;

namespace GitHubReleaseDownloader.Core.Tests
{
    [TestFixture]
    public class DownloaderTests
    {
        private Downloader _downloader;
        private Mock<IGitHubClient> _githubClientMock;
        private Mock<IReleaseClient> _releasesClientMock;
        private string _tempDirectory;
        private string _zipPath;

        [SetUp]
        public void Setup()
        {
            _githubClientMock = new Mock<IGitHubClient>();
            _releasesClientMock = new Mock<IReleaseClient>();
            _githubClientMock.Setup(c => c.Release).Returns(_releasesClientMock.Object);

            var mockMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(mockMessageHandler.Object);

            _downloader = new Downloader { GitHubClient = _githubClientMock.Object, HttpClient = httpClient };

            _tempDirectory = Path.Combine(Path.GetTempPath(), "DownloaderTests");
            Directory.CreateDirectory(_tempDirectory);

            _zipPath = Path.Combine(_tempDirectory, "asset.zip");
            using (var archive = ZipFile.Open(_zipPath, ZipArchiveMode.Create))
            {
                var entry = archive.CreateEntry("test.txt");
                using (var stream = entry.Open())
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("Hello World!");
                }
            }

            var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new ByteArrayContent(File.ReadAllBytes(_zipPath))
            };

            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
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
            var assets = await _downloader.GetReleaseAssetsAsync("https://github.com/owner/repo");

            // Assert
            Assert.That(assets.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetReleaseAssetsAsync_NoReleases_ReturnsEmptyList()
        {
            // Arrange
            _releasesClientMock.Setup(r => r.GetAll("owner", "repo")).ReturnsAsync(new List<IRelease>());

            // Act
            var assets = await _downloader.GetReleaseAssetsAsync("https://github.com/owner/repo");

            // Assert
            Assert.That(assets, Is.Empty);
        }

        [Test]
        public async Task DownloadAndExtractRelease_ValidAsset_DownloadsAndExtracts()
        {
            // Arrange
            var assetMock = new Mock<IReleaseAsset>();
            assetMock.Setup(a => a.BrowserDownloadUrl).Returns("http://example.com/asset.zip");
            assetMock.Setup(a => a.Name).Returns("asset.zip");

            // Act
            await _downloader.DownloadAndExtractRelease(assetMock.Object, _tempDirectory);

            // Assert
            var extractedFiles = Directory.GetFiles(_tempDirectory, "*", SearchOption.AllDirectories);
            Assert.That(extractedFiles.Any(f => f.EndsWith("test.txt")));
        }

        [Test]
        public void ParseRepoUrl_ValidUrl_ReturnsOwnerAndRepo()
        {
            // Act
            var (owner, repo) = _downloader.ParseRepoUrl("https://github.com/owner/repo");

            // Assert
            Assert.That(owner, Is.EqualTo("owner"));
            Assert.That(repo, Is.EqualTo("repo"));
        }
    }
}
