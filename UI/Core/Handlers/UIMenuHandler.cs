/* ----- ----- ----- ----- */
// UIMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Elements;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;

namespace Engine.UI.Core.Handlers
{
    public abstract class UIMenuHandler<THandler> : UIContainerHandler<THandler>
        where THandler : UIMenuHandler<THandler>
    {
        //protected IUiFactory _factory;
        protected UIMenu<THandler> _menu;
        //protected NavigationManager _navigationManager;

        protected virtual void OnInit((IUiFactory, UIMenu<THandler>) arg)
        {
            _factory = arg.Item1;
            _menu = arg.Item2;
            _navigationManager = _factory.Resolve<NavigationManager>();
        }
    }
}
