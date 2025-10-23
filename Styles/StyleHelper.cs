/* ----- ----- ----- ----- */
// StyleHelper.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/05/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace Engine.Styles
{
    public static class StyleHelper
    {
        // Default font
        public static class FontDefaults
        {
            public static readonly FontFamily FontFamily = FontFamily.GenericSansSerif;
            public const float FontSize = 10.0f;
            public static readonly FontStyle FontStyle = FontStyle.Regular;

            public static readonly Font DefaultFont = new Font(FontFamily, FontSize, FontStyle);
        }

        // Default brush
        public static readonly Brush DefaultBrush = Brushes.Black;

        // Default color
        public static readonly Color DefaultColor = Color.Black;

        // Font method
        public static Font GetFont(string fontKey = null, float? size = null, FontStyle? style = null)
        {
            try
            {
                // Default
                FontFamily fontFamily = FontDefaults.FontFamily;
                float fontSize = size ?? FontDefaults.FontSize;
                FontStyle fontStyle = style ?? FontDefaults.FontStyle;

                // If specified fontkey
                if (!string.IsNullOrEmpty(fontKey))
                {
                    try
                    {
                        fontFamily = FontManager.GetFont(fontKey, fontSize, fontStyle).FontFamily;
                    }
                    catch
                    {
                        fontFamily = GetSystemFont(fontKey);
                    }
                }

                return new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Pixel);
            }
            catch
            {
                return new Font(FontDefaults.FontFamily,
                                FontDefaults.FontSize,
                                FontDefaults.FontStyle);  // Return default if error
            }
        }

        private static FontFamily GetSystemFont(string fontKey)
        {
            try
            {
                // Try to get the system font by its name
                return new FontFamily(fontKey);
            }
            catch
            {
                // If the system font is not found, fall back to a default system font
                Console.WriteLine($"System font '{fontKey}' not found, using default system font.");
                return FontFamily.GenericSansSerif;
            }
        }

        // Brush methods
        public static Brush GetBrush(string colorValue = "#000000", float alpha = 1.0f)
        {
            try
            {
                Color color = GetColor(colorValue, alpha);
                return new SolidBrush(color);
            }
            catch
            {
                return DefaultBrush;  // Return default if error
            }
        }
        public static Brush GetBrush(Color color, float alpha = 1.0f)
        {
            try
            {
                Color colorWithAlpha = Color.FromArgb(ClampAlpha(alpha), color);
                return new SolidBrush(colorWithAlpha);
            }
            catch
            {
                return DefaultBrush;  // Return default if error
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