/* ----- ----- ----- ----- */
// UILayoutStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/30
// Update Date: 2026/09/30
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Mathematics;
using Engine.UI.Constants.Core;

namespace Engine.UI.Models
{
    /// <summary>
    /// An immutable, declarative set of layout settings - the equivalent of one CSS rule
    /// (class) - applied to an element's <see cref="UILayout"/> with
    /// <see cref="UILayout.Apply(UILayoutStyle)"/>.
    /// <para>
    /// Every property maps 1:1 onto the <see cref="UILayout"/> property of the same name.
    /// <see langword="null"/> means "not declared": applying the style leaves that setting
    /// as it is (so a style only touches what it declares, like a CSS rule, and several
    /// styles can be applied one after another). A consequence is that a style cannot set a
    /// nullable <see cref="UILayout"/> setting (an inset, a min/max, the aspect ratio,
    /// <see cref="UILayout.AlignSelf"/>) back to "unset"; those start unset, so this only
    /// matters when re-styling an element.
    /// </para>
    /// <para>
    /// Being a record, a style can be derived from another with <c>with</c>
    /// (<c>baseStyle with { Top = 10f }</c>), which is how shared rules are reused.
    /// </para>
    /// </summary>
    public sealed record UILayoutStyle
    {
        #region Legacy

        /// <summary><see cref="UILayout.Anchor"/>.</summary>
        public Anchor? Anchor { get; init; }

        /// <summary><see cref="UILayout.Alignment"/> (not read by any layout code).</summary>
        public Alignment? Alignment { get; init; }

        /// <summary><see cref="UILayout.SizePercent"/>.</summary>
        public Vector2F SizePercent { get; init; }

        /// <summary><see cref="UILayout.AutoUpdate"/>.</summary>
        public bool? AutoUpdate { get; init; }

        /// <summary><see cref="UILayout.IgnoreParentLayout"/>.</summary>
        public bool? IgnoreParentLayout { get; init; }

        #endregion

        #region Positioning

        /// <summary><see cref="UILayout.PositionMode"/>.</summary>
        public PositionMode? PositionMode { get; init; }

        /// <summary><see cref="UILayout.Display"/>.</summary>
        public DisplayMode? Display { get; init; }

        /// <summary><see cref="UILayout.Left"/>.</summary>
        public float? Left { get; init; }

        /// <summary><see cref="UILayout.Top"/>.</summary>
        public float? Top { get; init; }

        /// <summary><see cref="UILayout.Right"/>.</summary>
        public float? Right { get; init; }

        /// <summary><see cref="UILayout.Bottom"/>.</summary>
        public float? Bottom { get; init; }

        /// <summary><see cref="UILayout.Offset"/>.</summary>
        public Vector2F Offset { get; init; }

        #endregion

        #region Size

        /// <summary><see cref="UILayout.Width"/>.</summary>
        public LayoutSize? Width { get; init; }

        /// <summary><see cref="UILayout.Height"/>.</summary>
        public LayoutSize? Height { get; init; }

        /// <summary><see cref="UILayout.MinWidth"/>.</summary>
        public float? MinWidth { get; init; }

        /// <summary><see cref="UILayout.MaxWidth"/>.</summary>
        public float? MaxWidth { get; init; }

        /// <summary><see cref="UILayout.MinHeight"/>.</summary>
        public float? MinHeight { get; init; }

        /// <summary><see cref="UILayout.MaxHeight"/>.</summary>
        public float? MaxHeight { get; init; }

        /// <summary><see cref="UILayout.AspectRatio"/>.</summary>
        public float? AspectRatio { get; init; }

        /// <summary><see cref="UILayout.AspectFit"/>.</summary>
        public AspectFit? AspectFit { get; init; }

        #endregion

        #region Alignment and Spacing

        /// <summary><see cref="UILayout.AlignX"/>.</summary>
        public Alignment? AlignX { get; init; }

        /// <summary><see cref="UILayout.AlignY"/>.</summary>
        public Alignment? AlignY { get; init; }

        /// <summary><see cref="UILayout.Margin"/>.</summary>
        public PaddingF? Margin { get; init; }

        /// <summary><see cref="UILayout.Padding"/>.</summary>
        public PaddingF? Padding { get; init; }

        #endregion

        #region Container

        /// <summary><see cref="UILayout.Container"/>.</summary>
        public LayoutContainer? Container { get; init; }

        /// <summary><see cref="UILayout.Overflow"/>.</summary>
        public OverflowMode? Overflow { get; init; }

        /// <summary><see cref="UILayout.FlexDirection"/>.</summary>
        public FlexDirection? FlexDirection { get; init; }

        /// <summary><see cref="UILayout.FlexWrap"/>.</summary>
        public FlexWrap? FlexWrap { get; init; }

        /// <summary><see cref="UILayout.JustifyContent"/>.</summary>
        public JustifyContent? JustifyContent { get; init; }

        /// <summary><see cref="UILayout.AlignItems"/>.</summary>
        public FlexAlign? AlignItems { get; init; }

        /// <summary><see cref="UILayout.AlignContent"/>.</summary>
        public AlignContent? AlignContent { get; init; }

        /// <summary><see cref="UILayout.RowGap"/>.</summary>
        public float? RowGap { get; init; }

        /// <summary><see cref="UILayout.ColumnGap"/>.</summary>
        public float? ColumnGap { get; init; }

        #endregion

        #region Flex Item

        /// <summary><see cref="UILayout.FlexGrow"/>.</summary>
        public float? FlexGrow { get; init; }

        /// <summary><see cref="UILayout.FlexShrink"/>.</summary>
        public float? FlexShrink { get; init; }

        /// <summary><see cref="UILayout.FlexBasis"/>.</summary>
        public LayoutSize? FlexBasis { get; init; }

        /// <summary><see cref="UILayout.AlignSelf"/>.</summary>
        public FlexAlign? AlignSelf { get; init; }

        /// <summary><see cref="UILayout.Order"/>.</summary>
        public int? Order { get; init; }

        #endregion
    }
}
