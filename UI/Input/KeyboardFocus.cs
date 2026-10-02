/* ----- ----- ----- ----- */
// KeyboardFocus.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Constants.Components;
using Engine.UI.Core.Bases;

namespace Engine.UI.Input
{
    /// <summary>
    /// Which <see cref="IKeyboardInputTarget"/> receives the keyboard (at most one). Owned by
    /// <see cref="UIInputManager"/>: a mouse press gives the focus to the pressed element's
    /// handler when it is a target and takes it away otherwise; text input and editing keys go
    /// to the focused target. A target whose element is no longer shown (hidden, removed from
    /// the tree) or is disposed loses the focus before the next key reaches it.
    /// </summary>
    public sealed class KeyboardFocus
    {
#nullable enable
        /// <summary>The target receiving the keyboard, or null.</summary>
        public IKeyboardInputTarget? Focused { get; private set; }

        /// <summary>
        /// Gives the focus to <paramref name="target"/> (null: nobody). The previous target is
        /// told it lost the focus first, then the new one that it got it; nothing happens when
        /// <paramref name="target"/> already has it.
        /// </summary>
        public void SetFocus(IKeyboardInputTarget? target)
        {
            if (ReferenceEquals(target, Focused))
                return;

            var previous = Focused;
            Focused = target;
            previous?.OnFocusChanged(false);
            target?.OnFocusChanged(true);
        }
#nullable disable

        /// <summary>Takes the focus away from the focused target (if any).</summary>
        public void ClearFocus() => SetFocus(null);

        /// <summary>Sends a typed character to the focused target; control characters are ignored.</summary>
        /// <returns>Whether a target used it.</returns>
        public bool OnTextInput(char c)
        {
            if (char.IsControl(c) || !ValidateFocus())
                return false;
            return Focused.HandleTextInput(c);
        }

        /// <summary>
        /// Sends an editing key to the focused target; after a used <see cref="UIKey.Enter"/> or
        /// <see cref="UIKey.Escape"/> the focus ends.
        /// </summary>
        /// <returns>Whether a target used it.</returns>
        public bool OnKey(UIKey key)
        {
            if (!ValidateFocus())
                return false;

            bool handled = Focused.HandleKey(key);
            if (handled && (key == UIKey.Enter || key == UIKey.Escape))
                ClearFocus();
            return handled;
        }

        /// <summary>Drops the focus when its element is no longer shown; returns whether a target still has it.</summary>
        private bool ValidateFocus()
        {
            if (Focused == null)
                return false;
            if (!IsShown(Focused.TargetElement))
                ClearFocus();
            return Focused != null;
        }

        /// <summary>
        /// Whether <paramref name="element"/> is in the UI tree (its root is the root node), not
        /// disposed, enabled, and it and every ancestor are displayed and visible.
        /// </summary>
        private static bool IsShown(UIElementBase element)
        {
            if (element == null || element.IsDisposed || !element.IsEnabled)
                return false;

            UIElementBase current = element;
            while (true)
            {
                if (!current.IsDisplayed || !current.IsVisible)
                    return false;
                if (current.Parent == null)
                    return current.ElementType == UIElementType.Root;
                current = current.Parent;
            }
        }
    }
}
