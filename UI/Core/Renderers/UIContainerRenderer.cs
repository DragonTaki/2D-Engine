/* ----- ----- ----- ----- */
// UIContainerRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UIContainer{TElement, THandler, TRenderer}"/>. Draws the
    /// container's own background/border <c>Style</c> when one is assigned; children are
    /// drawn by the central render pipeline, not by this renderer.
    /// </summary>
    /// <typeparam name="TElement">The container element type this renderer draws.</typeparam>
    /// <typeparam name="THandler">The type of container handler this renderer is associated with.</typeparam>
    /// <typeparam name="TRenderer">The concrete renderer type (self-referencing).</typeparam>
    public class UIContainerRenderer<TElement, THandler, TRenderer>
        : UIRenderer<TElement, THandler, TRenderer>
        where TElement : UIContainer<TElement, THandler, TRenderer>
        where THandler : UIContainerHandler<TElement, THandler, TRenderer>
        where TRenderer : UIContainerRenderer<TElement, THandler, TRenderer>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="UIContainerRenderer{TElement, THandler, TRenderer}"/>.
        /// </summary>
        public UIContainerRenderer() : base() { }

        #endregion

        #region Rendering

        /// <summary>
        /// Renders the container's own style (background/border), if any; children are not drawn here.
        /// </summary>
        /// <param name="g">The <see cref="IGraphics"/> surface to draw on.</param>
        /// <param name="element">The container element being rendered.</param>
        public override void OnRender(IGraphics g, TElement element)
        {
            // Draws the container's own background/border only when a Style
            // is assigned; containers with none keep the previous no-op
            // behavior. Children are rendered by the central render
            // pipeline, not from here.
            element.Style?.Draw(g, element.GetCurrentAbsoluteBounds());
        }

        #endregion
    }
}
