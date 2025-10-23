/* ----- ----- ----- ----- */
// InvertedRoundedRectPath.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2025/05/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing.Drawing2D;

namespace Engine.GraphicsUtils.GraphicsPaths
{
    /// <summary>
    /// Create a rectangle with inward-rounded (concave) corners.
    /// </summary>
    public static class InvertedRoundedRectPath
    {
        /// <summary>
        /// Create an inward-rounded rectangle path.
        /// </summary>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <param name="cornerRadius">Optional: Radius of the inward corner curve (default: auto-calculated).</param>
        /// <returns>GraphicsPath representing the inward-rounded rectangle.</returns>
        public static GraphicsPath Create(float width, float height, float? cornerRadius = null)
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

            GraphicsPath path = new GraphicsPath();

            float cut = cornerRadius ?? System.Math.Min(width, height) * 0.1f;
            cut = System.MathF.Min(cut, System.MathF.Min(width, height) / 2f); // 避免超出尺寸

            // 依順時針方向從左上角開始建立封閉路徑
            path.StartFigure();

            // Top edge
            path.AddLine(cut, 0, width - cut, 0);                         // 上橫
            path.AddLine(width - cut, 0, width, cut);                    // 右上角切角

            // Right edge
            path.AddLine(width, cut, width, height - cut);              // 右側
            path.AddLine(width, height - cut, width - cut, height);     // 右下角切角

            // Bottom edge
            path.AddLine(width - cut, height, cut, height);             // 下橫
            path.AddLine(cut, height, 0, height - cut);                 // 左下角切角

            // Left edge
            path.AddLine(0, height - cut, 0, cut);                      // 左側
            path.AddLine(0, cut, cut, 0);                               // 左上角切角

            path.CloseFigure();

            return path;
        }
    }
}
