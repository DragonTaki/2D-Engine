/* ----- ----- ----- ----- */
// SkiaGraphics.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/10/01
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
        private readonly Stack<(int saveCount, int clipDepth)> _transformSaves = new();

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
            // Characters the font lacks are drawn with a fallback typeface (SkiaFont.DrawText).
            var skiaFont = (SkiaFont)font;
            skiaFont.DrawText(Native, text, x, y - skiaFont.Native.Metrics.Ascent, ((SkiaBrush)brush).Native);
        }

        public void DrawString(string text, IFont font, IBrush brush, RectangleF bounds, IStringFormat format)
        {
            // Line height and baseline come from the base font only, so the layout of text
            // the base font fully covers is unchanged; fallback glyphs share that baseline.
            var skiaFont = (SkiaFont)font;
            var skFont = skiaFont.Native;
            var paint = ((SkiaBrush)brush).Native;
            var fmt = (SkiaStringFormat)format;

            var metrics = skFont.Metrics;
            float lineHeight = metrics.Descent - metrics.Ascent + metrics.Leading;

            var lines = fmt.WordWrap
                ? WrapText(text, skiaFont, paint, bounds.Width)
                : SplitLines(text);

            if (fmt.EllipsisTrimming)
                lines = TrimWithEllipsis(lines, skiaFont, paint, bounds.Width, bounds.Height, lineHeight);

            float totalHeight = lineHeight * Math.Max(lines.Count, 1);

            // Top of the text block, offset down by ascent per line so the
            // glyphs' top edge lands at the aligned position.
            float startTopY = fmt.LineAlignment switch
            {
                TextAlign.Center => bounds.Y + (bounds.Height - totalHeight) / 2f,
                TextAlign.Far => bounds.Y + bounds.Height - totalHeight,
                _ => bounds.Y,
            };

            // Clip to the layout rectangle, as GDI+ DrawString does by default
            // (StringFormatFlags.NoClip unset).
            int saveCount = Native.Save();
            Native.ClipRect(ToSKRect(bounds));
            try
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    float lineWidth = skiaFont.MeasureText(line, paint);

                    float x = fmt.Alignment switch
                    {
                        TextAlign.Center => bounds.X + (bounds.Width - lineWidth) / 2f,
                        TextAlign.Far => bounds.X + bounds.Width - lineWidth,
                        _ => bounds.X,
                    };
                    float baselineY = startTopY + i * lineHeight - metrics.Ascent;

                    skiaFont.DrawText(Native, line, x, baselineY, paint);
                }
            }
            finally
            {
                Native.RestoreToCount(saveCount);
            }
        }

        /// <summary>
        /// Splits on explicit line breaks ("\n", "\r\n"), like GDI+ does even without wrapping.
        /// </summary>
        private static List<string> SplitLines(string text) =>
            new List<string>((text ?? string.Empty).Replace("\r\n", "\n").Split('\n'));

        /// <summary>
        /// GDI+ <c>StringTrimming.EllipsisCharacter</c>: keep only the lines that fit the
        /// box's height (at least one), and if anything was cut - more lines, or a last line
        /// wider than the box - end the last kept line with "…" trimmed to fit.
        /// </summary>
        private static List<string> TrimWithEllipsis(List<string> lines, SkiaFont skFont, SKPaint paint,
            float maxWidth, float maxHeight, float lineHeight)
        {
            const string Ellipsis = "\u2026";

            // Small tolerance: boxes are usually sized to exactly N * lineHeight (from
            // MeasureString), and float error must not drop the last of those N lines.
            int fitCount = lineHeight > 0 ? Math.Max(1, (int)(maxHeight / lineHeight + 0.01f)) : lines.Count;
            bool truncated = lines.Count > fitCount;
            var result = truncated ? lines.GetRange(0, fitCount) : new List<string>(lines);

            int last = result.Count - 1;
            if (last < 0)
                return result;

            string lastLine = result[last];
            if (!truncated && skFont.MeasureText(lastLine, paint) <= maxWidth)
                return result;

            while (lastLine.Length > 0 && skFont.MeasureText(lastLine + Ellipsis, paint) > maxWidth)
            {
                // Drop a whole surrogate pair at once, never leaving half of one.
                int cut = lastLine.Length >= 2 && char.IsSurrogatePair(lastLine, lastLine.Length - 2) ? 2 : 1;
                lastLine = lastLine.Substring(0, lastLine.Length - cut);
            }

            result[last] = lastLine.TrimEnd() + Ellipsis;
            return result;
        }

        public SizeF MeasureString(string text, IFont font)
        {
            var skFont = (SkiaFont)font;
            using var paint = new SKPaint();

            // Explicit line breaks count, as in GDI+ MeasureString.
            var lines = SplitLines(text);
            float width = 0f;
            foreach (var line in lines)
                width = Math.Max(width, skFont.MeasureText(line, paint));

            return new SizeF(width, ((SkiaFont)font).Height * lines.Count);
        }

        public SizeF MeasureString(string text, IFont font, int maxWidth)
        {
            var skFont = (SkiaFont)font;
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
        private static List<string> WrapText(string text, SkiaFont skFont, SKPaint paint, float maxWidth)
        {
            var lines = new List<string>();

            // Explicit line breaks always start a new line (an empty paragraph stays an
            // empty line), as in GDI+; each paragraph is then wrapped on its own.
            foreach (var paragraph in SplitLines(text))
            {
                if (paragraph.Length == 0)
                {
                    lines.Add(string.Empty);
                    continue;
                }

                int linesBefore = lines.Count;
                var current = "";
                foreach (var atom in TokenizeForWrap(paragraph))
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

                    // A single word wider than the whole line: break it between characters
                    // (as GDI+ does) instead of letting it overflow the box.
                    // Steps by whole code points so a surrogate pair is never split.
                    while (current.Length > CharLength(current, 0) && skFont.MeasureText(current, paint) > maxWidth)
                    {
                        int fit = CharLength(current, 0);
                        while (fit < current.Length)
                        {
                            int next = fit + CharLength(current, fit);
                            if (skFont.MeasureText(current.Substring(0, next), paint) > maxWidth)
                                break;
                            fit = next;
                        }
                        lines.Add(current.Substring(0, fit));
                        current = current.Substring(fit);
                    }
                }
                if (current.Trim().Length > 0)
                    lines.Add(current.TrimEnd());
                else if (lines.Count == linesBefore)
                    lines.Add(string.Empty);  // Whitespace-only paragraph: still one (blank) line
            }

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
                int length = CharLength(text, i);
                if (IsCjk(text, i))
                {
                    atoms.Add(text.Substring(i, length));
                    i += length;
                    continue;
                }

                bool isSpace = char.IsWhiteSpace(text[i]);
                int j = i + length;
                while (j < text.Length && char.IsWhiteSpace(text[j]) == isSpace && !IsCjk(text, j))
                    j += CharLength(text, j);
                atoms.Add(text.Substring(i, j - i));
                i = j;
            }
            return atoms;
        }

        /// <summary>
        /// Rough range check for "this character doesn't rely on spaces to
        /// separate words" scripts — CJK Radicals through CJK Unified
        /// Ideographs, CJK Compatibility Ideographs, and Halfwidth/Fullwidth
        /// Forms (covers Fullwidth Chinese punctuation), plus the supplementary
        /// ideograph planes (U+20000-U+3FFFF, CJK Extension B onward, written
        /// as surrogate pairs). Not exhaustive Unicode script detection, just
        /// enough for this project's actual Traditional Chinese UI text.
        /// </summary>
        private static bool IsCjk(string text, int index)
        {
            if (char.IsSurrogatePair(text, index))
            {
                int cp = char.ConvertToUtf32(text[index], text[index + 1]);
                return cp >= 0x20000 && cp <= 0x3FFFF;
            }
            char c = text[index];
            return (c >= 0x2E80 && c <= 0x9FFF) || (c >= 0xF900 && c <= 0xFAFF) || (c >= 0xFF00 && c <= 0xFFEF);
        }

        /// <summary>UTF-16 length of the code point at <paramref name="index"/>: 2 for a surrogate pair, else 1.</summary>
        private static int CharLength(string text, int index) =>
            char.IsSurrogatePair(text, index) ? 2 : 1;

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
            // Remember the exact save level: SetClip shares the canvas save stack, so a
            // plain Restore() in PopTransform would undo an unpaired clip instead of this.
            _transformSaves.Push((Native.Save(), _clipDepth));
            Native.Translate(offsetX, offsetY);
            Native.Scale(scale, scale);
        }

        public void PopTransform()
        {
            if (_transformSaves.Count == 0)
                return;

            var (saveCount, clipDepth) = _transformSaves.Pop();
            Native.RestoreToCount(saveCount);
            _clipDepth = clipDepth;
        }

        // Skia is always anti-aliased per-paint (each IBrush/IPen is created
        // with IsAntialias = true — see SkiaGraphicsFactory), so there is no
        // separate global quality mode to flip here.
        public void ApplyHighQualitySettings() { }

        private static SKRect ToSKRect(RectangleF r) => new SKRect(r.Left, r.Top, r.Right, r.Bottom);

        public void Dispose() => _ownedSurface?.Dispose();
    }
}
