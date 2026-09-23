/* ----- ----- ----- ----- */
// IGraphicsPath.cs
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
    /// A vector path built from lines and arcs, used by
    /// <see cref="IGraphics.FillPath(IBrush, IGraphicsPath)"/> and
    /// <see cref="IGraphics.DrawPath(IPen, IGraphicsPath)"/>. Created via
    /// <see cref="IGraphics.CreatePath"/>.
    /// </summary>
    public interface IGraphicsPath : IDisposable
    {
        void StartFigure();
        void CloseFigure();
        void AddLine(float x1, float y1, float x2, float y2);
        void AddArc(RectangleF bounds, float startAngle, float sweepAngle);
        void Transform(IMatrix matrix);
        IGraphicsPath Clone();
    }
}
