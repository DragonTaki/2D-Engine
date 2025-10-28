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

using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Allows combining multiple renderers in sequential order.
    /// </summary>
    public class CompositeRenderer<TElement, THandler, TRenderer> : UIRenderer<TElement, THandler, TRenderer>
        where TElement : UIElement<TElement, THandler, TRenderer>
        where THandler : UIHandler<TElement, THandler, TRenderer>
        where TRenderer : UIRenderer<TElement, THandler, TRenderer>
    {
        private readonly List<UIRenderer<TElement, THandler, TRenderer>> _renderers = new();
        public int ListCount => _renderers.Count;

        /// <summary>
        /// Adds a renderer into the composite pipeline.
        /// </summary>
        public CompositeRenderer<TElement, THandler, TRenderer> Add(UIRenderer<TElement, THandler, TRenderer> renderer)
        {
            if (renderer != null)
                _renderers.Add(renderer);
            return this;
        }

        public CompositeRenderer<TElement, THandler, TRenderer> Remove(UIRenderer<TElement, THandler, TRenderer> renderer)
        {
            if (renderer != null)
                _renderers.Remove(renderer);
            return this;
        }

        public CompositeRenderer<TElement, THandler, TRenderer> Clear()
        {
            _renderers.Clear();
            return this;
        }

        /// <summary>
        /// Draws all renderers in the order they were added.
        /// </summary>
        public override void OnRender(Graphics g, TElement element)
        {
            foreach (var renderer in _renderers)
                renderer.OnRender(g, element);
        }
    }
}
