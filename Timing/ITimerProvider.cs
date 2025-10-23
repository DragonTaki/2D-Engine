/* ----- ----- ----- ----- */
// ITimerProvider.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/10
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Timing
{
    public interface ITimerProvider
    {
        /// <summary>
        /// Triggered when each frame needs to be updated. (e.g. called at a fixed FPS)
        /// </summary>
        event Action OnAnimationFrame;

        /// <summary>
        /// The interval between current frame and the previous frame (seconds).
        /// </summary>
        float DeltaTimeInSeconds { get; }

        /// <summary>
        /// Start animation time event.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop animation time event.
        /// </summary>
        void Stop();
    }
}