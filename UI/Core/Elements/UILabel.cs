/* ----- ----- ----- ----- */
// UILabel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.UI.Constants;

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

        public UILabel() : base(type: UIElementType.Label)
        {

        }

        /// <summary>
        /// Draws the label's text.
        /// </summary>
        /// <param name="g">Graphics context</param>
        protected override void OnDraw(Graphics g)
        {
            if (string.IsNullOrEmpty(Text))
                return;

            RectangleF rect = GetCurrentAbsoluteBounds();
            using var brush = new SolidBrush(ForeColor);
            var stringFormat = GetStringFormat(TextAlign, WordWrap);

            g.DrawString(Text, Font, brush, rect, stringFormat);
        }

        /// <summary>
        /// Returns a StringFormat configured based on alignment and wrapping options.
        /// </summary>
        private StringFormat GetStringFormat(ContentAlignment align, bool wordWrap)
        {
            var format = new StringFormat
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
            return format;
        }
    }
}
