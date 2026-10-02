/* ----- ----- ----- ----- */
// UILabeledRowRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UILabeledRow"/>: the container <c>Style</c>, then the name at
    /// the left of the content box, vertically centered (one line; the point overload of
    /// DrawString is used, so the text is never wrapped or trimmed).
    /// </summary>
    public class UILabeledRowRenderer : UIContainerRenderer<UILabeledRow, UILabeledRowHandler, UILabeledRowRenderer>
    {
        public UILabeledRowRenderer() { }

        public override void OnRender(IGraphics g, UILabeledRow element)
        {
            base.OnRender(g, element);

            if (string.IsNullOrEmpty(element.Text) || element.Font == null || element.TextBrush == null)
                return;

            var box = element.GetCurrentAbsoluteContentBox();
            var size = g.MeasureString(element.Text, element.Font);
            g.DrawString(element.Text, element.Font, element.TextBrush, box.X, box.Y + (box.Height - size.Height) / 2f);
        }
    }
}
