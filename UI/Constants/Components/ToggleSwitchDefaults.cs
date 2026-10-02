/* ----- ----- ----- ----- */
// ToggleSwitchDefaults.cs
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
    /// Defaults of <c>UIToggleSwitch</c>: its size before any layout, and the style used
    /// when the switch has none of its own. Neutral placeholders; an app sets its own style.
    /// </summary>
    public static class ToggleSwitchDefaults
    {
        /// <summary>Size of a new switch (track width and height).</summary>
        public static readonly Vector2F Size = new Vector2F(64.0f, 32.0f);

        /// <summary>The style of a switch whose <c>Style</c> is null.</summary>
        public static readonly ToggleSwitchStyle Style = new ToggleSwitchStyle
        {
            TrackOnColor = Color.FromArgb(255, 76, 175, 80),     // #4CAF50
            TrackOffColor = Color.FromArgb(255, 158, 158, 158),  // #9E9E9E
            KnobColor = Color.White,
            BorderColor = Color.FromArgb(64, 0, 0, 0),
            BorderWidth = 1.0f,
            KnobInset = 3.0f,
            DisabledOpacity = 0.4f,
        };
    }
}
