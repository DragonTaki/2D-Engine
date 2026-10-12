/* ----- ----- ----- ----- */
// StatefulButtonStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Geometry;
using Engine.GraphicsUtils.GraphicsPaths;
using Engine.Mathematics;
using Engine.Platform;

namespace Engine.Styles
{
    public readonly record struct ButtonVisualState(bool Enabled, bool Hovered, bool Pressed);

    /// <summary>Additive state-aware drawing; ordinary IButtonDrawStyle callers still draw the normal state.</summary>
    public interface IStatefulButtonDrawStyle : IButtonDrawStyle
    {
        void Draw(IGraphics g, string text, LayoutF bounds, ButtonVisualState state);
    }

    /// <summary>Reusable rounded button with optional gradient, raised edge and interaction colours.</summary>
    public sealed class StatefulButtonStyle : IStatefulButtonDrawStyle
    {
        public Color FillColor { get; set; } = Color.RoyalBlue;
        public Color HoverColor { get; set; } = Color.CornflowerBlue;
        public Color PressedColor { get; set; } = Color.MidnightBlue;
        public Color TextColor { get; set; } = Color.White;
        public Color BorderColor { get; set; } = Color.Transparent;
        public Color? GradientEnd { get; set; }
        public float BorderWidth { get; set; } = 1f;
        public float CornerRadius { get; set; } = 8f;
        public float FontSize { get; set; } = 18f;
        public FontStyleFlags FontWeight { get; set; } = FontStyleFlags.Regular;
        public bool Raised { get; set; }
        public float DisabledOpacity { get; set; } = 0.4f;

        public void Draw(IGraphics g, LayoutF bounds) => Draw(g, string.Empty, bounds);
        public void Draw(IGraphics g, Vector2F position, Vector2F size) => Draw(g, new LayoutF(position, size));
        public void Draw(IGraphics g, string text, Vector2F position, Vector2F size) => Draw(g, text, new LayoutF(position, size));
        public void Draw(IGraphics g, string text, LayoutF bounds) => Draw(g, text, bounds, new ButtonVisualState(true, false, false));

        public void Draw(IGraphics g, string text, LayoutF bounds, ButtonVisualState state)
        {
            Color Fade(Color color) => state.Enabled ? color : Color.FromArgb((int)(color.A * DisabledOpacity), color);
            var rect = bounds.Inset(BorderWidth / 2f + (Raised ? 3f : 0f));
            if (rect.Width <= 0f || rect.Height <= 0f)
                return;
            using var path = RoundedRectPath.Create(rect.Width, rect.Height, CornerRadius);
            using var matrix = GraphicsBackend.Factory.CreateMatrix();
            matrix.Translate(rect.X, rect.Y + (state.Enabled && state.Pressed && Raised ? 2f : 0f));
            path.Transform(matrix);
            if (Raised)
            {
                using var shadow = GraphicsBackend.Factory.CreatePen(Fade(Color.FromArgb(65, 0, 0, 0)), 6f);
                g.DrawPath(shadow, path);
            }
            var fill = state.Enabled && state.Pressed ? PressedColor : state.Enabled && state.Hovered ? HoverColor : FillColor;
            using var brush = GradientEnd is Color end && !state.Hovered && !state.Pressed
                ? GraphicsBackend.Factory.CreateLinearGradientBrush(rect.ToRectangleF(), Fade(fill), Fade(end), GradientDirection.Vertical)
                : GraphicsBackend.Factory.CreateSolidBrush(Fade(fill));
            g.FillPath(brush, path);
            if (BorderWidth > 0f)
            {
                using var pen = GraphicsBackend.Factory.CreatePen(Fade(BorderColor), BorderWidth);
                g.DrawPath(pen, path);
            }
            using var font = GraphicsBackend.Factory.CreateFont(GraphicsBackend.Factory.GenericSansSerifFontFamily, FontSize, FontWeight);
            using var textBrush = GraphicsBackend.Factory.CreateSolidBrush(Fade(TextColor));
            ButtonText.DrawCentered(g, text, font, textBrush, bounds);
        }
    }
}
