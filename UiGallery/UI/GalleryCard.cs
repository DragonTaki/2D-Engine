/* ----- ----- ----- ----- */
// GalleryCard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Geometry;
using Engine.Platform;
using Engine.Styles;
using Engine.UI.Core.Elements;

namespace Engine.UiGallery.UI
{
    internal sealed class GalleryCard : UINode
    {
        public object SampleStyle { get; set; }
        public bool SamplesEnabled { get; set; } = true;
        public Color OutlineColor { get; set; } = Color.FromArgb(75, 130, 150, 175);

        public override void Draw(IGraphics g)
        {
            UpdateLayout();
            if (DisableRender || IsDisposed)
                return;
            var bounds = GetCurrentAbsoluteBounds();
            using var pen = GraphicsBackend.Factory.CreatePen(OutlineColor, 1f);
            g.DrawRectangle(pen, bounds.ToRectangleF());
            Color accent = Color.FromArgb(SamplesEnabled ? 255 : 100, 90, 165, 235);
            var sample = new RectangleF(bounds.X + 22f, bounds.Y + 24f, bounds.Width - 44f, 75f);
            if (SampleStyle is FrameStyle frame)
            {
                frame.Draw(g, sample, accent, null);
                sample.Y += 96f;
                frame.Draw(g, sample, accent, new FrameTail(sample.Top + sample.Height / 2f, 16f, 15f));
            }
            else if (SampleStyle is IBoxDrawStyle box)
                box.Draw(g, new LayoutF(sample.Location, sample.Size));
            else if (SampleStyle is BorderStyle border && border.Width > 0f)
            {
                using var borderPen = GraphicsBackend.Factory.CreatePen(border.Color, border.Width);
                g.DrawRectangle(borderPen, sample);
            }
            base.Draw(g);
        }
    }
}
