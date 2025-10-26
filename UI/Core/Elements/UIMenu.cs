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
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Elements;
using Engine.UI.Models;
using Engine.UI.Utils;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Engine層通用 Menu
    /// </summary>
    public abstract class UIMenu<THandler> : UIContainer<THandler>
        where THandler : UIMenuHandler<THandler>
    {
        #region Fields / Properties

        //protected IUiFactory _factory;
        //protected THandler _handler;
        //protected UIMenuRenderer<THandler> _renderer;
        protected UIMenuRenderer<THandler> _menuRenderer;
        protected UIScrollContainer ScrollContainer { get; private set; }
        protected List<UIButton> Buttons { get; } = new();
        public float ButtonSpacing { get; set; } = 10f;
        protected float ButtonMargin = 5.0f;
        public bool IsVerticalLayout { get; set; } = true;

        #endregion

        #region Constructor
        // 無參數建構子，C# 編譯器需要
        public UIMenu()
        {
            LocalPosition = Vector2F.Zero; // 或暫時預設
            Size = Vector2F.Zero;
        }
        public UIMenu(Vector2F position, Vector2F size)
        {
            LocalPosition.Base = position;
            Size = size;
        }
        #endregion

        #region Methods

        /// <summary>
        /// 通用初始化流程
        /// </summary>
        protected override void OnInit((IUiFactory, THandler) arg)
        {
            _factory = arg.Item1;
            _handler = arg.Item2;

            LocalPosition = MenuDefaults.Position;
            Size = MenuDefaults.Size;

            BuildScrollContainer();

            BuildButtons();

            UpdateScrollContentHeight();

            _menuRenderer = CreateMenuRenderer();
            if (_menuRenderer == null) throw new Exception("_menuRenderer is null!");
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
            UpdateScrollContentHeight();
        }

        protected void UpdateScrollContentHeight()
        {
            if (Buttons.Count == 0) return;
            var buttonHeight = Buttons[0].Size.Y;
            ScrollContainer.ContentHeight = Buttons.Count * (buttonHeight + ButtonSpacing) - ButtonSpacing;
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
                var btn = new UIButton()
                {
                    Text = label,
                    LocalPosition = new UIPosition(IsVerticalLayout ? new Vector2F(0, offset) : new Vector2F(offset, 0)),
                };
                btn.Action = onClick;

                Buttons.Add(btn);
                AddChild(btn);

                offset += IsVerticalLayout ? btn.Size.Y + ButtonSpacing : btn.Size.X + ButtonSpacing;
            }
        }

        public virtual void LayoutButtons()
        {
            float offset = 0f;
            foreach (var btn in Buttons)
            {
                btn.LocalPosition.Base = IsVerticalLayout ? new Vector2F(0, offset) : new Vector2F(offset, 0);
                offset += IsVerticalLayout ? btn.Size.Y + ButtonSpacing : btn.Size.X + ButtonSpacing;
            }
        }

        public RectangleF GetAbsClipRect() => ScrollContainer.GetAbsClippingRect();

        protected virtual UIMenuRenderer<THandler> CreateMenuRenderer()
        {
            return new UIMenuRenderer<THandler>(this);
        }

        public IReadOnlyList<UIButton> ButtonList => Buttons;

        protected override void OnDraw(Graphics g)
        {
            if (_menuRenderer == null) throw new Exception("_menuRenderer is null!");
            //_menuRenderer.Draw(g);
            _menuRenderer.Render(g, this); 
        }
        
        #endregion
    }
}
