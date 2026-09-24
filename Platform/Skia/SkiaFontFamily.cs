/* ----- ----- ----- ----- */
// SkiaFontFamily.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>SkiaSharp-backed <see cref="IFontFamily"/>.</summary>
    internal sealed class SkiaFontFamily : IFontFamily
    {
        public SKTypeface Native { get; }

        public SkiaFontFamily(SKTypeface native)
        {
            Native = native;
        }

        public string Name => Native.FamilyName;
    }
}
