/* ----- ----- ----- ----- */
// UIMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2026/09/30
// Version: v1.2
/* ----- ----- ----- ----- */

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    public class UIMenuHandler<TElement, THandler, TRenderer> : UIContainerHandler<TElement, THandler, TRenderer>
        where TElement : UIMenu<TElement, THandler, TRenderer>
        where THandler : UIMenuHandler<TElement, THandler, TRenderer>
        where TRenderer : UIMenuRenderer<TElement, THandler, TRenderer>
    {
        public UIMenuHandler() { }
        
        /// <summary>
        /// Brings the menu's scroll content height up to date right away and re-applies the
        /// scroll alignment. The scroll container computes its content size automatically
        /// (the lowest button bottom edge - see <see cref="UIScrollContainer.RefreshContentSize"/>),
        /// so this only runs its layout pass now, which positions flex-laid-out buttons,
        /// instead of waiting for the next draw.
        /// </summary>
        public void UpdateScrollContentHeight()
        {
            var menu = (UIMenu<TElement, THandler, TRenderer>)Element;
            if (menu.ButtonList.Count == 0) return;

            // Arrange layout-managed buttons now. Not for a still-pending legacy container:
            // UpdateLayout would run its one-shot legacy pass early, and that first-draw pass
            // is what rebases its scroll physics once the menu is in place.
            var scroll = menu.ScrollContainer;
            if (scroll.LayoutDirty && (scroll.IsLayoutManaged || !scroll.LegacyLayoutPending))
                scroll.UpdateLayout();
            scroll.RefreshContentSize(forceAlignment: true);
        }

    }
}
