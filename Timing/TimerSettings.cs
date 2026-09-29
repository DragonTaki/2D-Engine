/* ----- ----- ----- ----- */
// TimerSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/05/07
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Timing
{
    /// <summary>
    /// Holds constant timer settings for the game timers.
    /// </summary>
    public static class TimerSettings
    {
        /// <summary>
        /// Target animation frame rate. Also the reference tick rate that
        /// Physics2D's per-tick tuned constants and velocities are expressed in.
        /// </summary>
        public const int GameAnimationFPS = 60;

        /// <summary>
        /// Gets the timer interval in milliseconds for the game animation timer.
        /// Calculated as 1000ms divided by the target FPS.
        /// </summary>
        public const int GameAnimationInterval = 1000 / GameAnimationFPS;

        /// <summary>
        /// Upper bound on a single frame's delta time, in seconds. A stall (window
        /// drag, modal loop, GC pause, debugger break) would otherwise feed one huge
        /// step into physics/animation integration; standard "max frame time" guard.
        /// </summary>
        public const float MaxDeltaTimeInSeconds = 0.1f;

        /// <summary>
        /// Gets the timer interval in milliseconds for the red player's timer.
        /// </summary>
        public const int RedPlayerTimerInterval = 1000;

        /// <summary>
        /// Gets the timer interval in milliseconds for the black player's timer.
        /// </summary>
        public const int BlackPlayerTimerInterval = 1000;
        
    }
}