/* ----- ----- ----- ----- */
// SliderStyle.cs
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
    /// Looks of a slider (<c>UISlider</c>): a thin rounded track, its part left of the knob
    /// filled, a round knob at the value, and the value's text in a column at the right.
    /// </summary>
    public class SliderStyle
    {
        /// <summary>Fill of the track (the part right of the knob).</summary>
        public Color TrackColor { get; set; }

        /// <summary>Fill of the track's part left of the knob (min to the value).</summary>
        public Color FillColor { get; set; }

        /// <summary>Knob fill.</summary>
        public Color KnobColor { get; set; }

        /// <summary>Knob outline colour (not drawn when <see cref="KnobBorderWidth"/> is 0).</summary>
        public Color KnobBorderColor { get; set; }

        /// <summary>Knob outline width (design units; 0 = no outline).</summary>
        public float KnobBorderWidth { get; set; }

        /// <summary>Height of the track (design units).</summary>
        public float TrackHeight { get; set; }

        /// <summary>Diameter of the knob (design units); the track is inset by half of it on both ends so the knob stays inside.</summary>
        public float KnobDiameter { get; set; }

        /// <summary>Colour of the value's text.</summary>
        public Color LabelColor { get; set; }

        /// <summary>Width of the value's column at the slider's right (design units; 0 = no text).</summary>
        public float LabelWidth { get; set; }

        /// <summary>Space between the track's right end (knob included) and the value's column (design units).</summary>
        public float LabelGap { get; set; }

        /// <summary>Opacity (0 to 1) every colour is multiplied by while the slider is disabled.</summary>
        public float DisabledOpacity { get; set; }
    }
}
