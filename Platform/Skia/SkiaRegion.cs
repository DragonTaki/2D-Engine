/* ----- ----- ----- ----- */
// SkiaRegion.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>SkiaSharp-backed <see cref="IRegion"/>.</summary>
    internal sealed class SkiaRegion : IRegion
    {
        public SKRegion Native { get; }

        public SkiaRegion(SKRegion native)
        {
            Native = native;
        }

        public void Intersect(RectangleF bounds)
        {
            var rect = new SKRectI((int)bounds.Left, (int)bounds.Top, (int)bounds.Right, (int)bounds.Bottom);
            Native.Op(rect, SKRegionOperation.Intersect);
        }

        public void Dispose() => Native.Dispose();
    }
}
