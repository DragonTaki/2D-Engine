/* ----- ----- ----- ----- */
// UIElementBase.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2026/09/30
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using Engine.Geometry;
using Engine.Mathematics;
using Engine.Physics;
using Engine.Platform;
using Engine.UI.Constants.Components;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Interfaces;
using Engine.UI.Layout;
using Engine.UI.Models;

namespace Engine.UI.Core.Bases
{
    /// <summary>
    /// Base abstract class for all UI elements.
    /// Provides hierarchy management, layout computation, interaction, rendering, and physics integration.
    /// </summary>
    public abstract class UIElementBase : IDisposable
    {
        protected UIElementBase()
        {
            LayoutRules = new UILayout();
            _layoutDirty = true;
        }

        #region Core References

        /// <summary>
        /// Reference to the element's handler (non-generic version).
        /// Used to access logic and data binding operations.
        /// </summary>
        public UIHandlerBase HandlerBase { get; protected set; }

        /// <summary>
        /// Reference to the element's renderer (non-generic version).
        /// Used for drawing this UI element on screen.
        /// </summary>
        public UIRendererBase RendererBase { get; protected set; }

        /// <summary>
        /// Factory reference used for dependency injection and UI object creation.
        /// </summary>
        protected IUiFactory _factory;

        #endregion

        #region Identity

        /// <summary>
        /// Static counter used to assign unique instance IDs.
        /// </summary>
        protected static long s_nextId = 0;

        /// <summary>
        /// Unique ID assigned to this UI element instance.
        /// Useful for debugging, tracking, or caching.
        /// </summary>
        public long InstanceId { get; protected set; }

        /// <summary>
        /// Optional element classification type.
        /// Used to differentiate between buttons, labels, panels, etc.
        /// </summary>
        public UIElementType ElementType { get; set; } = UIElementType.Generic;

        /// <summary>
        /// Indicates whether this element should persist when the parent clears its children.
        /// </summary>
        public bool IsPersistent { get; set; } = false;

        /// <summary>
        /// Indicates whether this element has completed initialization.
        /// </summary>
        public bool IsInitialized { get; protected set; }

        #endregion

        #region Hierarchy

#nullable enable
        /// <summary>
        /// Reference to the parent UI element.
        /// Null if this element is the root of the hierarchy.
        /// </summary>
        public UIElementBase? Parent { get; set; }
#nullable disable

        /// <summary>
        /// Collection of child UI elements contained within this element.
        /// </summary>
        public List<UIElementBase> Children { get; } = new();

        /// <summary>
        /// Indicates whether the child order needs to be re-sorted.
        /// </summary>
        protected bool _isChildrenSortedDirty = true;

        /// <summary>
        /// Cached ascending Z-order child list.
        /// </summary>
        protected List<UIElementBase> _sortedChildrenAsc;

        /// <summary>
        /// Cached descending Z-order child list.
        /// </summary>
        protected List<UIElementBase> _sortedChildrenDesc;

        #endregion

        #region Layout & Geometry

        private UIPosition _localPosition = new UIPosition(Vector2F.Zero);
        private Vector2F _size = Vector2F.Zero;
        private Vector2F _declaredSize = Vector2F.Zero;
        private UILayout _layoutRules;

        /// <summary>
        /// True while the layout system writes its result into <see cref="LocalPosition"/>/
        /// <see cref="Size"/>, so those writes neither count as the declared size nor
        /// re-invalidate the parent that is doing the arranging.
        /// </summary>
        private bool _applyingLayoutResult;

        /// <summary>
        /// The element's position relative to its parent container.
        /// <para>
        /// Assigning a different position invalidates the layout (see <see cref="InvalidateLayout"/>).
        /// For a layout-managed element the position belongs to the layout system and is
        /// overwritten by the next pass - use <see cref="UILayout.Offset"/> to nudge it.
        /// Mutating the vectors in place does not invalidate anything.
        /// </para>
        /// </summary>
        public virtual UIPosition LocalPosition
        {
            get => _localPosition;
            set
            {
                bool changed = !SamePosition(_localPosition, value);
                _localPosition = value;
                if (changed)
                    OnGeometryAssigned();
            }
        }

        /// <summary>
        /// The element's visual size (Width, Height).
        /// <para>
        /// Assigning a different size invalidates the layout. Outside of a layout pass the
        /// value also becomes the <see cref="DeclaredSize"/>. Mutating the vector in place
        /// does not invalidate anything.
        /// </para>
        /// <para>
        /// The assigned vector is copied (like <see cref="UIPosition"/> copies its vectors):
        /// <see cref="Vector2F"/> is a mutable class and sizes are often assigned from shared
        /// static defaults (e.g. <c>Size = ButtonDefaults.Size</c>), so storing the instance
        /// would let an in-place write to one element's size change the default and every
        /// other element sharing it.
        /// </para>
        /// </summary>
        public virtual Vector2F Size
        {
            get => _size;
            set
            {
                bool changed = !SameVector(_size, value);
                _size = value is null ? null : new Vector2F(value.X, value.Y);
                if (!_applyingLayoutResult)
                    _declaredSize = _size;
                if (changed)
                    OnGeometryAssigned();
            }
        }

        /// <summary>
        /// The size last assigned to <see cref="Size"/> by code other than the layout system -
        /// what <see cref="LayoutSize.Declared"/> (the default Width/Height mode) resolves to,
        /// and the default intrinsic size of an element without in-flow children. Kept apart
        /// from <see cref="Size"/> so grow/shrink results never feed back into the next pass.
        /// </summary>
        public Vector2F DeclaredSize => _declaredSize;

        /// <summary>
        /// Defines layout constraints and rules for automatic positioning or anchoring.
        /// Changing any of its properties (or replacing it) invalidates the layout.
        /// </summary>
        public UILayout LayoutRules
        {
            get => _layoutRules;
            set
            {
                if (ReferenceEquals(_layoutRules, value))
                    return;
                if (_layoutRules != null)
                    _layoutRules.Changed -= InvalidateLayout;
                _layoutRules = value ?? new UILayout();
                _layoutRules.Changed += InvalidateLayout;
                InvalidateLayout();
            }
        }

        /// <summary>
        /// Cached final layout information representing absolute position and size.
        /// </summary>
        public LayoutF Bounds { get; protected set; } = LayoutF.Zero;

        /// <summary>
        /// Gets or sets the element’s current layout (position + size).
        /// Setting this value updates both LocalPosition and Size.
        /// </summary>
        public virtual LayoutF Layout
        {
            get => new LayoutF(LocalPosition.Current, Size);
            set
            {
                LocalPosition = new UIPosition(value.Position);
                Size = value.Size;
            }
        }

        /// <summary>
        /// Optional rectangular clipping region for rendering.
        /// Null means no clipping.
        /// </summary>
        public RectangleF? ClipRect { get; set; } = null;

        /// <summary>
        /// Indicates whether the layout must be recalculated: this element's layout-managed
        /// children need to be (re)arranged. Set by <see cref="InvalidateLayout"/>, cleared by
        /// the layout pass (<c>UIElement.UpdateLayout</c>, run before drawing).
        /// </summary>
        protected bool _layoutDirty = true;

        /// <summary>
        /// Public property exposing whether layout recomputation is required.
        /// </summary>
        public bool LayoutDirty
        {
            get => _layoutDirty;
            protected set => _layoutDirty = value;
        }

        /// <summary>
        /// Whether the one-shot legacy layout rules (<see cref="UILayout.Anchor"/>,
        /// <see cref="UILayout.SizePercent"/>) are still to be applied. They run once, on the
        /// element's first layout pass, exactly as before the layout system existed; later
        /// invalidations do not re-run them, so legacy elements keep their positions.
        /// </summary>
        public bool LegacyLayoutPending { get; protected set; } = true;

        /// <summary>
        /// Whether the parent's layout system positions and sizes this element
        /// (<see cref="PositionMode.Flow"/> or <see cref="PositionMode.Absolute"/>, and not
        /// <see cref="UILayout.IgnoreParentLayout"/>).
        /// </summary>
        public bool IsLayoutManaged =>
            LayoutRules.PositionMode != PositionMode.Legacy && !LayoutRules.IgnoreParentLayout;

        /// <summary>
        /// False when <see cref="UILayout.Display"/> is <see cref="DisplayMode.None"/>: the element
        /// takes no space, is not drawn and receives no input. Unlike <see cref="IsVisible"/>,
        /// which keeps the element's space in layout.
        /// </summary>
        public bool IsDisplayed => LayoutRules.Display != DisplayMode.None;

        #endregion

        #region Z-Order and Sorting

        protected int _zIndex = 0;

        /// <summary>
        /// Determines drawing and input order within parent.
        /// Higher values appear on top.
        /// </summary>
        public int ZIndex
        {
            get => _zIndex;
            set
            {
                if (_zIndex != value)
                {
                    _zIndex = value;
                    Parent?.NotifyChildOrderChanged();
                }
            }
        }

        #endregion

        #region Visibility / Interaction

        /// <summary>
        /// Determines whether the element is visible.
        /// Invisible elements are not drawn.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Whether the element may be used or interacted with (e.g. a button the game disabled).
        /// Only the owner sets it; scroll culling uses <see cref="IsClipped"/> instead.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// True while the element lies entirely outside its scroll viewport (set by
        /// <see cref="Engine.UI.Utils.UIElementUtils.UpdateVisibleState"/>): it is not drawn by its
        /// menu and receives no input, without touching <see cref="IsEnabled"/>.
        /// </summary>
        public bool IsClipped { get; set; } = false;

        /// <summary>
        /// Indicates if this element can receive interaction: visible, displayed, enabled and not clipped.
        /// </summary>
        public virtual bool IsInteractable => IsVisible && IsEnabled && IsDisplayed && !IsClipped;

        /// <summary>
        /// Allows hit testing even if invisible. Usually false.
        /// </summary>
        public virtual bool AllowHitWhenInvisible => false;

        /// <summary>
        /// Determines whether rendering should be skipped when invisible (or not displayed).
        /// </summary>
        public virtual bool DisableRender => !IsVisible || !IsDisplayed;

        #endregion

        #region Physics

#nullable enable
        /// <summary>
        /// Optional reference to a physics controller that affects position or velocity.
        /// Used primarily for animated UI or scroll containers.
        /// </summary>
        public virtual Physics2D? Physics { get; set; }
#nullable disable

        #endregion

        #region Lifecycle & Resource Management

        /// <summary>
        /// Indicates whether the element has been disposed.
        /// Prevents duplicate disposal operations.
        /// </summary>
        protected bool _disposed = false;

        /// <summary>
        /// Public property exposing whether the element has been disposed.
        /// </summary>
        public bool IsDisposed => _disposed;

        /// <summary>
        /// Releases all resources and detaches references held by this element.
        /// </summary>
        public abstract void Dispose();

        #endregion

        #region Hierarchy Management Methods

        /// <summary>
        /// Adds a child element to this container.
        /// </summary>
        /// <param name="child">The UI element to add.</param>
        public abstract void AddChild(UIElementBase child);

        /// <summary>
        /// Invoked after being added to a parent container.
        /// Used for initialization logic or dependency binding.
        /// </summary>
        public abstract void OnAddedToParent();

        /// <summary>
        /// Removes a child element from this container.
        /// </summary>
        /// <param name="child">The child element to remove.</param>
        public abstract void RemoveChild(UIElementBase child);

        /// <summary>
        /// Notifies this element that a child's Z-order has changed,
        /// prompting a resort of the child list.
        /// </summary>
        public abstract void NotifyChildOrderChanged();

        /// <summary>
        /// Returns the top-most root element in the hierarchy.
        /// </summary>
        /// <returns>The root UI element.</returns>
        public abstract UIElementBase GetRoot();

#nullable enable
        /// <summary>
        /// Performs deep hit testing, returning the most deeply nested UI element at the given point.
        /// </summary>
        /// <param name="point">Screen-space coordinates of the hit test.</param>
        /// <param name="isRootCall">True if this is the initial hit test call (used internally).</param>
        /// <returns>The deepest UI element hit, or null if none.</returns>
        public abstract UIElementBase? HitTestDeep(PointF point, bool isRootCall = true);
#nullable disable

        #endregion

        #region Layout & Update Methods

        /// <summary>
        /// Computes and updates layout positions based on parent layout and rules.
        /// </summary>
        public abstract void UpdateLayout();

        /// <summary>
        /// Marks this element's layout as needing recomputation before the next draw.
        /// <para>
        /// Called automatically when <see cref="Size"/>, <see cref="LocalPosition"/> or
        /// <see cref="LayoutRules"/> change and when children are added or removed. For a
        /// layout-managed element the parent is invalidated too (its arrangement, and possibly
        /// its Auto size, depend on this element), recursively up to the nearest non-managed
        /// ancestor, whose draw then runs the pass for the whole affected subtree.
        /// </para>
        /// </summary>
        public void InvalidateLayout()
        {
            _layoutDirty = true;
            if (Parent != null && (IsLayoutManaged || Parent.TracksChildGeometry))
                Parent.InvalidateLayout();
        }

        /// <summary>
        /// True for containers whose own state depends on where their children are, even
        /// legacy (non-managed) ones - e.g. <c>UIScrollContainer</c>'s automatic content size.
        /// A child of such a container invalidates it on every geometry change.
        /// </summary>
        protected internal virtual bool TracksChildGeometry => false;

        /// <summary>
        /// Invalidates this element and its whole subtree, e.g. when the viewport scale
        /// changes (pixel snapping depends on it). Legacy positions are not recomputed.
        /// </summary>
        public void InvalidateLayoutRecursive()
        {
            _layoutDirty = true;
            foreach (var child in Children.ToArray())
                child.InvalidateLayoutRecursive();
        }

        /// <summary>
        /// Returns the element's intrinsic (content) border-box size, used when a Width or
        /// Height is <see cref="SizeMode.Auto"/>.
        /// <para>
        /// The default measures the in-flow children (flex or not) plus padding; an element
        /// without in-flow children reports its <see cref="DeclaredSize"/>. Leaf elements with
        /// real content (e.g. <c>UILabel</c>) override this.
        /// </para>
        /// </summary>
        /// <param name="available">
        /// Border-box space available on each axis; <see cref="float.PositiveInfinity"/> when
        /// unconstrained (e.g. max-content width). Text should wrap to a finite width.
        /// </param>
        public virtual Vector2F MeasureIntrinsicSize(Vector2F available) =>
            LayoutEngine.MeasureContent(this, available);

        /// <summary>
        /// Called on every descendant of an element that the layout system just moved (the
        /// descendant's own LocalPosition is unchanged but its absolute position is not).
        /// Elements that cache absolute positions (e.g. scroll physics) resync here.
        /// </summary>
        protected internal virtual void OnAbsolutePositionChanged() { }

        /// <summary>
        /// Writes a layout pass result without treating it as a user assignment: the size
        /// does not become the <see cref="DeclaredSize"/> and the parent (which is doing the
        /// arranging) is not re-invalidated. Marks this element dirty so its own children
        /// are (re)arranged, and notifies descendants when it moved.
        /// </summary>
        /// <returns>True when the position or size actually changed.</returns>
        internal bool ApplyLayoutResult(Vector2F position, Vector2F size)
        {
            bool moved = !SameVector(LocalPosition.Current, position) || !SameVector(LocalPosition.Base, position);
            bool resized = !SameVector(Size, size);
            if (!moved && !resized)
                return false;

            _applyingLayoutResult = true;
            try
            {
                // Through the (virtual) setters, so overrides - UIScrollContainer rebasing its
                // physics - still see the change.
                if (moved)
                    LocalPosition = new UIPosition(position);
                if (resized)
                    Size = new Vector2F(size.X, size.Y);
            }
            finally
            {
                _applyingLayoutResult = false;
            }

            _layoutDirty = true;
            if (moved)
                NotifyDescendantsMoved(this);
            return true;
        }

        /// <summary>
        /// Calls <see cref="OnAbsolutePositionChanged"/> on every descendant of
        /// <paramref name="element"/> (their absolute positions changed with it).
        /// </summary>
        protected static void NotifyDescendantsMoved(UIElementBase element)
        {
            foreach (var child in element.Children.ToArray())
            {
                child.OnAbsolutePositionChanged();
                NotifyDescendantsMoved(child);
            }
        }

        /// <summary>
        /// A user (non-layout) assignment changed Size or LocalPosition: invalidate normally.
        /// A layout-result write only marks this element (its children need re-arranging),
        /// never the parent.
        /// </summary>
        private void OnGeometryAssigned()
        {
            if (_applyingLayoutResult)
                _layoutDirty = true;
            else
                InvalidateLayout();
        }

        private static bool SameVector(Vector2F a, Vector2F b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a is null || b is null)
                return false;
            return a.X == b.X && a.Y == b.Y;
        }

        private static bool SamePosition(UIPosition a, UIPosition b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a is null || b is null)
                return false;
            return SameVector(a.Base, b.Base) && SameVector(a.Current, b.Current);
        }

        /// <summary>
        /// Returns the current absolute bounds (in screen coordinates) of the element.
        /// </summary>
        /// <returns>The current layout in absolute coordinates.</returns>
        public abstract LayoutF GetCurrentAbsoluteBounds();

        /// <summary>
        /// Updates logic or animations for this element (called every frame).
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Asks for the element to be repainted (the root forwards it to the window).
        /// </summary>
        public abstract void RequestRedraw();

        /// <summary>
        /// Called once per frame to perform cleanup or state reset.
        /// </summary>
        public abstract void EndFrame();

        /// <summary>
        /// Resets this element’s state to its initial configuration.
        /// Often used when restarting or clearing UI.
        /// </summary>
        public abstract void Reset();

        #endregion

        #region Interaction Methods

        /// <summary>
        /// Handles mouse button press events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled.</returns>
        public abstract bool OnMouseDown(IMouseEvent e);

        /// <summary>
        /// Handles mouse move events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled.</returns>
        public abstract bool OnMouseMove(IMouseEvent e);

        /// <summary>
        /// Handles mouse button release events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled.</returns>
        public abstract bool OnMouseUp(IMouseEvent e);

        /// <summary>
        /// Handles mouse wheel scrolling events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled.</returns>
        public abstract bool OnMouseWheel(IMouseEvent e);

        /// <summary>
        /// Handles mouse click events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled.</returns>
        public abstract bool OnMouseClick(IMouseEvent e);

        #endregion

        #region Rendering

        /// <summary>
        /// Renders the visual representation of this element and its children.
        /// </summary>
        /// <param name="g">Graphics context used for drawing.</param>
        public abstract void Draw(IGraphics g);

        #endregion
    }
}
