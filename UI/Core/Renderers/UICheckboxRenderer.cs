/* ----- ----- ----- ----- */
// UICheckboxRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
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
    /// <summary>Draws a square, a primitive check mark, focus outline and right-hand label.</summary>
    public class UICheckboxRenderer : UIRenderer<UICheckbox, UICheckboxHandler, UICheckboxRenderer>
    {
        public override void OnRender(IGraphics g, UICheckbox element)
        {
            var bounds = element.GetCurrentAbsoluteBounds();
            if (bounds.Width <= 0f || bounds.Height <= 0f)
                return;
            var style = element.Style ?? CheckboxDefaults.Style;
            float opacity = element.IsEnabled ? 1f : Math.Clamp(style.DisabledOpacity, 0f, 1f);
            float side = Math.Max(0f, Math.Min(style.BoxSize, Math.Min(bounds.Width, bounds.Height)));
            if (side <= 0f)
                return;
            float border = Math.Clamp(style.BorderWidth, 0f, side / 2f);
            var box = new RectangleF(bounds.X + border / 2f, bounds.Y + (bounds.Height - side) / 2f + border / 2f,
                side - border, side - border);
            using var fill = GraphicsBackend.Factory.CreateSolidBrush(Fade(element.IsChecked ? style.CheckedColor : style.UncheckedColor, opacity));
            g.FillRectangle(fill, box);
            if (border > 0f)
            {
                using var pen = GraphicsBackend.Factory.CreatePen(Fade(element.IsFocused ? style.FocusColor : style.BorderColor, opacity), border);
                g.DrawRectangle(pen, box);
            }
            if (element.IsChecked && style.CheckWidth > 0f)
            {
                using var pen = GraphicsBackend.Factory.CreatePen(Fade(style.CheckColor, opacity), Math.Min(style.CheckWidth, side / 4f));
                g.DrawLine(pen, box.X + box.Width * 0.2f, box.Y + box.Height * 0.5f, box.X + box.Width * 0.42f, box.Y + box.Height * 0.72f);
                g.DrawLine(pen, box.X + box.Width * 0.42f, box.Y + box.Height * 0.72f, box.X + box.Width * 0.8f, box.Y + box.Height * 0.25f);
            }
            float offset = side + Math.Max(0f, style.LabelGap);
            if (bounds.Width <= offset || string.IsNullOrEmpty(element.Text))
                return;
            using var brush = GraphicsBackend.Factory.CreateSolidBrush(Fade(style.TextColor, opacity));
            using var format = GraphicsBackend.Factory.CreateStringFormat();
            format.Alignment = TextAlign.Near;
            format.LineAlignment = TextAlign.Center;
            format.WordWrap = false;
            format.EllipsisTrimming = true;
            g.DrawString(element.Text, element.Font, brush,
                new RectangleF(bounds.X + offset, bounds.Y, bounds.Width - offset, bounds.Height), format);
        }

        private static Color Fade(Color color, float opacity) =>
            Color.FromArgb((int)MathF.Round(color.A * opacity), color);
    }
}
