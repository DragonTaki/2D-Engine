/* ----- ----- ----- ----- */
// WeightedRandomSelector.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

namespace Engine.Randomization
{
    /// <summary>
    /// Helper class for choosing a weighted random element.
    /// </summary>
    public static class WeightedRandomSelector
    {
        /// <summary>
        /// Randomly selects one element from a list based on associated weights.
        /// </summary>
        /// <typeparam name="T">The type of the element to choose.</typeparam>
        /// <param name="items">The list of items to choose from.</param>
        /// <param name="weights">The list of weights corresponding to each item (must be same length as items).</param>
        /// <param name="random">An IRandomProvider instance for generating random values.</param>
        /// <returns>One element from the list, selected according to its weight.</returns>
        /// <exception cref="ArgumentException">Thrown when items and weights count mismatch, or total weight is non-positive.</exception>
        public static T ChooseByWeight<T>(IReadOnlyList<T> items, IReadOnlyList<float> weights, IRandomProvider random)
        {
            if (items.Count != weights.Count)
                throw new ArgumentException("Items and weights must have the same count.");
            if (items.Count == 0)
                throw new ArgumentException("Items must not be empty.");

            float totalWeight = 0f;
            for (int i = 0; i < weights.Count; i++)
                totalWeight += weights[i];

            if (totalWeight <= 0f)
                throw new ArgumentException("Total weight must be greater than 0.");

            float r = random.NextFloat(0, totalWeight);
            float accumulated = 0f;

            for (int i = 0; i < items.Count; i++)
            {
                accumulated += weights[i];
                if (r < accumulated)
                    return items[i];
            }

            // Fallback (should not occur if weights are valid)
            return items[^1];
        }
    }
}
