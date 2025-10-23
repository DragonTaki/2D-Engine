/* ----- ----- ----- ----- */
// GlobalRandom.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/10
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Randomization
{
    /// <summary>
    /// Provides a globally accessible random number provider.
    /// </summary>
    public static class GlobalRandom
    {
        /// <summary>
        /// The current global random number provider instance.
        /// Default is a <see cref="RandomTable"/> with size 10,000 and seed 12345.
        /// </summary>
        public static IRandomProvider Instance { get; set; } = new RandomTable(size: 10000, seed: 12345);
    }
}
