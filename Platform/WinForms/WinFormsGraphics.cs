/* ----- ----- ----- ----- */
// WinFormsGraphics.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Engine.Platform.WinForms
{
    /// <summary>GDI+-backed <see cref="IGraphics"/>, wrapping one paint pass's <see cref="Graphics"/>.</summary>
    internal sealed class WinFormsGraphics : IGraphics
    {
        public Graphics Native { get; }

        // GDI+'s Graphics.Restore() requires the exact GraphicsState object
        // Save() returned, so PushTransform/PopTransform need their own
        // stack rather than a bare counter.
        private readonly Stack<GraphicsState> _transformStates = new();

        /// <summary>
        /// Whether this instance owns (and should dispose) <see cref="Native"/>.
        /// False for a Graphics handed in by WinForms' own paint event — that
        /// one is disposed by WinForms itself, not by us.
        /// </summary>
        private readonly bool _ownsNative;

        public WinFormsGraphics(Graphics native, bool ownsNative = false)
        {
            Native = native;
            _ownsNative = ownsNative;
        }

        public void FillRectangle(IBrush brush, float x, float y, float width, float height) =>
            Native.FillRectangle(((WinFormsBrush)brush).Native, x, y, width, height);

        public void FillRectangle(IBrush brush, RectangleF bounds) =>
            Native.FillRectangle(((WinFormsBrush)brush).Native, bounds);

        public void DrawRectangle(IPen pen, float x, float y, float width, float height) =>
            Native.DrawRectangle(((WinFormsPen)pen).Native, x, y, width, height);

        public void DrawRectangle(IPen pen, RectangleF bounds) =>
            Native.DrawRectangle(((WinFormsPen)pen).Native, bounds.X, bounds.Y, bounds.Width, bounds.Height);

        public void FillEllipse(IBrush brush, float x, float y, float width, float height) =>
            Native.FillEllipse(((WinFormsBrush)brush).Native, x, y, width, height);

        public void DrawEllipse(IPen pen, float x, float y, float width, float height) =>
            Native.DrawEllipse(((WinFormsPen)pen).Native, x, y, width, height);

        public void DrawLine(IPen pen, float x1, float y1, float x2, float y2) =>
            Native.DrawLine(((WinFormsPen)pen).Native, x1, y1, x2, y2);

        public void FillPath(IBrush brush, IGraphicsPath path) =>
            Native.FillPath(((WinFormsBrush)brush).Native, ((WinFormsGraphicsPath)path).Native);

        public void DrawPath(IPen pen, IGraphicsPath path) =>
            Native.DrawPath(((WinFormsPen)pen).Native, ((WinFormsGraphicsPath)path).Native);

        public void FillRegion(IBrush brush, IRegion region) =>
            Native.FillRegion(((WinFormsBrush)brush).Native, ((WinFormsRegion)region).Native);

        public void DrawString(string text, IFont font, IBrush brush, float x, float y) =>
            Native.DrawString(text, ((WinFormsFont)font).Native, ((WinFormsBrush)brush).Native, x, y);

        public void DrawString(string text, IFont font, IBrush brush, RectangleF bounds, IStringFormat format) =>
            Native.DrawString(text, ((WinFormsFont)font).Native, ((WinFormsBrush)brush).Native, bounds, ((WinFormsStringFormat)format).Native);

        public SizeF MeasureString(string text, IFont font) =>
            Native.MeasureString(text, ((WinFormsFont)font).Native);

        public SizeF MeasureString(string text, IFont font, int maxWidth) =>
            Native.MeasureString(text, ((WinFormsFont)font).Native, maxWidth);

        public void SetClip(RectangleF bounds) => Native.SetClip(bounds);
        public void ResetClip() => Native.ResetClip();

        public void PushTransform(float scale, float offsetX, float offsetY)
        {
            _transformStates.Push(Native.Save());
            Native.TranslateTransform(offsetX, offsetY);
            Native.ScaleTransform(scale, scale);
        }

        public void PopTransform() => Native.Restore(_transformStates.Pop());

        public void ApplyHighQualitySettings()
        {
            Native.SmoothingMode = SmoothingMode.AntiAlias;
            Native.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Native.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Native.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        }

        public void Dispose()
        {
            if (_ownsNative)
                Native.Dispose();
        }
    }
}
