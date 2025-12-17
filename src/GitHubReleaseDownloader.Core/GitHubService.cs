using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubReleaseDownloader.Core
{
    public class GitHubService
    {
        private readonly IGitHubClient _githubClient;

        public GitHubService(IGitHubClient githubClient)
        {
            _githubClient = githubClient;
        }

        public async Task<IReadOnlyList<IReleaseAsset>> GetReleaseAssetsAsync(string repositoryUrl)
        {
            var (owner, repo) = ParseRepoUrl(repositoryUrl);
            var releases = await _githubClient.Release.GetAll(owner, repo);
            var latestRelease = releases.FirstOrDefault();

            if (latestRelease == null)
            {
                throw new Exception("No releases found for this repository.");
            }

            return latestRelease.Assets;
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
