/* ----- ----- ----- ----- */
// FontManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace Engine.Styles
{
    public static class FontManager
    {
        private static readonly PrivateFontCollection _fontCollection = new PrivateFontCollection();
        private static readonly Dictionary<string, FontFamily> _loadedFonts = new Dictionary<string, FontFamily>();

        // Load fonts by filename and assign a key (e.g., "MoeLI", "NotoSerif")
        public static void LoadFonts()
        {
            AddFont("NotoSerif", "NotoSerifCJKtc-Medium.otf");
            AddFont("MoeLI", "MoeLI.ttf");
        }

        private static void AddFont(string key, string fileName)
        {
            string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Font", fileName);
            if (!File.Exists(fontPath))
                throw new FileNotFoundException($"Font file not found: {fontPath}");

            _fontCollection.AddFontFile(fontPath);
            _loadedFonts[key] = _fontCollection.Families[_fontCollection.Families.Length - 1];
        }

        public static Font GetFont(string key, float size, FontStyle style = FontStyle.Regular)
        {
            if (!_loadedFonts.ContainsKey(key))
                throw new ArgumentException($"Font key '{key}' not found. Did you call LoadFonts()?");

            return new Font(_loadedFonts[key], size, style, GraphicsUnit.Pixel);
        }
    }
}