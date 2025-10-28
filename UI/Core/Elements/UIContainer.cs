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

        public override void Init(IUiFactory factory, THandler handler, TRenderer renderer)
        {
            Console.WriteLine($"[UIContainer]Init 3 generic Current type: {this?.GetType().FullName ?? "null"}, IsInitialized: {IsInitialized}");

            if (IsInitialized)
                return;

            IsInitialized = true;

            _factory = factory;

            OnBeforeInit(factory);

            // 綁定 Handler
            Handler = handler;
            Handler.Element = (TElement)(object)this;
            Console.WriteLine($"[UIContainer]Handler type: {Handler?.GetType().FullName ?? "null"}");

            // 綁定 Renderer
            Renderer = renderer;
            Renderer.Element = (TElement)(object)this;
            Console.WriteLine($"[UIContainer]Renderer type: {Renderer?.GetType().FullName ?? "null"}");

            BuildUIObjects();

            OnInit(factory);
            OnAfterInit(factory);
        }

        protected virtual void BuildUIObjects() { }

        #endregion

        public void Post(Action action)
        {
            _pendingActions.Add(action);
        }
    }
}
