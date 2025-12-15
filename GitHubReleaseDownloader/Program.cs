using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommandLine;
using Octokit;
using System.IO;
using System.IO.Compression;

public class Options
{
    [Option('r', "repository", Required = true, HelpText = "GitHub repository URL (e.g., https://github.com/owner/repo).")]
    public string RepositoryUrl { get; set; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<Options>(args)
            .WithParsedAsync(async o =>
            {
                try
                {
                    var (owner, repo) = ParseRepoUrl(o.RepositoryUrl);

                    var github = new GitHubClient(new ProductHeaderValue("GitHubReleaseDownloader"));
                    var releases = await github.Repository.Release.GetAll(owner, repo);
                    var latestRelease = releases.FirstOrDefault();

                    if (latestRelease == null)
                    {
                        Console.WriteLine("No releases found for this repository.");
                        return;
                    }

                    var zipAsset = latestRelease.Assets.FirstOrDefault(a => a.Name.EndsWith(".zip"));

                    if (zipAsset == null)
                    {
                        Console.WriteLine("No .zip asset found in the latest release.");
                        return;
                    }

                    Console.WriteLine($"Downloading {zipAsset.Name}...");
                    var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(zipAsset.BrowserDownloadUrl);
                    response.EnsureSuccessStatusCode();

                    var zipPath = Path.Combine(Directory.GetCurrentDirectory(), zipAsset.Name);
                    using (var fs = new FileStream(zipPath, System.IO.FileMode.Create))
                    {
                        await response.Content.CopyToAsync(fs);
                    }

                    Console.WriteLine("Download complete.");

                    var extractPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(zipAsset.Name));
                    if(Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                    Console.WriteLine($"Extracted to {extractPath}");

                    File.Delete(zipPath);
                    Console.WriteLine("Cleaned up zip file.");

                }
                catch (ApiException ex)
                {
                    Console.WriteLine($"Error interacting with the GitHub API: {ex.Message}");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error downloading the release asset: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
            });
    }

    private static (string owner, string repo) ParseRepoUrl(string url)
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
