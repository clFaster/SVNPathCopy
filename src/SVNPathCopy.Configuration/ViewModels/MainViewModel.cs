using System.ComponentModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SVNPathCopy.Core.Interfaces;
using SVNPathCopy.Core.Models;
using SVNPathCopy.Core.Services;

namespace SVNPathCopy.Configuration.ViewModels;

/// <summary>
///     ViewModel for the main configuration window.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private static readonly Guid _shellExtensionClsid = new("ED4DD0F3-E4E3-4F8A-AD97-7B76FC3E0965");
    private readonly IConfigurationService _configService;
    private readonly IShellExtensionService _shellExtensionService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    [NotifyPropertyChangedFor(nameof(IsEnabled))]
    private bool _extensionEnabled = true;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private bool _isShellExtensionRegistered;

    [ObservableProperty]
    private bool _isStatusError;

    [ObservableProperty]
    private UrlEncodingStyle _selectedEncodingStyle = UrlEncodingStyle.Path;

    [ObservableProperty]
    private string _shellExtensionPath = string.Empty;

    [ObservableProperty]
    private bool _showCopyWithoutRevision = true;

    [ObservableProperty]
    private bool _showCopyWithRevision = true;

    [ObservableProperty]
    private string _statusMessage = string.Empty;
    private bool _suppressAutosave;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainViewModel" /> class.
    /// </summary>
    public MainViewModel()
    {
        _configService = new RegistryConfigurationService();
        _shellExtensionService = new ShellExtensionService();
        LoadSettings();
        CheckShellExtensionStatus();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainViewModel" /> class with a custom configuration service.
    /// </summary>
    /// <param name="configService">The configuration service to use.</param>
    public MainViewModel(IConfigurationService configService)
        : this(configService, new ShellExtensionService()) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainViewModel" /> class with custom services.
    /// </summary>
    /// <param name="configService">The configuration service to use.</param>
    /// <param name="shellExtensionService">The shell extension service to use.</param>
    public MainViewModel(
        IConfigurationService configService,
        IShellExtensionService shellExtensionService
    )
    {
        _configService = configService;
        _shellExtensionService = shellExtensionService;
        LoadSettings();
        CheckShellExtensionStatus();
    }

    public string StatusText => ExtensionEnabled ? "Enabled" : "Disabled";

    public bool IsEnabled => ExtensionEnabled;

    public IReadOnlyList<UrlEncodingStyle> EncodingStyles { get; } = Enum.GetValues<UrlEncodingStyle>().ToList().AsReadOnly();

    private void LoadSettings()
    {
        try
        {
            _suppressAutosave = true;

            var settings = _configService.GetSettings();
            ExtensionEnabled = settings.Enabled;
            ShowCopyWithRevision = settings.ShowCopyWithRevision;
            ShowCopyWithoutRevision = settings.ShowCopyWithoutRevision;
            SelectedEncodingStyle = settings.UrlEncodingStyle;
        }
        finally
        {
            _suppressAutosave = false;
        }
    }

    private void CheckShellExtensionStatus()
    {
        IsShellExtensionRegistered = IsShellExtensionComRegistered(_shellExtensionClsid);
        ShellExtensionPath = _shellExtensionService.GetAssemblyPath() ?? "Not found";
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

    partial void OnExtensionEnabledChanged(bool value) => TryAutosave();

    partial void OnShowCopyWithRevisionChanged(bool value) => TryAutosave();

    partial void OnShowCopyWithoutRevisionChanged(bool value) => TryAutosave();

    partial void OnSelectedEncodingStyleChanged(UrlEncodingStyle value) => TryAutosave();

    private void TryAutosave()
    {
        if (_suppressAutosave)
        {
            return;
        }

        SaveSettings();
    }

    [RelayCommand]
    private void SaveSettings()
    {
        try
        {
            var settings = new SvnPathCopySettings
            {
                Enabled = ExtensionEnabled,
                ShowCopyWithRevision = ShowCopyWithRevision,
                ShowCopyWithoutRevision = ShowCopyWithoutRevision,
                UrlEncodingStyle = SelectedEncodingStyle,
            };

            _configService.SaveSettings(settings);
            IsStatusError = false;
            StatusMessage = "✓ Settings saved";
            _ = ClearStatusMessageAfterDelay();
        }
        catch (Exception ex)
        {
            IsStatusError = true;
            StatusMessage = $"Error saving settings: {ex.Message}";
        }
    }

    private async Task ClearStatusMessageAfterDelay()
    {
        await Task.Delay(3000);
        StatusMessage = string.Empty;
    }

    [RelayCommand]
    private void RefreshStatus()
    {
        LoadSettings();
        CheckShellExtensionStatus();
        IsStatusError = false;
        StatusMessage = "Status refreshed.";
    }

    [RelayCommand]
    private async Task InstallShellExtension()
    {
        if (IsProcessing)
        {
            return;
        }

        try
        {
            IsProcessing = true;
            StatusMessage =
                "Installing shell extension... Please accept the UAC prompt if it appears.";
            IsStatusError = false;

            await Task.Run(() => _shellExtensionService.Register());

            // Small delay to allow registry to update
            await Task.Delay(500);

            // Refresh the status
            CheckShellExtensionStatus();

            if (IsShellExtensionRegistered)
            {
                IsStatusError = false;
                StatusMessage =
                    "✓ Shell extension installed successfully. Please restart Windows Explorer to see the context menu (right-click taskbar → Task Manager → Find 'Windows Explorer' → Restart).";
            }
            else
            {
                IsStatusError = true;
                StatusMessage =
                    "⚠ Installation command completed but shell extension is not showing as registered. Try clicking 'Refresh Status'.";
            }
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
        {
            // ERROR_CANCELLED - User cancelled the UAC prompt
            IsStatusError = true;
            StatusMessage =
                "Installation cancelled. You must accept the UAC prompt to install the shell extension.";
        }
        catch (FileNotFoundException ex)
        {
            IsStatusError = true;
            StatusMessage = $"⚠ {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            IsStatusError = true;
            StatusMessage = $"⚠ {ex.Message}";
        }
        catch (Exception ex)
        {
            IsStatusError = true;
            StatusMessage = $"❌ Error installing shell extension: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private async Task UninstallShellExtension()
    {
        if (IsProcessing)
        {
            return;
        }

        try
        {
            IsProcessing = true;
            StatusMessage =
                "Uninstalling shell extension... Please accept the UAC prompt if it appears.";
            IsStatusError = false;

            await Task.Run(() => _shellExtensionService.Unregister());

            // Small delay to allow registry to update
            await Task.Delay(500);

            // Refresh the status
            CheckShellExtensionStatus();

            if (!IsShellExtensionRegistered)
            {
                IsStatusError = false;
                StatusMessage =
                    "✓ Shell extension uninstalled successfully. Please restart Windows Explorer to remove the context menu (right-click taskbar → Task Manager → Find 'Windows Explorer' → Restart).";
            }
            else
            {
                IsStatusError = true;
                StatusMessage =
                    "⚠ Uninstallation command completed but shell extension is still showing as registered. Try clicking 'Refresh Status'.";
            }
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
        {
            // ERROR_CANCELLED - User cancelled the UAC prompt
            IsStatusError = true;
            StatusMessage =
                "Uninstallation cancelled. You must accept the UAC prompt to uninstall the shell extension.";
        }
        catch (FileNotFoundException ex)
        {
            IsStatusError = true;
            StatusMessage = $"⚠ {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            IsStatusError = true;
            StatusMessage = $"⚠ {ex.Message}";
        }
        catch (Exception ex)
        {
            IsStatusError = true;
            StatusMessage = $"❌ Error uninstalling shell extension: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
