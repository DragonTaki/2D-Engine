/* ----- ----- ----- ----- */
// UIButtonRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UIButton{THandler}"/>. Handles the drawing
    /// of container elements, optionally delegating to child elements or applying
    /// container-specific visual effects.
    /// </summary>
    /// <typeparam name="THandler">The type of container handler this renderer is associated with.</typeparam>
    public class UIButtonRenderer : UIRenderer<UIButton, UIButtonHandler, UIButtonRenderer>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="UIButtonRenderer{THandler}"/>.
        /// </summary>
        public UIButtonRenderer() { }

        #endregion

        #region Rendering

        protected override void OnRender(Graphics g, UIButton element)
        {
            // Placeholder for rendering logic.
            // TODO: implement container-specific drawing, e.g., background, borders.
            // Optionally, iterate over Container.Children and invoke their renderers.

            //
        }

        #endregion
    }

    public class UIButtonRenderer<TEnum> : UIButtonRenderer
        where TEnum : Enum
    {
        //
    }
}
