/* ----- ----- ----- ----- */
// StyleHelper.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.Platform;

namespace Engine.Styles
{
    public static class StyleHelper
    {
        // Default font
        public static class FontDefaults
        {
            public const float FontSize = 10.0f;
            public const FontStyleFlags FontStyle = FontStyleFlags.Regular;
        }

        // Default color
        public static readonly Color DefaultColor = Color.Black;

        // Font method
        public static IFont GetFont(string fontKey = null, float? size = null, FontStyleFlags? style = null)
        {
            try
            {
                // Default
                IFontFamily fontFamily = GraphicsBackend.Factory.GenericSansSerifFontFamily;
                float fontSize = size ?? FontDefaults.FontSize;
                FontStyleFlags fontStyle = style ?? FontDefaults.FontStyle;

                // If specified fontkey
                if (!string.IsNullOrEmpty(fontKey))
                {
                    try
                    {
                        fontFamily = FontManager.GetFontFamily(fontKey);
                    }
                    catch
                    {
                        fontFamily = GraphicsBackend.Factory.GetSystemFontFamily(fontKey);
                    }
                }

                return GraphicsBackend.Factory.CreateFont(fontFamily, fontSize, fontStyle);
            }
            catch
            {
                return GraphicsBackend.Factory.CreateFont(
                    GraphicsBackend.Factory.GenericSansSerifFontFamily,
                    FontDefaults.FontSize,
                    FontDefaults.FontStyle);  // Return default if error
            }
        }

        // Brush methods
        public static IBrush GetBrush(string colorValue = "#000000", float alpha = 1.0f)
        {
            try
            {
                Color color = GetColor(colorValue, alpha);
                return GraphicsBackend.Factory.CreateSolidBrush(color);
            }
            catch
            {
                return GraphicsBackend.Factory.CreateSolidBrush(DefaultColor);  // Return default if error
            }
        }
        public static IBrush GetBrush(Color color, float alpha = 1.0f)
        {
            try
            {
                Color colorWithAlpha = Color.FromArgb(ClampAlpha(alpha), color);
                return GraphicsBackend.Factory.CreateSolidBrush(colorWithAlpha);
            }
            catch
            {
                return GraphicsBackend.Factory.CreateSolidBrush(DefaultColor);  // Return default if error
            }
        }

        // Color methods
        public static Color GetColor(string colorValue = "#000000", float alpha = 1.0f)
        {
            try
            {
                // Limit transparency to the range of 0.0 to 1.0
                alpha = Math.Clamp(alpha, 0f, 1f);

                if (colorValue.StartsWith('#'))
                {
                    // HEX format
                    Color hexColor = ColorTranslator.FromHtml(colorValue);
                    return Color.FromArgb(ClampAlpha(alpha), hexColor);
                }
                else if (colorValue.Contains(','))
                {
                    // RGB format
                    string cleaned = colorValue.Trim().Replace("(", "").Replace(")", "").Replace(" ", "");
                    string[] parts = cleaned.Split(',');

                    if (parts.Length == 3 &&
                        int.TryParse(parts[0], out int r) &&
                        int.TryParse(parts[1], out int g) &&
                        int.TryParse(parts[2], out int b))
                    {
                        return Color.FromArgb(ClampAlpha(alpha), r, g, b);
                    }
                }
                else
                {
                    // Name format, example: Black, White, Blue
                    Color namedColor = Color.FromName(colorValue.Trim());
                    if (namedColor.IsKnownColor)
                        return Color.FromArgb(ClampAlpha(alpha), namedColor);
                }
            }
            catch
            {
                return DefaultColor;  // Return default if error
            }

            return DefaultColor;  // Return default if error
        }
        public static Color GetColor(Color color, float alpha = 1.0f)
        {
            try
            {
                return Color.FromArgb(ClampAlpha(alpha), color);
            }
            catch
            {
                return DefaultColor;  // Return default if error
            }
        }
        private static int ClampAlpha(float alpha)
        {
            return Math.Max(0, Math.Min(255, (int)(alpha * 255)));
        }
    }
}
