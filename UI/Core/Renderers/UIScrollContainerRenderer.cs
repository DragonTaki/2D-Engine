/* ----- ----- ----- ----- */
// UIScrollContainerRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UIScrollContainer"/>: only the container <c>Style</c>. It does
    /// not clip the children to the viewport (menus and text boxes clip their own content).
    /// </summary>
    public class UIScrollContainerRenderer : UIContainerRenderer<UIScrollContainer, UIScrollContainerHandler, UIScrollContainerRenderer>
    {
        public UIScrollContainerRenderer() { }
    }
}
