/* ----- ----- ----- ----- */
// FramePacer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Timing
{
    /// <summary>Admits one frame at a monotonic deadline, independent of the host's timer precision.</summary>
    public sealed class FramePacer
    {
        private readonly long _timestampFrequency;
        private double _period;
        private double _nextDeadline;
        private long _lastTimestamp;
        private bool _started;

        /// <summary>Creates a frame scheduler for timestamps measured at the supplied frequency.</summary>
        /// <param name="framesPerSecond">Positive maximum frame rate.</param>
        /// <param name="timestampFrequency">Positive timestamp ticks per second.</param>
        public FramePacer(int framesPerSecond, long timestampFrequency)
        {
            if (timestampFrequency <= 0)
                throw new ArgumentOutOfRangeException(nameof(timestampFrequency));
            if (framesPerSecond <= 0)
                throw new ArgumentOutOfRangeException(nameof(framesPerSecond));
            _timestampFrequency = timestampFrequency;
            _period = timestampFrequency / (double)framesPerSecond;
        }

        /// <summary>Starts a new schedule; the first frame is due one period after this timestamp.</summary>
        /// <param name="timestamp">Current monotonic timestamp.</param>
        public void Reset(long timestamp)
        {
            _lastTimestamp = timestamp;
            _nextDeadline = timestamp + _period;
            _started = true;
        }

        /// <summary>Changes the frame rate and starts a fresh schedule without retaining overdue frames.</summary>
        /// <param name="framesPerSecond">Positive maximum frame rate.</param>
        /// <param name="timestamp">Current monotonic timestamp.</param>
        public void SetFrameRate(int framesPerSecond, long timestamp)
        {
            if (framesPerSecond <= 0)
                throw new ArgumentOutOfRangeException(nameof(framesPerSecond));
            _period = _timestampFrequency / (double)framesPerSecond;
            Reset(timestamp);
        }

        /// <summary>Admits a due frame and discards missed deadlines, without a catch-up burst.</summary>
        /// <param name="timestamp">Current monotonic timestamp.</param>
        /// <returns>True for one due frame; false for an early or repeated timestamp.</returns>
        public bool TryAdvance(long timestamp)
        {
            if (!_started)
            {
                Reset(timestamp);
                return false;
            }
            if (timestamp <= _lastTimestamp || timestamp < _nextDeadline)
                return false;
            _lastTimestamp = timestamp;
            _nextDeadline += _period;
            if (_nextDeadline <= timestamp)
                _nextDeadline = timestamp + _period;
            return true;
        }
    }
}
