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
    public abstract class UIElementBase
    {
        /// <summary>Reference to the handler (non-generic)</summary>
        public UIHandlerBase HandlerBase { get; protected set; }

        /// <summary>Reference to the renderer (non-generic)</summary>
        public UIRendererBase RendererBase { get; protected set; }


        protected IUiFactory _factory;

        #region Identity

        protected static long s_nextId = 0;

        /// <summary>Unique ID for tracking elements</summary>
        public long InstanceId { get; protected set; }

        /// <summary>Optional type classification</summary>
        public UIElementType ElementType { get; set; } = UIElementType.Generic;

        /// <summary>Whether element persists when parent clears children</summary>
        public bool IsPersistent { get; set; } = false;

        /// <summary>
        /// Tracks whether this element has been initialized.
        /// </summary>
        public bool IsInitialized { get; protected set; }

        #endregion

        #region Hierarchy

#nullable enable
        /// <summary>Parent UI element, null if root</summary>
        public UIElementBase? Parent { get; set; }
#nullable disable

        /// <summary>Child elements</summary>
        public List<UIElementBase> Children { get; } = new();

        protected bool _isChildrenSortedDirty = true;
        protected List<UIElementBase> _sortedChildrenAsc;
        protected List<UIElementBase> _sortedChildrenDesc;

        #endregion

        #region Layout & Position

        /// <summary>Relative position to parent</summary>
        public virtual UIPosition LocalPosition { get; set; } = new UIPosition(Vector2F.Zero);

        /// <summary>Size (Width, Height)</summary>
        public virtual Vector2F Size { get; set; } = Vector2F.Zero;

        /// <summary>Layout configuration (relative positioning rules)</summary>
        public UILayout LayoutRules { get; set; } = new UILayout();

        // Cached final layout (absolute position and size)
        public LayoutF Bounds { get; protected set; } = LayoutF.Zero;

        /// <summary>Actual layout (position + size)</summary>
        public virtual LayoutF Layout
        {
            get => new LayoutF(LocalPosition.Current, Size);
            set
            {
                LocalPosition = new UIPosition(value.Position);
                Size = value.Size;
            }
        }
        public RectangleF? ClipRect { get; set; } = null;

        /// <summary>Indicates whether layout has been calculated</summary>
        protected bool _layoutDirty = true;
        public bool LayoutDirty
        {
            get => _layoutDirty;
            protected set => _layoutDirty = value;
        }

        #endregion

        #region Sorting / ZIndex

        protected int _zIndex = 0;

        /// <summary>Controls draw/input order in parent</summary>
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

        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;

        public virtual bool IsInteractable => IsVisible && IsEnabled;
        public virtual bool AllowHitWhenInvisible => false;
        public virtual bool DisableRender => !IsVisible;

        #endregion

        #region Physics

#nullable enable
        public virtual Physics2D? Physics { get; set; }
#nullable disable

        #endregion

        #region Constructor / Init / Dispose

        protected bool _disposed = false;

        #endregion

        public abstract void Dispose();

        public abstract LayoutF GetCurrentAbsoluteBounds();

        public abstract void UpdateLayout();

        public abstract void AddChild(UIElementBase child);

        public abstract void OnAddedToParent();

        public abstract void RemoveChild(UIElementBase child);

        public abstract void NotifyChildOrderChanged();

        public abstract UIElementBase GetRoot();

#nullable enable
        public abstract UIElementBase? HitTestDeep(PointF point, bool isRootCall = true);
#nullable disable

        public abstract bool OnMouseDown(MouseEventArgs e);

        public abstract bool OnMouseMove(MouseEventArgs e);

        public abstract bool OnMouseUp(MouseEventArgs e);

        public abstract bool OnMouseWheel(MouseEventArgs e);

        public abstract bool OnMouseClick(MouseEventArgs e);

        public abstract void Update();

        public abstract void Draw(Graphics g);

        public abstract void EndFrame();
    }
}
