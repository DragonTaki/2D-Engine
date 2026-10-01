/* ----- ----- ----- ----- */
// SkiaFont.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Globalization;

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>SkiaSharp-backed <see cref="IFont"/>.</summary>
    internal sealed class SkiaFont : IFont
    {
        public SKFont Native { get; }
        public IFontFamily FontFamily { get; }
        public FontStyleFlags Style { get; }

        public SkiaFont(SKFont native, IFontFamily fontFamily, FontStyleFlags style)
        {
            Native = native;
            FontFamily = fontFamily;
            Style = style;
        }

        public float Size => Native.Size;

        /// <summary>Recommended line height in pixels, matching GDI+'s <c>Font.Height</c>.</summary>
        public float Height
        {
            get
            {
                var metrics = Native.Metrics;
                return metrics.Descent - metrics.Ascent + metrics.Leading;
            }
        }

        // Same size/style as Native, on a fallback typeface; created on first use, so a
        // font whose text is always covered by its own typeface never allocates any.
        private Dictionary<SKTypeface, SKFont> _fallbackFonts;

        /// <summary>
        /// Advance width of <paramref name="text"/> (single line) with per-character
        /// fallback (see <see cref="SkiaFontFallback"/>), matching <see cref="DrawText"/>.
        /// </summary>
        public float MeasureText(string text, SKPaint paint)
        {
            if (string.IsNullOrEmpty(text))
                return 0f;
            if (Native.ContainsGlyphs(text))
                return Native.MeasureText(text, paint);

            float width = 0f;
            int start = 0;
            while (start < text.Length)
            {
                var runFont = NextRun(text, start, out int end);
                width += runFont.MeasureText(text.AsSpan(start, end - start), paint);
                start = end;
            }
            return width;
        }

        /// <summary>
        /// Draws <paramref name="text"/> (single line) left-aligned at a baseline, run by
        /// run: characters this font's typeface lacks use a fallback typeface at the same
        /// size/style, on the same baseline.
        /// </summary>
        public void DrawText(SKCanvas canvas, string text, float x, float baselineY, SKPaint paint)
        {
            if (string.IsNullOrEmpty(text))
                return;
            if (Native.ContainsGlyphs(text))
            {
                canvas.DrawText(text, x, baselineY, SKTextAlign.Left, Native, paint);
                return;
            }

            int start = 0;
            while (start < text.Length)
            {
                var runFont = NextRun(text, start, out int end);
                string run = text.Substring(start, end - start);
                canvas.DrawText(run, x, baselineY, SKTextAlign.Left, runFont, paint);
                x += runFont.MeasureText(run, paint);
                start = end;
            }
        }

        /// <summary>
        /// Finds the maximal run starting at <paramref name="start"/> whose characters all
        /// use the same font, returning that font and the run's exclusive end index.
        /// </summary>
        private SKFont NextRun(string text, int start, out int end)
        {
            SKFont runFont = null;
            int i = start;
            while (i < text.Length)
            {
                int length = CodePointAt(text, i, out int codePoint);
                SKFont font = FontFor(text, i, codePoint);
                if (font != null)
                {
                    if (runFont == null)
                        runFont = font;
                    else if (!ReferenceEquals(font, runFont))
                        break;
                }
                i += length;
            }
            end = i;
            return runFont ?? Native;
        }

        /// <summary>
        /// The font for one code point, or null for one that just joins the current run
        /// (combining marks, variation selectors, joiners, control characters) - those
        /// must stay with the character they modify.
        /// </summary>
        private SKFont FontFor(string text, int index, int codePoint)
        {
            var category = char.IsSurrogatePair(text, index)
                ? CharUnicodeInfo.GetUnicodeCategory(codePoint)
                : CharUnicodeInfo.GetUnicodeCategory(text[index]);
            if (category == UnicodeCategory.NonSpacingMark || category == UnicodeCategory.EnclosingMark
                || category == UnicodeCategory.Format || category == UnicodeCategory.Control)
                return null;

            // A lone (unpaired) surrogate has no glyph anywhere; leave it to the base font.
            if (category == UnicodeCategory.Surrogate || Native.ContainsGlyph(codePoint))
                return Native;

            var typeface = SkiaFontFallback.Resolve(Native.Typeface ?? SKTypeface.Default, codePoint);
            return typeface == null ? Native : GetFallbackFont(typeface);
        }

        private SKFont GetFallbackFont(SKTypeface typeface)
        {
            _fallbackFonts ??= new Dictionary<SKTypeface, SKFont>(ReferenceEqualityComparer.Instance);
            if (_fallbackFonts.TryGetValue(typeface, out var font))
                return font;

            font = new SKFont(typeface, Native.Size, Native.ScaleX, Native.SkewX)
            {
                Embolden = Native.Embolden,
                Edging = Native.Edging,
                Hinting = Native.Hinting,
                Subpixel = Native.Subpixel,
                LinearMetrics = Native.LinearMetrics,
                EmbeddedBitmaps = Native.EmbeddedBitmaps,
                ForceAutoHinting = Native.ForceAutoHinting,
                BaselineSnap = Native.BaselineSnap,
            };
            _fallbackFonts[typeface] = font;
            return font;
        }

        /// <summary>Code point at <paramref name="index"/>; returns its UTF-16 length (a lone surrogate counts as itself).</summary>
        private static int CodePointAt(string text, int index, out int codePoint)
        {
            if (char.IsSurrogatePair(text, index))
            {
                codePoint = char.ConvertToUtf32(text[index], text[index + 1]);
                return 2;
            }
            codePoint = text[index];
            return 1;
        }

        public void Dispose()
        {
            if (_fallbackFonts != null)
            {
                foreach (var font in _fallbackFonts.Values)
                    font.Dispose();
                _fallbackFonts = null;
            }
            Native.Dispose();
        }
    }
}
