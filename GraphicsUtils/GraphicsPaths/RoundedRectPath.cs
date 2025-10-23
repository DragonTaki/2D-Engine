/* ----- ----- ----- ----- */
// RoundedRectPath.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/22
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Engine.GraphicsUtils.GraphicsPaths
{
    public static class RoundedRectPath
    {
        /// <summary>
        /// Create a rounded rectangle path (like a sticky label with slightly rounded corners).
        /// </summary>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <param name="cornerRadius">Optional: Radius of the corner (default: auto-calculated).</param>
        public static GraphicsPath Create(float width, float height, float? cornerRadius = null)
        {
            GraphicsPath path = new GraphicsPath();

            float radius = cornerRadius ?? Math.Min(width, height) * 0.08f;  // 8% of size (or override)
            radius = MathF.Min(radius, MathF.Min(width, height) / 2f);       // Avoid over-rounding

            float diameter = radius * 2;

            RectangleF topLeft = new RectangleF(0, 0, diameter, diameter);
            RectangleF topRight = new RectangleF(width - diameter, 0, diameter, diameter);
            RectangleF bottomRight = new RectangleF(width - diameter, height - diameter, diameter, diameter);
            RectangleF bottomLeft = new RectangleF(0, height - diameter, diameter, diameter);

            path.StartFigure();
            path.AddArc(topLeft, 180, 90);                           // Top-left corner
            path.AddLine(radius, 0, width - radius, 0);              // Top edge
            path.AddArc(topRight, 270, 90);                          // Top-right corner
            path.AddLine(width, radius, width, height - radius);     // Right edge
            path.AddArc(bottomRight, 0, 90);                         // Bottom-right corner
            path.AddLine(width - radius, height, radius, height);    // Bottom edge
            path.AddArc(bottomLeft, 90, 90);                         // Bottom-left corner
            path.AddLine(0, height - radius, 0, radius);             // Left edge
            path.CloseFigure();

            return path;
        }
    }
}