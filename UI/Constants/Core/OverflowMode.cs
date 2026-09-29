/* ----- ----- ----- ----- */
// OverflowMode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/29
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Constants.Core
{
    /// <summary>
    /// What a container does with children that extend past its content box.
    /// </summary>
    public enum OverflowMode
    {
        /// <summary>Children are drawn and hit-tested outside the box as well.</summary>
        Visible,

        /// <summary>Children are clipped to the content box (drawing and hit testing).</summary>
        Hidden,

        /// <summary>
        /// Clips like <see cref="Hidden"/>. Scrolling itself is not implemented by the
        /// layout system: use <c>UIScrollContainer</c> for scrollable content.
        /// </summary>
        Scroll
    }
}
