/* ----- ----- ----- ----- */
// UIElementBase.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Engine.Geometry;
using Engine.Mathematics;
using Engine.Physics;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Interfaces;
using Engine.UI.Models;

namespace Engine.UI.Core.Bases
{
    /// <summary>
    /// Base abstract class for all UI elements.
    /// Provides hierarchy management, layout computation, interaction, rendering, and physics integration.
    /// </summary>
    public abstract class UIElementBase
    {
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

        /// <summary>
        /// The element's position relative to its parent container.
        /// </summary>
        public virtual UIPosition LocalPosition { get; set; } = new UIPosition(Vector2F.Zero);

        /// <summary>
        /// The element's visual size (Width, Height).
        /// </summary>
        public virtual Vector2F Size { get; set; } = Vector2F.Zero;

        /// <summary>
        /// Defines layout constraints and rules for automatic positioning or anchoring.
        /// </summary>
        public UILayout LayoutRules { get; set; } = new UILayout();

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
        /// Indicates whether the layout must be recalculated.
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
        /// Determines whether the element can respond to user input.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Indicates if this element can receive interaction based on visibility and enabled state.
        /// </summary>
        public virtual bool IsInteractable => IsVisible && IsEnabled;

        /// <summary>
        /// Allows hit testing even if invisible. Usually false.
        /// </summary>
        public virtual bool AllowHitWhenInvisible => false;

        /// <summary>
        /// Determines whether rendering should be skipped when invisible.
        /// </summary>
        public virtual bool DisableRender => !IsVisible;

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
        /// Returns the current absolute bounds (in screen coordinates) of the element.
        /// </summary>
        /// <returns>The current layout in absolute coordinates.</returns>
        public abstract LayoutF GetCurrentAbsoluteBounds();

        /// <summary>
        /// Updates logic or animations for this element (called every frame).
        /// </summary>
        public abstract void Update();

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
        public abstract bool OnMouseDown(MouseEventArgs e);

        /// <summary>
        /// Handles mouse move events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled.</returns>
        public abstract bool OnMouseMove(MouseEventArgs e);

        /// <summary>
        /// Handles mouse button release events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled.</returns>
        public abstract bool OnMouseUp(MouseEventArgs e);

        /// <summary>
        /// Handles mouse wheel scrolling events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled.</returns>
        public abstract bool OnMouseWheel(MouseEventArgs e);

        /// <summary>
        /// Handles mouse click events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled.</returns>
        public abstract bool OnMouseClick(MouseEventArgs e);

        #endregion

        #region Rendering

        /// <summary>
        /// Renders the visual representation of this element and its children.
        /// </summary>
        /// <param name="g">Graphics context used for drawing.</param>
        public abstract void Draw(Graphics g);

        #endregion
    }
}
