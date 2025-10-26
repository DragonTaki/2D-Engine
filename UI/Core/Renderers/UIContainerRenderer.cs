/* ----- ----- ----- ----- */
// UIContainerRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    public class UIContainerRenderer<THandler> : UIRenderer
        where THandler : UIContainerHandler<THandler>
    {
        protected UIContainer<THandler> Container;

        public UIContainerRenderer(UIContainer<THandler> container)
        {
            Container = container;
        }

        public override void Render(Graphics g, UIElement element)
        {
            //
        }
    }
}
