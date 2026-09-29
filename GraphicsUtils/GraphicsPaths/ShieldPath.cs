/* ----- ----- ----- ----- */
// ShieldPath.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2026/09/24
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Engine.Platform;

namespace Engine.GraphicsUtils.GraphicsPaths
{
    public static class ShieldPath
    {
        /// <summary>
        /// Create a basic shield-shaped path.
        /// </summary>
        public static IGraphicsPath Create(float width, float height)
        {
            IGraphicsPath path = GraphicsBackend.Factory.CreatePath();

            width = MathF.Max(0f, width);
            height = MathF.Max(0f, height);

            // Fixed shape sizes, clamped so a small shield doesn't fold over itself (the
            // top corners need 2x curveHeight of width; the straight sides need
            // curveHeight + bottomPointHeight of height).
            float curveHeight = MathF.Min(20.0f, MathF.Min(width / 2f, height / 2f));
            float bottomPointHeight = MathF.Min(40.0f, height - curveHeight);

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
