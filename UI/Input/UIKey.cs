/* ----- ----- ----- ----- */
// UIKey.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Input
{
    /// <summary>
    /// The editing keys the UI handles (typed characters arrive as text input instead). The
    /// windowing backends translate their own key codes into these (the input adapters).
    /// </summary>
    public enum UIKey
    {
        /// <summary>Deletes the character before the caret.</summary>
        Backspace,

        /// <summary>Deletes the character after the caret.</summary>
        Delete,

        /// <summary>Confirms the edit (ends keyboard focus).</summary>
        Enter,

        /// <summary>Cancels the edit (ends keyboard focus).</summary>
        Escape,

        /// <summary>Caret one character left.</summary>
        Left,

        /// <summary>Caret one character right.</summary>
        Right,

        /// <summary>Caret to the start.</summary>
        Home,

        /// <summary>Caret to the end.</summary>
        End,
    }
}
