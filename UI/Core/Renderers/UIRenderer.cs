/* ----- ----- ----- ----- */
// UIRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/25
// Update Date: 2025/10/25
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

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

        protected virtual void OnRender(Graphics g, UIElement element)
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
        /// Sets <see cref="Element"/> temporarily, calls the main rendering method, and clears reference.
        /// </summary>
        /// <param name="g">Graphics context to draw on.</param>
        /// <param name="element">The UI element to render.</param>
        public override void Render(Graphics g, UIElementBase element)
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
        public virtual void OnRender(Graphics g, TElement element) { }

        #endregion

        #region Virtual Hooks

        /// <summary>
        /// Hook invoked before <see cref="OnRender"/>.
        /// Can be used for setup, measurement, or pre-render effects.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="element">UI element to render.</param>
        protected virtual void BeforeRender(Graphics g, TElement element) { }

        /// <summary>
        /// Hook invoked after <see cref="OnRender"/>.
        /// Can be used for overlays, debug visuals, or post-render adjustments.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="element">UI element rendered.</param>
        protected virtual void AfterRender(Graphics g, TElement element) { }

        /// <summary>
        /// 通知這個元素需要重繪
        /// </summary>
        public virtual void Invalidate()
        {
            // 如果有 UI 系統管理容器，可以通知上層重繪
            Element?.RequestRedraw();
        }

        #endregion
    }

    public abstract class UIRenderer<TElement> : UIRendererBase
        where TElement : UIElementBase
    {
        public void Render(Graphics g, TElement element)
        {
            OnRender(g, element);
        }

        protected abstract void OnRender(Graphics g, TElement element);
    }
}
