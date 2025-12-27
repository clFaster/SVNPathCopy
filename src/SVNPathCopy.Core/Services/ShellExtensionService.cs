using System.Diagnostics;
using Microsoft.Win32;
using SVNPathCopy.Core.Interfaces;

namespace SVNPathCopy.Core.Services;

/// <summary>
///     Service for managing shell extension registration.
/// </summary>
public class ShellExtensionService : IShellExtensionService
{
    private static readonly Guid _shellExtensionClsid = new("ED4DD0F3-E4E3-4F8A-AD97-7B76FC3E0965");

    /// <inheritdoc />
    public bool IsRegistered() => IsShellExtensionComRegistered(_shellExtensionClsid);

    /// <inheritdoc />
    public bool Register()
    {
        var assemblyPath = GetAssemblyPath();
        if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
        {
            throw new FileNotFoundException(
                $"Shell extension assembly not found. Searched locations around: {AppContext.BaseDirectory}"
            );
        }

        // Use regasm to register the shell extension
        var regasmPath = GetRegAsmPath();
        if (string.IsNullOrEmpty(regasmPath))
        {
            throw new FileNotFoundException(
                "RegAsm.exe not found. .NET Framework 4.x must be installed."
            );
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = regasmPath,
            Arguments = $"\"{assemblyPath}\" /codebase",
            UseShellExecute = true,
            Verb = "runas", // Request elevation - this will show UAC prompt
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start RegAsm.exe process.");
        }

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"RegAsm.exe failed with exit code {process.ExitCode}. Make sure you accepted the UAC prompt and have administrator privileges."
            );
        }

        return true;
    }

    /// <inheritdoc />
    public bool Unregister()
    {
        var assemblyPath = GetAssemblyPath();
        if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
        {
            throw new FileNotFoundException(
                $"Shell extension assembly not found. Searched locations around: {AppContext.BaseDirectory}"
            );
        }

        // Use regasm to unregister the shell extension
        var regasmPath = GetRegAsmPath();
        if (string.IsNullOrEmpty(regasmPath))
        {
            throw new FileNotFoundException(
                "RegAsm.exe not found. .NET Framework 4.x must be installed."
            );
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = regasmPath,
            Arguments = $"\"{assemblyPath}\" /unregister",
            UseShellExecute = true,
            Verb = "runas", // Request elevation - this will show UAC prompt
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start RegAsm.exe process.");
        }

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"RegAsm.exe failed with exit code {process.ExitCode}. Make sure you accepted the UAC prompt and have administrator privileges."
            );
        }

        return true;
    }

    /// <inheritdoc />
    public string? GetAssemblyPath()
    {
        // Try to find the shell extension DLL in common locations
        var baseDir = AppContext.BaseDirectory;

        // BaseDir for Configuration app during development is typically:
        // SVNPathCopy\src\SVNPathCopy.Configuration\bin\Debug\net10.0-windows\win-x64\
        // We need to navigate to:
        // SVNPathCopy\src\SVNPathCopy.ShellExtension\bin\x64\Debug\net48\

        var candidatePaths = new[]
        {
            // Same directory (for installed/packaged version)
            Path.Combine(baseDir, "SVNPathCopy.ShellExtension.dll"),
            // Development paths from Configuration output directory
            // From: src\SVNPathCopy.Configuration\bin\Debug\net10.0-windows\win-x64\
            // To:   src\SVNPathCopy.ShellExtension\bin\x64\Debug\net48\
            Path.Combine(
                baseDir,
                "..",
                "..",
                "..",
                "..",
                "..",
                "SVNPathCopy.ShellExtension",
                "bin",
                "x64",
                "Debug",
                "net48",
                "SVNPathCopy.ShellExtension.dll"
            ),
            Path.Combine(
                baseDir,
                "..",
                "..",
                "..",
                "..",
                "..",
                "SVNPathCopy.ShellExtension",
                "bin",
                "x64",
                "Release",
                "net48",
                "SVNPathCopy.ShellExtension.dll"
            ),
            Path.Combine(
                baseDir,
                "..",
                "..",
                "..",
                "..",
                "..",
                "SVNPathCopy.ShellExtension",
                "bin",
                "Debug",
                "net48",
                "SVNPathCopy.ShellExtension.dll"
            ),
            Path.Combine(
                baseDir,
                "..",
                "..",
                "..",
                "..",
                "..",
                "SVNPathCopy.ShellExtension",
                "bin",
                "Release",
                "net48",
                "SVNPathCopy.ShellExtension.dll"
            ),
            // Alternative paths (in case of different build output structure)
            Path.Combine(baseDir, "..", "SVNPathCopy.ShellExtension.dll"),
            Path.Combine(baseDir, "..", "..", "SVNPathCopy.ShellExtension.dll"),
            Path.Combine(
                baseDir,
                "..",
                "..",
                "..",
                "SVNPathCopy.ShellExtension",
                "bin",
                "x64",
                "Debug",
                "net48",
                "SVNPathCopy.ShellExtension.dll"
            ),
            Path.Combine(
                baseDir,
                "..",
                "..",
                "..",
                "SVNPathCopy.ShellExtension",
                "bin",
                "x64",
                "Release",
                "net48",
                "SVNPathCopy.ShellExtension.dll"
            ),
            Path.Combine(
                baseDir,
                "..",
                "..",
                "..",
                "SVNPathCopy.ShellExtension",
                "bin",
                "Debug",
                "net48",
                "SVNPathCopy.ShellExtension.dll"
            ),
            Path.Combine(
                baseDir,
                "..",
                "..",
                "..",
                "SVNPathCopy.ShellExtension",
                "bin",
                "Release",
                "net48",
                "SVNPathCopy.ShellExtension.dll"
            ),
        };

        foreach (var path in candidatePaths)
        {
            try
            {
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            catch
            {
                // Skip invalid paths
            }
        }

        // Check in Program Files (for installed version)
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        var installedPaths = new[]
        {
            // Installed location: "SVN Path Copy\ShellExtension\" subdirectory
            Path.Combine(
                programFiles,
                "SVN Path Copy",
                "ShellExtension",
                "SVNPathCopy.ShellExtension.dll"
            ),
            // Fallback: same directory as config app
            Path.Combine(programFiles, "SVN Path Copy", "SVNPathCopy.ShellExtension.dll"),
        };

        foreach (var instPath in installedPaths)
        {
            if (File.Exists(instPath))
            {
                return instPath;
            }
        }

        return null;
    }

    private static string? GetRegAsmPath()
    {
        // Try to find regasm.exe for .NET Framework
        var windowsDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        var netFrameworkPaths = new[]
        {
            Path.Combine(windowsDir, @"Microsoft.NET\Framework64\v4.0.30319\regasm.exe"),
            Path.Combine(windowsDir, @"Microsoft.NET\Framework\v4.0.30319\regasm.exe"),
        };

        foreach (var path in netFrameworkPaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        return null;
    }

    private static bool IsShellExtensionComRegistered(Guid clsid)
    {
        var clsidKeyPath = $@"SOFTWARE\Classes\CLSID\{{{clsid}}}\InprocServer32";

        foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        {
            if (RegistryKeyExists(RegistryHive.LocalMachine, view, clsidKeyPath))
            {
                return true;
            }

            if (RegistryKeyExists(RegistryHive.CurrentUser, view, clsidKeyPath))
            {
                return true;
            }
        }

        return false;
    }

    private static bool RegistryKeyExists(RegistryHive hive, RegistryView view, string subKeyPath)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, view);
            using var key = baseKey.OpenSubKey(subKeyPath);
            return key is not null;
        }
        catch
        {
            return false;
        }
    }
}
