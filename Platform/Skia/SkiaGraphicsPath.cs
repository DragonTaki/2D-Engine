/* ----- ----- ----- ----- */
// SkiaGraphicsPath.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>
    /// SkiaSharp-backed <see cref="IGraphicsPath"/>. GDI+'s <c>GraphicsPath</c>
    /// builds a figure via <c>AddLine</c>/<c>AddArc</c>/<c>AddBezier</c> calls
    /// that each implicitly connect from the current point; SkiaSharp's
    /// <c>SKPath</c> instead uses an explicit <c>MoveTo</c>/<c>LineTo</c>/
    /// <c>ArcTo</c>/<c>CubicTo</c> pen model. This class tracks whether the
    /// current figure is still empty so the first segment gets the implicit
    /// <c>MoveTo</c> that GDI+ would have done automatically.
    /// </summary>
    internal sealed class SkiaGraphicsPath : IGraphicsPath
    {
        public SKPath Native { get; }
        private bool _figureEmpty = true;

        public SkiaGraphicsPath(SKPath native)
        {
            Native = native;
        }

        public void StartFigure() => _figureEmpty = true;
        public void CloseFigure() => Native.Close();

        public void AddLine(float x1, float y1, float x2, float y2)
        {
            if (_figureEmpty)
            {
                Native.MoveTo(x1, y1);
                _figureEmpty = false;
            }
            else
            {
                Native.LineTo(x1, y1);
            }
            Native.LineTo(x2, y2);
        }

        public void AddArc(RectangleF bounds, float startAngle, float sweepAngle)
        {
            var oval = new SKRect(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
            Native.ArcTo(oval, startAngle, sweepAngle, forceMoveTo: _figureEmpty);
            _figureEmpty = false;
        }

        public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle) =>
            AddArc(new RectangleF(x, y, width, height), startAngle, sweepAngle);

        public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            if (_figureEmpty)
            {
                Native.MoveTo(x1, y1);
                _figureEmpty = false;
            }
            else
            {
                Native.LineTo(x1, y1);
            }
            Native.CubicTo(x2, y2, x3, y3, x4, y4);
        }

        public void Transform(IMatrix matrix) => Native.Transform(((SkiaMatrix)matrix).Native);

        public IGraphicsPath Clone() => new SkiaGraphicsPath(new SKPath(Native)) { _figureEmpty = _figureEmpty };

        public void Dispose() => Native.Dispose();
    }
}
