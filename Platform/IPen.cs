/* ----- ----- ----- ----- */
// IPen.cs
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
    /// Line dash pattern, mirroring the subset of GDI+'s <c>DashStyle</c>
    /// actually used in this codebase.
    /// </summary>
    public enum PenDashStyle
    {
        Solid,
        Dash
    }

    /// <summary>
    /// Where a pen's stroke sits relative to the path it outlines, mirroring
    /// the subset of GDI+'s <c>PenAlignment</c> actually used here.
    /// </summary>
    public enum PenLineAlignment
    {
        Center,
        Inset
    }

    /// <summary>
    /// A stroke style used by <see cref="IGraphics"/> outline operations.
    /// Created via <see cref="IGraphics.CreatePen(Color, float)"/>.
    /// </summary>
    public interface IPen : IDisposable
    {
        PenDashStyle DashStyle { get; set; }
        PenLineAlignment Alignment { get; set; }
    }
}
