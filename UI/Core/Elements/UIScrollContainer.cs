/* ----- ----- ----- ----- */
// UIScrollContainer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/10/25
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Windows.Forms;

using Engine.Mathematics;
using Engine.Physics;
using Engine.UI.Constants.Components;
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
    public class UIScrollContainer : UIElement, IPhysical2D
    {
        #region Fields

        /// <summary>
        /// Internal physics instance to handle scrolling movement.
        /// </summary>
        private readonly Physics2D _physics = new Physics2D();

        /// <summary>
        /// Input handler for mouse/touch scroll events.
        /// </summary>
        private readonly IScrollInputHandler _inputHandler;

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
                ApplyAlignment();
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

        #region Public Methods

        /// <summary>
        /// Updates scroll container every frame. Handles overscroll, inertia, and rebound behavior.
        /// </summary>
        public override void Update()
        {
            //Console.WriteLine($"ScrollY: {ScrollY}, gap: {-(ContentHeight - Size.Y)}, Physics.Position.Base: {Physics.Position.Base}, Physics.Position.Current: {Physics.Position.Current}, Physics.Position.Target: {Physics.Position.Target}");
            // If content fits within viewport, return to base position
            if (!OverContent)
            {
                // Moved
                if (ScrollY != 0)
                {
                    Physics.Position.Target = Physics.Position.Base;
                    Physics.Position.HasTarget = true;
                }
                // Already back to base position
                else
                {
                    Physics.Position.HasTarget = false;
                }
            }
            // Content is bigger than viewpoint
            else
            {
                //Console.WriteLine($"ScrollY: {ScrollY}, gap: {-(ContentHeight - Size.Y)}");
                // Moved
                if (ScrollY != 0)
                {
                    if (ScrollY > 0)
                    {
                        Physics.Position.Target = Physics.Position.Base;
                        Physics.Position.HasTarget = true;
                    }
                    else if (ScrollY < -(ContentHeight - Size.Y))
                    {
                        Physics.Position.Target = Physics.Position.Base - new Vector2F(0, ContentHeight - Size.Y);
                        Physics.Position.HasTarget = true;
                    }
                    else
                    {
                        Physics.Position.HasTarget = false;
                    }
                }
                // Already back to base position
                else
                {
                    Physics.Position.HasTarget = false;
                }
            }
        }

        /// <summary>
        /// Resets input delta after processing input each frame.
        /// </summary>
        public override void EndFrame()
        {
            _inputHandler.EndFrame();
        }

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

        #region Protected Methods (Mouse Events)

        /// <summary>
        /// Handles mouse down events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if event was handled, otherwise false.</returns>
        protected override bool HandleMouseDown(MouseEventArgs e)
        {
            return _inputHandler.OnMouseDown(e);
        }

        /// <summary>
        /// Handles mouse move events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if event was handled, otherwise false.</returns>
        protected override bool HandleMouseMove(MouseEventArgs e)
        {
            return _inputHandler.OnMouseMove(e);
        }

        /// <summary>
        /// Handles mouse up events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if event was handled, otherwise false.</returns>
        protected override bool HandleMouseUp(MouseEventArgs e)
        {
            return _inputHandler.OnMouseUp(e);
        }

        /// <summary>
        /// Handles mouse wheel events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if event was handled, otherwise false.</returns>
        protected override bool HandleMouseWheel(MouseEventArgs e)
        {
            return _inputHandler.OnMouseWheel(e);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies a physics target offset based on current scrolling velocity for rebound/inertia.
        /// </summary>
        /// <param name="velocity">Current vertical scrolling velocity.</param>
        private void ApplyVelocityBasedTarget(float velocity)
        {
            // For fast scroll, set the target position farther (e.g., 60px based on velocity)
            float targetOffset = Math.Sign(velocity) * Math.Min(Math.Abs(velocity), 60); // You can adjust this multiplier (e.g., 60) for higher velocities.

            // Apply target offset depending on scroll direction
            Physics.Position.Target = Physics.Position.Current + targetOffset;
            Physics.Position.HasTarget = true;
        }

        /// <summary>
        /// Applies current alignment mode (Top or Bottom) when content size changes.
        /// </summary>
        private void ApplyAlignment()
        {
            if (!OverContent)
            {
                // Content smaller than viewport: Reset to top
                ScrollY = 0;
                return;
            }

            switch (VerticalAlignment)
            {
                case ScrollAlignment.Top:
                    Physics.Position.Target = new Vector2F(Physics.Position.Base.X, GetCurrentAbsolutePosition().Y);
                    ScrollY = 0;
                    break;

                case ScrollAlignment.Bottom:
                    float absTopY = GetCurrentAbsolutePosition().Y;
                    float gapY = -(ContentHeight - Size.Y);
                    float absGapY = absTopY + gapY;
                    ScrollY = gapY;
                    Physics.Position.Target = new Vector2F(Physics.Position.Base.X, absGapY);
                    
                    //Physics.Position.Current = Physics.Position.Target;
                    //Console.WriteLine($"absTopY: {absTopY}, gapY: {gapY}, absGapY: {absGapY}, Physics.Position.Base: {Physics.Position.Base}, Physics.Position.Current: {Physics.Position.Current}, Physics.Position.Target: {Physics.Position.Target}");
                    break;
            }
            Physics.Position.HasTarget = false;
        }

        /// <summary>
        /// Checks whether the content exceeds viewport height.
        /// </summary>
        private bool OverContent => ContentHeight > AbsViewportBounds.Height;

        #endregion
    }
}
