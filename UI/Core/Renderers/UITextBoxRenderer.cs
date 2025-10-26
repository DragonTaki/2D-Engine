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
using Engine.UI.Elements;

namespace Engine.UI.Core.Renderers
{
    public class UITextBoxRenderer<THandler> : UIRenderer
        where THandler : UITextBoxHandler<THandler>
    {
        protected UITextBox<THandler> TextBox;
        protected CompositeRenderer _composite;

        public UITextBoxRenderer(UITextBox<THandler> textBox)
        {
            TextBox = textBox;
            _composite = new CompositeRenderer()
                .Add(new Outline(this))
                .Add(new Labels(this));
        }

        public override void Render(Graphics g, UIElement element)
        {
            _composite.Render(g, TextBox);
        }

        private class Outline : UIRenderer
        {
            private readonly UITextBoxRenderer<THandler> _parent;
            public Outline(UITextBoxRenderer<THandler> parent) => _parent = parent;
            public override void Render(Graphics g, UIElement element)
            {
                using (Pen debugPen = new Pen(Color.FromArgb(100, 128, 128, 128), 4))
                {
                    debugPen.DashStyle = DashStyle.Solid;

                    // 使用 UIElement 提供的絕對邊界
                    var textBox = _parent.TextBox;
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

        private class Labels : UIRenderer
        {
            private readonly UITextBoxRenderer<THandler> _parent;
            public Labels(UITextBoxRenderer<THandler> parent) => _parent = parent;

            public override void Render(Graphics g, UIElement element)
            {
                var textBox = _parent.TextBox;
                var labels = textBox._labels;
                var clip = textBox.GetAbsClipRect();

                foreach (var label in labels)
                {
                    label.ClipRect = clip;
                }
            }
        }
    }
}
