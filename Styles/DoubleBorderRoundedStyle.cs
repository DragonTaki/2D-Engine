/* ----- ----- ----- ----- */
// DoubleBorderRoundedStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

using Engine.Geometry;
using Engine.GraphicsUtils.GraphicsPaths;
using Engine.Mathematics;

namespace Engine.Styles
{
    public class DoubleBorderRoundedStyle : IBoxDrawStyle, IButtonDrawStyle
    {
        public float CornerRadius { get; set; }
        public float Margin { get; set; }
        public BorderStyle OuterBorder { get; set; }
        public BorderStyle InnerBorder { get; set; }

        public IBrushFactory BackgroundBrushFactory { get; set; }

        public Font Font { get; set; }
        public Brush TextBrush { get; set; }

        private void DrawBox(Graphics g, LayoutF bounds)
        {
            var outerGap = OuterBorder.Width;
            var innerGap = outerGap + Margin * 2 + InnerBorder.Width;

            var outerRect = bounds.Inset(outerGap / 2f);
            var innerRect = bounds.Inset(innerGap / 2f);

            using var outerPath = RoundedRectPath.Create(outerRect.Size.X, outerRect.Size.Y, CornerRadius);
            using var innerPath = RoundedRectPath.Create(innerRect.Size.X, innerRect.Size.Y, CornerRadius - Margin);

            using var outerMatrix = new Matrix();
            using var innerMatrix = new Matrix();
            outerMatrix.Translate(outerRect.Position.X, outerRect.Position.Y);
            innerMatrix.Translate(innerRect.Position.X, innerRect.Position.Y);
            outerPath.Transform(outerMatrix);
            innerPath.Transform(innerMatrix);

            using var brush = BackgroundBrushFactory.Create(bounds);
            g.FillPath(brush, outerPath);

            using var outerPen = new Pen(OuterBorder.Color, OuterBorder.Width);
            using var innerPen = new Pen(InnerBorder.Color, InnerBorder.Width);
            g.DrawPath(outerPen, outerPath);
            g.DrawPath(innerPen, innerPath);
        }

        // IBoxDrawStyle
        public void Draw(Graphics g, LayoutF bounds)
        {
            DrawBox(g, bounds);
        }

        public void Draw(Graphics g, Vector2F position, Vector2F size)
            => Draw(g, new LayoutF(position, size));

        // IButtonDrawStyle
        public void Draw(Graphics g, string text, LayoutF bounds)
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

        public void Draw(Graphics g, string text, Vector2F position, Vector2F size)
            => Draw(g, text, new LayoutF(position, size));
    }
}
