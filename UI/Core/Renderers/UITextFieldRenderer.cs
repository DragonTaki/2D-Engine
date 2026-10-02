/* ----- ----- ----- ----- */
// UITextFieldRenderer.cs
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
    /// Renderer for <see cref="UITextField"/>: the box (the focused box while focused), then -
    /// clipped to the text area (the box minus the style's inset) - the text or the placeholder,
    /// vertically centered and scrolled so the caret stays visible, and the blinking caret while
    /// focused.
    /// </summary>
    public class UITextFieldRenderer : UIRenderer<UITextField, UITextFieldHandler, UITextFieldRenderer>
    {
        /// <summary>Character measured for the line height (so an empty field has one).</summary>
        private const string LineMeasure = "|";

        public UITextFieldRenderer() { }

        public override void OnRender(IGraphics g, UITextField element)
        {
            var style = element.Style ?? TextFieldDefaults.Style;
            var bounds = element.GetCurrentAbsoluteBounds();
            if (bounds.Width <= 0f || bounds.Height <= 0f)
                return;

            var box = element.IsFocused ? (style.FocusedBox ?? style.Box) : style.Box;
            box?.Draw(g, bounds);

            var font = element.Font;
            if (font == null)
                return;

            float innerX = bounds.X + style.TextInset;
            float innerWidth = Math.Max(0f, bounds.Width - style.TextInset * 2f);
            if (innerWidth <= 0f)
                return;

            float opacity = element.IsEnabled ? 1f : Math.Clamp(style.DisabledOpacity, 0f, 1f);
            string text = element.Text;
            bool placeholder = text.Length == 0 && !element.IsFocused && !string.IsNullOrEmpty(element.Placeholder);
            string shown = placeholder ? element.Placeholder : text;

            float lineHeight = g.MeasureString(LineMeasure, font).Height;
            float y = bounds.Y + (bounds.Height - lineHeight) / 2f;

            // Keep the caret inside the text area: scroll just enough, and never past the text's end.
            float caretX = placeholder ? 0f : UITextField.MeasurePrefix(g, text, element.CaretIndex, font);
            float textWidth = placeholder ? 0f : UITextField.MeasurePrefix(g, text, text.Length, font);
            float scroll = element.TextScroll;
            if (caretX - scroll > innerWidth)
                scroll = caretX - innerWidth;
            if (caretX - scroll < 0f)
                scroll = caretX;
            scroll = Math.Clamp(scroll, 0f, Math.Max(0f, textWidth - innerWidth));
            element.TextScroll = scroll;

            // The clip is widened by the caret's width so a caret at either edge is not cut in half.
            g.SetClip(new RectangleF(innerX - style.CaretWidth, bounds.Y, innerWidth + style.CaretWidth * 2f, bounds.Height));
            try
            {
                if (shown.Length > 0)
                {
                    using var brush = GraphicsBackend.Factory.CreateSolidBrush(Fade(placeholder ? style.PlaceholderColor : style.TextColor, opacity));
                    g.DrawString(shown, font, brush, innerX - scroll, y);
                }

                bool caretOn = (Environment.TickCount64 - element.CaretMovedAt) / TextFieldDefaults.CaretBlinkMilliseconds % 2 == 0;
                if (element.IsFocused && caretOn && style.CaretWidth > 0f)
                {
                    using var caretBrush = GraphicsBackend.Factory.CreateSolidBrush(style.CaretColor);
                    g.FillRectangle(caretBrush, innerX + caretX - scroll - style.CaretWidth / 2f, y, style.CaretWidth, lineHeight);
                }
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
