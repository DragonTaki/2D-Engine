/* ----- ----- ----- ----- */
// SliderDefaults.cs
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
    /// Defaults of <c>UISlider</c>: its size before any layout, its range, and the style used
    /// when it has none of its own (neutral placeholders; an app sets its own style).
    /// </summary>
    public static class SliderDefaults
    {
        /// <summary>Size of a new slider (track and value column).</summary>
        public static readonly Vector2F Size = new Vector2F(320.0f, 32.0f);

        /// <summary>Range and step of a new slider.</summary>
        public const float Minimum = 0.0f, Maximum = 100.0f, Step = 1.0f;

        /// <summary>The style of a slider whose <c>Style</c> is null.</summary>
        public static readonly SliderStyle Style = new SliderStyle
        {
            TrackColor = Color.FromArgb(255, 189, 189, 189),  // #BDBDBD
            FillColor = Color.FromArgb(255, 33, 150, 243),    // #2196F3
            KnobColor = Color.White,
            KnobBorderColor = Color.FromArgb(255, 33, 150, 243),  // #2196F3
            KnobBorderWidth = 2.0f,
            TrackHeight = 4.0f,
            KnobDiameter = 18.0f,
            LabelColor = Color.Black,
            LabelWidth = 64.0f,
            LabelGap = 12.0f,
            DisabledOpacity = 0.4f,
        };
    }
}
