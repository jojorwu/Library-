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

namespace GitHubReleaseDownloader.Core.Tests
{
    [TestFixture]
    public class FileDownloaderServiceTests
    {
        private FileDownloaderService _fileDownloaderService;
        private string _tempDirectory;
        private string _zipPath;

        [SetUp]
        public void Setup()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(mockMessageHandler.Object);
            _fileDownloaderService = new FileDownloaderService(httpClient);

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
        public async Task DownloadAndExtractAsset_ValidAsset_DownloadsAndExtracts()
        {
            // Arrange
            var assetMock = new Mock<IReleaseAsset>();
            assetMock.Setup(a => a.BrowserDownloadUrl).Returns("http://example.com/asset.zip");
            assetMock.Setup(a => a.Name).Returns("asset.zip");

            // Act
            await _fileDownloaderService.DownloadAndExtractAsset(assetMock.Object, _tempDirectory);

            // Assert
            var extractedFiles = Directory.GetFiles(_tempDirectory, "*", SearchOption.AllDirectories);
            Assert.That(extractedFiles.Any(f => f.EndsWith("test.txt")));
        }
    }
}
