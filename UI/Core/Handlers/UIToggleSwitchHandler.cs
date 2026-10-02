/* ----- ----- ----- ----- */
// UIToggleSwitchHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.Platform;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    /// <summary>
    /// Handler for <see cref="UIToggleSwitch"/>: a click flips the switch while it is enabled
    /// and reports the new state through <see cref="ValueChanged"/>.
    /// </summary>
    public class UIToggleSwitchHandler : UIHandler<UIToggleSwitch, UIToggleSwitchHandler, UIToggleSwitchRenderer>
    {
#nullable enable
        /// <summary>Invoked with the new state after a click (or <see cref="Toggle"/>) flipped the switch.</summary>
        public Action<bool>? ValueChanged { get; set; }
#nullable disable

        public UIToggleSwitchHandler() { }

        /// <summary>
        /// Flips the switch and invokes <see cref="ValueChanged"/> with the new state; does
        /// nothing while the switch is disabled.
        /// </summary>
        /// <returns>Whether the switch was flipped.</returns>
        public bool Toggle()
        {
            if (!Element.IsEnabled)
                return false;

            Element.IsOn = !Element.IsOn;
            ValueChanged?.Invoke(Element.IsOn);
            return true;
        }

        #region Mouse Handling

        internal override bool HandleMouseClick(IMouseEvent e) => Toggle();

        #endregion
    }
}
