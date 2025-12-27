using Microsoft.Win32;
using SVNPathCopy.Core.Interfaces;
using SVNPathCopy.Core.Models;

namespace SVNPathCopy.Core.Services;

/// <summary>
///     Registry-based configuration service for SVNPathCopy settings.
/// </summary>
public sealed class RegistryConfigurationService : IConfigurationService
{
    private const string _registryKeyPath = @"SOFTWARE\SVNPathCopy";

    private const string _enabledValueName = "Enabled";
    private const string _showCopyWithRevisionValueName = "ShowCopyWithRevision";
    private const string _showCopyWithoutRevisionValueName = "ShowCopyWithoutRevision";
    private const string _urlEncodingStyleValueName = "UrlEncodingStyle";

    /// <inheritdoc />
    public SvnPathCopySettings GetSettings()
    {
        var settings = new SvnPathCopySettings();

        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(_registryKeyPath);
            if (key is null)
            {
                return settings; // Return defaults if key doesn't exist
            }

            settings.Enabled = GetBoolValue(key, _enabledValueName, true);
            settings.ShowCopyWithRevision = GetBoolValue(key, _showCopyWithRevisionValueName, true);
            settings.ShowCopyWithoutRevision = GetBoolValue(
                key,
                _showCopyWithoutRevisionValueName,
                true
            );
            settings.UrlEncodingStyle = GetEnumValue(
                key,
                _urlEncodingStyleValueName,
                UrlEncodingStyle.Path
            );
        }
        catch
        {
            // If registry access fails, return defaults
        }

        return settings;
    }

    /// <inheritdoc />
    public void SaveSettings(SvnPathCopySettings settings)
    {
        if (settings is null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        using RegistryKey? key = Registry.CurrentUser.CreateSubKey(_registryKeyPath);
        if (key is null)
        {
            throw new InvalidOperationException("Failed to create registry key.");
        }

        key.SetValue(_enabledValueName, settings.Enabled ? 1 : 0, RegistryValueKind.DWord);
        key.SetValue(
            _showCopyWithRevisionValueName,
            settings.ShowCopyWithRevision ? 1 : 0,
            RegistryValueKind.DWord
        );
        key.SetValue(
            _showCopyWithoutRevisionValueName,
            settings.ShowCopyWithoutRevision ? 1 : 0,
            RegistryValueKind.DWord
        );
        key.SetValue(
            _urlEncodingStyleValueName,
            settings.UrlEncodingStyle.ToString(),
            RegistryValueKind.String
        );
    }

    /// <inheritdoc />
    public void ResetToDefaults() => SaveSettings(new SvnPathCopySettings());

    private static bool GetBoolValue(RegistryKey key, string name, bool defaultValue)
    {
        object? value = key.GetValue(name);
        if (value is int intValue)
        {
            return intValue != 0;
        }

        return defaultValue;
    }

    private static TEnum GetEnumValue<TEnum>(RegistryKey key, string name, TEnum defaultValue)
        where TEnum : struct
    {
        string? value = key.GetValue(name) as string;
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        if (Enum.TryParse(value, true, out TEnum result))
        {
            return result;
        }

        return defaultValue;
    }
}
