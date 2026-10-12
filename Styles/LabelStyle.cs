/* ----- ----- ----- ----- */
// LabelStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Platform;

namespace Engine.Styles
{
    /// <summary>Reusable label typography. The caller owns the font returned by CreateFont.</summary>
    public sealed class LabelStyle
    {
        public float FontSize { get; set; } = 10f;
        public FontStyleFlags FontWeight { get; set; } = FontStyleFlags.Regular;
        public Color TextColor { get; set; } = Color.Black;
        public ContentAlign Alignment { get; set; } = ContentAlign.MiddleCenter;

        public IFont CreateFont() => GraphicsBackend.Factory.CreateFont(
            GraphicsBackend.Factory.GenericSansSerifFontFamily, FontSize, FontWeight);
    }
}
