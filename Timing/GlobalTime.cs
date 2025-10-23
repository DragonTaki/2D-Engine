/* ----- ----- ----- ----- */
// GlobalTime.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/11
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Timing
{
    /// <summary>
    /// Provides a global timer provider instance.
    /// </summary>
    public static class GlobalTime
    {
        /// <summary>
        /// Gets or sets the current timer provider used globally.
        /// </summary>
        public static ITimerProvider Timer { get; set; }
    }
}