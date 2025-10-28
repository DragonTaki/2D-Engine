/* ----- ----- ----- ----- */
// IResettable.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/28
// Update Date: 2025/10/28
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Core.Interfaces
{
    /// <summary>
    /// Represents an object that can reset its internal state without disposing it.
    /// </summary>
    public interface IResettable
    {
        /// <summary>
        /// Resets this instance to its initial state.
        /// </summary>
        void Reset();
    }
}
