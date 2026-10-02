/* ----- ----- ----- ----- */
// UIToggleSwitchRenderer.cs
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

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UIToggleSwitch"/>: the pill-shaped track (filled with the on or
    /// off colour, outlined), then the round knob at the left (off) or right (on) end. A
    /// disabled switch is drawn with every colour faded by the style's disabled opacity.
    /// </summary>
    public class UIToggleSwitchRenderer : UIRenderer<UIToggleSwitch, UIToggleSwitchHandler, UIToggleSwitchRenderer>
    {
        public UIToggleSwitchRenderer() { }

        public override void OnRender(IGraphics g, UIToggleSwitch element)
        {
            var style = element.Style ?? ToggleSwitchDefaults.Style;
            var bounds = element.GetCurrentAbsoluteBounds();
            if (bounds.Width <= 0f || bounds.Height <= 0f)
                return;

            float opacity = element.IsEnabled ? 1f : Math.Clamp(style.DisabledOpacity, 0f, 1f);

            // Track: a rounded rectangle whose corner radius is half its height (a pill),
            // inset by half the outline so the outline stays inside the bounds.
            var track = bounds.Inset(style.BorderWidth / 2f);
            using (var path = RoundedRectPath.Create(track.Width, track.Height, track.Height / 2f))
            using (var matrix = GraphicsBackend.Factory.CreateMatrix())
            {
                matrix.Translate(track.X, track.Y);
                path.Transform(matrix);

                using var trackBrush = GraphicsBackend.Factory.CreateSolidBrush(
                    Fade(element.IsOn ? style.TrackOnColor : style.TrackOffColor, opacity));
                g.FillPath(trackBrush, path);

                if (style.BorderWidth > 0f)
                {
                    using var pen = GraphicsBackend.Factory.CreatePen(Fade(style.BorderColor, opacity), style.BorderWidth);
                    g.DrawPath(pen, path);
                }
            }

            // Knob: a circle filling the track's height minus the inset on both sides.
            float diameter = Math.Max(0f, bounds.Height - style.KnobInset * 2f);
            if (diameter <= 0f)
                return;
            float x = element.IsOn
                ? bounds.X + bounds.Width - style.KnobInset - diameter
                : bounds.X + style.KnobInset;
            float y = bounds.Y + style.KnobInset;

            using var knobBrush = GraphicsBackend.Factory.CreateSolidBrush(Fade(style.KnobColor, opacity));
            g.FillEllipse(knobBrush, x, y, diameter, diameter);
        }

        /// <summary><paramref name="color"/> with its alpha multiplied by <paramref name="opacity"/>.</summary>
        private static Color Fade(Color color, float opacity) =>
            opacity >= 1f ? color : Color.FromArgb((int)MathF.Round(color.A * opacity), color);
    }
}
