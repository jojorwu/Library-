using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubReleaseDownloader.Core
{
    public interface IGitHubClient
    {
        IReleaseClient Release { get; }
    }

    public interface IReleaseClient
    {
        Task<IReadOnlyList<IRelease>> GetAll(string owner, string repo);
    }

    public interface IRelease
    {
        IReadOnlyList<IReleaseAsset> Assets { get; }
    }

    public interface IReleaseAsset
    {
        string BrowserDownloadUrl { get; }
        string Name { get; }
    }
}
