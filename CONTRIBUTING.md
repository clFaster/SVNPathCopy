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
- .NET 10 SDK
- .NET Framework 4.8 Developer Pack

### Getting Started

```powershell
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

## Questions?

Feel free to open an issue for any questions about contributing.
