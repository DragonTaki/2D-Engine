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
        private const int GameAnimationFPS = 60;

        /// <summary>
        /// Gets the timer interval in milliseconds for the game animation timer.
        /// Calculated as 1000ms divided by the target FPS.
        /// </summary>
        public const int GameAnimationInterval = 1000 / GameAnimationFPS;

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