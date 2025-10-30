/* ----- ----- ----- ----- */
// UIMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/27
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Styles;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;

namespace Engine.UI.Core.Renderers
{
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

        public override void OnRender(Graphics g, TElement element)
        {
            _composite.Render(g, element);
        }

        private class Outline : UIRenderer<TElement, THandler, TRenderer>
        {
            public Outline() { }
            public override void OnRender(Graphics g, TElement element)
            {
                using (Pen debugPen = new Pen(Color.FromArgb(100, 128, 128, 128), 4))
                {
                    debugPen.DashStyle = DashStyle.Dash;

                    // 使用 UIElement 提供的絕對邊界
                    var menu = (UIMenu<TElement, THandler, TRenderer>)element;
                    var bounds = menu.GetCurrentAbsoluteBounds();

                    // 可以加入 margin
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
            public override void OnRender(Graphics g, TElement element)
            {
                // 取得可見按鈕
                var menu = (UIMenu<TElement, THandler, TRenderer>)element;
                var buttons = menu.GetVisibleButtons();
                var clip = menu.GetAbsClipRect();

                g.SetClip(clip);
                foreach (var button in buttons)
                {
                    IButtonDrawStyle style = button.Style ?? UILayoutStyles.MainMenu.Button.Style;
                    style.Draw(g, button.Text, button.GetCurrentAbsolutePosition(), button.Size);
                }
                g.ResetClip();
            }
        }
    }
}
