using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GitHubReleaseDownloader.Core;
using GitHubReleaseDownloader.GUI.ViewModels;
using GitHubReleaseDownloader.GUI.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace GitHubReleaseDownloader.GUI
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<MainViewModel>(),
                };

                desktop.ShutdownRequested += (sender, e) =>
                {
                    var mainViewModel = Services.GetRequiredService<MainViewModel>();
                    var settingsService = Services.GetRequiredService<SettingsService>();
                    settingsService.SaveSettings(new Settings
                    {
                        RepositoryUrl = mainViewModel.RepositoryUrl,
                        DestinationPath = mainViewModel.DestinationPath
                    });
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGitHubClient>(new OctokitGitHubClient("GitHubReleaseDownloader"));
            services.AddSingleton<HttpClient>();
            services.AddTransient<GitHubService>();
            services.AddTransient<FileDownloaderService>();
            services.AddSingleton<SettingsService>();
            services.AddSingleton<MainViewModel>();
        }
    }
}
