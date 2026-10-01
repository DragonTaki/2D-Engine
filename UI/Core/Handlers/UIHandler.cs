/* ----- ----- ----- ----- */
// UIHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Infrastructure;

namespace Engine.UI.Core.Handlers
{
    /// <summary>
    /// Base UI handler: every mouse event is left unhandled and the frame callbacks do
    /// nothing until a subclass overrides them.
    /// </summary>
    public abstract class UIHandler : UIHandlerBase
    {
        protected IUiFactory _factory;
        protected NavigationManager _navigationManager;

        /// <summary>
        /// Tracks whether this element has been initialized.
        /// </summary>
        public bool IsInitialized { get; protected set; }

        public virtual void Init()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            RunInitHooks();
        }

        /// <summary>
        /// Runs the parameterless init hooks without touching <see cref="IsInitialized"/>,
        /// so Init overloads that already set the flag can still invoke them (as
        /// <c>UIElement.RunInitHooks</c> does for elements).
        /// </summary>
        protected void RunInitHooks()
        {
            BeforeInit();
            OnInit();
            AfterInit();
        }

        protected virtual void BeforeInit() { }

        protected virtual void OnInit() { }

        protected virtual void AfterInit() { }

        internal override bool HandleMouseDown(IMouseEvent e)
        {
            return false;  // By default the event is not handled; a subclass returns true to indicate it handled it
        }

        internal override bool HandleMouseMove(IMouseEvent e)
        {
            return false;
        }

        internal override bool HandleMouseUp(IMouseEvent e)
        {
            return false;
        }

        internal override bool HandleMouseWheel(IMouseEvent e)
        {
            return false;
        }

        internal override bool HandleMouseClick(IMouseEvent e)
        {
            return false;
        }

        internal override void OnUpdate() { }

        internal override void OnEndFrame() { }
    }

    /// <summary>
    /// Strongly-typed handler bound to one element type.
    /// </summary>
    public class UIHandler<TElement, THandler, TRenderer> : UIHandler
        where TElement : UIElement<TElement, THandler, TRenderer>
        where THandler : UIHandler<TElement, THandler, TRenderer>
        where TRenderer : UIRenderer<TElement, THandler, TRenderer>
    {
        public new TElement Element
        {
            get => (TElement)base.Element;
            protected internal set => base.Element = value;
        }

        public override void Init(IUiFactory factory, UIElementBase element)
        {
            if (IsInitialized) return;
            IsInitialized = true;

            Element = (TElement)element;
            _factory = factory;
            _navigationManager = _factory.Resolve<NavigationManager>();

            // Same order as UIElement's generic Init: the parameterless hooks run between
            // the factory Before and On hooks. Without this, an override of OnInit() etc.
            // never ran - this is the overload the UI factory calls, and it had already set
            // IsInitialized, so the parameterless Init() would return immediately.
            BeforeInit(factory);
            RunInitHooks();
            OnInit(factory);
            AfterInit(factory);
        }

        protected virtual void BeforeInit(IUiFactory factory) { }

        protected virtual void OnInit(IUiFactory factory) { }

        protected virtual void AfterInit(IUiFactory factory) { }
    }
}
