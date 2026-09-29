/* ----- ----- ----- ----- */
// DisplayMode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/29
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Constants.Core
{
    /// <summary>
    /// Whether an element takes part in layout at all (like CSS <c>display</c>).
    /// </summary>
    /// <remarks>
    /// Separate from <c>IsVisible</c>: an invisible element is not drawn but still
    /// occupies its space (CSS <c>visibility: hidden</c>); <see cref="None"/> removes it
    /// from layout, drawing and input entirely.
    /// </remarks>
    public enum DisplayMode
    {
        /// <summary>Takes part in layout normally.</summary>
        Normal,

        /// <summary>Removed from layout (takes no space), not drawn, not hit-tested.</summary>
        None
    }
}
