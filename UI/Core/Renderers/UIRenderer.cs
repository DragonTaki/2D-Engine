/* ----- ----- ----- ----- */
// UIRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/25
// Update Date: 2025/10/25
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.UI.Core.Elements;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Base class for all UI renderers. 
    /// Provides a unified interface and common utilities for drawing UI elements.
    /// </summary>
    public abstract class UIRenderer
    {
        /// <summary>
        /// Draws the specified UI element using the provided Graphics context.
        /// </summary>
        /// <param name="g">Graphics context to draw on.</param>
        /// <param name="element">Target UI element.</param>
        public abstract void Render(Graphics g, UIElement element);

        /// <summary>
        /// Optionally invoked before rendering; used for setup or prepass.
        /// </summary>
        protected virtual void BeforeRender(Graphics g, UIElement element) { }

        /// <summary>
        /// Optionally invoked after rendering; used for overlays or debug visuals.
        /// </summary>
        protected virtual void AfterRender(Graphics g, UIElement element) { }
    }
}
