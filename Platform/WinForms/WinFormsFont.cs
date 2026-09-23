/* ----- ----- ----- ----- */
// WinFormsFont.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Platform.WinForms
{
    /// <summary>GDI+-backed <see cref="IFont"/>.</summary>
    internal sealed class WinFormsFont : IFont
    {
        public Font Native { get; }
        public IFontFamily FontFamily { get; }

        public WinFormsFont(Font native, IFontFamily fontFamily)
        {
            Native = native;
            FontFamily = fontFamily;
        }

        public float Size => Native.Size;
        public float Height => Native.Height;

        public FontStyleFlags Style
        {
            get
            {
                var flags = FontStyleFlags.Regular;
                if (Native.Bold) flags |= FontStyleFlags.Bold;
                if (Native.Italic) flags |= FontStyleFlags.Italic;
                return flags;
            }
        }

        public void Dispose() => Native.Dispose();
    }
}
