/* ----- ----- ----- ----- */
// SliderMath.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.UI.Utils
{
    /// <summary>
    /// Value / position arithmetic of a slider (pure, no drawing; used by <c>UISlider</c>): the
    /// values a slider offers are <c>min</c>, <c>min + step</c>, <c>min + 2·step</c>, ... up to
    /// <c>max</c>, plus <c>max</c> itself when the range is not a whole number of steps; a value
    /// maps linearly onto the track (<c>min</c> at its left end, <c>max</c> at its right end).
    /// </summary>
    public static class SliderMath
    {
        /// <summary>
        /// The offered value closest to <paramref name="value"/>: kept within
        /// <paramref name="min"/>..<paramref name="max"/>, on a step from <paramref name="min"/>
        /// (or <paramref name="max"/> when that is closer). A step of 0 or less (or NaN) means
        /// no snapping; NaN as <paramref name="value"/> gives <paramref name="min"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="min"/> is above <paramref name="max"/> (or either is NaN).</exception>
        public static float Snap(float value, float min, float max, float step)
        {
            if (!(min <= max))
                throw new ArgumentException($"min {min} is above max {max}.", nameof(min));
            if (float.IsNaN(value))
                return min;

            double v = Math.Clamp((double)value, min, max);
            if (!(step > 0f))
                return (float)v;

            double steps = Math.Round((v - min) / step, MidpointRounding.AwayFromZero);
            double snapped = Math.Min(min + steps * step, max);
            // The last partial step: max is an offered value too.
            if (Math.Abs(max - v) < Math.Abs(v - snapped))
                snapped = max;
            // Clears the binary noise of min + n·step (e.g. 0.1 + 2·0.1) so equal values compare equal.
            return (float)Math.Clamp(Math.Round(snapped, 6), min, max);
        }

        /// <summary>Where <paramref name="value"/> lies in the range, 0 (min) to 1 (max); 0 for an empty range.</summary>
        public static float Fraction(float value, float min, float max)
        {
            if (!(max > min) || float.IsNaN(value))
                return 0f;
            return Math.Clamp((value - min) / (max - min), 0f, 1f);
        }

        /// <summary>
        /// The offered value at x coordinate <paramref name="x"/> on a track from
        /// <paramref name="trackLeft"/>, <paramref name="trackWidth"/> wide (left of it: min,
        /// right of it: max; a track of no width: min).
        /// </summary>
        public static float ValueAt(float x, float trackLeft, float trackWidth, float min, float max, float step)
        {
            float fraction = trackWidth > 0f ? Math.Clamp((x - trackLeft) / trackWidth, 0f, 1f) : 0f;
            return Snap(min + fraction * (max - min), min, max, step);
        }

        /// <summary>The x coordinate of <paramref name="value"/> on a track from <paramref name="trackLeft"/>, <paramref name="trackWidth"/> wide.</summary>
        public static float PositionOf(float value, float trackLeft, float trackWidth, float min, float max) =>
            trackLeft + Fraction(value, min, max) * Math.Max(0f, trackWidth);
    }
}
