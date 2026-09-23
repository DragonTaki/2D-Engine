/* ----- ----- ----- ----- */
// WinFormsGraphicsPath.cs
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
    /// <summary>GDI+-backed <see cref="IGraphicsPath"/>.</summary>
    internal sealed class WinFormsGraphicsPath : IGraphicsPath
    {
        public GraphicsPath Native { get; }

        public WinFormsGraphicsPath(GraphicsPath native)
        {
            Native = native;
        }

        public void StartFigure() => Native.StartFigure();
        public void CloseFigure() => Native.CloseFigure();
        public void AddLine(float x1, float y1, float x2, float y2) => Native.AddLine(x1, y1, x2, y2);
        public void AddArc(RectangleF bounds, float startAngle, float sweepAngle) => Native.AddArc(bounds, startAngle, sweepAngle);

        public void Transform(IMatrix matrix) => Native.Transform(((WinFormsMatrix)matrix).Native);

        public IGraphicsPath Clone() => new WinFormsGraphicsPath((GraphicsPath)Native.Clone());

        public void Dispose() => Native.Dispose();
    }
}
