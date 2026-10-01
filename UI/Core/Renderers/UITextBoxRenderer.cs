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

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UITextBox{TElement, THandler, TRenderer}"/>: the container
    /// <c>Style</c>, the background color, a debug outline, and the viewport clip for the
    /// line labels (which are drawn as children).
    /// </summary>
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
            // Bind the composite and its parts to this element (nothing else initializes it).
            _composite.Init(Element);
        }

        private void SetupRendererChildren()
        {
            if (_composite.ListCount == 0)
            {
                _composite
                    .Add(new Background())
                    .Add(new Outline())
                    .Add(new Labels());
            }
        }

        public override void OnRender(IGraphics g, TElement element)
        {
            // Container Style (background/border) first - overriding without calling base
            // meant a Style set on this container was never drawn.
            base.OnRender(g, element);
            _composite.Render(g, element);
        }

        /// <summary>
        /// Fills the text box with its BackgroundColor (previously never drawn).
        /// </summary>
        private class Background : UIRenderer<TElement, THandler, TRenderer>
        {
            public Background() { }
            public override void OnRender(IGraphics g, TElement element)
            {
                var textBox = (UITextBox<TElement, THandler, TRenderer>)element;
                if (textBox.BackgroundColor.A == 0)
                    return;

                using var brush = GraphicsBackend.Factory.CreateSolidBrush(textBox.BackgroundColor);
                g.FillRectangle(brush, textBox.GetCurrentAbsoluteBounds());
            }
        }

        private class Outline : UIRenderer<TElement, THandler, TRenderer>
        {
            public Outline() { }
            public override void OnRender(IGraphics g, TElement element)
            {
                using (IPen debugPen = GraphicsBackend.Factory.CreatePen(Color.FromArgb(100, 128, 128, 128), 4))
                {
                    debugPen.DashStyle = PenDashStyle.Solid;

                    // Use the absolute bounds provided by UIElement
                    var textBox = (UITextBox<TElement, THandler, TRenderer>)element;
                    var bounds = textBox.GetCurrentAbsoluteBounds();

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
