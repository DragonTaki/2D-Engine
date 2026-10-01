/* ----- ----- ----- ----- */
// SkiaStringFormat.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Platform.Skia
{
    /// <summary>
    /// Plain data holder for <see cref="IStringFormat"/> — SkiaSharp has no
    /// equivalent object; <see cref="SkiaGraphics"/> reads these fields
    /// directly when laying out a <c>DrawString(..., RectangleF, IStringFormat)</c>
    /// call. <see cref="WordWrap"/> is implemented (see
    /// <c>SkiaGraphics.WrapText</c>), and so is <see cref="EllipsisTrimming"/>
    /// (see <c>SkiaGraphics.TrimWithEllipsis</c>).
    /// </summary>
    internal sealed class SkiaStringFormat : IStringFormat
    {
        public TextAlign Alignment { get; set; } = TextAlign.Near;
        public TextAlign LineAlignment { get; set; } = TextAlign.Near;
        // Defaults to true to match a fresh GDI+ StringFormat (NoWrap unset).
        public bool WordWrap { get; set; } = true;
        public bool EllipsisTrimming { get; set; }

        public void Dispose() { }
    }
}
