/* ----- ----- ----- ----- */
// IUpdatable.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Core.Interfaces
{
    /// <summary>
    /// Interface for objects that require periodic update calls, 
    /// typically invoked once per frame in the UI or game loop.
    /// </summary>
    /// <remarks>
    /// Classes implementing IUpdatable should place any per-frame logic
    /// in the Update method, such as animations, physics calculations,
    /// UI state changes, or input processing.
    /// </remarks>
    public interface IUpdatable
    {
        /// <summary>
        /// Called once per frame or update cycle.
        /// Implementing classes should include logic that needs to run
        /// continuously while the object is active.
        /// </summary>
        void Update();
    }
}
