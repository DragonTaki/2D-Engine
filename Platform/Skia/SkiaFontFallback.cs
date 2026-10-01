/* ----- ----- ----- ----- */
// SkiaFontFallback.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>
    /// Per-character font fallback for the Skia backend. Skia draws every
    /// character with the one typeface it is given and shows a missing glyph
    /// ("tofu") for anything that typeface lacks; GDI+ on Windows instead
    /// does font linking behind the scenes. So text in a font without CJK
    /// glyphs (e.g. "Consolas", which isn't installed on macOS and resolves to
    /// the default Latin typeface) would render as boxes here.
    /// <para>
    /// For a character the base typeface lacks, this picks a fallback the same
    /// way browsers and text stacks do: first the system font manager's
    /// <see cref="SKFontManager.MatchCharacter(string, SKFontStyle, string[], int)"/>
    /// (biased to Traditional Chinese, this project's UI language), then the
    /// app's bundled fonts (registered by <see cref="SkiaGraphicsFactory.LoadFontFamily"/>
    /// - <c>NotoSerifCJKtc</c> covers CJK) in load order. Results are cached
    /// per (base family, style, code point) for the process lifetime, so the
    /// system lookup runs at most once per distinct character.
    /// </para>
    /// </summary>
    internal static class SkiaFontFallback
    {
        // Language hint for MatchCharacter: Han characters are shared between
        // zh-Hant / zh-Hans / ja / ko, and the hint picks the regional glyph shapes.
        private static readonly string[] Bcp47 = { "zh-Hant" };

        private static readonly object Sync = new();

        // Probe fonts (one per bundled typeface) used only for glyph-coverage checks.
        private static readonly List<SKFont> BundledProbes = new();

        // null value = no fallback covers this code point (draw it with the base font).
        private static readonly Dictionary<(string family, int weight, int width, SKFontStyleSlant slant, int codePoint), SKTypeface> Cache = new();

        /// <summary>Adds an app-bundled typeface as a fallback candidate (after the system match).</summary>
        public static void RegisterBundledTypeface(SKTypeface typeface)
        {
            if (typeface == null)
                return;
            lock (Sync)
            {
                BundledProbes.Add(new SKFont(typeface));
                // A newly available candidate may cover code points that previously had none.
                Cache.Clear();
            }
        }

        /// <summary>
        /// Returns a typeface that has a glyph for <paramref name="codePoint"/>, chosen for
        /// text whose base typeface is <paramref name="baseTypeface"/> (which lacks it), or
        /// null if nothing available covers it.
        /// </summary>
        public static SKTypeface Resolve(SKTypeface baseTypeface, int codePoint)
        {
            var style = baseTypeface.FontStyle;
            string family = baseTypeface.FamilyName ?? string.Empty;
            var key = (family, style.Weight, style.Width, style.Slant, codePoint);

            lock (Sync)
            {
                if (Cache.TryGetValue(key, out var cached))
                    return cached;

                var result = MatchSystem(family, style, codePoint) ?? MatchBundled(codePoint);
                Cache[key] = result;
                return result;
            }
        }

        private static SKTypeface MatchSystem(string family, SKFontStyle style, int codePoint)
        {
            SKTypeface match;
            try
            {
                match = SKFontManager.Default.MatchCharacter(family, style, Bcp47, codePoint);
            }
            catch (Exception)
            {
                return null;
            }

            // Not disposed when rejected: SkiaSharp hands back the same managed instance
            // for the same native typeface, so it may be one that is cached or in use.
            if (match == null || IsLastResort(match) || !Covers(match, codePoint))
                return null;
            return match;
        }

        private static SKTypeface MatchBundled(int codePoint)
        {
            foreach (var probe in BundledProbes)
            {
                if (probe.ContainsGlyph(codePoint))
                    return probe.Typeface;
            }
            return null;
        }

        // macOS answers MatchCharacter with ".LastResort" when no real font has the
        // character; its "glyphs" are placeholder boxes, so it is not a real match.
        private static bool IsLastResort(SKTypeface typeface) =>
            typeface.FamilyName != null && typeface.FamilyName.IndexOf("LastResort", StringComparison.OrdinalIgnoreCase) >= 0;

        private static bool Covers(SKTypeface typeface, int codePoint)
        {
            using var probe = new SKFont(typeface);
            return probe.ContainsGlyph(codePoint);
        }
    }
}
