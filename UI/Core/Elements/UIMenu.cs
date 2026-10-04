/* ----- ----- ----- ----- */
// UIMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2026/10/04
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Engine.Diagnostics;
using Engine.Geometry;
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
    /// Engine-level generic Menu.
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

        /// <summary>
        /// Width of the menu's own panel (the column its buttons and outline live in),
        /// measured from the menu's left edge, for a menu element that is wider than its
        /// panel - e.g. a screen-sized menu whose box also hosts the screen's other
        /// content next to the panel. <c>null</c> (default): the panel is the whole element.
        /// </summary>
        public float? PanelWidth { get; set; }

        #endregion

        #region Constructor

        public UIMenu() { }

        #endregion

        #region Methods

        /// <summary>
        /// Generic initialization flow: binds handler and renderer, then builds the scroll container, UI objects and buttons.
        /// </summary>
        public override void Init(IUiFactory factory, THandler handler, TRenderer renderer)
        {
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"[UIMenu]Init 3 generic Current type: {this?.GetType().FullName ?? "null"}, IsInitialized: {IsInitialized}");

            if (IsInitialized)
                return;

            IsInitialized = true;

            _factory = factory;

            OnBeforeInit(factory);

            // Bind Handler
            Handler = handler;
            Handler.Element = (TElement)(object)this;
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"[UIMenu]Handler type: {Handler?.GetType().FullName ?? "null"}");

            // Bind Renderer
            Renderer = renderer;
            Renderer.Element = (TElement)(object)this;
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"[UIMenu]Renderer type: {Renderer?.GetType().FullName ?? "null"}");

            RunInitHooks();

            BuildScrollContainer();
            BuildUIObjects();

            OnInit(factory);

            BuildButtons();
            Handler.UpdateScrollContentHeight();

            OnAfterInit(factory);
        }

        protected override void OnInit(IUiFactory factory)
        {
            LocalPosition = MenuDefaults.Position;
            Size = MenuDefaults.Size;
        }

        public virtual void BuildScrollContainer()
        {
            ScrollContainer = _factory.CreateScrollContainer();
            ScrollContainer.Layout = MenuDefaults.Scroll.Layout;
            ScrollContainer.OverscrollLimit = MenuDefaults.Scroll.OverscrollLimit;
            // Menus open at their first entry. (This said Bottom, but the scroll
            // container's first layout pass used to reset the scroll offset, so menus
            // always actually opened at the top; that reset is fixed now.)
            ScrollContainer.VerticalAlignment = ScrollAlignment.Top;
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

        /// <summary>
        /// The buttons the menu renderer draws: inside the scroll viewport (not
        /// <see cref="UIElementBase.IsClipped"/>, see <see cref="UIElementUtils.UpdateVisibleState"/>)
        /// and rendered at all - not <c>Display = None</c> or invisible, neither the button
        /// nor any element between it and this menu (e.g. a hidden group container). The
        /// render pipeline skips such elements (<see cref="UIElementBase.DisableRender"/>),
        /// but the menu draws its buttons itself, so a hidden button used to be drawn anyway.
        /// </summary>
        public List<UIButton> GetVisibleButtons()
        {
            UIElementUtils.UpdateVisibleState(Buttons, ScrollContainer.GetAbsClippingRect());
            return Buttons.Where(b => !b.IsClipped && IsRenderedInMenu(b)).ToList();
        }

        /// <summary>
        /// Whether <paramref name="element"/> and every ancestor below this menu is rendered.
        /// </summary>
        private bool IsRenderedInMenu(UIElementBase element)
        {
            for (var current = element; current != null && current != this; current = current.Parent)
                if (current.DisableRender)
                    return false;
            return true;
        }

        public virtual void SetupButtons(IEnumerable<(string label, Action onClick)> buttonDefs)
        {
            // Replace, don't just forget, the previous buttons: clearing only the list left
            // them in the scroll container (still drawn/hit-tested) and never released them.
            foreach (var old in Buttons)
                old.Dispose();  // also detaches it from its parent
            Buttons.Clear();
            float offset = 0f;
            foreach (var (label, onClick) in buttonDefs)
            {
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = label;
                button.Handler.Action = onClick;

                button.LocalPosition = new UIPosition(IsVerticalLayout ? new Vector2F(0, offset) : new Vector2F(offset, 0));

                Buttons.Add(button);
                // Inside the scroll container, like AddButton/BuildButtons - adding to the
                // menu itself put the buttons outside the scrolling content.
                ScrollContainer.AddChild(button);

                offset += IsVerticalLayout ? button.Size.Y + ButtonSpacing : button.Size.X + ButtonSpacing;
            }

            Handler.UpdateScrollContentHeight();
        }

        public virtual void LayoutButtons()
        {
            float offset = 0f;
            foreach (var button in Buttons)
            {
                // Set the whole position: writing only Base left Current (what drawing and
                // hit testing use) where it was, so the buttons never moved.
                button.LocalPosition = new UIPosition(IsVerticalLayout ? new Vector2F(0, offset) : new Vector2F(offset, 0));
                offset += IsVerticalLayout ? button.Size.Y + ButtonSpacing : button.Size.X + ButtonSpacing;
            }

            Handler.UpdateScrollContentHeight();
        }

        public RectangleF GetAbsClipRect() => ScrollContainer.GetAbsClippingRect();

        /// <summary>
        /// Absolute bounds of the menu's panel: the element's bounds, narrowed to
        /// <see cref="PanelWidth"/> when set.
        /// </summary>
        public LayoutF GetPanelAbsoluteBounds()
        {
            var bounds = GetCurrentAbsoluteBounds();
            if (PanelWidth is not float width)
                return bounds;
            return new LayoutF(bounds.Position, Math.Clamp(width, 0f, bounds.Size.X), bounds.Size.Y);
        }

        public IReadOnlyList<UIButton> ButtonList => Buttons;

        #endregion
    }
}
