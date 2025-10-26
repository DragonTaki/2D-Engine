/* ----- ----- ----- ----- */
// UIScrollContainerRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

public class UIScrollContainerRenderer : UIRenderer
{
    protected UIScrollContainer ScrollContainer;

    public UIScrollContainerRenderer(UIScrollContainer scrollContainer)
    {
        ScrollContainer = scrollContainer;
    }

    /// <summary>
    /// Draw the scroll container background and optionally visual scroll indicators.
    /// </summary>
    public override void Render(Graphics g, UIElement element)
    {
        if (ScrollContainer == null)
            return;

        var rect = ScrollContainer.GetAbsClippingRect();

        // Example: Draw semi-transparent background
        using (var brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
        {
            g.FillRectangle(brush, rect);
        }

        // Optional: draw border
        using (var pen = new Pen(Color.Gray, 2))
        {
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
