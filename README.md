# SVN Path Copy

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)

A Windows Explorer context menu extension that copies SVN repository URLs to your clipboard - with or without revision information.

> **Example:** `https://your-svn.org/your-repo/file.txt?p=42`

## Features

- 📋 **Copy SVN URL** - Copy the SVN repository URL for any file or folder
- 📋 **Copy SVN URL with Revision** - Include the last change revision number (`?p=XXX`)
- ⚙️ **Configuration App** - Modern WPF application to manage settings
- 🔧 **Enable/Disable** - Toggle the context menu extension without uninstalling
- 🎨 **Customizable** - Choose which menu items to show and URL encoding style

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

#### Silent Installation (Command Line)
```powershell
# Completely silent installation
msiexec /i SVNPathCopy-2.0.0.msi /qn

# Basic UI (progress bar only)
msiexec /i SVNPathCopy-2.0.0.msi /qb

# Silent uninstallation
msiexec /x SVNPathCopy-2.0.0.msi /qn

# Uninstall by product code (if MSI file not available)
msiexec /x {ED4DD0F3-E4E3-4F8A-AD97-7B76FC3E0966} /qn
```

### Manual Installation

If you prefer manual installation:

1. Build the solution (see [Building](#building))
2. Register the shell extension using `regsvr32` or SharpShell's Server Manager
3. Restart Windows Explorer

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

- Visual Studio 2022 17.8 or later
- .NET 10 SDK
- .NET Framework 4.8 Developer Pack
- WiX Toolset 6.x (for installer)

### Build Steps

```powershell
# Clone the repository
git clone https://github.com/clFaster/SVNPathCopy.git
cd SVNPathCopy

# Restore and build
dotnet restore SVNPathCopy.slnx
dotnet build SVNPathCopy.slnx --configuration Release

# Run tests
dotnet test SVNPathCopy.slnx
```

### Building the Installer

```powershell
# Build the installer (requires WiX Toolset 6)
dotnet build src/SVNPathCopy.Installer/SVNPathCopy.Installer.wixproj --configuration Release
```

The installer will be created at `src/SVNPathCopy.Installer/bin/Release/SVNPathCopy-2.0.0.msi`.

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
├── docs/
│   └── UPGRADE_PLAN.md             # Modernization documentation
└── SVNPathCopy/                    # Legacy project (deprecated)
```

## Dependencies

- [SharpShell](https://github.com/dwmkerr/sharpshell) - Windows Shell extension framework
- [SharpSVN](https://sharpsvn.open.collab.net/) - SVN client library for .NET
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM framework

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
2. Restart Windows Explorer: `taskkill /f /im explorer.exe && start explorer.exe`
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

## Changelog

### v2.0.0

- 🚀 Modernized to .NET 10 / .NET Framework 4.8
- ✨ New WPF Configuration application
- 📦 New WiX 5 installer (replaces deprecated .vdproj)
- 🔧 Configurable menu options and URL encoding
- 🏗️ Refactored to clean architecture with Core library
- ✅ Added unit tests
- 📚 Improved documentation

### v1.2.0

- Initial public release
- Basic context menu with SVN URL copy functionality
