/* ----- ----- ----- ----- */
// CompositeRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/25
// Update Date: 2025/10/25
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;
using Engine.UI.Core.Elements;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Allows combining multiple renderers in sequential order.
    /// </summary>
    public class CompositeRenderer : UIRenderer
    {
        private readonly List<UIRenderer> _renderers = new();

        /// <summary>
        /// Adds a renderer into the composite pipeline.
        /// </summary>
        public CompositeRenderer Add(UIRenderer renderer)
        {
            if (renderer != null)
                _renderers.Add(renderer);
            return this;
        }

        public CompositeRenderer Remove(UIRenderer renderer)
        {
            if (renderer != null)
                _renderers.Remove(renderer);
            return this;
        }

        public CompositeRenderer Clear()
        {
            _renderers.Clear();
            return this;
        }

        /// <summary>
        /// Draws all renderers in the order they were added.
        /// </summary>
        public override void Render(Graphics g, UIElement element)
        {
            foreach (var renderer in _renderers)
                renderer.Render(g, element);
        }
    }
}
