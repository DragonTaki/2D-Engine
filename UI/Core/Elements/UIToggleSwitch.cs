/* ----- ----- ----- ----- */
// UIToggleSwitch.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Styles;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// An on/off switch: a track with a knob at its left (off) or right (on) end. A click
    /// flips it and runs the handler's <see cref="UIToggleSwitchHandler.ValueChanged"/>; it
    /// draws itself (<see cref="UIToggleSwitchRenderer"/>) with <see cref="Style"/>.
    /// <para>
    /// Disabled (<see cref="Bases.UIElementBase.IsEnabled"/> false): still drawn, faded by
    /// <see cref="ToggleSwitchStyle.DisabledOpacity"/>, and ignores clicks (it is not hit-tested,
    /// so a click falls through to its parent).
    /// </para>
    /// </summary>
    public class UIToggleSwitch : UIElement<UIToggleSwitch, UIToggleSwitchHandler, UIToggleSwitchRenderer>
    {
        #region Properties

        /// <summary>
        /// Whether the switch is on. Setting it only changes what is shown (no
        /// <see cref="UIToggleSwitchHandler.ValueChanged"/>); a click goes through
        /// <see cref="UIToggleSwitchHandler.Toggle"/>, which does notify.
        /// </summary>
        public bool IsOn { get; set; }

        /// <summary>How the switch is drawn; null = <see cref="ToggleSwitchDefaults.Style"/>.</summary>
        public ToggleSwitchStyle Style { get; set; }

        #endregion

        #region Constructors

        /// <summary>Creates a switch that is off.</summary>
        public UIToggleSwitch()
            : base(zIndex: 0, isPersistent: false, type: UIElementType.Generic)
        {
        }

        #endregion

        /// <summary>Applies the default size (a layout style replaces it).</summary>
        protected override void OnInit()
        {
            Size = ToggleSwitchDefaults.Size;
        }
    }
}
