/* ----- ----- ----- ----- */
// UIElementUtils.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;

namespace Engine.UI.Utils
{
    /// <summary>
    /// Utility class providing helper functions for UIElement operations.
    /// </summary>
    public static class UIElementUtils
    {
        #region Methods

        /// <summary>
        /// Updates <see cref="UIElementBase.IsClipped"/> from visibility within a clipping area:
        /// elements entirely outside the clipping rectangle are clipped. <see cref="UIElementBase.IsEnabled"/>
        /// is not touched (it used to be, which re-enabled buttons the game had disabled).
        /// </summary>
        /// <typeparam name="T">Type of UIElement (or derived type)</typeparam>
        /// <param name="elements">Enumerable collection of UI elements to update</param>
        /// <param name="clippingRect">The clipping rectangle representing the visible viewport</param>
        public static void UpdateVisibleState<T>(IEnumerable<T> elements, RectangleF clippingRect)
            where T : UIElementBase
        {
            foreach (var element in elements)
            {
                // Absolute bounds (including the scroll offset), the same space as clippingRect.
                // LocalPosition is relative to the scroll content and never moves when
                // scrolling, so comparing it against the absolute viewport culled the wrong
                // buttons (e.g. ones scrolled into view from below stayed hidden).
                var bounds = element.GetCurrentAbsoluteBounds();
                float y = bounds.Position.Y;
                float h = bounds.Size.Y;

                // Clipped unless some portion is inside the clipping rectangle (IsEnabled stays the owner's).
                element.IsClipped = !(y + h > clippingRect.Top && y < clippingRect.Bottom);
            }
        }

        /// <summary>
        /// Retrieves the existing <see cref="UIOverlayNode"/> from the root of the element hierarchy,
        /// or creates a new one if none exists.
        /// </summary>
        /// <param name="element">A UI element to start searching from (typically any child)</param>
        /// <returns>The existing or newly created <see cref="UIOverlayNode"/> attached to the root node</returns>
        public static UIOverlayNode GetOrCreateOverlay(this UIElement element)
        {
            var root = element.GetRoot();  // Get the topmost root node
            var overlay = root.Children.OfType<UIOverlayNode>().FirstOrDefault(); // Search for existing overlay

            if (overlay == null)
            {
                overlay = new UIOverlayNode(); // Create new overlay if not found
                root.AddChild(overlay);         // Add overlay to root
            }

            return overlay;
        }

        #endregion
    }
}
