using NUnit.Framework;
using System.IO;
using GitHubReleaseDownloader.Core;

namespace GitHubReleaseDownloader.Core.Tests
{
    [TestFixture]
    public class SettingsServiceTests
    {
        private SettingsService _settingsService;
        private string _settingsFilePath;

        [SetUp]
        public void Setup()
        {
            _settingsService = new SettingsService();
            _settingsFilePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "GitHubReleaseDownloader", "settings.json");
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(_settingsFilePath))
            {
                File.Delete(_settingsFilePath);
            }
        }

        [Test]
        public void SaveAndLoadSettings_ValidSettings_SavesAndLoads()
        {
            // Arrange
            var settings = new Settings
            {
                RepositoryUrl = "https://github.com/owner/repo",
                DestinationPath = "/some/path"
            };

            // Act
            _settingsService.SaveSettings(settings);
            var loadedSettings = _settingsService.LoadSettings();

            // Assert
            Assert.That(loadedSettings.RepositoryUrl, Is.EqualTo(settings.RepositoryUrl));
            Assert.That(loadedSettings.DestinationPath, Is.EqualTo(settings.DestinationPath));
        }
    }
}
