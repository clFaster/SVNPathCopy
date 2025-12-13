namespace SVNPathCopy.Core.Interfaces;

/// <summary>
///     Interface for shell extension registration operations.
/// </summary>
public interface IShellExtensionService
{
    /// <summary>
    ///     Checks if the shell extension is registered.
    /// </summary>
    /// <returns>True if the extension is registered; otherwise, false.</returns>
    bool IsRegistered();

    /// <summary>
    ///     Registers the shell extension.
    /// </summary>
    /// <returns>True if registration succeeded; otherwise, false.</returns>
    bool Register();

    /// <summary>
    ///     Unregisters the shell extension.
    /// </summary>
    /// <returns>True if unregistration succeeded; otherwise, false.</returns>
    bool Unregister();

    /// <summary>
    ///     Gets the path to the shell extension assembly.
    /// </summary>
    /// <returns>The full path to the assembly, or null if not found.</returns>
    string? GetAssemblyPath();
}
