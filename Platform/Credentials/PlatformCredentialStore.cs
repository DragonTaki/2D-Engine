/* ----- ----- ----- ----- */
// PlatformCredentialStore.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.Security;

namespace Engine.Platform.Credentials
{
    /// <summary>
    /// Picks the <see cref="ICredentialStore"/> of the operating system the app runs on (at run
    /// time, not by the drawing backend: either launcher may run on Windows): Windows
    /// <see cref="DpapiCredentialStore"/>, macOS <see cref="KeychainCredentialStore"/>, anything
    /// else <see cref="UnsupportedCredentialStore"/>.
    /// </summary>
    public static class PlatformCredentialStore
    {
        /// <summary>Creates this system's store.</summary>
        /// <param name="dataFolder">The app's per-user data folder (the DPAPI store keeps its files under it).</param>
        /// <param name="serviceName">The app's name (the Keychain items' service, the DPAPI entropy).</param>
        /// <returns>The store; never null.</returns>
        public static ICredentialStore Create(string dataFolder, string serviceName)
        {
            ArgumentException.ThrowIfNullOrEmpty(dataFolder);
            ArgumentException.ThrowIfNullOrEmpty(serviceName);

            if (OperatingSystem.IsWindows())
                return new DpapiCredentialStore(dataFolder, serviceName);
            if (OperatingSystem.IsMacOS())
                return new KeychainCredentialStore(serviceName);
            return new UnsupportedCredentialStore();
        }
    }
}
