using SVNPathCopy.Core.Models;

namespace SVNPathCopy.Core.Interfaces;

/// <summary>
///     Interface for managing SVNPathCopy configuration settings.
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    ///     Gets the current settings.
    /// </summary>
    /// <returns>The current settings.</returns>
    SvnPathCopySettings GetSettings();

    /// <summary>
    ///     Saves the specified settings.
    /// </summary>
    /// <param name="settings">The settings to save.</param>
    void SaveSettings(SvnPathCopySettings settings);

    /// <summary>
    ///     Resets settings to defaults.
    /// </summary>
    void ResetToDefaults();
}
