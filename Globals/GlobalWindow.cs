/* ----- ----- ----- ----- */
// GlobalWindow.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.Geometry;
using Engine.Mathematics;

namespace Engine.Globals
{
    /// <summary>
    /// Provides global access to current window size and screen-related values.
    /// </summary>
    public static class GlobalWindow
    {
        /// <summary>
        /// Current window width in pixels.
        /// </summary>
        public static int Width { get; private set; }

        /// <summary>
        /// Current window height in pixels.
        /// </summary>
        public static int Height { get; private set; }

        /// <summary>
        /// Updates the global window dimensions.
        /// </summary>
        /// <param name="width">New window width.</param>
        /// <param name="height">New window height.</param>
        public static void UpdateSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Physical pixels per logical unit (2 on a Retina display, 1 otherwise).
        /// </summary>
        public static float PixelScale { get; private set; } = 1f;

        /// <summary>
        /// Updates <see cref="PixelScale"/>; non-positive values are ignored.
        /// </summary>
        public static void UpdatePixelScale(float scale)
        {
            if (scale > 0f)
                PixelScale = scale;
        }

        /// <summary>
        /// Current window width in logical units: device-independent units where
        /// 1 unit equals 1 physical pixel at <see cref="PixelScale"/> 1 (Apple "points",
        /// Windows "DIPs"). Content with fixed on-screen sizes should be authored in these
        /// and drawn under a <see cref="PixelScale"/> transform.
        /// </summary>
        public static int LogicalWidth => (int)MathF.Round(Width / PixelScale);

        /// <summary>
        /// Current window height in logical units (see <see cref="LogicalWidth"/>).
        /// </summary>
        public static int LogicalHeight => (int)MathF.Round(Height / PixelScale);

        /// <summary>
        /// Gets the current window size.
        /// </summary>
        public static SizeF Size => new SizeF(Width, Height);

        /// <summary>
        /// Gets the current center point of the window.
        /// </summary>
        public static Vector2F Center => new Vector2F(Width / 2f, Height / 2f);

        /// <summary>
        /// Gets the current window bounds as a LayoutF.
        /// </summary>
        public static LayoutF Bounds => new LayoutF(0, 0, Width, Height);
    }
}
