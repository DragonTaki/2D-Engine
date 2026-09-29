/* ----- ----- ----- ----- */
// FontManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.IO;

using Engine.Logging;
using Engine.Platform;

namespace Engine.Styles
{
    public static class FontManager
    {
        // Load fonts by filename and assign a key (e.g., "MoeLI", "NotoSerif")
        public static void LoadFonts()
        {
            AddFont("NotoSerif", "NotoSerifCJKtc-Medium.otf");
            AddFont("MoeLI", "MoeLI.ttf");
        }

        private static void AddFont(string key, string fileName)
        {
            string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Font", fileName);

            // A missing or unreadable font shouldn't abort startup (or skip the remaining
            // fonts): StyleHelper.GetFont already falls back when a key isn't loaded.
            try
            {
                GraphicsBackend.Factory.LoadFontFamily(key, fontPath);
            }
            catch (Exception ex) when (ex is IOException || ex is ArgumentException || ex is System.Runtime.InteropServices.ExternalException)
            {
                AppLogger.Log($"Failed to load font '{key}' from {fontPath}: {ex.Message}", LogLevel.ERROR);
            }
        }

        public static IFontFamily GetFontFamily(string key) => GraphicsBackend.Factory.GetLoadedFontFamily(key);

        public static IFont GetFont(string key, float size, FontStyleFlags style = FontStyleFlags.Regular) =>
            GraphicsBackend.Factory.CreateFont(GetFontFamily(key), size, style);
    }
}
