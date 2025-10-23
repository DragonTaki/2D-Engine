/* ----- ----- ----- ----- */
// ShieldPath.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2025/05/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing.Drawing2D;

namespace Engine.GraphicsUtils.GraphicsPaths
{
    public static class ShieldPath
    {
        /// <summary>
        /// Create a basic shield-shaped path.
        /// </summary>
        public static GraphicsPath Create(float width, float height)
        {
            GraphicsPath path = new GraphicsPath();

            float curveHeight = 20.0f;
            float bottomPointHeight = 40.0f;

            path.StartFigure();
            path.AddArc(0, 0, curveHeight * 2, curveHeight * 2, 180, 90); // Top-left corner
            path.AddLine(curveHeight, 0, width - curveHeight, 0);         // Top edge
            path.AddArc(width - curveHeight * 2, 0, curveHeight * 2, curveHeight * 2, 270, 90); // Top-right corner
            path.AddLine(width, curveHeight, width, height - bottomPointHeight); // Right edge

            path.AddBezier(width, height - bottomPointHeight,
                           width * 0.75f, height,
                           width * 0.25f, height,
                           0, height - bottomPointHeight); // Bottom tip

            path.AddLine(0, height - bottomPointHeight, 0, curveHeight); // Left edge
            path.CloseFigure();

            return path;
        }
    }
}