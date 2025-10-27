/* ----- ----- ----- ----- */
// UIContainerHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    public abstract class UIContainerHandler<TElement, THandler, TRenderer>
    : UIHandler<TElement, THandler, TRenderer>
    where TElement : UIContainer<TElement, THandler, TRenderer>
    where THandler : UIContainerHandler<TElement, THandler, TRenderer>
    where TRenderer : UIContainerRenderer<TElement, THandler, TRenderer>
    {
        public UIContainerHandler() { }
    }
}
