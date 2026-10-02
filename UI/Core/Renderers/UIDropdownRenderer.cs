/* ----- ----- ----- ----- */
// UIDropdownRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.Platform;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UIDropdown"/> (the closed box; the open list draws itself, see
    /// <see cref="UIDropdownListRenderer"/>): the box (the open box while the list is open), the
    /// chosen option's text vertically centered and clipped before the arrow, and the ▼ arrow at
    /// the right. A disabled dropdown's text and arrow are faded by the style's disabled opacity.
    /// </summary>
    public class UIDropdownRenderer : UIRenderer<UIDropdown, UIDropdownHandler, UIDropdownRenderer>
    {
        /// <summary>Character measured for the line height (so the text is centered the same with any option).</summary>
        private const string LineMeasure = "|";

        public UIDropdownRenderer() { }

        public override void OnRender(IGraphics g, UIDropdown element)
        {
            var style = element.Style ?? DropdownDefaults.Style;
            var bounds = element.GetCurrentAbsoluteBounds();
            if (bounds.Width <= 0f || bounds.Height <= 0f)
                return;

            var box = element.IsOpen ? (style.OpenBox ?? style.Box) : style.Box;
            box?.Draw(g, bounds);

            float opacity = element.IsEnabled ? 1f : Math.Clamp(style.DisabledOpacity, 0f, 1f);

            // The ▼ arrow: a triangle ArrowWidth wide and half as tall, vertically centered.
            float arrowWidth = Math.Max(0f, style.ArrowWidth);
            float arrowLeft = bounds.X + bounds.Width - style.ArrowInset - arrowWidth;
            if (arrowWidth > 0f)
            {
                float centerY = bounds.Y + bounds.Height / 2f;
                float half = arrowWidth / 4f;
                using var path = GraphicsBackend.Factory.CreatePath();
                path.StartFigure();
                path.AddLine(arrowLeft, centerY - half, arrowLeft + arrowWidth, centerY - half);
                path.AddLine(arrowLeft + arrowWidth, centerY - half, arrowLeft + arrowWidth / 2f, centerY + half);
                path.AddLine(arrowLeft + arrowWidth / 2f, centerY + half, arrowLeft, centerY - half);
                path.CloseFigure();
                using var arrowBrush = GraphicsBackend.Factory.CreateSolidBrush(Fade(style.ArrowColor, opacity));
                g.FillPath(arrowBrush, path);
            }

            var font = element.Font;
            string text = element.SelectedText;
            if (font == null || text.Length == 0)
                return;

            float textLeft = bounds.X + style.TextInset;
            float textWidth = (arrowWidth > 0f ? arrowLeft - style.TextInset : bounds.X + bounds.Width - style.TextInset) - textLeft;
            if (textWidth <= 0f)
                return;

            float lineHeight = g.MeasureString(LineMeasure, font).Height;
            float y = bounds.Y + (bounds.Height - lineHeight) / 2f;

            g.SetClip(new RectangleF(textLeft, bounds.Y, textWidth, bounds.Height));
            try
            {
                using var brush = GraphicsBackend.Factory.CreateSolidBrush(Fade(style.TextColor, opacity));
                g.DrawString(text, font, brush, textLeft, y);
            }
            finally
            {
                g.ResetClip();
            }
        }

        /// <summary><paramref name="color"/> with its alpha multiplied by <paramref name="opacity"/>.</summary>
        private static Color Fade(Color color, float opacity) =>
            opacity >= 1f ? color : Color.FromArgb((int)MathF.Round(color.A * opacity), color);
    }
}
