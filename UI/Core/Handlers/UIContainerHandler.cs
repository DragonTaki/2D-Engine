/* ----- ----- ----- ----- */
// UIMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Base;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;

namespace Engine.UI.Core.Handlers
{
    public abstract class UIContainerHandler<THandler> : InitializableOnceBase<(IUiFactory, UIContainer<THandler>)>
        where THandler : UIContainerHandler<THandler>
    {
        protected IUiFactory _factory;
        protected UIContainer<THandler> _container;
        protected NavigationManager _navigationManager;

        protected override void OnInit((IUiFactory, UIContainer<THandler>) arg)
        {
            _factory = arg.Item1;
            _container = arg.Item2;
            _navigationManager = _factory.Resolve<NavigationManager>();
        }

        #region Lifecycle

        public virtual void OnEnter() {}

        public virtual void OnExit() {}

        #endregion
    }
}
