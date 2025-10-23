/* ----- ----- ----- ----- */
// SingleBorderRoundedStyle.cs
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
    public class SingleBorderRoundedStyle : IBoxDrawStyle, IButtonDrawStyle
    {
        public float CornerRadius { get; set; }
        public BorderStyle BorderStyle { get; set; }

        public IBrushFactory BackgroundBrushFactory { get; set; }

        public Font Font { get; set; }
        public Brush TextBrush { get; set; }

        private void DrawBox(Graphics g, LayoutF bounds)
        {
            var gap = BorderStyle.Width;

            var rect = bounds.Inset(gap / 2f);

            using var path = RoundedRectPath.Create(rect.Size.X, rect.Size.Y, CornerRadius);

            using var matrix = new Matrix();
            matrix.Translate(rect.Position.X, rect.Position.Y);
            path.Transform(matrix);

            using var brush = BackgroundBrushFactory.Create(bounds);
            g.FillPath(brush, path);

            using var pen = new Pen(BorderStyle.Color, BorderStyle.Width);
            g.DrawPath(pen, path);
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
