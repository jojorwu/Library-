using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GitHubReleaseDownloader.Core;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using System.Collections.Generic;
using System.Linq;
using System;

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

        [ObservableProperty]
        private bool _isIdle = true;

        [ObservableProperty]
        private IReadOnlyList<IReleaseAsset> _assets;

        [ObservableProperty]
        private IReleaseAsset _selectedAsset;

        private readonly GitHubService _githubService;
        private readonly FileDownloaderService _fileDownloaderService;
        private readonly SettingsService _settingsService;
        private CancellationTokenSource _cancellationTokenSource;

        public MainViewModel(GitHubService githubService, FileDownloaderService fileDownloaderService, SettingsService settingsService)
        {
            _githubService = githubService;
            _fileDownloaderService = fileDownloaderService;
            _settingsService = settingsService;

            _fileDownloaderService.StatusChanged += (message) => Status = message;
            _fileDownloaderService.ProgressChanged += (progress) => Progress = progress;

            var settings = _settingsService.LoadSettings();
            RepositoryUrl = settings.RepositoryUrl;
            DestinationPath = settings.DestinationPath;

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(RepositoryUrl) || e.PropertyName == nameof(DestinationPath))
                {
                    _settingsService.SaveSettings(new Settings
                    {
                        RepositoryUrl = RepositoryUrl,
                        DestinationPath = DestinationPath
                    });
                }
            };
        }

        [RelayCommand]
        private async Task FetchAssetsAsync()
        {
            try
            {
                Assets = await _githubService.GetReleaseAssetsAsync(RepositoryUrl);
                SelectedAsset = Assets.FirstOrDefault();
            }
            catch (Exception ex)
            {
                var messageBox = MessageBoxManager.GetMessageBoxStandardWindow("Error", ex.Message, ButtonEnum.Ok, Icon.Error);
                messageBox.Show();
            }
        }

        [RelayCommand]
        private async Task DownloadAsync()
        {
            IsIdle = false;
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await _fileDownloaderService.DownloadAndExtractAsset(SelectedAsset, DestinationPath, _cancellationTokenSource.Token);
            }
            catch(Exception ex)
            {
                var messageBox = MessageBoxManager.GetMessageBoxStandardWindow("Error", ex.Message, ButtonEnum.Ok, Icon.Error);
                messageBox.Show();
            }
            IsIdle = true;
        }

        [RelayCommand]
        private void Cancel()
        {
            _cancellationTokenSource?.Cancel();
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
