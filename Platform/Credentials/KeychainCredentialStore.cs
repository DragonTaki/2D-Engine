/* ----- ----- ----- ----- */
// KeychainCredentialStore.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

using Engine.Logging;
using Engine.Security;

namespace Engine.Platform.Credentials
{
    /// <summary>
    /// macOS <see cref="ICredentialStore"/>: each secret is a generic password item in the user's
    /// login Keychain (service = the app's name, account = the key), through Security.framework
    /// (<c>SecItemAdd</c> / <c>SecItemCopyMatching</c> / <c>SecItemUpdate</c> / <c>SecItemDelete</c>)
    /// called directly - not the <c>security</c> command, which would put the secret in a process's
    /// arguments. Holds the server's login token for the automatic login (ONLINE-PLAY §8.25).
    /// </summary>
    [SupportedOSPlatform("macos")]
    public sealed class KeychainCredentialStore : ICredentialStore
    {
        private readonly string _serviceName;

        /// <summary>Creates the store.</summary>
        /// <param name="serviceName">The items' service (the app's name, e.g. <c>Chinese-Chess-v3</c>), shown in Keychain Access.</param>
        public KeychainCredentialStore(string serviceName)
        {
            ArgumentException.ThrowIfNullOrEmpty(serviceName);
            _serviceName = serviceName;
        }

        /// <inheritdoc/>
        /// <remarks>False when Security.framework's constants cannot be loaded (then every call fails).</remarks>
        public bool IsSupported => Native.Loaded;

        /// <inheritdoc/>
        public bool TryRead(string key, out string value)
        {
            CredentialKey.Validate(key);
            value = null;
            if (!IsSupported)
                return false;

            using var cf = new CfScope();
            IntPtr query = ItemQuery(cf, key,
                (Native.kSecReturnData, Native.kCFBooleanTrue),
                (Native.kSecMatchLimit, Native.kSecMatchLimitOne));
            int status = Native.SecItemCopyMatching(query, out IntPtr result);
            if (status == Native.ErrSecItemNotFound)
                return false;
            if (status != Native.ErrSecSuccess)
                return Fail("read", key, status);

            cf.Add(result);
            byte[] plain = ReadData(result);
            try
            {
                value = Encoding.UTF8.GetString(plain);
                return true;
            }
            finally
            {
                Array.Clear(plain);
            }
        }

        /// <inheritdoc/>
        public bool Write(string key, string value)
        {
            CredentialKey.Validate(key);
            ArgumentNullException.ThrowIfNull(value);
            if (!IsSupported)
                return false;

            byte[] plain = Encoding.UTF8.GetBytes(value);
            using var cf = new CfScope();
            try
            {
                IntPtr data = cf.Add(Native.CFDataCreate(IntPtr.Zero, plain, plain.Length));

                // Add first; an existing item (errSecDuplicateItem) gets its data replaced instead.
                IntPtr attributes = ItemQuery(cf, key, (Native.kSecValueData, data));
                int status = Native.SecItemAdd(attributes, IntPtr.Zero);
                if (status == Native.ErrSecDuplicateItem)
                {
                    IntPtr query = ItemQuery(cf, key);
                    IntPtr update = cf.Add(Dictionary((Native.kSecValueData, data)));
                    status = Native.SecItemUpdate(query, update);
                }
                return status == Native.ErrSecSuccess || Fail("write", key, status);
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
            if (!IsSupported)
                return false;

            using var cf = new CfScope();
            int status = Native.SecItemDelete(ItemQuery(cf, key));
            return status == Native.ErrSecSuccess || status == Native.ErrSecItemNotFound || Fail("delete", key, status);
        }

        #region Helpers

        /// <summary>
        /// A generic password query for <paramref name="key"/> in this service, plus
        /// <paramref name="extra"/> pairs; every object made is released with <paramref name="cf"/>.
        /// </summary>
        private IntPtr ItemQuery(CfScope cf, string key, params (IntPtr Key, IntPtr Value)[] extra)
        {
            var pairs = new (IntPtr Key, IntPtr Value)[3 + extra.Length];
            pairs[0] = (Native.kSecClass, Native.kSecClassGenericPassword);
            pairs[1] = (Native.kSecAttrService, cf.Add(CfString(_serviceName)));
            pairs[2] = (Native.kSecAttrAccount, cf.Add(CfString(key)));
            Array.Copy(extra, 0, pairs, 3, extra.Length);
            return cf.Add(Dictionary(pairs));
        }

        /// <summary>A new CFDictionary of <paramref name="pairs"/> (retains them; the caller releases it).</summary>
        private static IntPtr Dictionary(params (IntPtr Key, IntPtr Value)[] pairs)
        {
            var keys = new IntPtr[pairs.Length];
            var values = new IntPtr[pairs.Length];
            for (int i = 0; i < pairs.Length; i++)
                (keys[i], values[i]) = pairs[i];
            return Native.CFDictionaryCreate(IntPtr.Zero, keys, values, pairs.Length,
                Native.kCFTypeDictionaryKeyCallBacks, Native.kCFTypeDictionaryValueCallBacks);
        }

        /// <summary>A new CFString of <paramref name="text"/> (the caller releases it).</summary>
        private static IntPtr CfString(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Native.CFStringCreateWithBytes(IntPtr.Zero, bytes, bytes.Length, Native.CFStringEncodingUTF8, 0);
        }

        /// <summary>The bytes of a CFData.</summary>
        private static byte[] ReadData(IntPtr data)
        {
            long length = Native.CFDataGetLength(data);
            var bytes = new byte[length];
            if (length > 0)
                Marshal.Copy(Native.CFDataGetBytePtr(data), bytes, 0, (int)length);
            return bytes;
        }

        /// <summary>Logs a failed Keychain call; always false.</summary>
        private static bool Fail(string action, string key, int status)
        {
            AppLogger.Log($"[KeychainCredentialStore] Cannot {action} '{key}': OSStatus {status}", LogLevel.WARN);
            return false;
        }

        /// <summary>Collects Core Foundation objects made for one call and releases them all at the end.</summary>
        private sealed class CfScope : IDisposable
        {
            private readonly List<IntPtr> _objects = new List<IntPtr>();

            /// <summary>Remembers <paramref name="cfObject"/> (null pointers are skipped) and returns it.</summary>
            public IntPtr Add(IntPtr cfObject)
            {
                if (cfObject != IntPtr.Zero)
                    _objects.Add(cfObject);
                return cfObject;
            }

            public void Dispose()
            {
                foreach (var cfObject in _objects)
                    Native.CFRelease(cfObject);
                _objects.Clear();
            }
        }

        #endregion

        #region Native

        /// <summary>
        /// Security.framework and CoreFoundation entry points and constants (the constants keep
        /// Apple's exported symbol names, e.g. <c>kSecClass</c>, so they can be looked up).
        /// </summary>
        private static class Native
        {
            private const string SecurityLibrary = "/System/Library/Frameworks/Security.framework/Security";
            private const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

            public const int ErrSecSuccess = 0;
            public const int ErrSecDuplicateItem = -25299;
            public const int ErrSecItemNotFound = -25300;
            public const uint CFStringEncodingUTF8 = 0x08000100;

            public static readonly bool Loaded;
            public static readonly IntPtr kSecClass;
            public static readonly IntPtr kSecClassGenericPassword;
            public static readonly IntPtr kSecAttrService;
            public static readonly IntPtr kSecAttrAccount;
            public static readonly IntPtr kSecValueData;
            public static readonly IntPtr kSecReturnData;
            public static readonly IntPtr kSecMatchLimit;
            public static readonly IntPtr kSecMatchLimitOne;
            public static readonly IntPtr kCFBooleanTrue;
            public static readonly IntPtr kCFTypeDictionaryKeyCallBacks;
            public static readonly IntPtr kCFTypeDictionaryValueCallBacks;

            static Native()
            {
                try
                {
                    IntPtr security = NativeLibrary.Load(SecurityLibrary);
                    IntPtr coreFoundation = NativeLibrary.Load(CoreFoundationLibrary);
                    // The kSec* / kCFBoolean* exports are CFTypeRef variables (read the pointer they
                    // hold); the callback exports are the structs themselves (use their address).
                    kSecClass = Constant(security, "kSecClass");
                    kSecClassGenericPassword = Constant(security, "kSecClassGenericPassword");
                    kSecAttrService = Constant(security, "kSecAttrService");
                    kSecAttrAccount = Constant(security, "kSecAttrAccount");
                    kSecValueData = Constant(security, "kSecValueData");
                    kSecReturnData = Constant(security, "kSecReturnData");
                    kSecMatchLimit = Constant(security, "kSecMatchLimit");
                    kSecMatchLimitOne = Constant(security, "kSecMatchLimitOne");
                    kCFBooleanTrue = Constant(coreFoundation, "kCFBooleanTrue");
                    kCFTypeDictionaryKeyCallBacks = NativeLibrary.GetExport(coreFoundation, "kCFTypeDictionaryKeyCallBacks");
                    kCFTypeDictionaryValueCallBacks = NativeLibrary.GetExport(coreFoundation, "kCFTypeDictionaryValueCallBacks");
                    Loaded = true;
                }
                catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException || ex is BadImageFormatException)
                {
                    AppLogger.Log($"[KeychainCredentialStore] Security.framework not available: {ex.Message}", LogLevel.WARN);
                }
            }

            private static IntPtr Constant(IntPtr library, string name) =>
                Marshal.ReadIntPtr(NativeLibrary.GetExport(library, name));

            [DllImport(SecurityLibrary)]
            public static extern int SecItemAdd(IntPtr attributes, IntPtr result);

            [DllImport(SecurityLibrary)]
            public static extern int SecItemCopyMatching(IntPtr query, out IntPtr result);

            [DllImport(SecurityLibrary)]
            public static extern int SecItemUpdate(IntPtr query, IntPtr attributesToUpdate);

            [DllImport(SecurityLibrary)]
            public static extern int SecItemDelete(IntPtr query);

            [DllImport(CoreFoundationLibrary)]
            public static extern IntPtr CFDictionaryCreate(IntPtr allocator, IntPtr[] keys, IntPtr[] values, nint count, IntPtr keyCallBacks, IntPtr valueCallBacks);

            [DllImport(CoreFoundationLibrary)]
            public static extern IntPtr CFStringCreateWithBytes(IntPtr allocator, byte[] bytes, nint numBytes, uint encoding, byte isExternalRepresentation);

            [DllImport(CoreFoundationLibrary)]
            public static extern IntPtr CFDataCreate(IntPtr allocator, byte[] bytes, nint length);

            [DllImport(CoreFoundationLibrary)]
            public static extern nint CFDataGetLength(IntPtr data);

            [DllImport(CoreFoundationLibrary)]
            public static extern IntPtr CFDataGetBytePtr(IntPtr data);

            [DllImport(CoreFoundationLibrary)]
            public static extern void CFRelease(IntPtr cfObject);
        }

        #endregion
    }
}
