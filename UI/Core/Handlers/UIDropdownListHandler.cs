/* ----- ----- ----- ----- */
// UIDropdownListHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.Platform;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;
using Engine.UI.Input;

namespace Engine.UI.Core.Handlers
{
    /// <summary>
    /// Handler for <see cref="UIDropdownList"/>: hover highlights the option under the mouse, a
    /// click on an option chooses it (through the owner's <see cref="UIDropdownHandler.Select"/>),
    /// a press outside the list closes it, the wheel scrolls the list. Every mouse event is taken
    /// while the list is open, wherever it happens. Also a keyboard input target (a press anywhere
    /// gives it the focus while it covers the screen): Escape closes the list.
    /// </summary>
    public class UIDropdownListHandler : UIHandler<UIDropdownList, UIDropdownListHandler, UIDropdownListRenderer>, IKeyboardInputTarget
    {
        public UIDropdownListHandler() { }

        /// <summary>Whether the list is still open (its owner has not closed it).</summary>
        private bool IsOpen => Element.Owner != null && !Element.IsDisposed;

        /// <summary>Closes the list through its owner (no change of choice).</summary>
        private void CloseList() => Element.Owner?.Handler.Close();

        #region Mouse Handling

        /// <summary>A press outside the list closes it; inside, the release decides (<see cref="HandleMouseClick"/>).</summary>
        internal override bool HandleMouseDown(IMouseEvent e)
        {
            if (!IsOpen)
                return false;

            if (!Element.ComputeLayout().ListRect.Contains(e.X, e.Y))
                CloseList();
            return true;
        }

        /// <summary>Highlights the option under the mouse (none outside the list).</summary>
        internal override bool HandleMouseMove(IMouseEvent e)
        {
            if (!IsOpen)
                return false;

            Element.HoverIndex = Element.ComputeLayout().ItemIndexAt(e.X, e.Y, Element.Scroll);
            return true;
        }

        /// <summary>Scrolls the list one option per wheel notch; the page underneath never scrolls while the list is open.</summary>
        internal override bool HandleMouseWheel(IMouseEvent e)
        {
            if (!IsOpen)
                return false;

            var layout = Element.ComputeLayout();
            float notches = Math.Clamp(e.Delta / (float)DropdownDefaults.WheelNotchDelta, -1f, 1f);
            Element.Scroll = layout.ClampScroll(Element.Scroll - notches * layout.ItemHeight);
            Element.HoverIndex = layout.ItemIndexAt(e.X, e.Y, Element.Scroll);
            return true;
        }

        /// <summary>A click on an option chooses it and closes the list; elsewhere in the list nothing happens.</summary>
        internal override bool HandleMouseClick(IMouseEvent e)
        {
            if (!IsOpen)
                return false;

            int index = Element.ComputeLayout().ItemIndexAt(e.X, e.Y, Element.Scroll);
            if (index >= 0)
                Element.Owner.Handler.Select(index);
            return true;
        }

        /// <summary>
        /// Keeps the scroll within range (the list may have been cut shorter by a resize) and
        /// closes the list when its owner is no longer shown (hidden, disabled, removed or disposed).
        /// </summary>
        internal override void OnUpdate()
        {
            if (!IsOpen)
                return;

            if (!OwnerIsShown(Element.Owner))
            {
                CloseList();
                return;
            }

            Element.Scroll = Element.ComputeLayout().ClampScroll(Element.Scroll);
        }

        /// <summary>Whether <paramref name="owner"/> is enabled, not disposed, and it and every ancestor up to the root node are displayed and visible.</summary>
        private static bool OwnerIsShown(UIElementBase owner)
        {
            if (owner == null || owner.IsDisposed || !owner.IsEnabled)
                return false;

            for (var current = owner; ; current = current.Parent)
            {
                if (!current.IsDisplayed || !current.IsVisible)
                    return false;
                if (current.Parent == null)
                    return current.ElementType == UIElementType.Root;
            }
        }

        #endregion

        #region IKeyboardInputTarget

        UIElementBase IKeyboardInputTarget.TargetElement => Element;

        /// <summary>Typed characters are not used.</summary>
        public bool HandleTextInput(char c) => false;

        /// <summary>Escape closes the list; nothing else is used.</summary>
        public bool HandleKey(UIKey key)
        {
            if (key != UIKey.Escape || !IsOpen)
                return false;
            CloseList();
            return true;
        }

        /// <summary>Nothing to do: the list closes on a choice, a press outside it or Escape.</summary>
        public void OnFocusChanged(bool focused) { }

        #endregion
    }
}
