/* ----- ----- ----- ----- */
// UIMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Engine.Mathematics;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Models;
using Engine.UI.Utils;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Engine層通用 Menu
    /// </summary>
    public abstract class UIMenu<TElement, THandler, TRenderer>
    : UIContainer<TElement, THandler, TRenderer>
    where TElement : UIMenu<TElement, THandler, TRenderer>
    where THandler : UIMenuHandler<TElement, THandler, TRenderer>
    where TRenderer : UIMenuRenderer<TElement, THandler, TRenderer>
    {
        #region Fields / Properties
        public UIScrollContainer ScrollContainer { get; private set; }
        protected List<UIButton> Buttons { get; } = new();
        public float ButtonSpacing { get; set; } = 10f;
        protected float ButtonMargin = 5.0f;
        public bool IsVerticalLayout { get; set; } = true;

        #endregion

        #region Constructor

        public UIMenu() { }

        #endregion

        #region Methods

        /// <summary>
        /// 通用初始化流程
        /// </summary>
        public override void Init(IUiFactory factory, THandler handler, TRenderer renderer)
        {
            Console.WriteLine($"[UIMenu]Init 3 generic Current type: {this?.GetType().FullName ?? "null"}, IsInitialized: {IsInitialized}");
            if (IsInitialized) return;
            IsInitialized = true;
            _factory = factory;

            BeforeInit(factory);

            // 綁定 Handler
            Handler = handler;
            Handler.Element = (TElement)(object)this;
            Console.WriteLine($"[UIMenu]Handler type: {Handler?.GetType().FullName ?? "null"}");

            // 綁定 Renderer
            Renderer = renderer;
            Renderer.Element = (TElement)(object)this;
            Console.WriteLine($"[UIMenu]Renderer type: {Renderer?.GetType().FullName ?? "null"}");
            BuildScrollContainer();

            base.Init();

            OnInit(factory);
            AfterInit(factory);
        }

        protected override void OnInit(IUiFactory factory)
        {
            LocalPosition = MenuDefaults.Position;
            Size = MenuDefaults.Size;
        }

        protected override void AfterInit(IUiFactory factory)
        {
            BuildButtons();

            Handler.UpdateScrollContentHeight();
        }

        public virtual void BuildScrollContainer()
        {
            ScrollContainer = _factory.CreateScrollContainer();
            ScrollContainer.Layout = MenuDefaults.Scroll.Layout;
            ScrollContainer.OverscrollLimit = MenuDefaults.Scroll.OverscrollLimit;
            ScrollContainer.VerticalAlignment = ScrollAlignment.Bottom;
            AddChild(ScrollContainer);
        }

        protected abstract void BuildButtons();

        public void AddButton(UIButton button, Vector2F localPos)
        {
            button.LocalPosition = localPos;
            ScrollContainer.AddChild(button);
            Buttons.Add(button);
            Handler.UpdateScrollContentHeight();
        }

        public List<UIButton> GetVisibleButtons()
        {
            UIElementUtils.UpdateVisibleState(Buttons, ScrollContainer.GetAbsClippingRect());
            return Buttons.Where(b => b.IsEnabled).ToList();
        }

        public virtual void SetupButtons(IEnumerable<(string label, Action onClick)> buttonDefs)
        {
            Buttons.Clear();
            float offset = 0f;
            foreach (var (label, onClick) in buttonDefs)
            {
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = label;
                button.Handler.Action = onClick;

                button.LocalPosition = new UIPosition(IsVerticalLayout ? new Vector2F(0, offset) : new Vector2F(offset, 0));

                Buttons.Add(button);
                AddChild(button);

                offset += IsVerticalLayout ? button.Size.Y + ButtonSpacing : button.Size.X + ButtonSpacing;
            }
        }

        public virtual void LayoutButtons()
        {
            float offset = 0f;
            foreach (var button in Buttons)
            {
                button.LocalPosition.Base = IsVerticalLayout ? new Vector2F(0, offset) : new Vector2F(offset, 0);
                offset += IsVerticalLayout ? button.Size.Y + ButtonSpacing : button.Size.X + ButtonSpacing;
            }
        }

        public RectangleF GetAbsClipRect() => ScrollContainer.GetAbsClippingRect();

        public IReadOnlyList<UIButton> ButtonList => Buttons;
        
        #endregion
    }
}
