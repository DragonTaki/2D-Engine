/* ----- ----- ----- ----- */
// WinFormsBrush.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Platform.WinForms
{
    /// <summary>GDI+-backed <see cref="IBrush"/>.</summary>
    internal sealed class WinFormsBrush : IBrush
    {
        public Brush Native { get; }

        public WinFormsBrush(Brush native)
        {
            Native = native;
        }

        public void Dispose() => Native.Dispose();
    }
}
