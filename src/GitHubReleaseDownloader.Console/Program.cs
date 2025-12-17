using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommandLine;
using GitHubReleaseDownloader.Core;

public class Options
{
    [Option('r', "repository", Required = true, HelpText = "GitHub repository URL (e.g., https://github.com/owner/repo).")]
    public string RepositoryUrl { get; set; }

    [Option('o', "output", Required = false, HelpText = "The directory to extract the release to.")]
    public string OutputDirectory { get; set; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<Options>(args)
            .WithParsedAsync(async o =>
            {
                var githubService = new GitHubService(new OctokitGitHubClient("GitHubReleaseDownloader"));
                var fileDownloaderService = new FileDownloaderService(new HttpClient());

                fileDownloaderService.StatusChanged += Console.WriteLine;

                try
                {
                    var assets = await githubService.GetReleaseAssetsAsync(o.RepositoryUrl);
                    if (!assets.Any())
                    {
                        Console.WriteLine("No assets found in the latest release.");
                        return;
                    }

                    Console.WriteLine("Please select an asset to download:");
                    for (int i = 0; i < assets.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {assets[i].Name}");
                    }

                    int selection;
                    do
                    {
                        Console.Write($"Enter a number between 1 and {assets.Count}: ");
                    } while (!int.TryParse(Console.ReadLine(), out selection) || selection < 1 || selection > assets.Count);

                    var selectedAsset = assets[selection - 1];
                    var destinationPath = o.OutputDirectory ?? Environment.CurrentDirectory;
                    await fileDownloaderService.DownloadAndExtractAsset(selectedAsset, destinationPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            });
    }
}
