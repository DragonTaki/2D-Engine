/* ----- ----- ----- ----- */
// UIMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/27
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    public class UIMenuHandler<TElement, THandler, TRenderer> : UIContainerHandler<TElement, THandler, TRenderer>
        where TElement : UIMenu<TElement, THandler, TRenderer>
        where THandler : UIMenuHandler<TElement, THandler, TRenderer>
        where TRenderer : UIMenuRenderer<TElement, THandler, TRenderer>
    {
        public UIMenuHandler() { }
        
        public void UpdateScrollContentHeight()
        {
            var menu = (UIMenu<TElement, THandler, TRenderer>)Element;
            if (menu.ButtonList.Count == 0) return;

            // Content extent is the lowest button bottom edge, not N * (height + spacing):
            // that formula dropped the first button's offset (e.g. 40px on the main menu),
            // so 8 buttons measured exactly the viewport height, OverContent was false and
            // the container always rebounded to the top with the last button cut off.
            float contentHeight = 0f;
            foreach (var button in menu.ButtonList)
                contentHeight = Math.Max(contentHeight, button.LocalPosition.Base.Y + button.Size.Y);

            menu.ScrollContainer.ContentHeight = contentHeight;
        }

    }
}
