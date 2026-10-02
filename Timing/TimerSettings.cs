/* ----- ----- ----- ----- */
// TimerSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2026/10/02
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

namespace Engine.Timing
{
    /// <summary>
    /// Timer settings for the game timers: the animation frame rate (a runtime value the host
    /// sets from the player's settings) and the constants around it.
    /// </summary>
    public static class TimerSettings
    {
        /// <summary>The animation frame rate until a host sets <see cref="GameAnimationFPS"/>.</summary>
        public const int DefaultGameAnimationFPS = 60;

        /// <summary>Smallest / largest <see cref="GameAnimationFPS"/> (assigned values are clamped).</summary>
        public const int MinGameAnimationFPS = 1, MaxGameAnimationFPS = 1000;

        /// <summary>
        /// The reference tick rate <see cref="Physics.Physics2D"/>'s per-tick tuned constants and
        /// velocities are expressed in: one tick = 1/60 s, whatever the actual frame rate. Fixed
        /// on purpose - physics integrates real elapsed time in these ticks, so it behaves the
        /// same at any <see cref="GameAnimationFPS"/>.
        /// </summary>
        public const int PhysicsReferenceTickRate = 60;

        private static int s_gameAnimationFps = DefaultGameAnimationFPS;

        /// <summary>
        /// Target animation frame rate (frames per second), clamped to
        /// <see cref="MinGameAnimationFPS"/>..<see cref="MaxGameAnimationFPS"/>. The host sets it
        /// at startup and whenever the player changes it; a change raises
        /// <see cref="GameAnimationFpsChanged"/>, which the frame timers follow. Not the physics
        /// time unit (see <see cref="PhysicsReferenceTickRate"/>).
        /// </summary>
        public static int GameAnimationFPS
        {
            get => s_gameAnimationFps;
            set
            {
                int fps = Math.Clamp(value, MinGameAnimationFPS, MaxGameAnimationFPS);
                if (fps == s_gameAnimationFps)
                    return;
                s_gameAnimationFps = fps;
                GameAnimationFpsChanged?.Invoke(fps);
            }
        }

        /// <summary>Raised with the new value after <see cref="GameAnimationFPS"/> changed.</summary>
        public static event Action<int> GameAnimationFpsChanged;

        /// <summary>
        /// The timer interval in whole milliseconds for the game animation timer: 1000 ms divided
        /// by <see cref="GameAnimationFPS"/> (integer division, as before; at least 1).
        /// </summary>
        public static int GameAnimationInterval => Math.Max(1, 1000 / GameAnimationFPS);

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
