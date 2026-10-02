/* ----- ----- ----- ----- */
// UISliderRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.GraphicsUtils.GraphicsPaths;
using Engine.Platform;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Utils;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UISlider"/>: the rounded track, its part from the left end to the
    /// knob in the fill colour, the round (outlined) knob at the value, and the value's text
    /// vertically centered in the column at the right (clipped to it). A disabled slider is drawn
    /// with every colour faded by the style's disabled opacity.
    /// </summary>
    public class UISliderRenderer : UIRenderer<UISlider, UISliderHandler, UISliderRenderer>
    {
        /// <summary>Character measured for the line height (so every value's text is centered the same).</summary>
        private const string LineMeasure = "|";

        public UISliderRenderer() { }

        public override void OnRender(IGraphics g, UISlider element)
        {
            var style = element.Style ?? SliderDefaults.Style;
            var bounds = element.GetCurrentAbsoluteBounds();
            if (bounds.Width <= 0f || bounds.Height <= 0f)
                return;

            float opacity = element.IsEnabled ? 1f : Math.Clamp(style.DisabledOpacity, 0f, 1f);
            var (left, width, centerY) = element.GetTrack();
            float knobX = SliderMath.PositionOf(element.Value, left, width, element.Minimum, element.Maximum);

            // Track, then its filled part (both pills: corner radius half the height).
            float trackHeight = Math.Max(0f, style.TrackHeight);
            if (trackHeight > 0f && width > 0f)
            {
                FillPill(g, left, centerY - trackHeight / 2f, width, trackHeight, Fade(style.TrackColor, opacity));
                if (knobX > left)
                    FillPill(g, left, centerY - trackHeight / 2f, knobX - left, trackHeight, Fade(style.FillColor, opacity));
            }

            // Knob, outlined inside its diameter.
            float diameter = Math.Max(0f, style.KnobDiameter);
            if (diameter > 0f)
            {
                using var knobBrush = GraphicsBackend.Factory.CreateSolidBrush(Fade(style.KnobColor, opacity));
                g.FillEllipse(knobBrush, knobX - diameter / 2f, centerY - diameter / 2f, diameter, diameter);
                if (style.KnobBorderWidth > 0f)
                {
                    float inset = style.KnobBorderWidth / 2f;
                    using var pen = GraphicsBackend.Factory.CreatePen(Fade(style.KnobBorderColor, opacity), style.KnobBorderWidth);
                    g.DrawEllipse(pen, knobX - diameter / 2f + inset, centerY - diameter / 2f + inset,
                        Math.Max(0f, diameter - style.KnobBorderWidth), Math.Max(0f, diameter - style.KnobBorderWidth));
                }
            }

            // The value's text in the column at the right.
            var font = element.Font;
            if (font == null || style.LabelWidth <= 0f)
                return;
            string text = element.ValueText;
            if (string.IsNullOrEmpty(text))
                return;

            float labelLeft = bounds.X + bounds.Width - style.LabelWidth;
            float lineHeight = g.MeasureString(LineMeasure, font).Height;
            g.SetClip(new RectangleF(labelLeft, bounds.Y, style.LabelWidth, bounds.Height));
            try
            {
                using var textBrush = GraphicsBackend.Factory.CreateSolidBrush(Fade(style.LabelColor, opacity));
                g.DrawString(text, font, textBrush, labelLeft, bounds.Y + (bounds.Height - lineHeight) / 2f);
            }
            finally
            {
                g.ResetClip();
            }
        }

        /// <summary>Fills a rounded rectangle whose corner radius is half its height (a pill).</summary>
        private static void FillPill(IGraphics g, float x, float y, float width, float height, Color color)
        {
            using var path = RoundedRectPath.Create(width, height, Math.Min(width, height) / 2f);
            using var matrix = GraphicsBackend.Factory.CreateMatrix();
            matrix.Translate(x, y);
            path.Transform(matrix);
            using var brush = GraphicsBackend.Factory.CreateSolidBrush(color);
            g.FillPath(brush, path);
        }

        /// <summary><paramref name="color"/> with its alpha multiplied by <paramref name="opacity"/>.</summary>
        private static Color Fade(Color color, float opacity) =>
            opacity >= 1f ? color : Color.FromArgb((int)MathF.Round(color.A * opacity), color);
    }
}
