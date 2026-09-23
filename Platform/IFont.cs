/* ----- ----- ----- ----- */
// IFont.cs
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
    /// Style flags for a font, mirroring the subset of GDI+'s <c>FontStyle</c>
    /// actually used in this codebase. Combinable, e.g. <c>Bold | Italic</c>.
    /// </summary>
    [Flags]
    public enum FontStyleFlags
    {
        Regular = 0,
        Bold = 1,
        Italic = 2
    }

    /// <summary>
    /// A font family (typeface) independent of size/style, e.g. a loaded
    /// custom font or a named system font.
    /// </summary>
    public interface IFontFamily
    {
        string Name { get; }
    }

    /// <summary>
    /// A concrete font (family + size + style) used by <see cref="IGraphics"/>
    /// text operations. Always measured in pixels. Created via
    /// <see cref="IGraphics.CreateFont(IFontFamily, float, FontStyleFlags)"/>.
    /// </summary>
    public interface IFont : IDisposable
    {
        IFontFamily FontFamily { get; }
        float Size { get; }
        FontStyleFlags Style { get; }

        /// <summary>Line height in pixels.</summary>
        float Height { get; }
    }
}
