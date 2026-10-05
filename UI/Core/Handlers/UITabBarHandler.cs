/* ----- ----- ----- ----- */
// UITabBarHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Engine.Platform;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    /// <summary>
    /// Handler for <see cref="UITabBar"/>: a click on a tab other than the selected one
    /// selects it and reports the new index through <see cref="SelectionChanged"/>.
    /// </summary>
    public class UITabBarHandler : UIHandler<UITabBar, UITabBarHandler, UITabBarRenderer>
    {
#nullable enable
        /// <summary>Invoked with the new index after a click (or <see cref="Select"/>) changed the selected tab.</summary>
        public Action<int>? SelectionChanged { get; set; }
#nullable disable

        public UITabBarHandler() { }

        /// <summary>
        /// Selects tab <paramref name="index"/> and invokes <see cref="SelectionChanged"/>;
        /// nothing happens when it already is the selected tab or the bar is disabled.
        /// </summary>
        /// <returns>Whether the selection changed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is not a tab's.</exception>
        public bool Select(int index)
        {
            if (!Element.IsEnabled || index == Element.SelectedIndex)
                return false;

            Element.SelectedIndex = index;
            SelectionChanged?.Invoke(index);
            return true;
        }

        #region Mouse Handling

        /// <summary>A click on a tab selects it; a click in a gap between tabs is still taken (it is on the bar).</summary>
        protected internal override bool HandleMouseClick(IMouseEvent e)
        {
            if (!Element.IsEnabled)
                return false;

            int index = Element.TabIndexAt(e.Location);
            if (index >= 0)
                Select(index);
            return true;
        }

        #endregion
    }
}
