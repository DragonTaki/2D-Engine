/* ----- ----- ----- ----- */
// FlexSolver.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/29
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Engine.Mathematics;
using Engine.UI.Constants.Core;
using Engine.UI.Models;

namespace Engine.UI.Layout
{
    /// <summary>
    /// Pure flexbox layout, modeled on the CSS Flexible Box Layout algorithm
    /// (https://www.w3.org/TR/css-flexbox-1/#layout-algorithm) and Yoga:
    /// <list type="number">
    /// <item>Order the items (<see cref="UILayout.Order"/>, then child order).</item>
    /// <item>Flex base size from <see cref="UILayout.FlexBasis"/> (Auto = the main-axis
    /// Width/Height, Auto again = content), clamped by min/max to the hypothetical main size.</item>
    /// <item>Collect lines (one unless <see cref="FlexWrap.Wrap"/> and the main size is definite).</item>
    /// <item>Resolve flexible lengths per line: distribute positive free space by
    /// <see cref="UILayout.FlexGrow"/> or negative free space by <see cref="UILayout.FlexShrink"/>
    /// weighted by base size, freezing items that hit their min/max and repeating (CSS 9.7).</item>
    /// <item>Cross sizes: definite ones, aspect-ratio-derived ones, else content measured at
    /// the resolved main size; line cross size = the largest item (or the container's cross
    /// size for a single line).</item>
    /// <item><see cref="AlignContent"/> distributes lines; Stretch items fill their line.</item>
    /// <item><see cref="JustifyContent"/> distributes main-axis free space; align-self places
    /// items inside their line.</item>
    /// </list>
    /// Differences from CSS, kept simple on purpose: min sizes default to 0 (Yoga behavior,
    /// no automatic content-based minimum), no baseline alignment, no reverse directions.
    /// </summary>
    public static class FlexSolver
    {
        private const float Epsilon = 0.001f;

        /// <summary>Per-item working state for one solve.</summary>
        private sealed class FlexWork
        {
            public LayoutItem Item;
            public float MarginMainStart, MarginMainEnd, MarginCrossStart, MarginCrossEnd;
            public float? MinMain, MaxMain, MinCross, MaxCross;
            public float BaseSize, Hypothetical, Target;
            public bool Frozen;
            public float Violation;
            public float Cross;
            public bool StretchCross;
            public bool CrossFromRatio;
            public float MainPos, CrossPos;

            public float MarginMain => MarginMainStart + MarginMainEnd;
            public float MarginCross => MarginCrossStart + MarginCrossEnd;
        }

        private sealed class FlexLine
        {
            public readonly List<FlexWork> Items = new();
            public float Cross;
            public float CrossPos;
        }

        /// <summary>
        /// Lays out <paramref name="items"/> inside a flex container's content box.
        /// </summary>
        /// <param name="container">The container's rules (direction, wrap, justify, align, gaps).</param>
        /// <param name="innerWidth">Content box width, or <see cref="BoxSolver.Indefinite"/>.</param>
        /// <param name="innerHeight">Content box height, or <see cref="BoxSolver.Indefinite"/>.</param>
        /// <param name="items">The in-flow children. Results are written into each item, relative to the content box origin.</param>
        /// <param name="measuring">
        /// True when only the container's content size is wanted (its own size is Auto): free
        /// space is not grown into (fit-content), percentages/stretch count as Auto, and lines
        /// take their natural cross size.
        /// </param>
        /// <returns>The content size: widest line by the sum of line cross sizes (plus gaps).</returns>
        public static Vector2F Solve(UILayout container, float innerWidth, float innerHeight,
            IReadOnlyList<LayoutItem> items, bool measuring = false)
        {
            bool row = container.FlexDirection == FlexDirection.Row;
            float innerMain = row ? innerWidth : innerHeight;
            float innerCross = row ? innerHeight : innerWidth;
            float mainGap = row ? container.ColumnGap : container.RowGap;
            float crossGap = row ? container.RowGap : container.ColumnGap;
            bool mainDefinite = BoxSolver.IsDefinite(innerMain);
            bool crossDefinite = BoxSolver.IsDefinite(innerCross);

            // 1. Order (stable: ties keep child order).
            var works = items
                .OrderBy(i => i.Rules.Order).ThenBy(i => i.Index)
                .Select(i => CreateWork(i, row))
                .ToList();

            // 2. Flex base size and hypothetical main size.
            foreach (var w in works)
                ComputeBaseSize(w, row, innerMain, innerCross, measuring);

            // 3. Lines.
            var lines = CollectLines(works, container.FlexWrap == FlexWrap.Wrap && mainDefinite, innerMain, mainGap);

            // 4. Flexible lengths.
            foreach (var line in lines)
            {
                if (mainDefinite)
                    ResolveFlexibleLengths(line.Items, innerMain, mainGap, allowGrow: !measuring);
                else
                    foreach (var w in line.Items)
                        w.Target = w.Hypothetical;
            }

            // 5. Hypothetical cross sizes and line cross sizes.
            foreach (var w in works)
                ComputeCrossSize(w, container, row, innerCross, measuring);

            foreach (var line in lines)
                line.Cross = line.Items.Count == 0 ? 0f : line.Items.Max(w => w.Cross + w.MarginCross);

            float naturalCross = lines.Sum(l => l.Cross) + crossGap * Math.Max(0, lines.Count - 1);

            bool singleLine = container.FlexWrap == FlexWrap.NoWrap;
            if (singleLine && crossDefinite && !measuring && lines.Count == 1)
                lines[0].Cross = innerCross;

            // 6. Align content (multi-line containers only), then stretch items.
            if (!singleLine && crossDefinite && !measuring)
                AlignLines(lines, container.AlignContent, innerCross, crossGap);
            else
            {
                float pos = 0f;
                foreach (var line in lines)
                {
                    line.CrossPos = pos;
                    pos += line.Cross + crossGap;
                }
            }

            foreach (var line in lines)
            {
                foreach (var w in line.Items)
                {
                    if (w.StretchCross)
                        w.Cross = BoxSolver.Clamp(line.Cross - w.MarginCross, w.MinCross, w.MaxCross);

                    var align = w.Item.Rules.AlignSelf ?? container.AlignItems;
                    float factor = align switch
                    {
                        FlexAlign.Center => 0.5f,
                        FlexAlign.End => 1f,
                        _ => 0f
                    };
                    w.CrossPos = line.CrossPos + w.MarginCrossStart
                        + factor * (line.Cross - w.MarginCross - w.Cross);
                }
            }

            // 7. Justify content.
            float widestLine = 0f;
            foreach (var line in lines)
            {
                float used = line.Items.Sum(w => w.Target + w.MarginMain) + mainGap * Math.Max(0, line.Items.Count - 1);
                widestLine = Math.Max(widestLine, used);
                float free = mainDefinite && !measuring ? innerMain - used : 0f;
                JustifyLine(line.Items, container.JustifyContent, free, mainGap);
            }

            // Write results (with aspect-ratio fitting inside the item's flex box).
            foreach (var w in works)
            {
                float width = row ? w.Target : w.Cross;
                float height = row ? w.Cross : w.Target;
                float x = row ? w.MainPos : w.CrossPos;
                float y = row ? w.CrossPos : w.MainPos;

                float? ratio = BoxSolver.GetAspectRatio(w.Item.Rules);
                if (ratio.HasValue && !w.CrossFromRatio)
                {
                    float boxW = width, boxH = height;
                    BoxSolver.FitAspect(ratio.Value, w.Item.Rules.AspectFit, ref width, ref height);
                    x += BoxSolver.AlignFactor(w.Item.Rules.AlignX) * (boxW - width);
                    y += BoxSolver.AlignFactor(w.Item.Rules.AlignY) * (boxH - height);
                }

                w.Item.X = x;
                w.Item.Y = y;
                w.Item.Width = width;
                w.Item.Height = height;
            }

            return row ? new Vector2F(widestLine, naturalCross) : new Vector2F(naturalCross, widestLine);
        }

        #region Steps

        private static FlexWork CreateWork(LayoutItem item, bool row)
        {
            var r = item.Rules;
            var m = r.Margin;
            return new FlexWork
            {
                Item = item,
                MarginMainStart = row ? m.Left : m.Top,
                MarginMainEnd = row ? m.Right : m.Bottom,
                MarginCrossStart = row ? m.Top : m.Left,
                MarginCrossEnd = row ? m.Bottom : m.Right,
                MinMain = row ? r.MinWidth : r.MinHeight,
                MaxMain = row ? r.MaxWidth : r.MaxHeight,
                MinCross = row ? r.MinHeight : r.MinWidth,
                MaxCross = row ? r.MaxHeight : r.MaxWidth
            };
        }

        private static void ComputeBaseSize(FlexWork w, bool row, float innerMain, float innerCross, bool measuring)
        {
            var item = w.Item;
            var r = item.Rules;
            var mainProp = row ? r.Width : r.Height;
            var crossProp = row ? r.Height : r.Width;
            float declaredMain = row ? item.DeclaredWidth : item.DeclaredHeight;
            float declaredCross = row ? item.DeclaredHeight : item.DeclaredWidth;

            var basis = r.FlexBasis.Mode == SizeMode.Auto ? mainProp : r.FlexBasis;
            float stretchSpace = measuring ? BoxSolver.Indefinite : innerMain - w.MarginMain;
            float percentBase = measuring ? BoxSolver.Indefinite : innerMain;

            if (!BoxSolver.TryResolveLength(basis, declaredMain, stretchSpace, percentBase, out float size))
            {
                // Content-based: through the aspect ratio when the cross size is definite,
                // otherwise the max-content main size (measured unconstrained on the main axis).
                float? ratio = BoxSolver.GetAspectRatio(r);
                float crossSpace = innerCross - w.MarginCross;
                if (ratio.HasValue && crossProp.Mode != SizeMode.Stretch
                    && BoxSolver.TryResolveLength(crossProp, declaredCross, BoxSolver.Indefinite,
                        measuring ? BoxSolver.Indefinite : innerCross, out float cross))
                {
                    cross = BoxSolver.Clamp(cross, w.MinCross, w.MaxCross);
                    size = row ? cross * ratio.Value : cross / ratio.Value;
                }
                else
                {
                    var content = row
                        ? item.Measure(BoxSolver.Indefinite, crossSpace)
                        : item.Measure(crossSpace, BoxSolver.Indefinite);
                    size = row ? content.X : content.Y;
                }
            }

            w.BaseSize = Math.Max(0f, size);
            w.Hypothetical = BoxSolver.Clamp(w.BaseSize, w.MinMain, w.MaxMain);
        }

        private static List<FlexLine> CollectLines(List<FlexWork> works, bool wrap, float innerMain, float mainGap)
        {
            var lines = new List<FlexLine> { new FlexLine() };
            float lineUsed = 0f;

            foreach (var w in works)
            {
                var line = lines[lines.Count - 1];
                float outer = w.Hypothetical + w.MarginMain;
                float needed = line.Items.Count == 0 ? outer : lineUsed + mainGap + outer;

                if (wrap && line.Items.Count > 0 && needed > innerMain + Epsilon)
                {
                    line = new FlexLine();
                    lines.Add(line);
                    needed = outer;
                }

                line.Items.Add(w);
                lineUsed = needed;
            }

            return lines;
        }

        /// <summary>CSS Flexbox 9.7 "Resolving Flexible Lengths" for one line.</summary>
        private static void ResolveFlexibleLengths(List<FlexWork> line, float innerMain, float mainGap, bool allowGrow)
        {
            if (line.Count == 0)
                return;

            float gaps = mainGap * (line.Count - 1);
            float sumHypothetical = line.Sum(w => w.Hypothetical + w.MarginMain) + gaps;
            bool growing = sumHypothetical < innerMain;

            // Measuring (fit-content): never grow into free space.
            if (growing && !allowGrow)
            {
                foreach (var w in line)
                    w.Target = w.Hypothetical;
                return;
            }

            // Size inflexible items.
            foreach (var w in line)
            {
                var r = w.Item.Rules;
                float factor = growing ? r.FlexGrow : r.FlexShrink;
                w.Target = w.BaseSize;
                w.Frozen = factor <= 0f
                    || (growing && w.BaseSize > w.Hypothetical)
                    || (!growing && w.BaseSize < w.Hypothetical);
                if (w.Frozen)
                    w.Target = w.Hypothetical;
            }

            float initialFree = innerMain - gaps - line.Sum(w => w.MarginMain + (w.Frozen ? w.Target : w.BaseSize));

            for (int guard = 0; guard <= line.Count; guard++)
            {
                var unfrozen = line.Where(w => !w.Frozen).ToList();
                if (unfrozen.Count == 0)
                    break;

                float free = innerMain - gaps - line.Sum(w => w.MarginMain + (w.Frozen ? w.Target : w.BaseSize));

                float sumFactors = unfrozen.Sum(w => growing ? w.Item.Rules.FlexGrow : w.Item.Rules.FlexShrink);
                if (sumFactors < 1f)
                {
                    float scaled = initialFree * sumFactors;
                    if (Math.Abs(scaled) < Math.Abs(free))
                        free = scaled;
                }

                if (Math.Abs(free) > Epsilon)
                {
                    if (growing)
                    {
                        foreach (var w in unfrozen)
                            w.Target = w.BaseSize + free * (w.Item.Rules.FlexGrow / sumFactors);
                    }
                    else
                    {
                        float sumScaled = unfrozen.Sum(w => w.Item.Rules.FlexShrink * w.BaseSize);
                        foreach (var w in unfrozen)
                        {
                            float ratio = sumScaled > 0f ? w.Item.Rules.FlexShrink * w.BaseSize / sumScaled : 0f;
                            w.Target = w.BaseSize - Math.Abs(free) * ratio;
                        }
                    }
                }
                else
                {
                    foreach (var w in unfrozen)
                        w.Target = w.BaseSize;
                }

                // Clamp and freeze min/max violators.
                float totalViolation = 0f;
                foreach (var w in unfrozen)
                {
                    float clamped = BoxSolver.Clamp(w.Target, w.MinMain, w.MaxMain);
                    w.Violation = clamped - w.Target;
                    w.Target = clamped;
                    totalViolation += w.Violation;
                }

                if (Math.Abs(totalViolation) <= Epsilon)
                {
                    foreach (var w in unfrozen)
                        w.Frozen = true;
                }
                else
                {
                    foreach (var w in unfrozen)
                        if (totalViolation > 0f ? w.Violation > 0f : w.Violation < 0f)
                            w.Frozen = true;
                }
            }
        }

        private static void ComputeCrossSize(FlexWork w, UILayout container, bool row, float innerCross, bool measuring)
        {
            var item = w.Item;
            var r = item.Rules;
            var crossProp = row ? r.Height : r.Width;
            float declaredCross = row ? item.DeclaredHeight : item.DeclaredWidth;
            var align = r.AlignSelf ?? container.AlignItems;
            float? ratio = BoxSolver.GetAspectRatio(r);

            w.StretchCross = false;
            w.CrossFromRatio = false;

            // An Auto cross size with an aspect ratio follows the resolved main size.
            if (ratio.HasValue && crossProp.Mode == SizeMode.Auto)
            {
                w.Cross = BoxSolver.Clamp(row ? w.Target / ratio.Value : w.Target * ratio.Value, w.MinCross, w.MaxCross);
                w.CrossFromRatio = true;
                return;
            }

            if (crossProp.Mode != SizeMode.Stretch
                && BoxSolver.TryResolveLength(crossProp, declaredCross, BoxSolver.Indefinite,
                    measuring ? BoxSolver.Indefinite : innerCross, out float cross))
            {
                w.Cross = BoxSolver.Clamp(cross, w.MinCross, w.MaxCross);
                return;
            }

            // Content cross size at the resolved main size (text wraps to it). A Stretch size,
            // or Auto with align Stretch, then fills the line; the content size still counts
            // towards the line's own cross size.
            float crossSpace = innerCross - w.MarginCross;
            var content = row ? item.Measure(w.Target, crossSpace) : item.Measure(crossSpace, w.Target);
            w.Cross = BoxSolver.Clamp(row ? content.Y : content.X, w.MinCross, w.MaxCross);
            w.StretchCross = crossProp.Mode == SizeMode.Stretch
                || (crossProp.Mode == SizeMode.Auto && align == FlexAlign.Stretch);
        }

        private static void AlignLines(List<FlexLine> lines, AlignContent align, float innerCross, float crossGap)
        {
            float used = lines.Sum(l => l.Cross) + crossGap * Math.Max(0, lines.Count - 1);
            float free = innerCross - used;
            int n = lines.Count;

            float start = 0f, between = crossGap;
            if (free > 0f)
            {
                switch (align)
                {
                    case AlignContent.Center: start = free / 2f; break;
                    case AlignContent.End: start = free; break;
                    case AlignContent.Stretch:
                        foreach (var line in lines)
                            line.Cross += free / n;
                        break;
                    case AlignContent.SpaceBetween:
                        if (n > 1) between += free / (n - 1);
                        break;
                    case AlignContent.SpaceAround:
                        start = free / n / 2f;
                        between += free / n;
                        break;
                }
            }
            else if (free < 0f)
            {
                // Overflow: CSS falls back to start for space-between/stretch and center for space-around.
                switch (align)
                {
                    case AlignContent.Center:
                    case AlignContent.SpaceAround: start = free / 2f; break;
                    case AlignContent.End: start = free; break;
                }
            }

            float pos = start;
            foreach (var line in lines)
            {
                line.CrossPos = pos;
                pos += line.Cross + between;
            }
        }

        private static void JustifyLine(List<FlexWork> line, JustifyContent justify, float free, float mainGap)
        {
            int n = line.Count;
            if (n == 0)
                return;

            float start = 0f, between = mainGap;
            switch (justify)
            {
                case JustifyContent.Center: start = free / 2f; break;
                case JustifyContent.End: start = free; break;
                case JustifyContent.SpaceBetween:
                    if (free > 0f && n > 1) between += free / (n - 1);
                    break;
                case JustifyContent.SpaceAround:
                    if (free > 0f) { start = free / n / 2f; between += free / n; }
                    else start = free / 2f;
                    break;
                case JustifyContent.SpaceEvenly:
                    if (free > 0f) { start = free / (n + 1); between += free / (n + 1); }
                    else start = free / 2f;
                    break;
            }

            float pos = start;
            foreach (var w in line)
            {
                w.MainPos = pos + w.MarginMainStart;
                pos += w.MarginMain + w.Target + between;
            }
        }

        #endregion
    }
}
