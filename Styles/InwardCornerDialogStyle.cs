/* ----- ----- ----- ----- */
// InwardCornerDialogStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.GraphicsUtils.GraphicsPaths;
using Engine.Mathematics;
using Engine.Platform;

namespace Engine.Styles
{
    /// <summary>
    /// Dialog style with a single border and inward-rounded corners.
    /// </summary>
    public class InwardCornerDialogStyle : IBoxDrawStyle
    {
        public float CornerRadius { get; set; }
        public BorderStyle BorderStyle { get; set; }
        public IBrushFactory BackgroundBrushFactory { get; set; }

        /// <summary>
        /// Draw a dialog box with inward-rounded corners.
        /// </summary>
        public void Draw(IGraphics g, LayoutF bounds)
        {
            var gap = BorderStyle.Width;

            var rect = bounds.Inset(gap / 2f);

            using var path = InvertedRoundedRectPath.Create(rect.Size.X, rect.Size.Y, CornerRadius);

            using var matrix = GraphicsBackend.Factory.CreateMatrix();
            matrix.Translate(rect.Position.X, rect.Position.Y);
            path.Transform(matrix);

            using var brush = BackgroundBrushFactory.Create(bounds);
            g.FillPath(brush, path);

            using var pen = GraphicsBackend.Factory.CreatePen(BorderStyle.Color, BorderStyle.Width);
            g.DrawPath(pen, path);
        }

        /// <summary>
        /// Draw a dialog box at a given position and size.
        /// </summary>
        public void Draw(IGraphics g, Vector2F position, Vector2F size)
            => Draw(g, new LayoutF(position, size));
    }
}
