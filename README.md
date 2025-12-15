# GitHub Release Downloader

This is a simple tool to download and extract the latest release from a GitHub repository. It is available as a graphical user interface (GUI) application and a command-line interface (CLI) tool.

## GUI Application

The GUI application provides a user-friendly interface for downloading and extracting releases.

### Usage

1.  Download the latest `GitHubReleaseDownloader.GUI.exe` from the [releases page](https://github.com/YOUR_USERNAME/YOUR_REPOSITORY/releases).
2.  Run the executable.
3.  Enter the URL of the GitHub repository.
4.  Click the "Download and Install" button.

The application will download the latest release, extract it to a directory in the same folder as the executable, and clean up the downloaded zip file.

### Building from Source

To build the GUI application from source, you will need the .NET 8.0 SDK.

1.  Clone the repository.
2.  Navigate to the `src/GitHubReleaseDownloader.GUI` directory.
3.  Run the following command to publish the application:

```bash
dotnet publish -c Release --self-contained true -r win-x64 /p:PublishSingleFile=true
```

This will create a self-contained executable in the `bin/Release/net8.0/win-x64/publish` directory.

## CLI Tool

The CLI tool provides a command-line interface for downloading and extracting releases.

### Usage

To use the tool, you need to provide the URL of the GitHub repository.

```bash
dotnet run --project src/GitHubReleaseDownloader.Console -- -r <repository_url>
```

### Arguments

-   `-r`, `--repository`: **(Required)** The URL of the GitHub repository.

### Example

```bash
dotnet run --project src/GitHubReleaseDownloader.Console -- -r https://github.com/octokit/octokit.net
```
