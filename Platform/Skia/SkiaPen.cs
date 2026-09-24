/* ----- ----- ----- ----- */
// SkiaPen.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>SkiaSharp-backed <see cref="IPen"/> — an <see cref="SKPaint"/> configured to stroke.</summary>
    internal sealed class SkiaPen : IPen
    {
        public SKPaint Native { get; }

        public SkiaPen(SKPaint native)
        {
            Native = native;
        }

        public PenDashStyle DashStyle
        {
            get => Native.PathEffect != null ? PenDashStyle.Dash : PenDashStyle.Solid;
            set
            {
                Native.PathEffect?.Dispose();
                Native.PathEffect = value == PenDashStyle.Dash
                    ? SKPathEffect.CreateDash(new[] { Native.StrokeWidth * 3, Native.StrokeWidth * 3 }, 0)
                    : null;
            }
        }

        // Skia strokes are always centered on the path; there is no separate
        // "inset" stroke alignment. Nothing in this codebase actually uses
        // Inset, so this is stored but has no effect.
        public PenLineAlignment Alignment { get; set; }

        public void Dispose()
        {
            Native.PathEffect?.Dispose();
            Native.Dispose();
        }
    }
}
