/* ----- ----- ----- ----- */
// GlobalWindow.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

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
        /// Gets the current window size.
        /// </summary>
        public static PointF Size => new PointF(Width, Height);

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
