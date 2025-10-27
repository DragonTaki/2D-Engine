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
    /// Defines the contract for all UI input handler modules.
    /// Implementations of this interface are responsible for processing
    /// specific mouse or user input events (such as drag, click, wheel, etc.)
    /// and optionally updating internal state or emitting events accordingly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface allows different input behavior modules (e.g., <c>DragHandler</c>,
    /// <c>ScrollHandler</c>, <c>SelectionHandler</c>) to be implemented independently.
    /// </para>
    /// <para>
    /// Each method returns a <see cref="bool"/> value indicating whether the event was handled.
    /// Returning <c>true</c> means the handler has consumed the event and it should not propagate further.
    /// </para>
    /// </remarks>
    public interface IInputHandler
    {
        #region Mouse Event Handlers
        
        /// <summary>
        /// Called when the mouse button is pressed down.
        /// </summary>
        /// <param name="e">Mouse event data including button, click count, and coordinates.</param>
        /// <returns>
        /// <c>true</c> if the handler has processed the mouse down event;  
        /// otherwise, <c>false</c> if the event should continue to other handlers.
        /// </returns>
        bool OnMouseDown(MouseEventArgs e);

        /// <summary>
        /// Called when the mouse is moved.
        /// </summary>
        /// <param name="e">Mouse event data containing new cursor position.</param>
        /// <returns>
        /// <c>true</c> if movement is handled (e.g., during dragging);  
        /// <c>false</c> if ignored or below movement threshold.
        /// </returns>
        bool OnMouseMove(MouseEventArgs e);

        /// <summary>
        /// Called when the mouse button is released.
        /// Typically used to finalize drag or click detection.
        /// </summary>
        /// <param name="e">Mouse event data including release position and button information.</param>
        /// <returns>
        /// <c>true</c> if release was processed;  
        /// <c>false</c> if not applicable for this handler.
        /// </returns>
        bool OnMouseUp(MouseEventArgs e);

        /// <summary>
        /// Called when the mouse wheel is scrolled.
        /// Typically used for zoom or scroll behaviors.
        /// </summary>
        /// <param name="e">Mouse wheel event data, including delta value and modifier keys.</param>
        /// <returns>
        /// <c>true</c> if the handler processed the wheel event (e.g., scrolling content);  
        /// otherwise, <c>false</c>.
        /// </returns>
        bool OnMouseWheel(MouseEventArgs e);

        /// <summary>
        /// Called when a mouse click event is detected (press and release without drag).
        /// </summary>
        /// <param name="e">Mouse event data, including click position and button.</param>
        /// <returns>
        /// <c>true</c> if the handler handled the click event;  
        /// <c>false</c> if unhandled and should propagate further.
        /// </returns>
        bool OnMouseClick(MouseEventArgs e);

        #endregion

        #region Frame Lifecycle

        /// <summary>
        /// Called once per frame to allow input handlers to finalize or reset temporary data.
        /// </summary>
        /// <remarks>
        /// Common use cases include:
        /// <list type="bullet">
        /// <item>Clearing accumulated deltas after processing.</item>
        /// <item>Updating internal states for next frame.</item>
        /// <item>Ending continuous gestures gracefully.</item>
        /// </list>
        /// </remarks>
        void EndFrame();

        #endregion
    }
}
