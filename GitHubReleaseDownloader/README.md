# GitHub Release Downloader

This is a simple command-line tool to download and extract the latest release from a GitHub repository.

## Prerequisites

To build and run this project, you need to have the .NET 8.0 SDK or newer installed. You can download it from the official .NET website:

[https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

## Usage

To use the tool, you need to provide the URL of the GitHub repository.

```bash
dotnet run -- -r <repository_url>
```

### Arguments

- `-r`, `--repository`: **(Required)** The URL of the GitHub repository.

## Example

```bash
dotnet run -- -r https://github.com/octokit/octokit.net
```

This command will find the latest release of the `octokit/octokit.net` repository, download the `.zip` asset, and extract it to a directory with the same name as the asset (without the `.zip` extension).

## Building

To build the project, you can run the following command from the `GitHubReleaseDownloader` directory:

```bash
dotnet build
```
