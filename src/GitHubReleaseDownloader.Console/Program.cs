using System;
using System.Threading.Tasks;
using CommandLine;
using GitHubReleaseDownloader.Core;

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
                var downloader = new Downloader();
                downloader.StatusChanged += Console.WriteLine;
                await downloader.DownloadAndExtractRelease(o.RepositoryUrl);
            });
    }
}
