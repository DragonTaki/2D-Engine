/* ----- ----- ----- ----- */
// WinFormsFontFamily.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Platform.WinForms
{
    /// <summary>GDI+-backed <see cref="IFontFamily"/>.</summary>
    internal sealed class WinFormsFontFamily : IFontFamily
    {
        public FontFamily Native { get; }

        public WinFormsFontFamily(FontFamily native)
        {
            Native = native;
        }

        public string Name => Native.Name;
    }
}
