/* ----- ----- ----- ----- */
// UITextBoxRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Linq;

using Engine.Platform;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Elements;

namespace Engine.UI.Core.Renderers
{
    public class UITextBoxRenderer<TElement, THandler, TRenderer> : UIContainerRenderer<TElement, THandler, TRenderer>
        where TElement : UITextBox<TElement, THandler, TRenderer>
        where THandler : UITextBoxHandler<TElement, THandler, TRenderer>
        where TRenderer : UITextBoxRenderer<TElement, THandler, TRenderer>
    {
        protected CompositeRenderer<TElement, THandler, TRenderer> _composite = new();

        public UITextBoxRenderer() { }

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
                    .Add(new Labels());
            }
        }

        public override void OnRender(IGraphics g, TElement element)
        {
            _composite.Render(g, element);
        }

        private class Outline : UIRenderer<TElement, THandler, TRenderer>
        {
            public Outline() { }
            public override void OnRender(IGraphics g, TElement element)
            {
                using (IPen debugPen = GraphicsBackend.Factory.CreatePen(Color.FromArgb(100, 128, 128, 128), 4))
                {
                    debugPen.DashStyle = PenDashStyle.Solid;

                    // 使用 UIElement 提供的絕對邊界
                    var textBox = (UITextBox<TElement, THandler, TRenderer>)element;
                    var bounds = textBox.GetCurrentAbsoluteBounds();

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

        private class Labels : UIRenderer<TElement, THandler, TRenderer>
        {
            public Labels() { }
            public override void OnRender(IGraphics g, TElement element)
            {
                var textBox = (UITextBox<TElement, THandler, TRenderer>)element;
                var labels = textBox.ScrollContainer.Children.OfType<UILabel>();
                var clip = textBox.GetAbsClipRect();

                foreach (var label in labels)
                {
                    label.ClipRect = clip;
                }
            }
        }
    }
}
