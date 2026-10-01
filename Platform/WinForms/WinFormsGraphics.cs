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

        // One saved state per open SetClip, restored by the matching ResetClip (see
        // IGraphics.SetClip), and for each PushTransform the number of clips open at the
        // push: GDI+ Restore discards every later Save, so PopTransform drops those too.
        // CA1416 is suppressed only around the code added for nested clips, so the build's
        // warning baseline doesn't move: this whole folder compiles only for net9.0-windows.
#pragma warning disable CA1416
        private readonly Stack<GraphicsState> _clipStates = new();
#pragma warning restore CA1416
        private readonly Stack<int> _clipDepthAtTransform = new();

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

        // Nested like the Skia backend: intersect with the clip already in effect, and undo
        // only this clip on ResetClip. Graphics.SetClip/ResetClip would replace the outer clip
        // and then clear it entirely, so a clipped child inside a clipped container let the
        // container's remaining children draw unclipped.
#pragma warning disable CA1416
        public void SetClip(RectangleF bounds)
        {
            _clipStates.Push(Native.Save());
            Native.IntersectClip(bounds);
        }

        public void ResetClip()
        {
            if (_clipStates.Count > 0)
                Native.Restore(_clipStates.Pop());
        }
#pragma warning restore CA1416

        public void PushTransform(float scale, float offsetX, float offsetY)
        {
            _clipDepthAtTransform.Push(_clipStates.Count);
            _transformStates.Push(Native.Save());
            Native.TranslateTransform(offsetX, offsetY);
            Native.ScaleTransform(scale, scale);
        }

        public void PopTransform()
        {
            Native.Restore(_transformStates.Pop());
            int clipDepth = _clipDepthAtTransform.Pop();
#pragma warning disable CA1416
            while (_clipStates.Count > clipDepth)
                _clipStates.Pop();
#pragma warning restore CA1416
        }

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
