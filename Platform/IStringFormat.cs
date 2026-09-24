/* ----- ----- ----- ----- */
// IStringFormat.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Platform
{
    /// <summary>
    /// Text alignment along one axis, mirroring the subset of GDI+'s
    /// <c>StringAlignment</c> actually used here.
    /// </summary>
    public enum TextAlign
    {
        Near,
        Center,
        Far
    }

    /// <summary>
    /// A 9-way anchor within a box, mirroring the subset of WinForms'
    /// <c>ContentAlignment</c> actually used here (e.g. for
    /// <c>UILabel.TextAlign</c>).
    /// </summary>
    public enum ContentAlign
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    /// <summary>
    /// Text layout options passed to
    /// <see cref="IGraphics.DrawString(string, IFont, IBrush, System.Drawing.RectangleF, IStringFormat)"/>.
    /// Created via <see cref="IGraphics.CreateStringFormat"/>.
    /// </summary>
    public interface IStringFormat : IDisposable
    {
        TextAlign Alignment { get; set; }
        TextAlign LineAlignment { get; set; }
        bool WordWrap { get; set; }

        /// <summary>Whether overflowing text is truncated with an ellipsis ("…").</summary>
        bool EllipsisTrimming { get; set; }
    }
}
