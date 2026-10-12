/* ----- ----- ----- ----- */
// IMainThreadQueue.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Timing
{
    /// <summary>
    /// Hands work from any thread (e.g. a network receive loop) to the UI / main thread: work is
    /// <see cref="Post"/>ed from anywhere and runs when the frame loop calls <see cref="Drain"/>
    /// (once per frame, on the UI thread), so it may touch the UI tree and game state safely.
    /// </summary>
    public interface IMainThreadQueue
    {
        /// <summary>
        /// Queues <paramref name="action"/> to run on the main thread at the next <see cref="Drain"/>.
        /// Safe to call from any thread.
        /// </summary>
        /// <param name="action">The work to run; must not be null.</param>
        void Post(Action action);

        /// <summary>
        /// Runs the queued actions on the calling thread (the frame loop's, i.e. the UI thread), in
        /// the order they were posted. An action that throws is logged and the rest still run.
        /// </summary>
        void Drain();
    }
}
