/* ----- ----- ----- ----- */
// AspectFit.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/29
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Constants.Core
{
    /// <summary>
    /// How an aspect-ratio-locked element fits the box its size modes resolved to
    /// (like CSS <c>object-fit</c>).
    /// </summary>
    public enum AspectFit
    {
        /// <summary>Largest box of the ratio that fits inside the resolved box.</summary>
        Contain,

        /// <summary>Smallest box of the ratio that covers the resolved box (overflows it).</summary>
        Cover
    }
}
