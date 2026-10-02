/* ----- ----- ----- ----- */
// ToggleSwitchStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Styles
{
    /// <summary>
    /// Looks of a toggle switch (<c>UIToggleSwitch</c>): a pill-shaped track whose colour
    /// shows the state, and a round knob at the track's left (off) or right (on) end.
    /// </summary>
    public class ToggleSwitchStyle
    {
        /// <summary>Track fill while the switch is on.</summary>
        public Color TrackOnColor { get; set; }

        /// <summary>Track fill while the switch is off.</summary>
        public Color TrackOffColor { get; set; }

        /// <summary>Knob fill.</summary>
        public Color KnobColor { get; set; }

        /// <summary>Track outline colour (not drawn when <see cref="BorderWidth"/> is 0).</summary>
        public Color BorderColor { get; set; }

        /// <summary>Track outline width (design units; 0 = no outline).</summary>
        public float BorderWidth { get; set; }

        /// <summary>Gap between the knob and the track's edges (design units).</summary>
        public float KnobInset { get; set; }

        /// <summary>Opacity (0 to 1) every colour is multiplied by while the switch is disabled.</summary>
        public float DisabledOpacity { get; set; }
    }
}
