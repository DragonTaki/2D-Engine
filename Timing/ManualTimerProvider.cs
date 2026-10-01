/* ----- ----- ----- ----- */
// ManualTimerProvider.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Timing
{
    /// <summary>
    /// An <see cref="ITimerProvider"/> with no timer of its own — the host
    /// (e.g. a Silk.NET window's per-frame Update callback) calls
    /// <see cref="Tick(float)"/> once per frame with the elapsed time. Used
    /// by windowing backends that already drive their own frame loop, so we
    /// don't run a second, competing timer alongside it.
    /// </summary>
    public class ManualTimerProvider : ITimerProvider
    {
        private bool _running;

        public float DeltaTimeInSeconds { get; private set; }
        public float ElapsedTimeInSeconds { get; private set; }

        public event Action OnAnimationFrame;

        public void Start() => _running = true;
        public void Stop() => _running = false;

        /// <summary>Advances the clock by <paramref name="deltaTimeInSeconds"/> and fires <see cref="OnAnimationFrame"/>.</summary>
        public void Tick(float deltaTimeInSeconds)
        {
            if (!_running) return;

            // The frame delta is capped for integration; the elapsed total keeps the real
            // (uncapped) time, like WinFormsTimerProvider's stopwatch-based elapsed time,
            // but never runs backward on a negative (or NaN) delta from the host.
            DeltaTimeInSeconds = float.IsNaN(deltaTimeInSeconds)
                ? 0f
                : Math.Clamp(deltaTimeInSeconds, 0f, TimerSettings.MaxDeltaTimeInSeconds);
            if (deltaTimeInSeconds > 0f)
                ElapsedTimeInSeconds += deltaTimeInSeconds;
            OnAnimationFrame?.Invoke();
        }
    }
}
