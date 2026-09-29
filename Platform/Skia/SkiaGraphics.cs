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

        public void DrawString(string text, IFont font, IBrush brush, float x, float y)
        {
            // GDI+ treats (x, y) as the top-left of the text, Skia's DrawText as the
            // baseline origin; shift down by the ascent (negative in Skia) to match.
            var skFont = ((SkiaFont)font).Native;
            Native.DrawText(text, x, y - skFont.Metrics.Ascent, SKTextAlign.Left, skFont, ((SkiaBrush)brush).Native);
        }

        public void DrawString(string text, IFont font, IBrush brush, RectangleF bounds, IStringFormat format)
        {
            var skFont = ((SkiaFont)font).Native;
            var paint = ((SkiaBrush)brush).Native;
            var fmt = (SkiaStringFormat)format;

            var metrics = skFont.Metrics;
            float lineHeight = metrics.Descent - metrics.Ascent + metrics.Leading;

            // fmt.EllipsisTrimming (truncating an overflowing final line
            // with "…") is still not implemented — only WordWrap is, since
            // that's the gap that actually made CJK text render wrong
            // (see WrapText's doc comment), not just look slightly worse.
            var lines = fmt.WordWrap
                ? WrapText(text, skFont, paint, bounds.Width)
                : new List<string> { text };

            float totalHeight = lineHeight * Math.Max(lines.Count, 1);

            // Top of the text block, offset down by ascent per line so the
            // glyphs' top edge lands at the aligned position.
            float startTopY = fmt.LineAlignment switch
            {
                TextAlign.Center => bounds.Y + (bounds.Height - totalHeight) / 2f,
                TextAlign.Far => bounds.Y + bounds.Height - totalHeight,
                _ => bounds.Y,
            };

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                float lineWidth = skFont.MeasureText(line, paint);

                float x = fmt.Alignment switch
                {
                    TextAlign.Center => bounds.X + (bounds.Width - lineWidth) / 2f,
                    TextAlign.Far => bounds.X + bounds.Width - lineWidth,
                    _ => bounds.X,
                };
                float baselineY = startTopY + i * lineHeight - metrics.Ascent;

                Native.DrawText(line, x, baselineY, SKTextAlign.Left, skFont, paint);
            }
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

            var lines = WrapText(text, skFont, paint, maxWidth);
            float maxLineWidth = 0f;
            foreach (var line in lines)
                maxLineWidth = Math.Max(maxLineWidth, skFont.MeasureText(line, paint));

            return new SizeF(maxLineWidth, lineHeight * Math.Max(lines.Count, 1));
        }

        /// <summary>
        /// Greedily packs "atoms" (see <see cref="TokenizeForWrap"/>) onto
        /// lines no wider than <paramref name="maxWidth"/>. Shared by
        /// <see cref="MeasureString(string, IFont, int)"/> and the
        /// <see cref="DrawString(string, IFont, IBrush, RectangleF, IStringFormat)"/>
        /// overload, so what gets measured (to size a box, e.g.
        /// <c>UIConfirmDialog</c>) always matches what actually gets drawn.
        /// </summary>
        private static List<string> WrapText(string text, SKFont skFont, SKPaint paint, float maxWidth)
        {
            var atoms = TokenizeForWrap(text);
            var lines = new List<string>();
            var current = "";

            foreach (var atom in atoms)
            {
                string candidate = current + atom;
                if (current.Length > 0 && skFont.MeasureText(candidate, paint) > maxWidth)
                {
                    lines.Add(current.TrimEnd());
                    // Drop a whitespace atom that would otherwise lead the new line.
                    current = atom.TrimStart();
                }
                else
                {
                    current = candidate;
                }
            }
            if (current.Trim().Length > 0)
                lines.Add(current.TrimEnd());

            return lines;
        }

        /// <summary>
        /// Splits text into substrings that concatenate back into the
        /// original exactly: each CJK character (see <see cref="IsCjk"/>) is
        /// its own atom (individually breakable), while runs of whitespace
        /// or non-CJK characters are each kept together as one atom (a
        /// whole word only breaks at whitespace, not mid-word). Splitting
        /// purely on spaces (the simplest approach) would silently never
        /// wrap CJK text at all — this project's UI text is overwhelmingly
        /// Traditional Chinese, and CJK doesn't use spaces between
        /// characters or words.
        /// </summary>
        private static List<string> TokenizeForWrap(string text)
        {
            var atoms = new List<string>();
            int i = 0;
            while (i < text.Length)
            {
                char c = text[i];
                if (IsCjk(c))
                {
                    atoms.Add(c.ToString());
                    i++;
                    continue;
                }

                bool isSpace = char.IsWhiteSpace(c);
                int j = i + 1;
                while (j < text.Length && char.IsWhiteSpace(text[j]) == isSpace && !IsCjk(text[j]))
                    j++;
                atoms.Add(text.Substring(i, j - i));
                i = j;
            }
            return atoms;
        }

        /// <summary>
        /// Rough range check for "this character doesn't rely on spaces to
        /// separate words" scripts — CJK Radicals through CJK Unified
        /// Ideographs, CJK Compatibility Ideographs, and Halfwidth/Fullwidth
        /// Forms (covers Fullwidth Chinese punctuation). Not exhaustive
        /// Unicode script detection, just enough for this project's actual
        /// Traditional Chinese UI text.
        /// </summary>
        private static bool IsCjk(char c) =>
            (c >= 0x2E80 && c <= 0x9FFF) || (c >= 0xF900 && c <= 0xFAFF) || (c >= 0xFF00 && c <= 0xFFEF);

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

        public void PushTransform(float scale, float offsetX, float offsetY)
        {
            Native.Save();
            Native.Translate(offsetX, offsetY);
            Native.Scale(scale, scale);
        }

        public void PopTransform() => Native.Restore();

        // Skia is always anti-aliased per-paint (each IBrush/IPen is created
        // with IsAntialias = true — see SkiaGraphicsFactory), so there is no
        // separate global quality mode to flip here.
        public void ApplyHighQualitySettings() { }

        private static SKRect ToSKRect(RectangleF r) => new SKRect(r.Left, r.Top, r.Right, r.Bottom);

        public void Dispose() => _ownedSurface?.Dispose();
    }
}
