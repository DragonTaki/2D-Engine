/* ----- ----- ----- ----- */
// WinFormsRegion.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Platform.WinForms
{
    /// <summary>GDI+-backed <see cref="IRegion"/>.</summary>
    internal sealed class WinFormsRegion : IRegion
    {
        public Region Native { get; }

        public WinFormsRegion(Region native)
        {
            Native = native;
        }

        public void Intersect(RectangleF bounds) => Native.Intersect(bounds);

        public void Dispose() => Native.Dispose();
    }
}
