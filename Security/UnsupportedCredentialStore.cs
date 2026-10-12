/* ----- ----- ----- ----- */
// UnsupportedCredentialStore.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

namespace Engine.Security
{
    /// <summary>
    /// The <see cref="ICredentialStore"/> of a platform without a protected store (e.g. Linux for
    /// now): keeps nothing, so the player logs in by hand every time (reads and writes fail; a
    /// delete succeeds, since nothing is held). Secrets are never written
    /// anywhere unprotected instead.
    /// </summary>
    public sealed class UnsupportedCredentialStore : ICredentialStore
    {
        /// <inheritdoc/>
        public bool IsSupported => false;

        /// <inheritdoc/>
        public bool TryRead(string key, out string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            value = null;
            return false;
        }

        /// <inheritdoc/>
        public bool Write(string key, string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ArgumentNullException.ThrowIfNull(value);
            return false;
        }

        /// <inheritdoc/>
        public bool Delete(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            return true;  // Nothing is ever kept, so the store never holds it.
        }
    }
}
