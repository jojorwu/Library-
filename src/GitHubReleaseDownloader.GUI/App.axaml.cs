using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Core.IGitHubClient>(new Core.OctokitGitHubClient("GitHubReleaseDownloader"));
            services.AddSingleton<HttpClient>();
            services.AddTransient<Core.GitHubService>();
            services.AddTransient<Core.FileDownloaderService>();
            services.AddSingleton<Core.SettingsService>();
            services.AddSingleton<MainViewModel>();
        }
    }
}
