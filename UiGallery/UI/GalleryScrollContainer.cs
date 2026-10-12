/* ----- ----- ----- ----- */
// GalleryScrollContainer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Platform;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Input;

namespace Engine.UiGallery.UI
{
    internal sealed class GalleryScrollContainer : UIScrollContainer
    {
        public GalleryScrollContainer(IScrollInputHandler scroll) : base(scroll) { }

        public override void Draw(IGraphics g)
        {
            g.SetClip(AbsViewportBounds);
            try { base.Draw(g); }
            finally { g.ResetClip(); }
        }

        public override UIElementBase HitTestDeep(PointF point, bool isRootCall = true) =>
            AbsViewportBounds.Contains(point) ? base.HitTestDeep(point, isRootCall) : null;
    }
}
