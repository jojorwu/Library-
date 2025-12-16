using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GitHubReleaseDownloader.Core;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace GitHubReleaseDownloader.GUI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _repositoryUrl = "";

        [ObservableProperty]
        private string _destinationPath = Directory.GetCurrentDirectory();

        [ObservableProperty]
        private string _status = "Ready.";

        [ObservableProperty]
        private double _progress;

        private readonly Downloader _downloader;

        public MainViewModel()
        {
            _downloader = new Downloader();
            _downloader.StatusChanged += (message) => Status = message;
            _downloader.ProgressChanged += (progress) => Progress = progress;
        }

        [RelayCommand]
        private async Task DownloadAsync()
        {
            await _downloader.DownloadAndExtractRelease(RepositoryUrl, DestinationPath);
        }

        [RelayCommand]
        private async Task BrowseAsync()
        {
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var result = await desktop.MainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Select Destination Folder",
                    AllowMultiple = false
                });

                if (result.Count > 0)
                {
                    DestinationPath = result[0].Path.LocalPath;
                }
            }
        }
    }
}
