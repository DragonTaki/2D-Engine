/* ----- ----- ----- ----- */
// DoubleBorderRoundedStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.GraphicsUtils.GraphicsPaths;
using Engine.Mathematics;
using Engine.Platform;

namespace Engine.Styles
{
    public class DoubleBorderRoundedStyle : IBoxDrawStyle, IButtonDrawStyle
    {
        public float CornerRadius { get; set; }
        public float Margin { get; set; }
        public BorderStyle OuterBorder { get; set; }
        public BorderStyle InnerBorder { get; set; }

        public IBrushFactory BackgroundBrushFactory { get; set; }

        public IFont Font { get; set; }
        public IBrush TextBrush { get; set; }

        private void DrawBox(IGraphics g, LayoutF bounds)
        {
            var outerGap = OuterBorder.Width;
            var innerGap = outerGap + Margin * 2 + InnerBorder.Width;

            var outerRect = bounds.Inset(outerGap / 2f);
            var innerRect = bounds.Inset(innerGap / 2f);

            using var outerPath = RoundedRectPath.Create(outerRect.Size.X, outerRect.Size.Y, CornerRadius);
            using var innerPath = RoundedRectPath.Create(innerRect.Size.X, innerRect.Size.Y, CornerRadius - Margin);

            using var outerMatrix = GraphicsBackend.Factory.CreateMatrix();
            using var innerMatrix = GraphicsBackend.Factory.CreateMatrix();
            outerMatrix.Translate(outerRect.Position.X, outerRect.Position.Y);
            innerMatrix.Translate(innerRect.Position.X, innerRect.Position.Y);
            outerPath.Transform(outerMatrix);
            innerPath.Transform(innerMatrix);

            using var brush = BackgroundBrushFactory.Create(bounds);
            g.FillPath(brush, outerPath);

            using var outerPen = GraphicsBackend.Factory.CreatePen(OuterBorder.Color, OuterBorder.Width);
            using var innerPen = GraphicsBackend.Factory.CreatePen(InnerBorder.Color, InnerBorder.Width);
            g.DrawPath(outerPen, outerPath);
            g.DrawPath(innerPen, innerPath);
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

            if (!string.IsNullOrEmpty(text) && TextBrush != null)
            {
                var textSize = g.MeasureString(text, Font);
                float textX = bounds.Position.X + (bounds.Size.X - textSize.Width) / 2f;
                float textY = bounds.Position.Y + (bounds.Size.Y - textSize.Height) / 2f;
                g.DrawString(text, Font, TextBrush, textX, textY);
            }
        }

        public void Draw(IGraphics g, string text, Vector2F position, Vector2F size)
            => Draw(g, text, new LayoutF(position, size));
    }
}
