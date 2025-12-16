# GitHub Release Downloader

This is a simple tool to download and extract the latest release from a GitHub repository. It is available as a graphical user interface (GUI) application and a command-line interface (CLI) tool.

## GUI Application

The GUI application provides a user-friendly interface for downloading and extracting releases.

### Features

*   **Download and Extract:** Downloads and extracts a selected release asset from a GitHub repository.
*   **Asset Selection:** Fetches and displays a list of available assets for the latest release.
*   **Progress Bar:** Displays the progress of the download.
*   **Custom Destination:** Allows you to select a custom destination folder for the extracted files.
*   **Persistent Settings:** Remembers the last-used repository URL and destination path.
*   **Graceful Cancellation:** Allows you to cancel the download and extraction process.
*   **UI Feedback:** Disables the UI during a download to prevent multiple operations.
*   **Error Reporting:** Displays error messages in modal dialogs.

### Usage

1.  Download the latest `GitHubReleaseDownloader.GUI.exe` from the [releases page](https://github.com/YOUR_USERNAME/YOUR_REPOSITORY/releases).
2.  Run the executable.
3.  Enter the URL of the GitHub repository and click "Fetch Assets".
4.  Select the desired asset from the dropdown menu.
5.  (Optional) Click the "Browse..." button to select a custom destination folder.
6.  Click the "Download and Install" button.
7.  (Optional) Click the "Cancel" button to stop the download.

The application will download the selected asset, extract it to the specified directory, and clean up the downloaded file.

### Building from Source

To build the GUI application from source, you will need the .NET 8.0 SDK.

1.  Clone the repository.
2.  Open a terminal in the root of the repository.
3.  Run the following command to publish the application:

```bash
dotnet publish src/GitHubReleaseDownloader.GUI -c Release --self-contained true -r win-x64 /p:PublishSingleFile=true
```

This will create a self-contained executable in the `src/GitHubReleaseDownloader.GUI/bin/Release/net8.0/win-x64/publish` directory.

## CLI Tool

The CLI tool provides a command-line interface for downloading and extracting releases.

### Usage

To use the tool, you need to provide the URL of the GitHub repository. The tool will then fetch the available assets and prompt you to select one.

```bash
dotnet run --project src/GitHubReleaseDownloader.Console -- -r <repository_url> -o <output_directory>
```

### Arguments

*   `-r`, `--repository`: **(Required)** The URL of the GitHub repository.
*   `-o`, `--output`: (Optional) The directory to extract the release to. Defaults to the current directory.

### Example

```bash
dotnet run --project src/GitHubReleaseDownloader.Console -- -r https://github.com/octokit/octokit.net -o "C:\MyAwesomeApp"
```
