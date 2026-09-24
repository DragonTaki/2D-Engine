/* ----- ----- ----- ----- */
// SkiaFont.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>SkiaSharp-backed <see cref="IFont"/>.</summary>
    internal sealed class SkiaFont : IFont
    {
        public SKFont Native { get; }
        public IFontFamily FontFamily { get; }
        public FontStyleFlags Style { get; }

        public SkiaFont(SKFont native, IFontFamily fontFamily, FontStyleFlags style)
        {
            Native = native;
            FontFamily = fontFamily;
            Style = style;
        }

        public float Size => Native.Size;

        /// <summary>Recommended line height in pixels, matching GDI+'s <c>Font.Height</c>.</summary>
        public float Height
        {
            get
            {
                var metrics = Native.Metrics;
                return metrics.Descent - metrics.Ascent + metrics.Leading;
            }
        }

        public void Dispose() => Native.Dispose();
    }
}
