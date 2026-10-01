/* ----- ----- ----- ----- */
// UILabelRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.Platform;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UILabel"/>. Draws the label's text (or its inline text
    /// fragments) within the label's absolute bounds, honoring TextAlign, WordWrap and ClipRect.
    /// </summary>
    public class UILabelRenderer : UIRenderer<UILabel, UILabelHandler, UILabelRenderer>
    {
        private UILabel Label => (UILabel)Element;

        #region Constructor

        public UILabelRenderer() { }

        #endregion

        /// <summary>
        /// Returns a StringFormat configured based on alignment and wrapping options.
        /// </summary>
        public IStringFormat GetStringFormat(ContentAlign align, bool wordWrap)
        {
            if (Label.CachedFormat != null && Label.LastAlign == align && Label.LastWrap == wordWrap)
                return Label.CachedFormat;

            Label.CachedFormat?.Dispose();
            var format = GraphicsBackend.Factory.CreateStringFormat();
            format.WordWrap = wordWrap;
            format.EllipsisTrimming = true;
            format.LineAlignment = align switch
            {
                ContentAlign.TopLeft or ContentAlign.TopCenter or ContentAlign.TopRight => TextAlign.Near,
                ContentAlign.MiddleLeft or ContentAlign.MiddleCenter or ContentAlign.MiddleRight => TextAlign.Center,
                _ => TextAlign.Far,
            };
            format.Alignment = align switch
            {
                ContentAlign.TopLeft or ContentAlign.MiddleLeft or ContentAlign.BottomLeft => TextAlign.Near,
                ContentAlign.TopCenter or ContentAlign.MiddleCenter or ContentAlign.BottomCenter => TextAlign.Center,
                _ => TextAlign.Far,
            };
            Label.CachedFormat = format;
            Label.LastAlign = align;
            Label.LastWrap = wordWrap;
            return Label.CachedFormat;
        }

        public IBrush GetBrush()
        {
            if (Label.CachedBrush == null || Label.LastForeColor != Label.ForeColor)
            {
                Label.CachedBrush?.Dispose();
                Label.CachedBrush = GraphicsBackend.Factory.CreateSolidBrush(Label.ForeColor);
                Label.LastForeColor = Label.ForeColor;
            }
            return Label.CachedBrush;
        }

        /// <summary>
        /// Draws the label's text fragments as one line of consecutive runs, each in its own
        /// color and bold/italic style, aligned within <paramref name="rect"/> by TextAlign.
        /// (Fragments are inline runs - e.g. AppLogger's welcome message is one fragment
        /// per character - but they used to be stacked one per line, in the plain font.)
        /// </summary>
        private void DrawFragmentsInline(IGraphics g, RectangleF rect)
        {
            var fragments = Label.Fragments;
            var widths = new float[fragments.Count];
            float totalWidth = 0f, lineHeight = 0f;

            for (int i = 0; i < fragments.Count; i++)
            {
                var font = Label.GetFragmentFont(fragments[i].Bold, fragments[i].Italic);
                var size = g.MeasureString(fragments[i].Text ?? string.Empty, font);
                widths[i] = size.Width;
                totalWidth += size.Width;
                lineHeight = Math.Max(lineHeight, size.Height);
            }

            float x = Label.TextAlign switch
            {
                ContentAlign.TopCenter or ContentAlign.MiddleCenter or ContentAlign.BottomCenter => rect.X + (rect.Width - totalWidth) / 2f,
                ContentAlign.TopRight or ContentAlign.MiddleRight or ContentAlign.BottomRight => rect.Right - totalWidth,
                _ => rect.X,
            };
            float y = Label.TextAlign switch
            {
                ContentAlign.MiddleLeft or ContentAlign.MiddleCenter or ContentAlign.MiddleRight => rect.Y + (rect.Height - lineHeight) / 2f,
                ContentAlign.BottomLeft or ContentAlign.BottomCenter or ContentAlign.BottomRight => rect.Bottom - lineHeight,
                _ => rect.Y,
            };

            for (int i = 0; i < fragments.Count; i++)
            {
                if (!string.IsNullOrEmpty(fragments[i].Text))
                {
                    var font = Label.GetFragmentFont(fragments[i].Bold, fragments[i].Italic);
                    using var brush = GraphicsBackend.Factory.CreateSolidBrush(fragments[i].Color);
                    g.DrawString(fragments[i].Text, font, brush, x, y);
                }
                x += widths[i];
            }
        }

        #region Rendering

        /// <summary>
        /// Renders the label: inline fragments if any are set, otherwise the plain text.
        /// </summary>
        /// <param name="g">The <see cref="IGraphics"/> surface to draw on.</param>
        /// <param name="element">The label element being rendered.</param>
        public override void OnRender(IGraphics g, UILabel element)
        {
            bool clipped = element.ClipRect.HasValue;
            if (clipped)
                g.SetClip(element.ClipRect.Value);

            // try/finally: the fragments branch returns early, and an unpaired SetClip
            // leaks the clip on GDI+ and unbalances Skia's canvas save stack.
            try
            {
                RectangleF rect = element.GetCurrentAbsoluteBounds();

                if (Label.Fragments != null && Label.Fragments.Count > 0)
                {
                    DrawFragmentsInline(g, rect);
                    return;
                }

                // Original plain-text mode (no fragments)
                if (!string.IsNullOrEmpty(Label.Text))
                {
                    using (var brush = GraphicsBackend.Factory.CreateSolidBrush(Color.FromArgb(128, Color.Red))) // Semi-transparent red (debug background behind the text)
                    {
                        g.FillRectangle(brush, rect);
                    }
                    g.DrawString(Label.Text, Label.Font, GetBrush(), rect, GetStringFormat(Label.TextAlign, Label.WordWrap));
                }
            }
            finally
            {
                if (clipped)
                    g.ResetClip();
            }
        }

        #endregion
    }
}
