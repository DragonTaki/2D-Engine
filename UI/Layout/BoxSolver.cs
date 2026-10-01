/* ----- ----- ----- ----- */
// BoxSolver.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/30
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Engine.Mathematics;
using Engine.UI.Constants.Core;
using Engine.UI.Models;

namespace Engine.UI.Layout
{
    /// <summary>
    /// Pure layout math shared by every arrangement: per-axis size resolution (size modes,
    /// min/max, aspect ratio), plus placement of non-flex in-flow children and of absolute
    /// children. All sizes are border-box; margins are outside them.
    /// <para>
    /// Coordinates are relative to the box handed in. <see cref="float.PositiveInfinity"/>
    /// stands for an indefinite (unconstrained) length.
    /// </para>
    /// </summary>
    public static class BoxSolver
    {
        #region Helpers

        /// <summary>An indefinite length.</summary>
        public const float Indefinite = float.PositiveInfinity;

        /// <summary>True for a real, finite length.</summary>
        public static bool IsDefinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

        /// <summary>0 for Start/None, 0.5 for Center, 1 for End.</summary>
        public static float AlignFactor(Alignment alignment) => alignment switch
        {
            Alignment.Center => 0.5f,
            Alignment.End => 1f,
            _ => 0f
        };

        /// <summary>Applies max then min (so min wins, as in CSS) and never returns a negative size.</summary>
        public static float Clamp(float value, float? min, float? max)
        {
            if (max.HasValue && value > max.Value)
                value = max.Value;
            if (min.HasValue && value < min.Value)
                value = min.Value;
            return float.IsNaN(value) || value < 0f ? 0f : value;
        }

        /// <summary>The usable aspect ratio, or null when unlocked.</summary>
        public static float? GetAspectRatio(UILayout rules) =>
            rules.AspectRatio is float r && r > 0f && IsDefinite(r) ? r : null;

        /// <summary>
        /// Resolves one axis to a definite length when its mode allows it without measuring.
        /// </summary>
        /// <param name="size">The axis' size setting.</param>
        /// <param name="declared">The element's declared size on this axis.</param>
        /// <param name="stretchSpace">Space a Stretch size fills (already minus margins); indefinite = can't stretch.</param>
        /// <param name="percentBase">What a Percent size is a fraction of; indefinite = behaves as Auto.</param>
        /// <param name="length">The resolved length.</param>
        /// <returns>False when the axis is content-sized (Auto, or Percent/Stretch against an indefinite space).</returns>
        public static bool TryResolveLength(LayoutSize size, float declared, float stretchSpace, float percentBase, out float length)
        {
            switch (size.Mode)
            {
                case SizeMode.Fixed:
                    length = size.Value ?? declared;
                    return true;

                case SizeMode.Percent:
                    if (IsDefinite(percentBase))
                    {
                        length = size.Value.GetValueOrDefault() * percentBase;
                        return true;
                    }
                    break;

                case SizeMode.Stretch:
                    if (IsDefinite(stretchSpace))
                    {
                        length = Math.Max(0f, stretchSpace);
                        return true;
                    }
                    break;
            }

            length = 0f;
            return false;
        }

        /// <summary>
        /// Fits a box of <paramref name="ratio"/> (width / height) into or over the given box.
        /// </summary>
        public static void FitAspect(float ratio, AspectFit fit, ref float width, ref float height)
        {
            if (width <= 0f || height <= 0f)
                return;

            bool wider = width / height > ratio;
            if (fit == AspectFit.Contain)
            {
                if (wider) width = height * ratio;
                else height = width / ratio;
            }
            else
            {
                if (wider) height = width / ratio;
                else width = height * ratio;
            }
        }

        #endregion

        #region Size Resolution

        /// <summary>
        /// Resolves an item's border-box size on both axes.
        /// <para>
        /// Order: each axis by its size mode (Fixed / Percent / Stretch; Auto and indefinite
        /// ones by measuring - width first, then height at that width, so text wraps); an
        /// aspect ratio derives a content-sized axis from the other; min/max clamp; finally,
        /// when both axes were definite and a ratio is set, the size is fitted into that box
        /// (Contain/Cover). The caller aligns the result inside the space it offered.
        /// </para>
        /// </summary>
        /// <param name="item">The item to size.</param>
        /// <param name="stretchW">Space a Stretch width fills (minus margins), or indefinite.</param>
        /// <param name="stretchH">Space a Stretch height fills (minus margins), or indefinite.</param>
        /// <param name="percentW">Base for a Percent width, or indefinite.</param>
        /// <param name="percentH">Base for a Percent height, or indefinite.</param>
        /// <param name="measureW">Available width passed to the measure hook (for wrapping).</param>
        /// <param name="measureH">Available height passed to the measure hook.</param>
        /// <param name="forceStretchW">Width is stretched regardless of its mode (both horizontal insets set).</param>
        /// <param name="forceStretchH">Height is stretched regardless of its mode (both vertical insets set).</param>
        /// <returns>The resolved border-box size.</returns>
        public static Vector2F ResolveSize(LayoutItem item,
            float stretchW, float stretchH,
            float percentW, float percentH,
            float measureW, float measureH,
            bool forceStretchW = false, bool forceStretchH = false) =>
            ResolveSize(item, stretchW, stretchH, percentW, percentH, measureW, measureH,
                forceStretchW, forceStretchH, out _);

        /// <summary>
        /// <see cref="ResolveSize(LayoutItem, float, float, float, float, float, float, bool, bool)"/>,
        /// also returning the box the size was resolved to before aspect-ratio fitting
        /// (equal to the result when no fitting happened), so callers can align a fitted
        /// element inside it.
        /// </summary>
        public static Vector2F ResolveSize(LayoutItem item,
            float stretchW, float stretchH,
            float percentW, float percentH,
            float measureW, float measureH,
            bool forceStretchW, bool forceStretchH,
            out Vector2F box)
        {
            var rules = item.Rules;
            float? ratio = GetAspectRatio(rules);

            float w, h;
            bool definiteW = forceStretchW
                ? TryResolveLength(LayoutSize.Stretch, item.DeclaredWidth, stretchW, percentW, out w)
                : TryResolveLength(rules.Width, item.DeclaredWidth, stretchW, percentW, out w);
            bool definiteH = forceStretchH
                ? TryResolveLength(LayoutSize.Stretch, item.DeclaredHeight, stretchH, percentH, out h)
                : TryResolveLength(rules.Height, item.DeclaredHeight, stretchH, percentH, out h);

            if (definiteW && definiteH)
            {
                w = Clamp(w, rules.MinWidth, rules.MaxWidth);
                h = Clamp(h, rules.MinHeight, rules.MaxHeight);
                box = new Vector2F(w, h);
                if (ratio.HasValue)
                    FitAspect(ratio.Value, rules.AspectFit, ref w, ref h);
                return new Vector2F(w, h);
            }

            if (definiteW)
            {
                w = Clamp(w, rules.MinWidth, rules.MaxWidth);
                h = ratio.HasValue ? w / ratio.Value : item.Measure(w, measureH).Y;
                h = Clamp(h, rules.MinHeight, rules.MaxHeight);
                box = new Vector2F(w, h);
                return new Vector2F(w, h);
            }

            if (definiteH)
            {
                h = Clamp(h, rules.MinHeight, rules.MaxHeight);
                w = ratio.HasValue ? h * ratio.Value : item.Measure(measureW, h).X;
                w = Clamp(w, rules.MinWidth, rules.MaxWidth);
                box = new Vector2F(w, h);
                return new Vector2F(w, h);
            }

            // Both content-sized: width from the content, height at that width.
            var content = item.Measure(measureW, measureH);
            w = Clamp(content.X, rules.MinWidth, rules.MaxWidth);
            if (ratio.HasValue)
                h = w / ratio.Value;
            else
                h = w == content.X ? content.Y : item.Measure(w, measureH).Y;
            h = Clamp(h, rules.MinHeight, rules.MaxHeight);
            box = new Vector2F(w, h);
            return new Vector2F(w, h);
        }

        #endregion

        #region Placement

        /// <summary>
        /// Places an in-flow child of a non-flex container: sized by its own rules inside the
        /// content box and aligned per axis by <see cref="UILayout.AlignX"/>/<see cref="UILayout.AlignY"/>.
        /// </summary>
        /// <param name="item">The child.</param>
        /// <param name="boxX">Content box left, in the container's coordinates.</param>
        /// <param name="boxY">Content box top.</param>
        /// <param name="boxW">Content box width.</param>
        /// <param name="boxH">Content box height.</param>
        public static void PlaceFlow(LayoutItem item, float boxX, float boxY, float boxW, float boxH)
        {
            var rules = item.Rules;
            var margin = rules.Margin;
            float spaceW = boxW - margin.Horizontal;
            float spaceH = boxH - margin.Vertical;

            var size = ResolveSize(item, spaceW, spaceH, boxW, boxH, spaceW, spaceH);

            item.Width = size.X;
            item.Height = size.Y;
            item.X = boxX + margin.Left + AlignFactor(rules.AlignX) * (spaceW - size.X);
            item.Y = boxY + margin.Top + AlignFactor(rules.AlignY) * (spaceH - size.Y);
        }

        /// <summary>
        /// Places an absolute child against the container's padding box (origin 0,0):
        /// per axis, both insets set = stretch between them; one inset = pinned to that edge;
        /// none = aligned by AlignX/AlignY. Margins apply inside the insets.
        /// </summary>
        /// <param name="item">The child.</param>
        /// <param name="boxW">Padding box width (the container's border-box width).</param>
        /// <param name="boxH">Padding box height.</param>
        public static void PlaceAbsolute(LayoutItem item, float boxW, float boxH)
        {
            var rules = item.Rules;
            var margin = rules.Margin;

            float spaceW = boxW - (rules.Left ?? 0f) - (rules.Right ?? 0f) - margin.Horizontal;
            float spaceH = boxH - (rules.Top ?? 0f) - (rules.Bottom ?? 0f) - margin.Vertical;
            bool stretchX = rules.Left.HasValue && rules.Right.HasValue;
            bool stretchY = rules.Top.HasValue && rules.Bottom.HasValue;

            var size = ResolveSize(item, spaceW, spaceH, boxW, boxH, spaceW, spaceH, stretchX, stretchY, out var fitBox);

            item.Width = size.X;
            item.Height = size.Y;
            item.X = PlaceAbsoluteAxis(rules.Left, rules.Right, margin.Left, margin.Right, boxW, spaceW, fitBox.X, size.X, rules.AlignX);
            item.Y = PlaceAbsoluteAxis(rules.Top, rules.Bottom, margin.Top, margin.Bottom, boxH, spaceH, fitBox.Y, size.Y, rules.AlignY);
        }

        /// <summary>
        /// One axis of <see cref="PlaceAbsolute"/>: the start coordinate of an absolute child
        /// (both insets: aligned in the space between them; one inset: pinned to that edge;
        /// none: aligned in the box).
        /// </summary>
        /// <param name="slot">The size before aspect-ratio fitting; a fitted element is aligned inside it.</param>
        private static float PlaceAbsoluteAxis(float? start, float? end, float marginStart, float marginEnd,
            float box, float space, float slot, float size, Alignment align)
        {
            if (start.HasValue && end.HasValue)
                return start.Value + marginStart + AlignFactor(align) * (space - size);
            if (start.HasValue)
                return start.Value + marginStart + AlignFactor(align) * (slot - size);
            if (end.HasValue)
                return box - end.Value - marginEnd - slot + AlignFactor(align) * (slot - size);
            return marginStart + AlignFactor(align) * (space - size);
        }

        #endregion

        #region Measurement

        /// <summary>
        /// Content size of a non-flex container: the largest margin box among its in-flow
        /// children (they overlap). Percent/Stretch children count as Auto here, because the
        /// container's own size is what is being determined (CSS: percentages of an
        /// indefinite size behave as auto).
        /// </summary>
        /// <param name="items">In-flow children.</param>
        /// <param name="spaceW">Available content width (may be indefinite).</param>
        /// <param name="spaceH">Available content height (may be indefinite).</param>
        public static Vector2F MeasureFlow(IReadOnlyList<LayoutItem> items, float spaceW, float spaceH)
        {
            float width = 0f, height = 0f;
            foreach (var item in items)
            {
                var margin = item.Rules.Margin;
                var size = ResolveSize(item, Indefinite, Indefinite, Indefinite, Indefinite,
                    spaceW - margin.Horizontal, spaceH - margin.Vertical);
                width = Math.Max(width, size.X + margin.Horizontal);
                height = Math.Max(height, size.Y + margin.Vertical);
            }
            return new Vector2F(width, height);
        }

        #endregion
    }
}
