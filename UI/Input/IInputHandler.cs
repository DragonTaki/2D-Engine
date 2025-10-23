/* ----- ----- ----- ----- */
// IInputHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Windows.Forms;

namespace Engine.UI.Input
{
    /// <summary>
    /// Defines the contract for input handler modules. Each module processes specific input events.
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// Called when mouse button is pressed.
        /// </summary>
        bool OnMouseDown(MouseEventArgs e);

        /// <summary>
        /// Called when mouse moves.
        /// </summary>
        bool OnMouseMove(MouseEventArgs e);

        /// <summary>
        /// Called when mouse button is released.
        /// </summary>
        bool OnMouseUp(MouseEventArgs e);

        /// <summary>
        /// Called when mouse wheel scrolls.
        /// </summary>
        bool OnMouseWheel(MouseEventArgs e);

        /// <summary>
        /// Called when mouse clicks.
        /// </summary>
        bool OnMouseClick(MouseEventArgs e);

        /// <summary>
        /// Called at the end of each frame to finalize per-frame state (e.g., reset delta).
        /// </summary>
        void EndFrame();
    }
}
