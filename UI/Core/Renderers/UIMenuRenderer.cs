/* ----- ----- ----- ----- */
// UIMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    public class UIMenuRenderer<THandler>
        where THandler : UIMenuHandler<THandler>
    {
        protected UIMenu<THandler> Menu;

        public UIMenuRenderer(UIMenu<THandler> menu)
        {
            Menu = menu;
        }

        public virtual void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw background
            //using var bgBrush = new SolidBrush(Color.FromArgb(200, 50, 50, 50));
            //g.FillRectangle(bgBrush, _menu.GetCurrentAbsoluteBounds());

            DrawOutline(g);

            // Draw title
            //_menu.TitleText?.Render(g);

            // Draw buttons (每個 UIButton 自己渲染)
            //foreach (var btn in _menu.Buttons)
                //btn.Render(g);
        }

        private void DrawOutline(Graphics g)
        {
            using (Pen debugPen = new Pen(Color.FromArgb(100, 128, 128, 128), 4))
            {
                debugPen.DashStyle = DashStyle.Dash;

                // 使用 UIElement 提供的絕對邊界
                var bounds = Menu.GetCurrentAbsoluteBounds();

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
}
