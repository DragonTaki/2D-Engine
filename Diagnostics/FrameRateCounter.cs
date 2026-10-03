/* ----- ----- ----- ----- */
// FrameRateCounter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Diagnostics;

namespace Engine.Diagnostics
{
    /// <summary>
    /// Measures the frame rate from real frame times: count the frames drawn, and every
    /// <see cref="SampleSeconds"/> divide the frame intervals by the time they took. Pure
    /// arithmetic on timestamps (no graphics), so it can be tested without a window.
    /// </summary>
    public sealed class FrameRateCounter
    {
        private bool _started;
        private double _windowStart;
        private int _intervals;

        /// <param name="sampleSeconds">How long each measurement averages over (positive).</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sampleSeconds"/> is not positive.</exception>
        public FrameRateCounter(double sampleSeconds = 0.5)
        {
            if (!(sampleSeconds > 0))
                throw new ArgumentOutOfRangeException(nameof(sampleSeconds), sampleSeconds, "The sample time must be positive.");
            SampleSeconds = sampleSeconds;
        }

        /// <summary>How long each measurement averages over, in seconds.</summary>
        public double SampleSeconds { get; }

        /// <summary>The last measured frame rate (frames per second); null until the first sample is complete.</summary>
        public double? FramesPerSecond { get; private set; }

        /// <summary>Records a frame drawn now (a monotonic <see cref="Stopwatch"/> timestamp).</summary>
        public void AddFrame() => AddFrame(Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency);

        /// <summary>
        /// Records a frame drawn at <paramref name="timeSeconds"/> (any monotonic clock, in
        /// seconds). The first frame only starts the clock; a time earlier than the current
        /// sample's start (a clock that went back) restarts the sample.
        /// </summary>
        public void AddFrame(double timeSeconds)
        {
            if (!_started || timeSeconds < _windowStart)
            {
                _started = true;
                _windowStart = timeSeconds;
                _intervals = 0;
                return;
            }

            _intervals++;
            double elapsed = timeSeconds - _windowStart;
            if (elapsed >= SampleSeconds)
            {
                FramesPerSecond = _intervals / elapsed;
                _windowStart = timeSeconds;
                _intervals = 0;
            }
        }

        /// <summary>Forgets every frame and the last measurement (e.g. while the display is off, so it restarts fresh).</summary>
        public void Reset()
        {
            _started = false;
            _intervals = 0;
            FramesPerSecond = null;
        }
    }
}
