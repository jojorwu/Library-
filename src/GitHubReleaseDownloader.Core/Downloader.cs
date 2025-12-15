using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Octokit;

namespace GitHubReleaseDownloader.Core
{
    public class Downloader
    {
        public event Action<string>? StatusChanged;

        public async Task DownloadAndExtractRelease(string repositoryUrl)
        {
            try
            {
                OnStatusChanged("Parsing repository URL...");
                var (owner, repo) = ParseRepoUrl(repositoryUrl);

                OnStatusChanged("Connecting to GitHub...");
                var github = new GitHubClient(new ProductHeaderValue("GitHubReleaseDownloader"));
                var releases = await github.Repository.Release.GetAll(owner, repo);
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
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(zipAsset.BrowserDownloadUrl);
                response.EnsureSuccessStatusCode();

                var zipPath = Path.Combine(Path.GetTempPath(), zipAsset.Name);
                using (var fs = new FileStream(zipPath, System.IO.FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                }
                OnStatusChanged("Download complete.");

                var extractPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(zipAsset.Name));
                if(Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }

                OnStatusChanged($"Extracting to {extractPath}...");
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                OnStatusChanged("Extraction complete.");

                File.Delete(zipPath);
                OnStatusChanged("Cleaned up temporary files.");
            }
            catch (ApiException ex)
            {
                OnStatusChanged($"Error interacting with the GitHub API: {ex.Message}");
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

        private (string owner, string repo) ParseRepoUrl(string url)
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
