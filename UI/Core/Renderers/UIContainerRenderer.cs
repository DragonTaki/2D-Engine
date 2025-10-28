/* ----- ----- ----- ----- */
// UIContainerRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UIContainer{THandler}"/>. Handles the drawing
    /// of container elements, optionally delegating to child elements or applying
    /// container-specific visual effects.
    /// </summary>
    /// <typeparam name="THandler">The type of container handler this renderer is associated with.</typeparam>
    public class UIContainerRenderer<TElement, THandler, TRenderer>
    : UIRenderer<TElement, THandler, TRenderer>
    where TElement : UIContainer<TElement, THandler, TRenderer>
    where THandler : UIContainerHandler<TElement, THandler, TRenderer>
    where TRenderer : UIContainerRenderer<TElement, THandler, TRenderer>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="UIContainerRenderer{THandler}"/>.
        /// </summary>
        public UIContainerRenderer() : base() { }

        #endregion

        #region Rendering

        /// <summary>
        /// Performs the rendering of the container and its child elements.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object to draw on.</param>
        /// <param name="element">The UI element being rendered (should match <see cref="Container"/>).</param>
        public override void OnRender(Graphics g, TElement element)
        {
            // Placeholder for rendering logic.
            // TODO: implement container-specific drawing, e.g., background, borders.
            // Optionally, iterate over Container.Children and invoke their renderers.

            //
        }

        #endregion
    }
}
