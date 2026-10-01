/* ----- ----- ----- ----- */
// RangeF.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/16
// Version: v1.1
/* ----- ----- ----- ----- */

using Engine.Randomization;

namespace Engine.Mathematics
{
    /// <summary>
    /// Represents a float range between a minimum and a maximum value.
    /// <see cref="GetRandom"/> samples it half-open, [Min, Max).
    /// </summary>
    public class RangeF
    {
        /// <summary>
        /// The minimum value of the range (inclusive; <see cref="GetRandom"/> can return it).
        /// </summary>
        public float Min { get; set; }

        /// <summary>
        /// The maximum value of the range (<see cref="GetRandom"/> samples up to, but never returns, this value).
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// Initializes a new instance of the RangeF class with the given minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum value (inclusive).</param>
        /// <param name="max">The maximum value (the exclusive upper bound when sampling).</param>
        public RangeF(float min, float max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Returns a random float value within the half-open range [Min, Max), using the global random instance.
        /// </summary>
        /// <returns>A float in [Min, Max).</returns>
        public float GetRandom()
        {
            return GlobalRandom.Instance.NextFloat(Min, Max);
        }
    }
}
