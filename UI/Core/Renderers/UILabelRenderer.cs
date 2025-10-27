/* ----- ----- ----- ----- */
// UILabelRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UILabel{THandler}"/>. Handles the drawing
    /// of container elements, optionally delegating to child elements or applying
    /// container-specific visual effects.
    /// </summary>
    /// <typeparam name="THandler">The type of container handler this renderer is associated with.</typeparam>
    public class UILabelRenderer : UIRenderer<UILabel, UILabelHandler, UILabelRenderer>
    {
        private UILabel Label => (UILabel)Element;

        #region Constructor

        public UILabelRenderer() { }

        #endregion

        /// <summary>
        /// Returns a StringFormat configured based on alignment and wrapping options.
        /// </summary>
        public StringFormat GetStringFormat(ContentAlignment align, bool wordWrap)
        {
            if (Label._cachedFormat != null && Label._lastAlign == align && Label._lastWrap == wordWrap)
                return Label._cachedFormat;

            Label._cachedFormat?.Dispose();
            Label._cachedFormat = new StringFormat
            {
                FormatFlags = wordWrap ? 0 : StringFormatFlags.NoWrap,
                Trimming = StringTrimming.EllipsisCharacter,
                LineAlignment = align switch
                {
                    ContentAlignment.TopLeft or ContentAlignment.TopCenter or ContentAlignment.TopRight => StringAlignment.Near,
                    ContentAlignment.MiddleLeft or ContentAlignment.MiddleCenter or ContentAlignment.MiddleRight => StringAlignment.Center,
                    _ => StringAlignment.Far,
                },
                Alignment = align switch
                {
                    ContentAlignment.TopLeft or ContentAlignment.MiddleLeft or ContentAlignment.BottomLeft => StringAlignment.Near,
                    ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter => StringAlignment.Center,
                    _ => StringAlignment.Far,
                }
            };
            Label._lastAlign = align;
            Label._lastWrap = wordWrap;
            return Label._cachedFormat;
        }

        public Brush GetBrush()
        {
            if (Label._cachedBrush == null || Label._cachedBrush.Color != Label.ForeColor)
            {
                Label._cachedBrush?.Dispose();
                Label._cachedBrush = new SolidBrush(Label.ForeColor);
            }
            return Label._cachedBrush;
        }

        #region Rendering

        /// <summary>
        /// Performs the rendering of the container and its child elements.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object to draw on.</param>
        /// <param name="element">The UI element being rendered (should match <see cref="Container"/>).</param>
        protected override void OnRender(Graphics g, UILabel element)
        {
            var _label = (UILabel)element;

            if (_label.ClipRect.HasValue)
                g.SetClip(_label.ClipRect.Value);

            RectangleF rect = _label.GetCurrentAbsoluteBounds();

            if (Label._fragments != null && Label._fragments.Count > 0)
            {
                float y = rect.Y;
                foreach (var frag in Label._fragments)
                {
                    using var brush = new SolidBrush(frag.Color);
                    g.DrawString(frag.Text, Label.Font, brush, rect.X, y);
                    y += Label.Font.Height; // 或使用 frag.LineHeight
                }
                return;
            }

            // 原本單純文字模式
            if (!string.IsNullOrEmpty(Label.Text))
            {
                using (var brush = new SolidBrush(Color.FromArgb(128, Color.Red))) // 半透明紅色
                {
                    g.FillRectangle(brush, rect);
                }
                g.DrawString(Label.Text, Label.Font, GetBrush(), rect, GetStringFormat(Label.TextAlign, Label.WordWrap));
            }

            if (element.ClipRect.HasValue)
                g.ResetClip();
        }

        #endregion
    }
}
