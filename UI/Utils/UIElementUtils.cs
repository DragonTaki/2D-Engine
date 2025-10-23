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
        /// Updates the <see cref="UIElement.IsEnabled"/> property based on visibility within a specified clipping area.
        /// Elements outside the clipping rectangle are marked as disabled.
        /// </summary>
        /// <typeparam name="T">Type of UIElement (or derived type)</typeparam>
        /// <param name="elements">Enumerable collection of UI elements to update</param>
        /// <param name="clippingRect">The clipping rectangle representing the visible viewport</param>
        public static void UpdateVisibleState<T>(IEnumerable<T> elements, RectangleF clippingRect) where T : UIElement
        {
            foreach (var element in elements)
            {
                float y = element.LocalPosition.Current.Y;  // Current Y position relative to parent/root
                float h = element.Size.Y;                   // Height of the element

                // Enable element if any portion is visible inside clipping rectangle
                element.IsEnabled = y + h > clippingRect.Top && y < clippingRect.Bottom;
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
