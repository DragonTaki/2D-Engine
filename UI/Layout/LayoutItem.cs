/* ----- ----- ----- ----- */
// LayoutItem.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/29
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.Mathematics;
using Engine.UI.Models;

namespace Engine.UI.Layout
{
    /// <summary>
    /// One child as seen by the layout solvers: its rules, declared size and measure hook
    /// in, its resolved border-box rectangle out. Keeps <see cref="BoxSolver"/> and
    /// <see cref="FlexSolver"/> independent of the element classes (pure layout math).
    /// </summary>
    public sealed class LayoutItem
    {
        #region Inputs

        /// <summary>The child's layout rules.</summary>
        public UILayout Rules { get; }

        /// <summary>The child's declared width (what <see cref="LayoutSize.Declared"/> resolves to).</summary>
        public float DeclaredWidth { get; }

        /// <summary>The child's declared height.</summary>
        public float DeclaredHeight { get; }

        /// <summary>Position among its siblings, the tie-breaker for <see cref="UILayout.Order"/>.</summary>
        public int Index { get; }

        /// <summary>Opaque reference back to whatever this item stands for (e.g. the element).</summary>
        public object Tag { get; }

        private readonly Func<Vector2F, Vector2F> _measure;

        #endregion

        #region Results

        /// <summary>Resolved left edge, relative to the box the solver was given.</summary>
        public float X { get; internal set; }

        /// <summary>Resolved top edge, relative to the box the solver was given.</summary>
        public float Y { get; internal set; }

        /// <summary>Resolved border-box width.</summary>
        public float Width { get; internal set; }

        /// <summary>Resolved border-box height.</summary>
        public float Height { get; internal set; }

        #endregion

        #region Measure Cache

        // Measuring can be expensive (text) and the solvers ask for the same constraint
        // more than once per pass, so the last two answers are kept.
        private float _cacheW1 = float.NaN, _cacheH1 = float.NaN, _cacheW2 = float.NaN, _cacheH2 = float.NaN;
        private Vector2F _cacheResult1, _cacheResult2;

        #endregion

        public LayoutItem(UILayout rules, float declaredWidth, float declaredHeight, int index,
            Func<Vector2F, Vector2F> measure, object tag = null)
        {
            Rules = rules ?? new UILayout();
            DeclaredWidth = declaredWidth;
            DeclaredHeight = declaredHeight;
            Index = index;
            _measure = measure;
            Tag = tag;
        }

        /// <summary>
        /// Intrinsic border-box size for the given available border-box space
        /// (<see cref="float.PositiveInfinity"/> = unconstrained). Without a measure hook the
        /// declared size is the intrinsic size.
        /// </summary>
        public Vector2F Measure(float availableWidth, float availableHeight)
        {
            if (_measure == null)
                return new Vector2F(DeclaredWidth, DeclaredHeight);

            if (_cacheResult1 != null && _cacheW1.Equals(availableWidth) && _cacheH1.Equals(availableHeight))
                return _cacheResult1;
            if (_cacheResult2 != null && _cacheW2.Equals(availableWidth) && _cacheH2.Equals(availableHeight))
                return _cacheResult2;

            var result = _measure(new Vector2F(Math.Max(0f, availableWidth), Math.Max(0f, availableHeight)))
                ?? new Vector2F(DeclaredWidth, DeclaredHeight);

            _cacheW2 = _cacheW1; _cacheH2 = _cacheH1; _cacheResult2 = _cacheResult1;
            _cacheW1 = availableWidth; _cacheH1 = availableHeight; _cacheResult1 = result;
            return result;
        }
    }
}
