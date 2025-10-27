/* ----- ----- ----- ----- */
// UIHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Windows.Forms;

using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
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

            OnInit();
        }

        protected virtual void BeforeInit() { }

        protected virtual void OnInit() { }

        protected virtual void AfterInit() { }

        internal override bool HandleMouseDown(MouseEventArgs e)
        {
            return false;  // The default is not to process, and the subclass can return true to indicate successful processing
        }

        internal override bool HandleMouseMove(MouseEventArgs e)
        {
            return false;
        }

        internal override bool HandleMouseUp(MouseEventArgs e)
        {
            return false;
        }

        internal override bool HandleMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        internal override bool HandleMouseClick(MouseEventArgs e)
        {
            return false;
        }

        internal override void OnUpdate() { }

        internal override void OnEndFrame() { }
    }

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

            BeforeInit(factory);
            OnInit(factory);
            AfterInit(factory);
        }

        protected virtual void BeforeInit(IUiFactory factory) { }

        protected virtual void OnInit(IUiFactory factory) { }

        protected virtual void AfterInit(IUiFactory factory) { }
    }
}
