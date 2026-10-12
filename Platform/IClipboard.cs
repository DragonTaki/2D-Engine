/* ----- ----- ----- ----- */
// IClipboard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Platform
{
    /// <summary>
    /// Read access to the system clipboard's text, for pasting into text fields (ONLINE-PLAY §8.29).
    /// Each windowing backend has its own (<c>Engine.Platform.WinForms.WinFormsClipboard</c>,
    /// <c>Engine.Platform.Skia.SilkClipboard</c>); without one, <see cref="UnsupportedClipboard"/>.
    /// The launcher registers its backend's clipboard in DI and hands it to
    /// <c>UIInputManager</c>. Only reading: nothing copies out of the game (and never out of a
    /// password field).
    /// </summary>
    public interface IClipboard
    {
        /// <summary>
        /// The clipboard's text as is (it may hold line breaks or other control characters; the
        /// receiving field filters them). Call on the UI thread.
        /// </summary>
        /// <returns>The text; empty (never null) when the clipboard holds no text or cannot be read. Never throws for a clipboard failure (it is logged).</returns>
        string GetText();
    }
}
