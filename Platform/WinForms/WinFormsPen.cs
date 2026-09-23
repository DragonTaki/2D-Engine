/* ----- ----- ----- ----- */
// WinFormsPen.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Engine.Platform.WinForms
{
    /// <summary>GDI+-backed <see cref="IPen"/>.</summary>
    internal sealed class WinFormsPen : IPen
    {
        public Pen Native { get; }

        public WinFormsPen(Pen native)
        {
            Native = native;
        }

        PenDashStyle IPen.DashStyle
        {
            get => Native.DashStyle == System.Drawing.Drawing2D.DashStyle.Dash ? PenDashStyle.Dash : PenDashStyle.Solid;
            set => Native.DashStyle = value == PenDashStyle.Dash ? System.Drawing.Drawing2D.DashStyle.Dash : System.Drawing.Drawing2D.DashStyle.Solid;
        }

        public PenLineAlignment Alignment
        {
            get => Native.Alignment == PenAlignment.Inset ? PenLineAlignment.Inset : PenLineAlignment.Center;
            set => Native.Alignment = value == PenLineAlignment.Inset ? PenAlignment.Inset : PenAlignment.Center;
        }

        public void Dispose() => Native.Dispose();
    }
}
