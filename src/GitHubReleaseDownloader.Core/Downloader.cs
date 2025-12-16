using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitHubReleaseDownloader.Core
{
    public class Downloader
    {
        public IGitHubClient GitHubClient { get; set; }
        public HttpClient HttpClient { get; set; }
        public event Action<string>? StatusChanged;
        public event Action<double>? ProgressChanged;

        public Downloader()
        {
            GitHubClient = new OctokitGitHubClient("GitHubReleaseDownloader");
            HttpClient = new HttpClient();
        }

        public async Task DownloadAndExtractRelease(string repositoryUrl, string destinationPath)
        {
            try
            {
                OnStatusChanged("Parsing repository URL...");
                var (owner, repo) = ParseRepoUrl(repositoryUrl);

                OnStatusChanged("Connecting to GitHub...");
                var releases = await GitHubClient.Release.GetAll(owner, repo);
                var latestRelease = releases.FirstOrDefault();

                if (latestRelease == null)
                {
                    OnStatusChanged("No releases found for this repository.");
                    return;
                }

                var zipAsset = latestRelease.Assets.FirstOrDefault(a => a.Name.EndsWith(".zip"));
                if (zipAsset == null)
                {
                    OnStatusChanged("No .zip asset found in the latest release.");
                    return;
                }

                OnStatusChanged($"Downloading {zipAsset.Name}...");
                using (var response = await HttpClient.GetAsync(zipAsset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength;
                    var zipPath = Path.Combine(Path.GetTempPath(), zipAsset.Name);

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(zipPath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        long totalBytesRead = 0;
                        int bytesRead;
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                            if (totalBytes.HasValue)
                            {
                                OnProgressChanged((double)totalBytesRead / totalBytes.Value);
                            }
                        }
                    }
                }
                OnStatusChanged("Download complete.");

                if (Directory.Exists(destinationPath))
                {
                    Directory.Delete(destinationPath, true);
                }

                OnStatusChanged($"Extracting to {destinationPath}...");
                ZipFile.ExtractToDirectory(Path.Combine(Path.GetTempPath(), zipAsset.Name), destinationPath);
                OnStatusChanged("Extraction complete.");

                File.Delete(Path.Combine(Path.GetTempPath(), zipAsset.Name));
                OnStatusChanged("Cleaned up temporary files.");
            }
            catch (HttpRequestException ex)
            {
                OnStatusChanged($"Error downloading the release asset: {ex.Message}");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"An unexpected error occurred: {ex.Message}");
            }
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(status);
        }

        private void OnProgressChanged(double progress)
        {
            ProgressChanged?.Invoke(progress);
        }

        public (string owner, string repo) ParseRepoUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Trim('/').Split('/');
            if (segments.Length < 2)
            {
                throw new ArgumentException("Invalid GitHub repository URL.");
            }
            return (segments[0], segments[1]);
        }
    }
}
