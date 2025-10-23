/* ----- ----- ----- ----- */
// IUiContainer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/23
// Update Date: 2025/10/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.UI.Core.Interfaces
{
    /// <summary>
    /// Represents a UI container that can manage execution of actions and track its disposal state.
    /// </summary>
    /// <remarks>
    /// This interface is typically used to queue actions that should be executed
    /// on the UI thread or within the container's context.
    /// </remarks>
    public interface IUiContainer
    {
        /// <summary>
        /// Gets a value indicating whether this UI container has been disposed.
        /// </summary>
        /// <returns>
        /// True if the container has been disposed; otherwise, false.
        /// </returns>
        bool IsDisposed { get; }

        /// <summary>
        /// Posts an action to be executed by the container, typically on the UI thread.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> delegate to execute.
        /// </param>
        /// <remarks>
        /// This method allows deferred execution of code within the container's context.
        /// If the container is disposed, the action may be ignored or not executed.
        /// </remarks>
        void Post(Action action);
    }
}
