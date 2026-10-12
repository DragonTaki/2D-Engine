/* ----- ----- ----- ----- */
// ICredentialStore.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Security
{
    /// <summary>
    /// A small secret store protected by the operating system for the current user (Windows: DPAPI,
    /// macOS: the Keychain; the implementations are in <c>Engine.Platform.Credentials</c>). The game
    /// keeps the server's long-lived login token here so the player is logged in automatically
    /// (ONLINE-PLAY §8.25); nothing secret goes into the plain settings file.
    /// <para>
    /// Keys are short names chosen by the caller (letters, digits, <c>.</c>, <c>_</c>, <c>-</c>).
    /// No member throws for a storage failure: it is logged and reported through the return
    /// value, so a broken store only means logging in by hand.
    /// </para>
    /// </summary>
    public interface ICredentialStore
    {
        /// <summary>Whether this platform has a store (false: nothing is kept, every call fails).</summary>
        bool IsSupported { get; }

        /// <summary>Reads the secret stored under <paramref name="key"/>.</summary>
        /// <param name="key">The secret's name.</param>
        /// <param name="value">The secret, or null when the result is false.</param>
        /// <returns>Whether a secret was found and read.</returns>
        bool TryRead(string key, out string value);

        /// <summary>Stores <paramref name="value"/> under <paramref name="key"/>, replacing an earlier one.</summary>
        /// <param name="key">The secret's name.</param>
        /// <param name="value">The secret (not null; may be empty).</param>
        /// <returns>Whether it was stored.</returns>
        bool Write(string key, string value);

        /// <summary>Removes the secret stored under <paramref name="key"/> (e.g. on logout).</summary>
        /// <param name="key">The secret's name.</param>
        /// <returns>Whether the store no longer holds it (also true when there was none).</returns>
        bool Delete(string key);
    }
}
