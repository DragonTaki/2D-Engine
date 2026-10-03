/* ----- ----- ----- ----- */
// PositionMode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/29
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Constants.Core
{
    /// <summary>
    /// How an element's box is determined.
    /// </summary>
    public enum PositionMode
    {
        /// <summary>
        /// Default. The element keeps its own <c>LocalPosition</c>/<c>Size</c>, plus the
        /// one-shot <c>Anchor</c>/<c>Margin</c>/<c>SizePercent</c> rules of
        /// <c>UIElement.UpdateLayout</c> - exactly the behavior before the layout system.
        /// Ignored by the parent's flex/flow arrangement (out of flow).
        /// </summary>
        Legacy,

        /// <summary>
        /// In flow: laid out by the parent - by its flex rules when the parent is a flex
        /// container, otherwise sized and aligned by this element's own rules inside the
        /// parent's content box (like CSS <c>position: static</c>).
        /// </summary>
        Flow,

        /// <summary>
        /// Out of flow: positioned by the Left/Top/Right/Bottom insets relative to the
        /// parent's padding box (like CSS <c>position: absolute</c>).
        /// </summary>
        Absolute
    }
}
