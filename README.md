# SVN Path Copy <img src="docs/SVNPathCopy-Icon.png" alt="StatusSwift Logo" width="30" height="30" style="vertical-align: middle;"/>

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)

A Windows Explorer context menu extension that copies SVN repository URLs to your clipboard - with or without revision information.

> **Example:** `https://your-svn.org/your-repo/file.txt?p=42`

## Features

- **Copy SVN URL** - Copy the SVN repository URL for any file or folder
- **Copy SVN URL with Revision** - Include the last change revision number (`?p=XXX`)
- **Configuration App** - Modern WPF application to manage settings
- **Customizable** - Choose which menu items to show and URL encoding style

## Installation

### Prerequisites

- Windows 10/11 (64-bit)
- .NET Framework 4.8 Runtime (included in Windows 10/11)
- .NET 10 Desktop Runtime (for Configuration App)

### Download

Download the latest installer from the [Releases](https://github.com/clFaster/SVNPathCopy/releases) page.

### Installation Options

#### GUI Installation
Double-click the MSI installer and follow the prompts.

## Usage

1. Right-click on any file or folder in an SVN working copy
2. Choose one of the following:
   - **Copy SVN URL** - Copies the URL without revision
   - **Copy SVN URL with REV** - Copies the URL with `?p=revision` appended
3. Paste the URL wherever you need it

### Configuration

Run the **SVN Path Copy Configuration** app from the Start Menu to:

- Enable/disable the context menu extension
- Show/hide specific menu items
- Change URL encoding style
- View extension status

## Building

### Requirements

- .NET 10 SDK
- .NET Framework 4.8 Developer Pack
- WiX Toolset 6.x (for installer)

### Build Steps

```powershell
# Restore and build
dotnet restore SVNPathCopy.slnx
dotnet build SVNPathCopy.slnx

# Run tests
dotnet test SVNPathCopy.slnx
```

### Building the Installer

```powershell
# Build the installer (requires WiX Toolset 6)
dotnet build src/SVNPathCopy.Installer/SVNPathCopy.Installer.wixproj
```

## Project Structure

```
SVNPathCopy/
├── src/
│   ├── SVNPathCopy.Core/           # .NET Standard 2.0 - Shared library
│   ├── SVNPathCopy.ShellExtension/ # .NET Framework 4.8 - Shell extension
│   ├── SVNPathCopy.Configuration/  # .NET 10 WPF - Configuration app
│   └── SVNPathCopy.Installer/      # WiX 6 - MSI Installer
├── tests/
│   └── SVNPathCopy.Tests/          # Unit tests (NUnit + Shouldly)
└── docs/
    └── ...                         # TBA
```

## Configuration Storage

Settings are stored in the Windows Registry:

```
HKEY_CURRENT_USER\SOFTWARE\SVNPathCopy
├── Enabled (DWORD)
├── ShowCopyWithRevision (DWORD)
├── ShowCopyWithoutRevision (DWORD)
└── UrlEncodingStyle (STRING)
```

## Troubleshooting

### Context menu doesn't appear

1. Verify the shell extension is registered (check Configuration App)
2. Restart Windows Explorer
3. Ensure the file/folder is in an SVN working copy

### "Item is not under version control" error

The selected file hasn't been added to SVN. Run `svn add <file>` first.

### "Item is scheduled for addition" error (with revision)

The file hasn't been committed yet. Commit your changes to get a revision number.

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the GPL-3.0 License - see the [LICENSE](LICENSE) file for details.