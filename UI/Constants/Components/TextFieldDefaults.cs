/* ----- ----- ----- ----- */
// TextFieldDefaults.cs
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
    /// Defaults of <c>UITextField</c>: its size before any layout, the longest text, the caret
    /// blink period, and the style used when the field has none of its own (neutral placeholders).
    /// </summary>
    public static class TextFieldDefaults
    {
        /// <summary>Size of a new text field.</summary>
        public static readonly Vector2F Size = new Vector2F(240.0f, 40.0f);

        /// <summary>Longest text of a new field (characters).</summary>
        public const int MaxLength = 256;

        /// <summary>The caret is shown for this long, then hidden for this long (milliseconds).</summary>
        public const int CaretBlinkMilliseconds = 530;

        /// <summary>Corner radius of the default boxes.</summary>
        private const float CornerRadius = 4.0f;

        /// <summary>The style of a field whose <c>Style</c> is null.</summary>
        public static readonly TextFieldStyle Style = new TextFieldStyle
        {
            Box = new SingleBorderRoundedStyle
            {
                CornerRadius = CornerRadius,
                BorderStyle = new BorderStyle { Color = Color.FromArgb(255, 158, 158, 158), Width = 1.0f },  // #9E9E9E
                BackgroundBrushFactory = new SolidBrushFactory(Color.White),
            },
            FocusedBox = new SingleBorderRoundedStyle
            {
                CornerRadius = CornerRadius,
                BorderStyle = new BorderStyle { Color = Color.FromArgb(255, 33, 150, 243), Width = 2.0f },  // #2196F3
                BackgroundBrushFactory = new SolidBrushFactory(Color.White),
            },
            TextColor = Color.Black,
            PlaceholderColor = Color.FromArgb(255, 158, 158, 158),
            CaretColor = Color.Black,
            CaretWidth = 2.0f,
            TextInset = 8.0f,
            DisabledOpacity = 0.4f,
        };
    }
}
