/* ----- ----- ----- ----- */
// DpapiCredentialStore.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

using Engine.Logging;
using Engine.Security;

namespace Engine.Platform.Credentials
{
    /// <summary>
    /// Windows <see cref="ICredentialStore"/>: each secret is encrypted with DPAPI
    /// (<see cref="ProtectedData"/>, <see cref="DataProtectionScope.CurrentUser"/>: only this Windows
    /// user on this machine can decrypt it) and written to <c>&lt;key&gt;.bin</c> in a
    /// <c>Credentials</c> folder under the app's per-user data folder. Holds the server's login
    /// token for the automatic login (ONLINE-PLAY §8.25).
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class DpapiCredentialStore : ICredentialStore
    {
        /// <summary>Subfolder of the data folder holding the encrypted files.</summary>
        public const string FolderName = "Credentials";

        /// <summary>Extension of an encrypted secret's file.</summary>
        public const string FileExtension = ".bin";

        private readonly string _folder;
        private readonly byte[] _entropy;

        /// <summary>Creates the store.</summary>
        /// <param name="dataFolder">The app's per-user data folder (the one the settings file is in).</param>
        /// <param name="serviceName">The app's name; mixed into the encryption (DPAPI entropy) so other apps of the same user cannot decrypt the files with a plain call.</param>
        public DpapiCredentialStore(string dataFolder, string serviceName)
        {
            ArgumentException.ThrowIfNullOrEmpty(dataFolder);
            ArgumentException.ThrowIfNullOrEmpty(serviceName);
            _folder = Path.Combine(dataFolder, FolderName);
            _entropy = Encoding.UTF8.GetBytes(serviceName);
        }

        /// <inheritdoc/>
        public bool IsSupported => true;

        /// <inheritdoc/>
        public bool TryRead(string key, out string value)
        {
            CredentialKey.Validate(key);
            value = null;
            string path = PathOf(key);
            if (!File.Exists(path))
                return false;

            byte[] plain = null;
            try
            {
                plain = ProtectedData.Unprotect(File.ReadAllBytes(path), _entropy, DataProtectionScope.CurrentUser);
                value = Encoding.UTF8.GetString(plain);
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is CryptographicException)
            {
                AppLogger.Log($"[DpapiCredentialStore] Cannot read '{key}': {ex.Message}", LogLevel.WARN);
                return false;
            }
            finally
            {
                if (plain != null)
                    Array.Clear(plain);
            }
        }

        /// <inheritdoc/>
        public bool Write(string key, string value)
        {
            CredentialKey.Validate(key);
            ArgumentNullException.ThrowIfNull(value);

            string path = PathOf(key);
            string temp = path + ".tmp";
            byte[] plain = Encoding.UTF8.GetBytes(value);
            try
            {
                byte[] encrypted = ProtectedData.Protect(plain, _entropy, DataProtectionScope.CurrentUser);
                Directory.CreateDirectory(_folder);
                // Written beside it and then moved over it, so a failed write never leaves half a file.
                File.WriteAllBytes(temp, encrypted);
                File.Move(temp, path, overwrite: true);
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is CryptographicException)
            {
                AppLogger.Log($"[DpapiCredentialStore] Cannot write '{key}': {ex.Message}", LogLevel.WARN);
                TryDeleteFile(temp);
                return false;
            }
            finally
            {
                Array.Clear(plain);
            }
        }

        /// <inheritdoc/>
        public bool Delete(string key)
        {
            CredentialKey.Validate(key);
            return TryDeleteFile(PathOf(key));
        }

        /// <summary>The encrypted file of <paramref name="key"/> (already validated as a file name).</summary>
        private string PathOf(string key) => Path.Combine(_folder, key + FileExtension);

        /// <summary>Deletes <paramref name="path"/> if it exists; returns whether it is gone.</summary>
        private static bool TryDeleteFile(string path)
        {
            try
            {
                // File.Delete throws when the folder is missing; nothing to delete then.
                if (File.Exists(path))
                    File.Delete(path);
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                AppLogger.Log($"[DpapiCredentialStore] Cannot delete {path}: {ex.Message}", LogLevel.WARN);
                return false;
            }
        }
    }
}
