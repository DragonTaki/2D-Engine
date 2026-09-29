/* ----- ----- ----- ----- */
// SizeMode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/29
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Constants.Core
{
    /// <summary>
    /// How one axis of an element's size is resolved (see <c>Engine.UI.Models.LayoutSize</c>).
    /// </summary>
    public enum SizeMode
    {
        /// <summary>A fixed length in design units (or the element's declared Size when no value is given).</summary>
        Fixed,

        /// <summary>A fraction (0-1) of the parent's content size on the same axis.</summary>
        Percent,

        /// <summary>Fills the available space on this axis, minus the element's margins.</summary>
        Stretch,

        /// <summary>The element's intrinsic (content) size, from <c>MeasureIntrinsicSize</c>.</summary>
        Auto
    }
}
