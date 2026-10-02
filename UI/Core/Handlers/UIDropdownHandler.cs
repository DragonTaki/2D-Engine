/* ----- ----- ----- ----- */
// UIDropdownHandler.cs
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
using Engine.UI.Utils;

namespace Engine.UI.Core.Handlers
{
    /// <summary>
    /// Handler for <see cref="UIDropdown"/>: a click opens the list (or closes it), choosing an
    /// option reports it through <see cref="SelectionChanged"/>. It is also a keyboard input
    /// target, so the press that opens the list gives it the focus and Escape closes the list.
    /// A disabled dropdown does not open.
    /// </summary>
    public class UIDropdownHandler : UIHandler<UIDropdown, UIDropdownHandler, UIDropdownRenderer>, IKeyboardInputTarget
    {
#nullable enable
        /// <summary>Invoked with the new index after an option other than the chosen one was picked (<see cref="Select"/>).</summary>
        public Action<int>? SelectionChanged { get; set; }
#nullable disable

        public UIDropdownHandler() { }

        /// <summary>
        /// Opens the list on the UI root's overlay layer, scrolled so the chosen option shows.
        /// Does nothing while disabled, already open, without options, or when the dropdown is
        /// not in the UI tree (its root is not the root node).
        /// </summary>
        /// <returns>Whether the list was opened.</returns>
        public bool Open()
        {
            if (!Element.IsEnabled || Element.IsOpen || Element.Options.Count == 0 || _factory == null)
                return false;
            if (Element.GetRoot().ElementType != UIElementType.Root)
                return false;

            var overlay = Element.GetOrCreateOverlay();
            var list = _factory.CreateElement<UIDropdownList, UIDropdownListHandler, UIDropdownListRenderer>();
            list.Owner = Element;
            list.ZIndex = DropdownDefaults.ListZIndex;
            overlay.AddChild(list);
            // Sized now (it covers the overlay), not at the next draw, so a press right away hits it.
            overlay.PerformLayout();
            Element.List = list;

            list.HoverIndex = -1;
            list.Scroll = list.ComputeLayout().ScrollToShow(Element.SelectedIndex, 0f);
            Element.RequestRedraw();
            return true;
        }

        /// <summary>Closes the list without changing the choice (nothing happens while closed).</summary>
        public void Close()
        {
            var list = Element?.List;
            if (list == null)
                return;

            Element.List = null;
            list.Owner = null;
            // Hidden first: the press that closed it may still be routed its release.
            list.IsVisible = false;
            list.IsEnabled = false;
            list.Dispose();
            Element.RequestRedraw();
        }

        /// <summary>
        /// Chooses option <paramref name="index"/> and closes the list; invokes
        /// <see cref="SelectionChanged"/> when it differs from the chosen one.
        /// </summary>
        /// <returns>Whether <paramref name="index"/> is an option (out of range: nothing happens).</returns>
        public bool Select(int index)
        {
            if (index < 0 || index >= Element.Options.Count)
                return false;

            Close();
            if (index == Element.SelectedIndex)
                return true;

            Element.SelectedIndex = index;
            SelectionChanged?.Invoke(index);
            return true;
        }

        #region IKeyboardInputTarget

        UIElementBase IKeyboardInputTarget.TargetElement => Element;

        /// <summary>Typed characters are not used.</summary>
        public bool HandleTextInput(char c) => false;

        /// <summary>Escape closes an open list; nothing else is used.</summary>
        public bool HandleKey(UIKey key)
        {
            if (key != UIKey.Escape || !Element.IsOpen)
                return false;
            Close();
            return true;
        }

        /// <summary>Nothing to do: the list closes on its own (a press outside it, Escape, a choice).</summary>
        public void OnFocusChanged(bool focused) { }

        #endregion

        #region Mouse Handling

        /// <summary>A click opens the list, or closes it when open.</summary>
        internal override bool HandleMouseClick(IMouseEvent e)
        {
            if (!Element.IsEnabled)
                return false;

            if (Element.IsOpen)
                Close();
            else
                Open();
            return true;
        }

        #endregion
    }
}
