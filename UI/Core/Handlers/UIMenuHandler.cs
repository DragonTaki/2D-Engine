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
        protected UIMenu<THandler> _menu;

        protected override void OnInit((IUiFactory, UIContainer<THandler>) arg)
        {
            _factory = arg.Item1;

            if (arg.Item2 is UIMenu<THandler> menu)
            {
                _menu = menu;
                OnMenuInit((_factory, menu));
            }

            _navigationManager = _factory.Resolve<NavigationManager>();

            OnMenuInit(arg);
        }

        protected virtual void OnMenuInit((IUiFactory, UIContainer<THandler>) arg) { }
    }
}
