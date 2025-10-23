/* ----- ----- ----- ----- */
// IScreen.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/18
// Update Date: 2025/05/18
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Core.Interfaces
{
    /// <summary>
    /// Interface for UI screens that can be displayed and removed in the navigation system.
    /// </summary>
    /// <remarks>
    /// Classes implementing IScreen should manage their own state when being
    /// entered or exited. Typical implementations include pausing animations,
    /// updating UI elements, or resetting temporary data when the screen becomes active/inactive.
    /// </remarks>
    public interface IScreen
    {
        /// <summary>
        /// Called when the screen is navigated to and becomes the active view.
        /// </summary>
        /// <remarks>
        /// Implementations may initialize or refresh content here.
        /// This method is invoked once per entry into the navigation stack.
        /// </remarks>
        void OnEnter();

        /// <summary>
        /// Called when the screen is about to be removed or replaced in the navigation stack.
        /// </summary>
        /// <remarks>
        /// Implementations may stop animations, save state, or perform cleanup here.
        /// This method is invoked once per exit from the navigation stack.
        /// </remarks>
        void OnExit();
    }
}
