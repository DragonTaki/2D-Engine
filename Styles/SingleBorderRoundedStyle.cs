/* ----- ----- ----- ----- */
// SingleBorderRoundedStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2026/10/01
// Version: v2.1
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.GraphicsUtils.GraphicsPaths;
using Engine.Mathematics;
using Engine.Platform;

namespace Engine.Styles
{
    public class SingleBorderRoundedStyle : IBoxDrawStyle, IButtonDrawStyle
    {
        public float CornerRadius { get; set; }
        public BorderStyle BorderStyle { get; set; }

        public IBrushFactory BackgroundBrushFactory { get; set; }

        public IFont Font { get; set; }
        public IBrush TextBrush { get; set; }

        private void DrawBox(IGraphics g, LayoutF bounds)
        {
            var gap = BorderStyle.Width;

            var rect = bounds.Inset(gap / 2f);

            using var path = RoundedRectPath.Create(rect.Size.X, rect.Size.Y, CornerRadius);

            using var matrix = GraphicsBackend.Factory.CreateMatrix();
            matrix.Translate(rect.Position.X, rect.Position.Y);
            path.Transform(matrix);

            using var brush = BackgroundBrushFactory.Create(bounds);
            g.FillPath(brush, path);

            using var pen = GraphicsBackend.Factory.CreatePen(BorderStyle.Color, BorderStyle.Width);
            g.DrawPath(pen, path);
        }

        // IBoxDrawStyle
        public void Draw(IGraphics g, LayoutF bounds)
        {
            DrawBox(g, bounds);
        }

        public void Draw(IGraphics g, Vector2F position, Vector2F size)
            => Draw(g, new LayoutF(position, size));

        // IButtonDrawStyle
        public void Draw(IGraphics g, string text, LayoutF bounds)
        {
            DrawBox(g, bounds);

            // Centered, one line per line break (skipped without a font or brush).
            ButtonText.DrawCentered(g, text, Font, TextBrush, bounds);
        }

        public void Draw(IGraphics g, string text, Vector2F position, Vector2F size)
            => Draw(g, text, new LayoutF(position, size));
    }
}
