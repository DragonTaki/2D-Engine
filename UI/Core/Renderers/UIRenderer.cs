/* ----- ----- ----- ----- */
// UIRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/25
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Base class for all UI renderers.
    /// Provides a unified interface and common utilities for drawing UI elements.
    /// </summary>
    public abstract class UIRenderer : UIRendererBase
    {
        /// <summary>
        /// Tracks whether this element has been initialized.
        /// </summary>
        public bool IsInitialized { get; protected set; }

        protected virtual void OnRender(IGraphics g, UIElement element)
        {
            // basic draw logic
        }
    }

    public abstract class UIRenderer<TElement, THandler, TRenderer> : UIRenderer
        where TElement : UIElement<TElement, THandler, TRenderer>
        where THandler : UIHandler<TElement, THandler, TRenderer>
        where TRenderer : UIRenderer<TElement, THandler, TRenderer>
    {
        public new TElement Element
        {
            get => (TElement)base.Element;
            protected internal set => base.Element = value;
        }

        public override void Init(UIElementBase element)
        {
            if (IsInitialized) return;
            IsInitialized = true;

            Element = (TElement)element;

            BeforeInit();
            OnInit();
            AfterInit();
        }

        protected virtual void BeforeInit() { }

        protected virtual void OnInit() { }

        protected virtual void AfterInit() { }

        #region Public Methods

        /// <summary>
        /// Entry point to render a UI element.
        /// Casts the element to <typeparamref name="TElement"/> and runs <see cref="BeforeRender"/>,
        /// <see cref="OnRender"/> and <see cref="AfterRender"/> in that order. <see cref="Element"/>
        /// is not touched here; it is bound once in <see cref="Init"/>.
        /// </summary>
        /// <param name="g">Graphics context to draw on.</param>
        /// <param name="element">The UI element to render.</param>
        public override void Render(IGraphics g, UIElementBase element)
        {
            TElement _element = (TElement)element;

            // Optional pre-render setup
            BeforeRender(g, _element);

            // Main render logic implemented in derived class
            OnRender(g, _element);

            // Optional post-render actions
            AfterRender(g, _element);
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Core rendering logic to be implemented by derived classes.
        /// </summary>
        /// <param name="g">Graphics context to draw on.</param>
        /// <param name="element">The UI element being rendered.</param>
        public virtual void OnRender(IGraphics g, TElement element) { }

        #endregion

        #region Virtual Hooks

        /// <summary>
        /// Hook invoked before <see cref="OnRender"/>.
        /// Can be used for setup, measurement, or pre-render effects.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="element">UI element to render.</param>
        protected virtual void BeforeRender(IGraphics g, TElement element) { }

        /// <summary>
        /// Hook invoked after <see cref="OnRender"/>.
        /// Can be used for overlays, debug visuals, or post-render adjustments.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="element">UI element rendered.</param>
        protected virtual void AfterRender(IGraphics g, TElement element) { }

        /// <summary>
        /// Notifies that this element needs to be redrawn.
        /// </summary>
        public virtual void Invalidate()
        {
            // If a UI system manages the container, this can notify the upper layer to redraw
            Element?.RequestRedraw();
        }

        #endregion
    }

    public abstract class UIRenderer<TElement> : UIRendererBase
        where TElement : UIElementBase
    {
        public void Render(IGraphics g, TElement element)
        {
            OnRender(g, element);
        }

        // UIElement.Draw calls the non-generic RendererBase.Render; without this
        // override that call landed on the empty base and nothing was drawn.
        public override void Render(IGraphics g, UIElementBase element) =>
            OnRender(g, (TElement)element);

        protected abstract void OnRender(IGraphics g, TElement element);
    }
}
