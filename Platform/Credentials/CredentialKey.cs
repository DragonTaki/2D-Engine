/* ----- ----- ----- ----- */
// CredentialKey.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Platform.Credentials
{
    /// <summary>
    /// The key rule shared by the stores: a non-empty name of ASCII letters, digits, <c>.</c>,
    /// <c>_</c> and <c>-</c> (so it is a safe file name for the DPAPI store and a plain account
    /// name for the Keychain), at most <see cref="MaxLength"/> characters.
    /// </summary>
    internal static class CredentialKey
    {
        /// <summary>Longest key (characters).</summary>
        public const int MaxLength = 64;

        /// <summary>Throws when <paramref name="key"/> breaks the rule.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="key"/> is empty, too long, or has another character.</exception>
        public static void Validate(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            if (key.Length > MaxLength)
                throw new ArgumentException($"Credential key longer than {MaxLength} characters.", nameof(key));
            foreach (char c in key)
            {
                if (!char.IsAsciiLetterOrDigit(c) && c != '.' && c != '_' && c != '-')
                    throw new ArgumentException($"Credential key '{key}' has a character other than letters, digits, '.', '_' or '-'.", nameof(key));
            }
            if (key == "." || key == "..")
                throw new ArgumentException($"Credential key '{key}' is not a name.", nameof(key));
        }
    }
}
