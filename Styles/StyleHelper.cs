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

using Engine.Logging;
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
                    catch (ArgumentException)
                    {
                        // Not a loaded font key: try it as a system font name.
                        fontFamily = GraphicsBackend.Factory.GetSystemFontFamily(fontKey);
                    }
                }

                return GraphicsBackend.Factory.CreateFont(fontFamily, fontSize, fontStyle);
            }
            catch (Exception ex)
            {
                AppLogger.Log($"[StyleHelper] GetFont('{fontKey}') failed, using default font: {ex.Message}", LogLevel.WARN);
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
            catch (Exception ex)
            {
                AppLogger.Log($"[StyleHelper] GetBrush failed, using default color: {ex.Message}", LogLevel.WARN);
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
            catch (Exception ex)
            {
                AppLogger.Log($"[StyleHelper] GetBrush failed, using default color: {ex.Message}", LogLevel.WARN);
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
                    string hex = colorValue.Trim();
                    if (hex.Length == 9)
                    {
                        // #RRGGBBAA (CSS order). ColorTranslator.FromHtml would read these
                        // 8 digits as AARRGGBB, e.g. "#716c6cff" -> RGB 6C6CFF instead of 716C6C.
                        int rgba = Convert.ToInt32(hex.Substring(1), 16);
                        Color rgb = Color.FromArgb((rgba >> 24) & 0xFF, (rgba >> 16) & 0xFF, (rgba >> 8) & 0xFF);
                        float hexAlpha = (rgba & 0xFF) / 255f;
                        return Color.FromArgb(ClampAlpha(alpha * hexAlpha), rgb);
                    }

                    Color hexColor = ColorTranslator.FromHtml(hex);
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
            catch (Exception ex)
            {
                AppLogger.Log($"[StyleHelper] Invalid color '{colorValue}', using default: {ex.Message}", LogLevel.WARN);
                return DefaultColor;  // Return default if error
            }

            AppLogger.Log($"[StyleHelper] Unrecognized color '{colorValue}', using default.", LogLevel.WARN);
            return DefaultColor;  // Return default if error
        }

        public static Color GetColor(Color color, float alpha = 1.0f)
        {
            try
            {
                return Color.FromArgb(ClampAlpha(alpha), color);
            }
            catch (Exception ex)
            {
                AppLogger.Log($"[StyleHelper] GetColor failed, using default: {ex.Message}", LogLevel.WARN);
                return DefaultColor;  // Return default if error
            }
        }

        private static int ClampAlpha(float alpha)
        {
            return Math.Max(0, Math.Min(255, (int)(alpha * 255)));
        }
    }
}
