/* ----- ----- ----- ----- */
// UILayout.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2026/09/30
// Version: v2.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Engine.Mathematics;
using Engine.UI.Constants.Core;

namespace Engine.UI.Models
{
    /// <summary>
    /// Represents layout configuration parameters that define how a UI element is positioned
    /// and sized relative to its parent container.
    /// <para>
    /// Two groups of settings live here:
    /// <list type="bullet">
    /// <item><b>Legacy</b> (<see cref="Anchor"/>, <see cref="SizePercent"/>, <see cref="Alignment"/>,
    /// <see cref="AutoUpdate"/>): applied once by <c>UIElement.UpdateLayout</c> when
    /// <see cref="PositionMode"/> is <see cref="Constants.Core.PositionMode.Legacy"/> (the default).</item>
    /// <item><b>Layout system</b> (everything else): used when <see cref="PositionMode"/> is
    /// <see cref="Constants.Core.PositionMode.Flow"/> or <see cref="Constants.Core.PositionMode.Absolute"/>,
    /// and - for the container settings (<see cref="Padding"/>, <see cref="Container"/>, flex,
    /// <see cref="Overflow"/>) - by any element that has such children.</item>
    /// </list>
    /// <see cref="Margin"/>, <see cref="Display"/> and <see cref="IgnoreParentLayout"/> apply to both.
    /// Every per-axis setting is independent for X and Y. See docs/LAYOUT.md for the full model.
    /// </para>
    /// <para>
    /// Every setter raises <see cref="Changed"/> when the value actually changes; the owning
    /// element listens and invalidates its layout. Mutating a <see cref="Vector2F"/> value in
    /// place (e.g. <c>Offset.X = 3</c>) bypasses that - assign a new vector instead, or call
    /// <c>InvalidateLayout()</c> on the element.
    /// </para>
    /// </summary>
    public class UILayout
    {
        #region Change Notification

        /// <summary>
        /// Raised after any property changes value. The owning element subscribes to this to
        /// invalidate its layout.
        /// </summary>
        public event Action Changed;

        private void Set<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;
            field = value;
            Changed?.Invoke();
        }

        #endregion

        #region Legacy Properties

        private Anchor _anchor = Anchor.None;
        private PaddingF _margin = PaddingF.Zero;
        private Alignment _alignment = Alignment.None;
        private Vector2F _sizePercent = null;
        private bool _autoUpdate = true;
        private bool _ignoreParentLayout = false;

        /// <summary>
        /// Gets or sets the anchor rule that determines which edges of the element
        /// remain fixed relative to the parent container.
        /// <para>
        /// For example, <see cref="Anchor.TopLeft"/> keeps the element attached to
        /// the parent’s top-left corner even if the parent resizes.
        /// </para>
        /// <para>
        /// Legacy mode only. Anchoring both opposite edges (<see cref="Anchor.StretchX"/> /
        /// <see cref="Anchor.StretchY"/>) stretches the element between them, minus the
        /// margins. The layout-system equivalent is <see cref="PositionMode.Absolute"/> with
        /// insets (see <see cref="ConvertLegacyAnchorToAbsolute"/>).
        /// </para>
        /// </summary>
        public Anchor Anchor { get => _anchor; set => Set(ref _anchor, value); }

        /// <summary>
        /// Gets or sets the margin offset from the parent’s edges, measured in pixels.
        /// <para>
        /// Legacy mode: spacing between the element’s anchored edge and its parent boundaries.
        /// Layout system: space around the element's border box (CSS <c>margin</c>); it is
        /// excluded from the element's size and kept free between it and its neighbours.
        /// </para>
        /// </summary>
        public PaddingF Margin { get => _margin; set => Set(ref _margin, value); }

        /// <summary>
        /// Gets or sets the alignment rule applied when the element is centered within the parent.
        /// <para>
        /// Not read by any layout code: legacy centering is driven by <see cref="Anchor.Center"/>
        /// alone, and the layout system uses the per-axis <see cref="AlignX"/>/<see cref="AlignY"/>
        /// instead of this single value.
        /// </para>
        /// </summary>
        public Alignment Alignment { get => _alignment; set => Set(ref _alignment, value); }

        /// <summary>
        /// Gets or sets the proportional size of the element relative to its parent container.
        /// <para>
        /// The value range is between 0 and 1 (e.g., <c>(1, 0.5)</c> makes the element as wide as
        /// its parent, and half its height).
        /// </para>
        /// <para>
        /// If set to <see langword="null"/>, absolute pixel sizing is used instead.
        /// Legacy mode only; the layout-system equivalent is <see cref="LayoutSize.Percent"/>.
        /// </para>
        /// </summary>
        public Vector2F SizePercent { get => _sizePercent; set => Set(ref _sizePercent, value); }

        /// <summary>
        /// Gets or sets a value indicating whether this layout should automatically
        /// recalculate when the parent container changes size.
        /// <para>
        /// Legacy mode only: when <see langword="false"/>, the parent's one-shot legacy layout
        /// pass does not recurse into this element (it is still laid out on its own first
        /// draw). Layout-managed elements are always re-laid out when invalidated.
        /// </para>
        /// </summary>
        public bool AutoUpdate { get => _autoUpdate; set => Set(ref _autoUpdate, value); }

        /// <summary>
        /// Gets or sets a value indicating whether this layout should ignore
        /// the parent’s layout rules (e.g., for scrollable content or overlay layers).
        /// <para>
        /// When <see langword="true"/>, the parent container will not reposition or resize
        /// this element automatically - neither the legacy anchor rules nor the layout
        /// system (the element is treated as out of layout whatever its <see cref="PositionMode"/>).
        /// </para>
        /// </summary>
        public bool IgnoreParentLayout { get => _ignoreParentLayout; set => Set(ref _ignoreParentLayout, value); }

        #endregion

        #region Positioning

        private PositionMode _positionMode = PositionMode.Legacy;
        private DisplayMode _display = DisplayMode.Normal;
        private float? _left, _top, _right, _bottom;
        private Vector2F _offset = new Vector2F(0, 0);

        /// <summary>
        /// Legacy (default, pre-layout-system behavior), Flow (laid out by the parent) or
        /// Absolute (placed by insets). See <see cref="Constants.Core.PositionMode"/>.
        /// </summary>
        public PositionMode PositionMode { get => _positionMode; set => Set(ref _positionMode, value); }

        /// <summary>
        /// <see cref="DisplayMode.None"/> removes the element from layout, drawing and input
        /// (CSS <c>display: none</c>). Unlike <c>IsVisible = false</c>, which only stops drawing
        /// and input but keeps the element's space in its parent's layout.
        /// </summary>
        public DisplayMode Display { get => _display; set => Set(ref _display, value); }

        /// <summary>Absolute mode: distance from the parent's padding-box left edge (null = unset).</summary>
        public float? Left { get => _left; set => Set(ref _left, value); }

        /// <summary>Absolute mode: distance from the parent's padding-box top edge (null = unset).</summary>
        public float? Top { get => _top; set => Set(ref _top, value); }

        /// <summary>Absolute mode: distance from the parent's padding-box right edge (null = unset).</summary>
        public float? Right { get => _right; set => Set(ref _right, value); }

        /// <summary>Absolute mode: distance from the parent's padding-box bottom edge (null = unset).</summary>
        public float? Bottom { get => _bottom; set => Set(ref _bottom, value); }

        /// <summary>
        /// Visual offset added after layout (CSS <c>position: relative</c> + <c>left/top</c>):
        /// moves the element without affecting its siblings or its parent's size.
        /// Layout-managed elements only.
        /// </summary>
        public Vector2F Offset { get => _offset; set => Set(ref _offset, value ?? new Vector2F(0, 0)); }

        #endregion

        #region Size

        private LayoutSize _width, _height;
        private float? _minWidth, _maxWidth, _minHeight, _maxHeight;
        private float? _aspectRatio;
        private AspectFit _aspectFit = AspectFit.Contain;

        /// <summary>Width mode (border-box: includes padding). Default: the declared <c>Size.X</c>.</summary>
        public LayoutSize Width { get => _width; set => Set(ref _width, value); }

        /// <summary>Height mode (border-box: includes padding). Default: the declared <c>Size.Y</c>.</summary>
        public LayoutSize Height { get => _height; set => Set(ref _height, value); }

        /// <summary>Lower bound on the resolved width (null = none). Wins over <see cref="MaxWidth"/>.</summary>
        public float? MinWidth { get => _minWidth; set => Set(ref _minWidth, value); }

        /// <summary>Upper bound on the resolved width (null = none).</summary>
        public float? MaxWidth { get => _maxWidth; set => Set(ref _maxWidth, value); }

        /// <summary>Lower bound on the resolved height (null = none). Wins over <see cref="MaxHeight"/>.</summary>
        public float? MinHeight { get => _minHeight; set => Set(ref _minHeight, value); }

        /// <summary>Upper bound on the resolved height (null = none).</summary>
        public float? MaxHeight { get => _maxHeight; set => Set(ref _maxHeight, value); }

        /// <summary>
        /// Locks width / height to this ratio (null or &lt;= 0 = unlocked). If exactly one axis
        /// is <see cref="SizeMode.Auto"/>, it is derived from the other; if both axes resolve to
        /// definite sizes, the element is fitted into that box per <see cref="AspectFit"/> and
        /// aligned inside it by <see cref="AlignX"/>/<see cref="AlignY"/>.
        /// </summary>
        public float? AspectRatio { get => _aspectRatio; set => Set(ref _aspectRatio, value); }

        /// <summary>How an aspect-ratio-locked element fits its resolved box.</summary>
        public AspectFit AspectFit { get => _aspectFit; set => Set(ref _aspectFit, value); }

        /// <summary>Sets <see cref="Width"/> and <see cref="Height"/> together.</summary>
        public void SetSize(LayoutSize width, LayoutSize height)
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Alignment

        private Alignment _alignX = Alignment.None;
        private Alignment _alignY = Alignment.None;

        /// <summary>
        /// Horizontal placement when the element is narrower than the space available to it
        /// (non-flex Flow, Absolute without horizontal insets, aspect-ratio fitting).
        /// <see cref="Alignment.None"/> behaves as Start.
        /// </summary>
        public Alignment AlignX { get => _alignX; set => Set(ref _alignX, value); }

        /// <summary>Vertical counterpart of <see cref="AlignX"/>.</summary>
        public Alignment AlignY { get => _alignY; set => Set(ref _alignY, value); }

        #endregion

        #region Container

        private PaddingF _padding = PaddingF.Zero;
        private LayoutContainer _container = LayoutContainer.None;
        private OverflowMode _overflow = OverflowMode.Visible;

        /// <summary>
        /// Space between this element's border box and its content box, where layout-managed
        /// children are placed (Absolute children use the padding box, i.e. ignore it).
        /// </summary>
        public PaddingF Padding { get => _padding; set => Set(ref _padding, value); }

        /// <summary>How in-flow children are arranged: independently (None) or as a flex container.</summary>
        public LayoutContainer Container { get => _container; set => Set(ref _container, value); }

        /// <summary>Whether children are clipped to the content box when drawn and hit-tested.</summary>
        public OverflowMode Overflow { get => _overflow; set => Set(ref _overflow, value); }

        #endregion

        #region Flex Container

        private FlexDirection _flexDirection = FlexDirection.Row;
        private FlexWrap _flexWrap = FlexWrap.NoWrap;
        private JustifyContent _justifyContent = JustifyContent.Start;
        private FlexAlign _alignItems = FlexAlign.Stretch;
        private AlignContent _alignContent = AlignContent.Stretch;
        private float _rowGap, _columnGap;

        /// <summary>Main axis (CSS <c>flex-direction</c>). Default Row.</summary>
        public FlexDirection FlexDirection { get => _flexDirection; set => Set(ref _flexDirection, value); }

        /// <summary>Single line or wrapping (CSS <c>flex-wrap</c>). Default NoWrap.</summary>
        public FlexWrap FlexWrap { get => _flexWrap; set => Set(ref _flexWrap, value); }

        /// <summary>Main-axis free space distribution (CSS <c>justify-content</c>). Default Start.</summary>
        public JustifyContent JustifyContent { get => _justifyContent; set => Set(ref _justifyContent, value); }

        /// <summary>Default cross-axis alignment of items (CSS <c>align-items</c>). Default Stretch.</summary>
        public FlexAlign AlignItems { get => _alignItems; set => Set(ref _alignItems, value); }

        /// <summary>Distribution of wrapped lines (CSS <c>align-content</c>). Default Stretch.</summary>
        public AlignContent AlignContent { get => _alignContent; set => Set(ref _alignContent, value); }

        /// <summary>Vertical gap between rows (CSS <c>row-gap</c>).</summary>
        public float RowGap { get => _rowGap; set => Set(ref _rowGap, value); }

        /// <summary>Horizontal gap between columns (CSS <c>column-gap</c>).</summary>
        public float ColumnGap { get => _columnGap; set => Set(ref _columnGap, value); }

        /// <summary>Sets <see cref="RowGap"/> and <see cref="ColumnGap"/> together (CSS <c>gap</c>).</summary>
        public void SetGap(float gap)
        {
            RowGap = gap;
            ColumnGap = gap;
        }

        #endregion

        #region Flex Item

        private float _flexGrow = 0f;
        private float _flexShrink = 1f;
        private LayoutSize _flexBasis = LayoutSize.Auto;
        private FlexAlign? _alignSelf = null;
        private int _order = 0;

        /// <summary>Share of positive free space this item takes (CSS <c>flex-grow</c>). Default 0.</summary>
        public float FlexGrow { get => _flexGrow; set => Set(ref _flexGrow, value); }

        /// <summary>
        /// Shrink factor when items overflow the line (CSS <c>flex-shrink</c>, weighted by the
        /// item's base size). Default 1, as in CSS.
        /// </summary>
        public float FlexShrink { get => _flexShrink; set => Set(ref _flexShrink, value); }

        /// <summary>
        /// Initial main size before growing/shrinking (CSS <c>flex-basis</c>).
        /// <see cref="SizeMode.Auto"/> (default) uses the main-axis <see cref="Width"/>/<see cref="Height"/>.
        /// </summary>
        public LayoutSize FlexBasis { get => _flexBasis; set => Set(ref _flexBasis, value); }

        /// <summary>Overrides the container's <see cref="AlignItems"/> for this item (null = inherit).</summary>
        public FlexAlign? AlignSelf { get => _alignSelf; set => Set(ref _alignSelf, value); }

        /// <summary>Layout order among siblings (CSS <c>order</c>); ties keep child order. Does not affect drawing.</summary>
        public int Order { get => _order; set => Set(ref _order, value); }

        #endregion

        #region Styles

        /// <summary>
        /// Applies a declarative <see cref="UILayoutStyle"/>: every setting the style declares
        /// (non-null) is assigned to the property of the same name; everything else is left
        /// as it is. Vectors are copied, so elements sharing one style never share a mutable
        /// vector.
        /// </summary>
        public void Apply(UILayoutStyle style)
        {
            if (style == null)
                throw new ArgumentNullException(nameof(style));

            // Legacy
            if (style.Anchor is Anchor anchor) Anchor = anchor;
            if (style.Alignment is Alignment alignment) Alignment = alignment;
            if (style.SizePercent is Vector2F sizePercent) SizePercent = new Vector2F(sizePercent.X, sizePercent.Y);
            if (style.AutoUpdate is bool autoUpdate) AutoUpdate = autoUpdate;
            if (style.IgnoreParentLayout is bool ignoreParentLayout) IgnoreParentLayout = ignoreParentLayout;

            // Positioning
            if (style.PositionMode is PositionMode positionMode) PositionMode = positionMode;
            if (style.Display is DisplayMode display) Display = display;
            if (style.Left.HasValue) Left = style.Left;
            if (style.Top.HasValue) Top = style.Top;
            if (style.Right.HasValue) Right = style.Right;
            if (style.Bottom.HasValue) Bottom = style.Bottom;
            if (style.Offset is Vector2F offset) Offset = new Vector2F(offset.X, offset.Y);

            // Size
            if (style.Width is LayoutSize width) Width = width;
            if (style.Height is LayoutSize height) Height = height;
            if (style.MinWidth.HasValue) MinWidth = style.MinWidth;
            if (style.MaxWidth.HasValue) MaxWidth = style.MaxWidth;
            if (style.MinHeight.HasValue) MinHeight = style.MinHeight;
            if (style.MaxHeight.HasValue) MaxHeight = style.MaxHeight;
            if (style.AspectRatio.HasValue) AspectRatio = style.AspectRatio;
            if (style.AspectFit is AspectFit aspectFit) AspectFit = aspectFit;

            // Alignment and spacing
            if (style.AlignX is Alignment alignX) AlignX = alignX;
            if (style.AlignY is Alignment alignY) AlignY = alignY;
            if (style.Margin is PaddingF margin) Margin = margin;
            if (style.Padding is PaddingF padding) Padding = padding;

            // Container
            if (style.Container is LayoutContainer container) Container = container;
            if (style.Overflow is OverflowMode overflow) Overflow = overflow;
            if (style.FlexDirection is FlexDirection flexDirection) FlexDirection = flexDirection;
            if (style.FlexWrap is FlexWrap flexWrap) FlexWrap = flexWrap;
            if (style.JustifyContent is JustifyContent justifyContent) JustifyContent = justifyContent;
            if (style.AlignItems is FlexAlign alignItems) AlignItems = alignItems;
            if (style.AlignContent is AlignContent alignContent) AlignContent = alignContent;
            if (style.RowGap is float rowGap) RowGap = rowGap;
            if (style.ColumnGap is float columnGap) ColumnGap = columnGap;

            // Flex item
            if (style.FlexGrow is float flexGrow) FlexGrow = flexGrow;
            if (style.FlexShrink is float flexShrink) FlexShrink = flexShrink;
            if (style.FlexBasis is LayoutSize flexBasis) FlexBasis = flexBasis;
            if (style.AlignSelf.HasValue) AlignSelf = style.AlignSelf;
            if (style.Order is int order) Order = order;
        }

        #endregion

        #region Migration Helpers

        /// <summary>
        /// Translates this element's legacy <see cref="Anchor"/>/<see cref="Margin"/>/<see cref="SizePercent"/>
        /// rules into their layout-system equivalents and switches to
        /// <see cref="Constants.Core.PositionMode.Absolute"/> (for migrating existing layouts):
        /// Left/Top/Right/Bottom anchors become 0 insets (the margin still applies on top),
        /// opposite anchors stretch, CenterX/CenterY become centered alignment, and
        /// <see cref="SizePercent"/> becomes <see cref="LayoutSize.Percent"/>.
        /// </summary>
        /// <remarks>
        /// One difference: legacy centering ignores margins, the layout system centers inside
        /// the space left after both margins (identical when margins are zero or symmetric).
        /// </remarks>
        public void ConvertLegacyAnchorToAbsolute()
        {
            // Same precedence as the legacy pass: centering overrides edge anchors.
            bool centerX = _anchor.HasFlag(Anchor.CenterX);
            bool centerY = _anchor.HasFlag(Anchor.CenterY);

            Left = !centerX && _anchor.HasFlag(Anchor.Left) ? 0f : null;
            Right = !centerX && _anchor.HasFlag(Anchor.Right) ? 0f : null;
            Top = !centerY && _anchor.HasFlag(Anchor.Top) ? 0f : null;
            Bottom = !centerY && _anchor.HasFlag(Anchor.Bottom) ? 0f : null;

            if (centerX)
                AlignX = Alignment.Center;
            if (centerY)
                AlignY = Alignment.Center;

            if (_sizePercent != null)
            {
                Width = LayoutSize.Percent(_sizePercent.X);
                Height = LayoutSize.Percent(_sizePercent.Y);
            }

            PositionMode = PositionMode.Absolute;
        }

        #endregion
    }
}
