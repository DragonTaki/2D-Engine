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
using System.Drawing;

using Engine.Mathematics;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Engine層通用 UIContainer
    /// </summary>
    public abstract class UIContainer<THandler> : UIElement, IUiContainer, IInitializableOnce<(IUiFactory, THandler)>
        where THandler : UIContainerHandler<THandler>
    {
        #region Fields / Properties

        protected IUiFactory _factory;
        protected THandler _handler;
        public THandler Handler => _handler;
        protected UIContainerRenderer<THandler> _renderer;
        protected readonly List<Action> _pendingActions = new();

        #endregion

        #region Constructor
        // 無參數建構子，C# 編譯器需要
        public UIContainer()
        {
            LocalPosition = Vector2F.Zero; // 或暫時預設
            Size = Vector2F.Zero;
        }
        public UIContainer(Vector2F position, Vector2F size)
        {
            LocalPosition.Base = position;
            Size = size;
        }
        #endregion

        #region Methods

        public void Init((IUiFactory, THandler) arg)
        {
            if (IsInitialized) return;
            IsInitialized = true;
            OnInit(arg);
        }
        /// <summary>
        /// 通用初始化流程
        /// </summary>
        protected virtual void OnInit((IUiFactory, THandler) arg)
        {
            _factory = arg.Item1;
            _handler = arg.Item2;

            LocalPosition = ContainerDefaults.Position;
            Size = ContainerDefaults.Size;

            BuildUIObjects();

            _renderer = CreateRenderer();
        }

        protected virtual void BuildUIObjects() {}

        protected virtual UIContainerRenderer<THandler> CreateRenderer()
        {
            return new UIContainerRenderer<THandler>(this);
        }

        #endregion

        #region Lifecycle

        public virtual void OnEnter()
        {
            _handler?.OnEnter();
        }

        public virtual void OnExit()
        {
            _handler?.OnExit();
        }

        #endregion

        #region Update / Draw

        protected override void OnUpdate()
        {
            //
        }

        protected override void OnDraw(Graphics g)
        {
            _renderer.Render(g, this);
        }

        #endregion

        public void Post(Action action)
        {
            _pendingActions.Add(action);
        }
    }
}
