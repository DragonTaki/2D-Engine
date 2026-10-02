/* ----- ----- ----- ----- */
// DropdownDefaults.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Mathematics;
using Engine.Styles;

namespace Engine.UI.Constants.Components
{
    /// <summary>
    /// Defaults of <c>UIDropdown</c>: its size before any layout, where its open list sits among
    /// the overlay's children, and the style used when it has none of its own (neutral placeholders).
    /// </summary>
    public static class DropdownDefaults
    {
        /// <summary>Size of a new dropdown (the closed box).</summary>
        public static readonly Vector2F Size = new Vector2F(240.0f, 40.0f);

        /// <summary>
        /// Z-index of an open list on the overlay layer: above the screens, below a dialog and
        /// its mask (<c>DialogManager</c> uses <see cref="int.MaxValue"/> and one less).
        /// </summary>
        public const int ListZIndex = int.MaxValue - 2;

        /// <summary>Wheel delta of one notch (the WinForms convention, which the backends follow).</summary>
        public const int WheelNotchDelta = 120;

        /// <summary>Corner radius of the default boxes.</summary>
        private const float CornerRadius = 4.0f;

        /// <summary>The style of a dropdown whose <c>Style</c> is null.</summary>
        public static readonly DropdownStyle Style = new DropdownStyle
        {
            Box = new SingleBorderRoundedStyle
            {
                CornerRadius = CornerRadius,
                BorderStyle = new BorderStyle { Color = Color.FromArgb(255, 158, 158, 158), Width = 1.0f },  // #9E9E9E
                BackgroundBrushFactory = new SolidBrushFactory(Color.White),
            },
            OpenBox = new SingleBorderRoundedStyle
            {
                CornerRadius = CornerRadius,
                BorderStyle = new BorderStyle { Color = Color.FromArgb(255, 33, 150, 243), Width = 2.0f },  // #2196F3
                BackgroundBrushFactory = new SolidBrushFactory(Color.White),
            },
            ListBox = new SingleBorderRoundedStyle
            {
                CornerRadius = CornerRadius,
                BorderStyle = new BorderStyle { Color = Color.FromArgb(255, 158, 158, 158), Width = 1.0f },  // #9E9E9E
                BackgroundBrushFactory = new SolidBrushFactory(Color.White),
            },
            TextColor = Color.Black,
            SelectedTextColor = Color.FromArgb(255, 33, 150, 243),  // #2196F3
            HoverColor = Color.FromArgb(255, 227, 242, 253),        // #E3F2FD
            ArrowColor = Color.FromArgb(255, 97, 97, 97),           // #616161
            ArrowWidth = 12.0f,
            ArrowInset = 12.0f,
            TextInset = 10.0f,
            ItemHeight = 36.0f,
            MaxVisibleItems = 8,
            ListGap = 4.0f,
            ListPadding = 4.0f,
            ScrollBarColor = Color.FromArgb(128, 0, 0, 0),
            ScrollBarWidth = 4.0f,
            DisabledOpacity = 0.4f,
        };
    }
}
