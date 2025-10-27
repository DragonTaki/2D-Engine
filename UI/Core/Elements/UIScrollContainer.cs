/* ----- ----- ----- ----- */
// UIScrollContainer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/10/25
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Mathematics;
using Engine.Physics;
using Engine.UI.Constants.Components;
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
        /// </summary>
        private float _contentHeight;
        public float ContentHeight
        {
            get => _contentHeight;
            set
            {
                _contentHeight = value;
                if (ScrollHandler != null)
                    ScrollHandler.ApplyAlignment();
                else
                    _pendingApplyAlignment = true;
            }
        }

        /// <summary>
        /// Maximum overscroll allowed at edges.
        /// </summary>
        public float OverscrollLimit { get; set; } = 40.0f;

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
                    Physics.Position.Current.X,  // X axis no changed
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
                    Physics.Velocity.Current.X,  // X axis no changed
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
                var absPos = GetCurrentAbsolutePosition();

                if (Physics != null)
                {
                    Physics.Position = absPos;
                }
            }
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
                AllowWheel = true
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
