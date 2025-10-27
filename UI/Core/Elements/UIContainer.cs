/* ----- ----- ----- ----- */
// UIContainer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Engine.UI.Constants.Components;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Engine層通用 UIContainer
    /// </summary>
    public abstract class UIContainer<TElement, THandler, TRenderer> : UIElement<TElement, THandler, TRenderer>, IUiContainer
        where TElement : UIContainer<TElement, THandler, TRenderer>
        where THandler : UIContainerHandler<TElement, THandler, TRenderer>
        where TRenderer : UIContainerRenderer<TElement, THandler, TRenderer>
    {
        #region Fields / Properties

        public readonly List<Action> _pendingActions = new();

        #endregion

        #region Constructor

        protected UIContainer(int zIndex = 0, bool isPersistent = false, UIElementType type = UIElementType.Generic)
            : base(zIndex, isPersistent, type)
        { }

        #endregion

        #region Methods

        protected override void AfterInit(IUiFactory factory)
        {
            BuildUIObjects();
        }

        protected virtual void BuildUIObjects() { }

        #endregion

        public void Post(Action action)
        {
            _pendingActions.Add(action);
        }
    }
}
