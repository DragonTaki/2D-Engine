/* ----- ----- ----- ----- */
// UIMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/27
// Version: v1.1
/* ----- ----- ----- ----- */

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
            var buttonHeight = menu.ButtonList[0].Size.Y;
            menu.ScrollContainer.ContentHeight = menu.ButtonList.Count * (buttonHeight + menu.ButtonSpacing) - menu.ButtonSpacing;
        }

    }
}
