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
    /// Represents a float range with inclusive minimum and maximum values.
    /// </summary>
    public class RangeF
    {
        /// <summary>
        /// The minimum value of the range (inclusive).
        /// </summary>
        public float Min { get; set; }

        /// <summary>
        /// The maximum value of the range (inclusive).
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// Initializes a new instance of the RangeF class with the given minimum and maximum values.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        public RangeF(float min, float max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Returns a random float value within the range [Min, Max], using the global random instance.
        /// </summary>
        /// <returns>A float between Min and Max.</returns>
        public float GetRandom()
        {
            return GlobalRandom.Instance.NextFloat(Min, Max);
        }
    }
}
