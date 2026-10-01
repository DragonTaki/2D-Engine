/* ----- ----- ----- ----- */
// InvertedRoundedRectPath.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2026/09/24
// Version: v1.1
/* ----- ----- ----- ----- */

using Engine.Platform;

namespace Engine.GraphicsUtils.GraphicsPaths
{
    /// <summary>
    /// Create a rectangle with inward-cut corners. The live implementation cuts each
    /// corner with a straight 45-degree chamfer (the concave-arc variant is the
    /// commented-out block in <see cref="Create"/>).
    /// </summary>
    public static class InvertedRoundedRectPath
    {
        /// <summary>
        /// Create an inward-cut (chamfered) rectangle path.
        /// </summary>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <param name="cornerRadius">Optional: size of each 45-degree corner cut, measured along each edge (default: 10% of the smaller side; clamped to half the smaller side).</param>
        /// <returns>GraphicsPath representing the chamfered rectangle.</returns>
        public static IGraphicsPath Create(float width, float height, float? cornerRadius = null)
        {
            /*
            GraphicsPath path = new GraphicsPath();

            float radius = cornerRadius ?? Math.Min(width, height) * 0.15f;
            radius = MathF.Min(radius, MathF.Min(width, height) / 2f);
            float diameter = radius * 2;

            // Define centers for arcs
            PointF topLeftCenter = new PointF(radius, radius);
            PointF topRightCenter = new PointF(width - radius, radius);
            PointF bottomRightCenter = new PointF(width - radius, height - radius);
            PointF bottomLeftCenter = new PointF(radius, height - radius);

            path.StartFigure();

            // Top edge (left to right, then dip into corner)
            path.AddLine(radius, 0, width - radius, 0);
            path.AddArc(new RectangleF(topRightCenter.X - radius, topRightCenter.Y - radius, diameter, diameter), 270, -90);

            // Right edge
            path.AddLine(width, radius, width, height - radius);
            path.AddArc(new RectangleF(bottomRightCenter.X - radius, bottomRightCenter.Y - radius, diameter, diameter), 0, -90);

            // Bottom edge
            path.AddLine(width - radius, height, radius, height);
            path.AddArc(new RectangleF(bottomLeftCenter.X - radius, bottomLeftCenter.Y - radius, diameter, diameter), 90, -90);

            // Left edge
            path.AddLine(0, height - radius, 0, radius);
            path.AddArc(new RectangleF(topLeftCenter.X - radius, topLeftCenter.Y - radius, diameter, diameter), 180, -90);

            path.CloseFigure();
            return path;*/

            IGraphicsPath path = GraphicsBackend.Factory.CreatePath();

            width = System.MathF.Max(0f, width);
            height = System.MathF.Max(0f, height);

            float cut = cornerRadius ?? System.Math.Min(width, height) * 0.1f;
            cut = System.MathF.Min(cut, System.MathF.Min(width, height) / 2f); // Keep the cut within the rectangle's size
            cut = System.MathF.Max(0f, cut); // A negative cut would make the path self-intersect

            // Build a closed path clockwise, starting from the top-left corner
            path.StartFigure();

            // Top edge
            path.AddLine(cut, 0, width - cut, 0);                         // Top edge
            path.AddLine(width - cut, 0, width, cut);                    // Top-right chamfer

            // Right edge
            path.AddLine(width, cut, width, height - cut);              // Right edge
            path.AddLine(width, height - cut, width - cut, height);     // Bottom-right chamfer

            // Bottom edge
            path.AddLine(width - cut, height, cut, height);             // Bottom edge
            path.AddLine(cut, height, 0, height - cut);                 // Bottom-left chamfer

            // Left edge
            path.AddLine(0, height - cut, 0, cut);                      // Left edge
            path.AddLine(0, cut, cut, 0);                               // Top-left chamfer

            path.CloseFigure();

            return path;
        }
    }
}
