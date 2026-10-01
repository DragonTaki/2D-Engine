/* ----- ----- ----- ----- */
// UIMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2026/09/30
// Version: v2.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Platform;
using Engine.Styles;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UIMenu{TElement, THandler, TRenderer}"/>: the container
    /// <c>Style</c> over the menu panel, a dashed debug outline, then the visible buttons
    /// (clipped to the scroll viewport).
    /// </summary>
    public class UIMenuRenderer<TElement, THandler, TRenderer> : UIContainerRenderer<TElement, THandler, TRenderer>
        where TElement : UIMenu<TElement, THandler, TRenderer>
        where THandler : UIMenuHandler<TElement, THandler, TRenderer>
        where TRenderer : UIMenuRenderer<TElement, THandler, TRenderer>
    {
        protected CompositeRenderer<TElement, THandler, TRenderer> _composite = new();

        public UIMenuRenderer() { }

        protected override void AfterInit()
        {
            SetupRendererChildren();
            // Bind the composite and its parts to this element (nothing else initializes it).
            _composite.Init(Element);
        }

        private void SetupRendererChildren()
        {
            if (_composite.ListCount == 0)
            {
                _composite
                    .Add(new Outline())
                    .Add(new Buttons());
            }
        }

        public override void OnRender(IGraphics g, TElement element)
        {
            // Container Style (background/border) first - overriding without calling base
            // meant a Style set on this container was never drawn. Drawn over the menu's
            // panel, not the whole element: a screen-sized menu (PanelWidth set) would
            // otherwise paint its panel background over the rest of the screen.
            element.Style?.Draw(g, element.GetPanelAbsoluteBounds());
            _composite.Render(g, element);
        }

        private class Outline : UIRenderer<TElement, THandler, TRenderer>
        {
            public Outline() { }
            public override void OnRender(IGraphics g, TElement element)
            {
                using (IPen debugPen = GraphicsBackend.Factory.CreatePen(Color.FromArgb(100, 128, 128, 128), 4))
                {
                    debugPen.DashStyle = PenDashStyle.Dash;

                    // The panel's absolute bounds (the whole element unless PanelWidth is set)
                    var menu = (UIMenu<TElement, THandler, TRenderer>)element;
                    var bounds = menu.GetPanelAbsoluteBounds();

                    // TODO: Make this outline margin configurable instead of a fixed 3 units.
                    float margin = 3.0f;
                    var rect = new RectangleF(
                        bounds.X + margin,
                        bounds.Y + margin,
                        bounds.Width - margin * 2,
                        bounds.Height - margin * 2
                    );

                    g.DrawRectangle(debugPen, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
        }

        private class Buttons : UIRenderer<TElement, THandler, TRenderer>
        {
            public Buttons() { }
            public override void OnRender(IGraphics g, TElement element)
            {
                // Get the visible buttons
                var menu = (UIMenu<TElement, THandler, TRenderer>)element;
                var buttons = menu.GetVisibleButtons();
                var clip = menu.GetAbsClipRect();

                // Balanced with try/finally like the other SetClip sites: a throwing style
                // would leak the clip on GDI+ and unbalance Skia's save stack.
                g.SetClip(clip);
                try
                {
                    foreach (var button in buttons)
                    {
                        IButtonDrawStyle style = button.Style ?? DefaultStyles.DefaultButtonStyle;
                        style.Draw(g, button.Text, button.GetCurrentAbsolutePosition(), button.Size);
                    }
                }
                finally
                {
                    g.ResetClip();
                }
            }
        }
    }
}
