/* ----- ----- ----- ----- */
// IMenuPanel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Core.Interfaces
{
    /// <summary>
    /// A menu panel whose outline can be turned off, for a panel whose frame is drawn by someone
    /// else (e.g. the main menu draws its open submenu's speech-bubble frame).
    /// </summary>
    public interface IMenuPanel
    {
        /// <summary>Whether the menu's renderer draws the panel's outline itself (default true).</summary>
        bool DrawsOutline { get; set; }
    }
}
