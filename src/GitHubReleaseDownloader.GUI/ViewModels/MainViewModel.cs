using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GitHubReleaseDownloader.Core;
using System.Threading.Tasks;

namespace GitHubReleaseDownloader.GUI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _repositoryUrl = "";

        [ObservableProperty]
        private string _status = "Ready.";

        private readonly Downloader _downloader;

        public MainViewModel()
        {
            _downloader = new Downloader();
            _downloader.StatusChanged += (message) => Status = message;
        }

        [RelayCommand]
        private async Task DownloadAsync()
        {
            await _downloader.DownloadAndExtractRelease(RepositoryUrl);
        }
    }
}
