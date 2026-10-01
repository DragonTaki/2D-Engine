/* ----- ----- ----- ----- */
// ButtonText.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.Platform;

namespace Engine.Styles
{
    /// <summary>
    /// Button label drawing shared by the button draw styles: the text centered in the
    /// button, one line per explicit line break.
    /// </summary>
    public static class ButtonText
    {
        /// <summary>
        /// Draws <paramref name="text"/> centered in <paramref name="bounds"/>. Each line
        /// (split on <c>\n</c> / <c>\r\n</c>) is measured and centered on its own, and the
        /// block of lines is centered vertically. A single line is placed exactly as the
        /// styles always did.
        /// <para>
        /// Line by line because the point overload of <see cref="IGraphics.DrawString(string, IFont, IBrush, float, float)"/>
        /// doesn't break lines on the Skia backend (only the rectangle overload does), and
        /// GDI+ would left-align the lines inside the block instead of centering each one.
        /// </para>
        /// </summary>
        public static void DrawCentered(IGraphics g, string text, IFont font, IBrush brush, LayoutF bounds)
        {
            // Text needs both a brush and a font (MeasureString/DrawString with a null font throw).
            if (string.IsNullOrEmpty(text) || brush == null || font == null)
                return;

            var lines = text.Replace("\r\n", "\n").Split('\n');
            var widths = new float[lines.Length];
            var heights = new float[lines.Length];
            float totalHeight = 0f;
            for (int i = 0; i < lines.Length; i++)
            {
                // An empty line still takes a line's height.
                var size = g.MeasureString(lines[i].Length == 0 ? " " : lines[i], font);
                widths[i] = lines[i].Length == 0 ? 0f : size.Width;
                heights[i] = size.Height;
                totalHeight += size.Height;
            }

            float y = bounds.Position.Y + (bounds.Size.Y - totalHeight) / 2f;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > 0)
                {
                    float x = bounds.Position.X + (bounds.Size.X - widths[i]) / 2f;
                    g.DrawString(lines[i], font, brush, x, y);
                }
                y += heights[i];
            }
        }
    }
}
