/* ----- ----- ----- ----- */
// MainThreadQueue.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Concurrent;

using Engine.Logging;

namespace Engine.Timing
{
    /// <summary>
    /// The <see cref="IMainThreadQueue"/>: a lock-free <see cref="ConcurrentQueue{T}"/> any thread
    /// posts to, drained by the launcher's frame loop once per frame (before the UI update).
    /// </summary>
    public sealed class MainThreadQueue : IMainThreadQueue
    {
        private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        /// <inheritdoc/>
        public void Post(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);
            _actions.Enqueue(action);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Only the actions queued when the drain starts run now; anything they (or other
        /// threads) post meanwhile waits for the next drain, so an action that re-posts itself
        /// cannot stall the frame.
        /// </remarks>
        public void Drain()
        {
            int count = _actions.Count;
            for (int i = 0; i < count && _actions.TryDequeue(out var action); i++)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    AppLogger.Log($"[MainThreadQueue] A posted action failed: {ex}", LogLevel.ERROR);
                }
            }
        }
    }
}
