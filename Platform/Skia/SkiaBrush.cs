/* ----- ----- ----- ----- */
// SkiaBrush.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>SkiaSharp-backed <see cref="IBrush"/> — an <see cref="SKPaint"/> configured to fill.</summary>
    internal sealed class SkiaBrush : IBrush
    {
        public SKPaint Native { get; }

        public SkiaBrush(SKPaint native)
        {
            Native = native;
        }

        public void Dispose() => Native.Dispose();
    }
}
