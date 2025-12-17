using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubReleaseDownloader.Core
{
    public class FileDownloaderService
    {
        private readonly HttpClient _httpClient;

        public event Action<string>? StatusChanged;
        public event Action<double>? ProgressChanged;

        public FileDownloaderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DownloadAndExtractAsset(IReleaseAsset asset, string destinationPath, CancellationToken cancellationToken = default)
        {
            try
            {
                OnStatusChanged($"Downloading {asset.Name}...");
                using (var response = await _httpClient.GetAsync(asset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength;
                    var zipPath = Path.Combine(Path.GetTempPath(), asset.Name);

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
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
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during download and extraction: {ex.Message}", ex);
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
    }
}
