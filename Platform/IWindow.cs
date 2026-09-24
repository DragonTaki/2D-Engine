/* ----- ----- ----- ----- */
// IWindow.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Platform
{
    /// <summary>
    /// The application's main window, mirroring the subset of WinForms' <c>Form</c>
    /// actually used here (requesting a repaint).
    /// </summary>
    public interface IWindow
    {
        /// <summary>Requests that the window redraw on its next paint pass.</summary>
        void Invalidate();
    }
}
