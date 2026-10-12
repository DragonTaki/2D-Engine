/* ----- ----- ----- ----- */
// CheckboxStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Styles
{
    /// <summary>Colours and dimensions of a checkbox and its right-hand label.</summary>
    public class CheckboxStyle
    {
        public Color CheckedColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color UncheckedColor { get; set; } = Color.FromArgb(158, 158, 158);
        public Color BorderColor { get; set; } = Color.Gray;
        public Color CheckColor { get; set; } = Color.White;
        public Color TextColor { get; set; } = Color.Black;
        public Color FocusColor { get; set; } = Color.Gold;
        public float BoxSize { get; set; } = 24f;
        public float LabelGap { get; set; } = 12f;
        public float BorderWidth { get; set; } = 1f;
        public float CheckWidth { get; set; } = 2f;
        public float DisabledOpacity { get; set; } = 0.4f;
    }
}
