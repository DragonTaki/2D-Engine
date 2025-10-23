/* ----- ----- ----- ----- */
// TimerManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/05/10
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Engine.Timing
{
    /// <summary>
    /// Timer manager that uses a Stopwatch and a Windows Forms Timer
    /// to provide a fixed interval animation timer with delta time calculation.
    /// </summary>
    public class TimerManager : ITimerProvider
    {
        private Stopwatch _animationStopwatch;
        private Timer _animationTimer;
        private long _lastAnimationTimestamp;

        /// <summary>
        /// Gets the elapsed time in seconds since the last animation frame.
        /// </summary>
        public float DeltaTimeInSeconds { get; private set; }

        /// <summary>
        /// Gets the total elapsed time in seconds since the timer started.
        /// </summary>
        public float ElapsedTimeInSeconds => _animationStopwatch.ElapsedMilliseconds / 1000f;

        /// <summary>
        /// Event invoked on every animation frame tick.
        /// </summary>
        public event Action OnAnimationFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerManager"/> class.
        /// </summary>
        public TimerManager()
        {
            _animationStopwatch = new Stopwatch();
            _lastAnimationTimestamp = 0;

            _animationTimer = new Timer { Interval = TimerSettings.GameAnimationInterval };
            _animationTimer.Tick += (s, e) =>
            {
                if (!_animationStopwatch.IsRunning) return;

                long current = _animationStopwatch.ElapsedMilliseconds;
                DeltaTimeInSeconds = (current - _lastAnimationTimestamp) / 1000f;
                _lastAnimationTimestamp = current;

                OnAnimationFrame?.Invoke();
            };
        }

        /// <summary>
        /// Starts the animation stopwatch and timer.
        /// </summary>
        public void Start() => StartTimers();

        /// <summary>
        /// Stops the animation timer.
        /// </summary>
        public void Stop() => StopTimers();

        /// <summary>
        /// Starts or restarts the stopwatch and timer, resetting elapsed time.
        /// </summary>
        public void StartTimers()
        {
            _animationStopwatch.Restart();
            _lastAnimationTimestamp = 0;
            _animationTimer.Start();
        }

        /// <summary>
        /// Stops the animation timer.
        /// </summary>
        public void StopTimers()
        {
            _animationTimer.Stop();
        }
    }
}
