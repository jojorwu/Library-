using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace GitHubReleaseDownloader.Core
{
    public class OctokitGitHubClient : IGitHubClient
    {
        private readonly GitHubClient _client;

        public OctokitGitHubClient(string productName)
        {
            _client = new GitHubClient(new ProductHeaderValue(productName));
            Release = new OctokitReleaseClient(_client.Repository.Release);
        }

        public IReleaseClient Release { get; }
    }

    public class OctokitReleaseClient : IReleaseClient
    {
        private readonly IReleasesClient _client;

        public OctokitReleaseClient(IReleasesClient client)
        {
            _client = client;
        }

        public async Task<IReadOnlyList<IRelease>> GetAll(string owner, string repo)
        {
            var releases = await _client.GetAll(owner, repo);
            return releases.Select(r => new OctokitRelease(r)).ToList();
        }
    }

    public class OctokitRelease : IRelease
    {
        private readonly Release _release;

        public OctokitRelease(Release release)
        {
            _release = release;
            Assets = _release.Assets.Select(a => new OctokitReleaseAsset(a)).ToList();
        }

        public IReadOnlyList<IReleaseAsset> Assets { get; }
    }

    public class OctokitReleaseAsset : IReleaseAsset
    {
        private readonly ReleaseAsset _asset;

        public OctokitReleaseAsset(ReleaseAsset asset)
        {
            _asset = asset;
        }

        public string BrowserDownloadUrl => _asset.BrowserDownloadUrl;
        public string Name => _asset.Name;
    }
}
