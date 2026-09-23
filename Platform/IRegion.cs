/* ----- ----- ----- ----- */
// IRegion.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace Engine.Platform
{
    /// <summary>
    /// An arbitrarily-shaped fillable area, used by
    /// <see cref="IGraphics.FillRegion(IBrush, IRegion)"/>. Created via
    /// <see cref="IGraphics.CreateRegion(IGraphicsPath)"/>.
    /// </summary>
    public interface IRegion : IDisposable
    {
        /// <summary>Restricts this region to its intersection with <paramref name="bounds"/>.</summary>
        void Intersect(RectangleF bounds);
    }
}
