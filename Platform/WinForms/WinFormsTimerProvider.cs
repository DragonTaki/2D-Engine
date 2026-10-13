/* ----- ----- ----- ----- */
// WinFormsTimerProvider.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2026/10/12
// Version: v2.2
/* ----- ----- ----- ----- */

using System;
using System.Diagnostics;
using System.Windows.Forms;

using Engine.Timing;

namespace Engine.Platform.WinForms
{
    /// <summary>
    /// Timer manager that uses a Stopwatch and a Windows Forms Timer
    /// to poll fractional frame deadlines with delta time calculation.
    /// The frame schedule follows <see cref="TimerSettings.GameAnimationFPS"/> (also when it changes
    /// at runtime). A WinForms timer is driven by window messages and the system timer
    /// resolution (about 15.6 ms by default on Windows), so it rarely ticks faster than about
    /// 64 times per second whatever the setting; delta time is measured, so motion stays right.
    /// </summary>
    public class WinFormsTimerProvider : ITimerProvider, IDisposable
    {
        private Stopwatch _animationStopwatch;
        private Timer _animationTimer;
        private long _lastAnimationTimestamp;
        private readonly FramePacer _framePacer;

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
        /// Initializes a new instance of the <see cref="WinFormsTimerProvider"/> class.
        /// </summary>
        public WinFormsTimerProvider()
        {
            _animationStopwatch = new Stopwatch();
            _lastAnimationTimestamp = 0;

            _framePacer = new FramePacer(TimerSettings.GameAnimationFPS, Stopwatch.Frequency);
            _animationTimer = new Timer { Interval = Math.Max(1, TimerSettings.GameAnimationInterval / 2) };
            TimerSettings.GameAnimationFpsChanged += OnGameAnimationFpsChanged;
            _animationTimer.Tick += (s, e) =>
            {
                if (!_animationStopwatch.IsRunning) return;

                // Stopwatch ticks, not whole milliseconds, so dt isn't quantized to 15/16/31 ms.
                long current = _animationStopwatch.ElapsedTicks;
                if (!_framePacer.TryAdvance(current)) return;
                float delta = (float)((current - _lastAnimationTimestamp) / (double)Stopwatch.Frequency);
                DeltaTimeInSeconds = Math.Clamp(delta, 0f, TimerSettings.MaxDeltaTimeInSeconds);
                _lastAnimationTimestamp = current;

                OnAnimationFrame?.Invoke();
            };
        }

        /// <summary>The player changed the frame rate: tick at the new interval (on the UI thread, where settings change).</summary>
        // Same CA1416 exemption as the WinForms input code (this file only runs in the WinForms launcher).
#pragma warning disable CA1416
        private void OnGameAnimationFpsChanged(int fps)
        {
            _framePacer.SetFrameRate(fps, _animationStopwatch.ElapsedTicks);
            _animationTimer.Interval = Math.Max(1, TimerSettings.GameAnimationInterval / 2);
        }
#pragma warning restore CA1416

        /// <summary>
        /// Starts the animation stopwatch and timer.
        /// </summary>
        public void Start() => StartTimers();

        /// <summary>
        /// Stops the animation timer and the stopwatch.
        /// </summary>
        public void Stop() => StopTimers();

        /// <summary>
        /// Starts or restarts the stopwatch and timer, resetting elapsed time.
        /// </summary>
        public void StartTimers()
        {
            _animationStopwatch.Restart();
            _lastAnimationTimestamp = 0;
            _framePacer.Reset(0);
            _animationTimer.Start();
        }

        /// <summary>
        /// Stops the animation timer and the stopwatch.
        /// </summary>
        public void StopTimers()
        {
            _animationTimer.Stop();
            // Also stop the clock, so ElapsedTimeInSeconds doesn't keep advancing while stopped.
            _animationStopwatch.Stop();
        }

        /// <summary>
        /// Stops and releases the underlying WinForms timer (a native window-message timer) and
        /// stops following frame rate changes.
        /// </summary>
        public void Dispose()
        {
            TimerSettings.GameAnimationFpsChanged -= OnGameAnimationFpsChanged;
            StopTimers();
            _animationTimer.Dispose();
        }
    }
}
