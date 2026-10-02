/* ----- ----- ----- ----- */
// IKeyboardInputTarget.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Bases;

namespace Engine.UI.Input
{
    /// <summary>
    /// Something that takes keyboard input while it has the keyboard focus
    /// (<see cref="KeyboardFocus"/>): an element's handler implements it, and pressing the
    /// mouse on that element gives it the focus.
    /// </summary>
    public interface IKeyboardInputTarget
    {
        /// <summary>The element the input is for (the focus is dropped when it is no longer shown or is disposed).</summary>
        UIElementBase TargetElement { get; }

        /// <summary>A typed character (never a control character).</summary>
        /// <returns>Whether it was used.</returns>
        bool HandleTextInput(char c);

        /// <summary>An editing key. <see cref="UIKey.Enter"/> and <see cref="UIKey.Escape"/> end the focus after this returns true.</summary>
        /// <returns>Whether it was used.</returns>
        bool HandleKey(UIKey key);

        /// <summary>The focus was given to (<paramref name="focused"/> true) or taken from this target.</summary>
        void OnFocusChanged(bool focused);
    }
}
