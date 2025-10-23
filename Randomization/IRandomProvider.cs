/* ----- ----- ----- ----- */
// IRandomProvider.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/10
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Randomization
{
    /// <summary>
    /// Interface for generating random values of various numeric types.
    /// </summary>
    public interface IRandomProvider
    {
        /// <summary>
        /// Returns the next random integer.
        /// </summary>
        int NextInt();

        /// <summary>
        /// Returns the next random integer in the range [0, max).
        /// </summary>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>An integer in [0, max).</returns>
        int NextInt(int max);

        /// <summary>
        /// Returns the next random integer in the range [min, max).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>An integer in [min, max).</returns>
        int NextInt(int min, int max);

        /// <summary>
        /// Returns the next random float in the range [0.0, 1.0).
        /// </summary>
        float NextFloat();

        /// <summary>
        /// Returns the next random float in the range [0.0, max).
        /// </summary>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>A float in [0.0, max).</returns>
        float NextFloat(float max);

        /// <summary>
        /// Returns the next random float in the range [min, max).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>A float in [min, max).</returns>
        float NextFloat(float min, float max);

        /// <summary>
        /// Returns the next random double in the range [0.0, 1.0).
        /// </summary>
        double NextDouble();

        /// <summary>
        /// Returns the next random double in the range [0.0, max).
        /// </summary>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>A double in [0.0, max).</returns>
        double NextDouble(double max);

        /// <summary>
        /// Returns the next random double in the range [min, max).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>A double in [min, max).</returns>
        double NextDouble(double min, double max);
    }
}
