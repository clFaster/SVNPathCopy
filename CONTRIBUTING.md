# Contributing to SVN Path Copy

Thank you for your interest in contributing to SVN Path Copy! This document provides guidelines and information for contributors.

## Code of Conduct

Please read and follow our [Code of Conduct](CODE_OF_CONDUCT.md).

## How to Contribute

### Reporting Bugs

Before creating a bug report, please check existing issues to avoid duplicates.

When reporting a bug, include:

1. **Description**: Clear description of the problem
2. **Steps to Reproduce**: Detailed steps to reproduce the issue
3. **Expected Behavior**: What you expected to happen
4. **Actual Behavior**: What actually happened
5. **Environment**: Windows version, .NET version, SVN client version
6. **Screenshots**: If applicable

### Suggesting Features

Feature requests are welcome! Please include:

1. **Use Case**: Describe the problem you're trying to solve
2. **Proposed Solution**: Your idea for the feature
3. **Alternatives**: Any alternatives you've considered

### Pull Requests

1. Fork the repository
2. Create a feature branch from `main`
3. Make your changes
4. Write or update tests as needed
5. Ensure all tests pass
6. Update documentation if needed
7. Submit a pull request

## Development Setup

### Prerequisites

- Windows 10/11
- Visual Studio 2022 (17.8+) or JetBrains Rider
- .NET 10 SDK
- .NET Framework 4.8 Developer Pack
- Git

### Getting Started

```powershell
# Clone your fork
git clone https://github.com/YOUR-USERNAME/SVNPathCopy.git
cd SVNPathCopy

# Add upstream remote
git remote add upstream https://github.com/clFaster/SVNPathCopy.git

# Install dependencies and build
dotnet restore
dotnet build
```

### Running Tests

```powershell
dotnet test
```

### Testing the Shell Extension

Shell extensions require special handling for testing:

1. Build the ShellExtension project
2. Use SharpShell's Server Manager to install/uninstall
3. Restart Explorer after registration changes

## Code Style

### C# Guidelines

- Use C# 12 features where appropriate
- Use file-scoped namespaces
- Enable nullable reference types
- Follow Microsoft's [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

### Naming Conventions

- `PascalCase` for public members, types, namespaces
- `camelCase` for local variables, parameters
- `_camelCase` for private fields
- `I` prefix for interfaces
- Descriptive names over abbreviations

### Documentation

- XML documentation for all public APIs
- Inline comments for complex logic
- Update README.md for user-facing changes

## Project Structure

```
src/
├── SVNPathCopy.Core/           # Shared library (.NET Standard 2.0)
│   ├── Interfaces/             # Service interfaces
│   ├── Models/                 # DTOs and settings
│   └── Services/               # Service implementations
│
├── SVNPathCopy.ShellExtension/ # Shell extension (.NET Framework 4.8)
│   ├── Services/               # SVN service implementation
│   └── Resources/              # Icons and images
│
├── SVNPathCopy.Configuration/  # WPF app (.NET 10)
│   ├── ViewModels/             # MVVM view models
│   ├── Converters/             # Value converters
│   └── Resources/              # App resources
│
└── SVNPathCopy.Installer/      # WiX installer
    └── *.wxs                   # WiX source files
```

## Commit Messages

Follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation only
- `style`: Code style changes
- `refactor`: Code refactoring
- `test`: Adding/updating tests
- `chore`: Maintenance tasks

Examples:
```
feat(shell): add keyboard shortcut support
fix(config): handle missing registry key gracefully
docs: update installation instructions
```

## Release Process

1. Update version numbers in all `.csproj` files
2. Update CHANGELOG in README.md
3. Create a release branch: `release/v2.x.x`
4. Build and test the installer
5. Create a GitHub release with the MSI attached
6. Merge release branch to `main`

## Questions?

Feel free to open an issue for any questions about contributing.
