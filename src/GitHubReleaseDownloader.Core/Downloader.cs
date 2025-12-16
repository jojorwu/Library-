using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubReleaseDownloader.Core
{
    public class Downloader
    {
        public IGitHubClient GitHubClient { get; set; }
        public HttpClient HttpClient { get; set; }
        public event Action<string>? StatusChanged;
        public event Action<double>? ProgressChanged;
        public event Action<string>? ErrorOccurred;

        public Downloader()
        {
            GitHubClient = new OctokitGitHubClient("GitHubReleaseDownloader");
            HttpClient = new HttpClient();
        }

        public async Task<IReadOnlyList<IReleaseAsset>> GetReleaseAssetsAsync(string repositoryUrl)
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
                    OnErrorOccurred("No releases found for this repository.");
                    return new List<IReleaseAsset>();
                }

                return latestRelease.Assets;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"An unexpected error occurred: {ex.Message}");
                return new List<IReleaseAsset>();
            }
        }

        public async Task DownloadAndExtractRelease(IReleaseAsset asset, string destinationPath, CancellationToken cancellationToken = default)
        {
            try
            {
                OnStatusChanged($"Downloading {asset.Name}...");
                using (var response = await HttpClient.GetAsync(asset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength;
                    var zipPath = Path.Combine(Path.GetTempPath(), asset.Name);

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(zipPath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        long totalBytesRead = 0;
                        int bytesRead;
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                            totalBytesRead += bytesRead;
                            if (totalBytes.HasValue)
                            {
                                OnProgressChanged((double)totalBytesRead / totalBytes.Value);
                            }
                        }
                    }
                }
                OnStatusChanged("Download complete.");

                cancellationToken.ThrowIfCancellationRequested();

                if (Directory.Exists(destinationPath))
                {
                    Directory.Delete(destinationPath, true);
                }

                OnStatusChanged($"Extracting to {destinationPath}...");
                ZipFile.ExtractToDirectory(Path.Combine(Path.GetTempPath(), asset.Name), destinationPath);
                OnStatusChanged("Extraction complete.");

                File.Delete(Path.Combine(Path.GetTempPath(), asset.Name));
                OnStatusChanged("Cleaned up temporary files.");
            }
            catch (OperationCanceledException)
            {
                OnStatusChanged("Download canceled.");
            }
            catch (HttpRequestException ex)
            {
                OnErrorOccurred($"Error downloading the release asset: {ex.Message}");
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"An unexpected error occurred: {ex.Message}");
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

        private void OnErrorOccurred(string message)
        {
            ErrorOccurred?.Invoke(message);
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
