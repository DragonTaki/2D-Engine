/* ----- ----- ----- ----- */
// SkiaGraphics.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Collections.Generic;

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>SkiaSharp-backed <see cref="IGraphics"/>, wrapping one frame's <see cref="SKCanvas"/>.</summary>
    internal sealed class SkiaGraphics : IGraphics
    {
        public SKCanvas Native { get; }

        private readonly SKSurface _ownedSurface;
        private int _clipDepth;

        public SkiaGraphics(SKCanvas native, SKSurface ownedSurface = null)
        {
            Native = native;
            _ownedSurface = ownedSurface;
        }

        /// <summary>Creates a throwaway off-screen surface purely for text measurement.</summary>
        public static SkiaGraphics CreateMeasurementContext()
        {
            var surface = SKSurface.Create(new SKImageInfo(1, 1));
            return new SkiaGraphics(surface.Canvas, surface);
        }

        public void FillRectangle(IBrush brush, float x, float y, float width, float height) =>
            Native.DrawRect(x, y, width, height, ((SkiaBrush)brush).Native);

        public void FillRectangle(IBrush brush, RectangleF bounds) =>
            Native.DrawRect(ToSKRect(bounds), ((SkiaBrush)brush).Native);

        public void DrawRectangle(IPen pen, float x, float y, float width, float height) =>
            Native.DrawRect(x, y, width, height, ((SkiaPen)pen).Native);

        public void DrawRectangle(IPen pen, RectangleF bounds) =>
            Native.DrawRect(ToSKRect(bounds), ((SkiaPen)pen).Native);

        public void FillEllipse(IBrush brush, float x, float y, float width, float height) =>
            Native.DrawOval(new SKRect(x, y, x + width, y + height), ((SkiaBrush)brush).Native);

        public void DrawEllipse(IPen pen, float x, float y, float width, float height) =>
            Native.DrawOval(new SKRect(x, y, x + width, y + height), ((SkiaPen)pen).Native);

        public void DrawLine(IPen pen, float x1, float y1, float x2, float y2) =>
            Native.DrawLine(x1, y1, x2, y2, ((SkiaPen)pen).Native);

        public void FillPath(IBrush brush, IGraphicsPath path) =>
            Native.DrawPath(((SkiaGraphicsPath)path).Native, ((SkiaBrush)brush).Native);

        public void DrawPath(IPen pen, IGraphicsPath path) =>
            Native.DrawPath(((SkiaGraphicsPath)path).Native, ((SkiaPen)pen).Native);

        public void FillRegion(IBrush brush, IRegion region) =>
            Native.DrawRegion(((SkiaRegion)region).Native, ((SkiaBrush)brush).Native);

        public void DrawString(string text, IFont font, IBrush brush, float x, float y) =>
            Native.DrawText(text, x, y, SKTextAlign.Left, ((SkiaFont)font).Native, ((SkiaBrush)brush).Native);

        public void DrawString(string text, IFont font, IBrush brush, RectangleF bounds, IStringFormat format)
        {
            var skFont = ((SkiaFont)font).Native;
            var paint = ((SkiaBrush)brush).Native;
            var fmt = (SkiaStringFormat)format;

            float textWidth = skFont.MeasureText(text, paint);
            var metrics = skFont.Metrics;
            float lineHeight = metrics.Descent - metrics.Ascent + metrics.Leading;

            float x = fmt.Alignment switch
            {
                TextAlign.Center => bounds.X + (bounds.Width - textWidth) / 2f,
                TextAlign.Far => bounds.X + bounds.Width - textWidth,
                _ => bounds.X,
            };

            // Baseline Y: top of the text box, offset down by ascent so the
            // glyphs' top edge lands at the aligned position.
            float topY = fmt.LineAlignment switch
            {
                TextAlign.Center => bounds.Y + (bounds.Height - lineHeight) / 2f,
                TextAlign.Far => bounds.Y + bounds.Height - lineHeight,
                _ => bounds.Y,
            };
            float baselineY = topY - metrics.Ascent;

            Native.DrawText(text, x, baselineY, SKTextAlign.Left, skFont, paint);
        }

        public SizeF MeasureString(string text, IFont font)
        {
            var skFont = ((SkiaFont)font).Native;
            using var paint = new SKPaint();
            float width = skFont.MeasureText(text, paint);
            return new SizeF(width, ((SkiaFont)font).Height);
        }

        public SizeF MeasureString(string text, IFont font, int maxWidth)
        {
            var skFont = ((SkiaFont)font).Native;
            using var paint = new SKPaint();
            float lineHeight = ((SkiaFont)font).Height;

            // Skia has no built-in wrap-measuring — greedily wrap by word.
            var words = text.Split(' ');
            var lines = new List<string>();
            var current = "";
            float maxLineWidth = 0f;

            foreach (var word in words)
            {
                var candidate = current.Length == 0 ? word : current + " " + word;
                if (skFont.MeasureText(candidate, paint) > maxWidth && current.Length > 0)
                {
                    lines.Add(current);
                    maxLineWidth = Math.Max(maxLineWidth, skFont.MeasureText(current, paint));
                    current = word;
                }
                else
                {
                    current = candidate;
                }
            }
            if (current.Length > 0)
            {
                lines.Add(current);
                maxLineWidth = Math.Max(maxLineWidth, skFont.MeasureText(current, paint));
            }

            return new SizeF(maxLineWidth, lineHeight * Math.Max(lines.Count, 1));
        }

        public void SetClip(RectangleF bounds)
        {
            Native.Save();
            Native.ClipRect(ToSKRect(bounds));
            _clipDepth++;
        }

        public void ResetClip()
        {
            if (_clipDepth > 0)
            {
                Native.Restore();
                _clipDepth--;
            }
        }

        // Skia is always anti-aliased per-paint (each IBrush/IPen is created
        // with IsAntialias = true — see SkiaGraphicsFactory), so there is no
        // separate global quality mode to flip here.
        public void ApplyHighQualitySettings() { }

        private static SKRect ToSKRect(RectangleF r) => new SKRect(r.Left, r.Top, r.Right, r.Bottom);

        public void Dispose() => _ownedSurface?.Dispose();
    }
}
