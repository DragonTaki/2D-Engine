/* ----- ----- ----- ----- */
// UIContainer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Engine.Diagnostics;
using Engine.Styles;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Engine-level generic UIContainer.
    /// </summary>
    public abstract class UIContainer<TElement, THandler, TRenderer> : UIElement<TElement, THandler, TRenderer>, IUiContainer
        where TElement : UIContainer<TElement, THandler, TRenderer>
        where THandler : UIContainerHandler<TElement, THandler, TRenderer>
        where TRenderer : UIContainerRenderer<TElement, THandler, TRenderer>
    {
        #region Fields / Properties

        /// <summary>
        /// Actions queued by <see cref="Post"/>. Only containers whose handler drains this
        /// queue run them (today only the game board's handler); the base container handler does not.
        /// </summary>
        public readonly List<Action> PendingActions = new();

        /// <summary>
        /// Optional background/border style for this container, drawn by
        /// <see cref="UIContainerRenderer{TElement, THandler, TRenderer}"/>
        /// when set. Left <c>null</c> by default, so containers with no
        /// style assigned draw nothing at this layer (unchanged behavior).
        /// </summary>
        public IBoxDrawStyle Style { get; set; }

        #endregion

        #region Constructor

        protected UIContainer(int zIndex = 0, bool isPersistent = false, UIElementType type = UIElementType.Generic)
            : base(zIndex, isPersistent, type)
        { }

        #endregion

        #region Methods

        public override void Init(IUiFactory factory, THandler handler, TRenderer renderer)
        {
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"[UIContainer]Init 3 generic Current type: {this?.GetType().FullName ?? "null"}, IsInitialized: {IsInitialized}");

            if (IsInitialized)
                return;

            IsInitialized = true;

            _factory = factory;

            OnBeforeInit(factory);

            // Bind Handler
            Handler = handler;
            Handler.Element = (TElement)(object)this;
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"[UIContainer]Handler type: {Handler?.GetType().FullName ?? "null"}");

            // Bind Renderer
            Renderer = renderer;
            Renderer.Element = (TElement)(object)this;
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"[UIContainer]Renderer type: {Renderer?.GetType().FullName ?? "null"}");

            RunInitHooks();

            BuildUIObjects();

            OnInit(factory);
            OnAfterInit(factory);
        }

        /// <summary>
        /// Hook for building child elements, run during Init after the parameterless init hooks.
        /// </summary>
        protected virtual void BuildUIObjects() { }

        /// <inheritdoc/>
        public void Post(Action action)
        {
            PendingActions.Add(action);
        }

        #endregion
    }
}
