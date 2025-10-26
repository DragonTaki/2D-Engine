/* ----- ----- ----- ----- */
// UILabel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/10/25
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

using Engine.UI.Constants.Components;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Represents a basic label UI element for displaying text.
    /// </summary>
    public class UILabel : UIElement
    {
        /// <summary>
        /// Text content to be displayed.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Font used to render the text.
        /// </summary>
        public Font Font { get; set; } = SystemFonts.DefaultFont;

        /// <summary>
        /// Color of the text.
        /// </summary>
        public Color ForeColor { get; set; } = Color.Black;

        /// <summary>
        /// Text alignment within the label bounds.
        /// </summary>
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleCenter;

        /// <summary>
        /// Indicates whether text wrapping is enabled.
        /// </summary>
        public bool WordWrap { get; set; } = true;

#nullable enable
        private SolidBrush? _cachedBrush;

        private StringFormat? _cachedFormat;
#nullable disable

        private ContentAlignment _lastAlign;

        private bool _lastWrap;
        public bool IsSelectable { get; set; } = false;

        public int SelectionStart { get; private set; }

        public int SelectionEnd { get; private set; }

#nullable enable
        public List<TextFragment>? _fragments;
#nullable disable

        public UILabel() : base(type: UIElementType.Label)
        {

        }

        public void SetTextFragments(List<TextFragment> fragments)
        {
            _fragments = fragments;
        }

        /// <summary>
        /// Returns a StringFormat configured based on alignment and wrapping options.
        /// </summary>
        public StringFormat GetStringFormat(ContentAlignment align, bool wordWrap)
        {
            if (_cachedFormat != null && _lastAlign == align && _lastWrap == wordWrap)
                return _cachedFormat;

            _cachedFormat?.Dispose();
            _cachedFormat = new StringFormat
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
            _lastAlign = align;
            _lastWrap = wordWrap;
            return _cachedFormat;
        }

        public Brush GetBrush()
        {
            if (_cachedBrush == null || _cachedBrush.Color != ForeColor)
            {
                _cachedBrush?.Dispose();
                _cachedBrush = new SolidBrush(ForeColor);
            }
            return _cachedBrush;
        }

        /// <summary>
        /// Draws the label's text.
        /// </summary>
        /// <param name="g">Graphics context</param>
        protected override void OnDraw(Graphics g)
        {
            if (ClipRect.HasValue)
                g.SetClip(ClipRect.Value);

            RectangleF rect = GetCurrentAbsoluteBounds();

            if (_fragments != null && _fragments.Count > 0)
            {
                float y = rect.Y;
                foreach (var frag in _fragments)
                {
                    using var brush = new SolidBrush(frag.Color);
                    g.DrawString(frag.Text, Font, brush, rect.X, y);
                    y += Font.Height; // 或使用 frag.LineHeight
                }
                return;
            }

            // 原本單純文字模式
            if (!string.IsNullOrEmpty(Text))
            {
                using (var brush = new SolidBrush(Color.FromArgb(128, Color.Red))) // 半透明紅色
                {
                    g.FillRectangle(brush, rect);
                }
                g.DrawString(Text, Font, GetBrush(), rect, GetStringFormat(TextAlign, WordWrap));
            }

            if (ClipRect.HasValue)
                g.ResetClip();
        }

        /*protected override bool HandleMouseDown(MouseEventArgs e)
        {
            if (!IsSelectable) return false;

            SelectionStart = GetCharIndexAtPoint(e.Location);
            SelectionEnd = SelectionStart;
            return true; // 表示事件已處理，不再往上傳遞
        }*/

        public override void Dispose()
        {
            _cachedBrush?.Dispose();
            _cachedFormat?.Dispose();
            base.Dispose();
        }

    }
}
