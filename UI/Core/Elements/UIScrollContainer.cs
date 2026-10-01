/* ----- ----- ----- ----- */
// UIScrollContainer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2026/09/30
// Version: v1.3
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.Mathematics;
using Engine.Physics;
using Engine.UI.Constants.Components;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;
using Engine.UI.Input;
using Engine.UI.Models;
using static Engine.UI.Input.ScrollInputHandler;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Defines vertical alignment behavior for scroll content.
    /// </summary>
    public enum ScrollAlignment
    {
        Top,
        Bottom
    }

    /// <summary>
    /// Provides a reusable vertical scroll container that supports dragging, scrolling, inertia, and edge elasticity.
    /// </summary>
    public class UIScrollContainer : UIContainer<UIScrollContainer, UIScrollContainerHandler, UIScrollContainerRenderer>, IPhysical2D
    {
        public UIScrollContainerHandler ScrollHandler => (UIScrollContainerHandler)Handler;
        private bool _pendingApplyAlignment = false;

        #region Fields

        /// <summary>
        /// Internal physics instance to handle scrolling movement.
        /// </summary>
        private readonly Physics2D _physics = new Physics2D();

        /// <summary>
        /// Input handler for mouse/touch scroll events.
        /// </summary>
        private readonly IScrollInputHandler _inputHandler;
        public IScrollInputHandler InputHandler => _inputHandler;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying Physics2D instance.
        /// </summary>
        Physics2D IPhysical2D.Physics => _physics;

#nullable enable
        /// <summary>
        /// Gets the Physics2D instance for UIElement.
        /// </summary>
        public override Physics2D? Physics => _physics;
#nullable disable

        /// <summary>
        /// Gets the current position object from Physics2D.
        /// </summary>
        public Position Position => _physics.Position;

        /// <summary>
        /// Gets the current velocity object from Physics2D.
        /// </summary>
        public Velocity Velocity => _physics.Velocity;

        /// <summary>
        /// Gets the current acceleration object from Physics2D.
        /// </summary>
        public Acceleration Acceleration => _physics.Acceleration;

        /// <summary>
        /// Returns the viewport rectangle in absolute coordinates.
        /// </summary>
        public RectangleF AbsViewportBounds
        {
            get
            {
                var position = Physics?.Position.Base ?? Vector2F.Zero;

                return new RectangleF(position.ToPointF(), Size.ToSizeF());
            }
        }

        /// <summary>
        /// Total content height of the scrollable area.
        /// <para>
        /// Computed automatically by default (<see cref="AutoContentSize"/>, see
        /// <see cref="RefreshContentSize"/>): the lowest bottom edge among the displayed
        /// children, recomputed by the layout pass whenever children are added, removed,
        /// moved or resized. Nobody has to set it.
        /// </para>
        /// <para>
        /// Assigning it is the <b>manual override</b> for special cases (content that isn't
        /// made of child elements, extra trailing space, ...): it turns
        /// <see cref="AutoContentSize"/> off, so the assigned value stays until auto sizing is
        /// switched back on. Either way a new value re-applies the scroll alignment.
        /// </para>
        /// </summary>
        private float _contentHeight;
        public float ContentHeight
        {
            get => _contentHeight;
            set
            {
                AutoContentSize = false;
                SetContentHeight(value);
            }
        }

        /// <summary>
        /// The one path every content-height change goes through (automatic or manual):
        /// store it and re-apply the scroll alignment, which OverContent, MinNormalScrollY,
        /// ClampToOverscrollRange and the inertia/rebound logic all read from.
        /// </summary>
        private void SetContentHeight(float value)
        {
            _contentHeight = value;
            if (ScrollHandler != null)
                ScrollHandler.ApplyAlignment();
            else
                _pendingApplyAlignment = true;
        }

        private bool _autoContentSize = true;

        /// <summary>
        /// Whether <see cref="ContentHeight"/>/<see cref="ContentWidth"/> follow the children
        /// automatically (default <see langword="true"/>, like CSS overflow or Unity's
        /// ScrollRect + ContentSizeFitter). Assigning <see cref="ContentHeight"/> sets this to
        /// <see langword="false"/>; setting it back to <see langword="true"/> recomputes at once.
        /// </summary>
        public bool AutoContentSize
        {
            get => _autoContentSize;
            set
            {
                if (_autoContentSize == value)
                    return;
                _autoContentSize = value;
                if (value)
                    RefreshContentSize(forceAlignment: true);
            }
        }

        /// <summary>
        /// Horizontal content extent (rightmost right edge among the displayed children, plus
        /// padding), maintained alongside <see cref="ContentHeight"/> when
        /// <see cref="AutoContentSize"/> is on. Informational: the container scrolls vertically only.
        /// </summary>
        public float ContentWidth { get; private set; }

        /// <summary>
        /// Recomputes the automatic content size from the children's current rectangles:
        /// the lowest bottom edge (and rightmost right edge) among displayed Legacy and Flow
        /// children - for a flex container that is where the flex solver put its items -
        /// including their bottom/right margins and this container's bottom/right padding.
        /// Absolute children are overlays and don't count (as in CSS).
        /// <para>
        /// Runs automatically after every layout pass of this container; call it directly to
        /// get the value right away (e.g. after appending children in the same frame).
        /// No-op while <see cref="AutoContentSize"/> is off.
        /// </para>
        /// </summary>
        /// <param name="forceAlignment">
        /// Re-apply the scroll alignment even if the height didn't change. By default only a
        /// real change (more than a design unit) does, so e.g. a window resize, which only
        /// re-snaps the children, doesn't jump the scroll position.
        /// </param>
        public void RefreshContentSize(bool forceAlignment = false)
        {
            if (!_autoContentSize)
                return;

            float bottom = 0f, right = 0f;
            foreach (var child in Children)
            {
                if (!child.IsDisplayed || (child.IsLayoutManaged && child.LayoutRules.PositionMode == PositionMode.Absolute))
                    continue;

                var margin = child.IsLayoutManaged ? child.LayoutRules.Margin : Constants.Core.PaddingF.Zero;
                var position = child.LocalPosition.Current;
                bottom = Math.Max(bottom, position.Y + child.Size.Y + margin.Bottom);
                right = Math.Max(right, position.X + child.Size.X + margin.Right);
            }

            bool hasChildren = bottom > 0f || right > 0f;
            var padding = LayoutRules.Padding;
            float height = hasChildren ? bottom + padding.Bottom : 0f;
            ContentWidth = hasChildren ? right + padding.Right : 0f;

            // Sub-unit changes (pixel snapping re-rounding the children after a window
            // resize) update the value without re-aligning, so the scroll position is kept.
            if (forceAlignment || Math.Abs(height - _contentHeight) > ContentChangeTolerance)
                SetContentHeight(height);
            else
                _contentHeight = height;
        }

        /// <summary>
        /// Content-height changes up to this size (design units) are treated as re-rounding,
        /// not as new content: they don't re-apply the scroll alignment.
        /// </summary>
        private const float ContentChangeTolerance = 1f;

        /// <summary>The automatic content size depends on every child's rectangle.</summary>
        protected internal override bool TracksChildGeometry => _autoContentSize;

        /// <summary>After each layout pass: keep the automatic content size current.</summary>
        protected override void OnChildrenArranged() => RefreshContentSize();

        /// <summary>
        /// Maximum overscroll allowed at edges.
        /// </summary>
        /// <remarks>
        /// Not what bounds overscroll: by design the content can be pulled until its far
        /// edge meets the opposite viewport edge (see <see cref="ClampToOverscrollRange"/>).
        /// </remarks>
        public float OverscrollLimit { get; set; } = 40.0f;

        /// <summary>
        /// Scroll offset at which content bottom meets the viewport bottom (0 if it fits).
        /// ScrollY between this and 0 is the normal, non-overscrolled range.
        /// </summary>
        public float MinNormalScrollY => OverContent ? -(ContentHeight - Size.Y) : 0f;

        /// <summary>
        /// Keeps ScrollY within the allowed overscroll range: pulling up stops when the
        /// content's bottom edge reaches the viewport top, pulling down stops when the
        /// content's top edge reaches the viewport bottom.
        /// </summary>
        public void ClampToOverscrollRange()
        {
            float min = -ContentHeight;
            float max = Size.Y;
            if (ScrollY < min)
                ScrollY = min;
            else if (ScrollY > max)
                ScrollY = max;
        }

        /// <summary>
        /// Defines how content is aligned vertically when initialized or refreshed.
        /// </summary>
        public ScrollAlignment VerticalAlignment { get; set; } = ScrollAlignment.Top;

        /// <summary>
        /// Base horizontal scroll offset.
        /// </summary>
        public float BaseScrollX { get; set; } = 0.0f;

        /// <summary>
        /// Base vertical scroll offset.
        /// </summary>
        public float BaseScrollY { get; set; } = 0.0f;

        /// <summary>
        /// Current vertical scroll offset relative to base position.
        /// </summary>
        public float ScrollY
        {
            get => Physics.Position.Current.Y - Physics.Position.Base.Y;
            set
            {
                Physics.Position.Current = new Vector2F(
                    Physics.Position.Current.X,  // X axis unchanged
                    Physics.Position.Base.Y + value
                );
            }
        }

        /// <summary>
        /// Current vertical scroll velocity relative to base velocity.
        /// </summary>
        public float ScrollVelocity
        {
            get => Physics.Velocity.Current.Y - Physics.Velocity.Base.Y;
            set
            {
                Physics.Velocity.Current = new Vector2F(
                    Physics.Velocity.Current.X,  // X axis unchanged
                    Physics.Velocity.Base.Y + value
                );
            }
        }

        /// <summary>
        /// Local position override that updates physics base position.
        /// </summary>
        public override UIPosition LocalPosition
        {
            get => base.LocalPosition;
            set
            {
                base.LocalPosition = value;
                RebasePhysics();
            }
        }

        /// <summary>
        /// Keeps the physics position in sync when this container's absolute position changes
        /// (LocalPosition set, attached to a parent). Moves Base to the new absolute position
        /// and shifts Current/Target by the same delta, so the scroll offset (Current - Base)
        /// is preserved. Replacing the whole Position here used to zero the offset (e.g. the
        /// first layout pass undid alignment), and OnAddedToParent only moved Current, leaving
        /// Base - which AbsViewportBounds and ScrollY use - at the pre-attach position.
        /// </summary>
        private void RebasePhysics()
        {
            if (Physics == null)
                return;

            var position = Physics.Position;
            var absPos = GetCurrentAbsolutePosition();
            var delta = absPos - position.Base;

            position.Base = new Vector2F(absPos.X, absPos.Y);
            position.Current = position.Current + delta;
            position.Target = position.Target + delta;
        }

        public override void OnAddedToParent() => RebasePhysics();

        /// <summary>
        /// The layout system moved an ancestor: this container's absolute position changed
        /// without its LocalPosition setter running, so rebase here too.
        /// </summary>
        protected internal override void OnAbsolutePositionChanged() => RebasePhysics();

        /// <summary>
        /// Undoes the constructor's registration with the shared scroll input handler, so a
        /// disposed container isn't kept alive, hit-tested or scrolled by it.
        /// </summary>
        protected override void DisposeUI()
        {
            base.DisposeUI();
            _inputHandler.UnregisterScrollTarget(this);
        }

        /// <summary>
        /// Container size override.
        /// </summary>
        public override Vector2F Size
        {
            get => base.Size;
            set
            {
                base.Size = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="UIScrollContainer"/>.
        /// </summary>
        /// <param name="scroll">The scroll input handler to handle drag and wheel events.</param>
        public UIScrollContainer(IScrollInputHandler scroll)
            : base(zIndex: 0, isPersistent: false, type: UIElementType.ScrollContainer)
        {
            _inputHandler = scroll;
            // Register and bind
            scroll.RegisterScrollTarget(this, Physics, () => this.AbsViewportBounds, new ScrollBehavior
            {
                AllowDragY = true,
                AllowDragX = false,
                AllowWheel = true,
                // Handler is bound after construction, hence the lazy lookups.
                OnRelease = velocity => ScrollHandler?.StartInertia(velocity.Y),
                OnPress = () => ScrollHandler?.StopInertia()
            });

            Physics.Movement.CanSpring = true;
            Physics.Movement.CanDamping = true;
        }

        #endregion

        protected override void OnInit()
        {
            base.OnInit();

            if (_pendingApplyAlignment)
            {
                ScrollHandler.ApplyAlignment();
                _pendingApplyAlignment = false;
            }
        }

        #region Public Methods

        /// <summary>
        /// Returns the visible area in absolute coordinates for clipping content.
        /// </summary>
        /// <returns>The absolute viewport rectangle.</returns>
        public RectangleF GetAbsClippingRect()
        {
            return AbsViewportBounds;
        }

        /// <summary>
        /// Returns the visual offset of the content relative to viewport.
        /// </summary>
        /// <returns>Vertical offset of content.</returns>
        public float GetContentOffsetY()
        {
            return -ScrollY;
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Checks whether the content exceeds viewport height.
        /// </summary>
        public bool OverContent => ContentHeight > AbsViewportBounds.Height;

        #endregion
    }
}
