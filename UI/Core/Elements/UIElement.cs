/* ----- ----- ----- ----- */
// UIElement.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/10/24
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Engine.Geometry;
using Engine.Mathematics;
using Engine.Physics;
using Engine.UI.Constants.Components;
using Engine.UI.Constants.Core;
using Engine.UI.Constants.Events;
using Engine.UI.Core.Interfaces;
using Engine.UI.Input;
using Engine.UI.Models;

namespace Engine.UI.Core.Elements
{
    public class UIElement : IUpdatable, IDrawable, IInputHandler
    {
        #region Identity

        private static long s_nextId = 0;

        /// <summary>Unique ID for tracking elements</summary>
        public long InstanceId { get; }

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
        public UIElement? Parent { get; set; }
#nullable disable

        /// <summary>Child elements</summary>
        public List<UIElement> Children { get; } = new();

        private bool _isChildrenSortedDirty = true;
        private List<UIElement> _sortedChildrenAsc;
        private List<UIElement> _sortedChildrenDesc;

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

        /// <summary>Indicates whether layout has been calculated</summary>
        protected bool _layoutDirty = true;

        #endregion

        #region Sorting / ZIndex

        private int _zIndex = 0;

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


        #region Constructor / Dispose

        private bool _disposed = false;

        public UIElement(int zIndex = 0, bool isPersistent = false, UIElementType type = UIElementType.Generic)
        {
            InstanceId = Interlocked.Increment(ref s_nextId);
            _zIndex = zIndex;
            IsPersistent = isPersistent;
            ElementType = type;
        }

        public virtual void Dispose()
        {
            if (_disposed) return;

            // Dispose children
            foreach (var child in Children.ToList())
                child.Dispose();

            RemoveAllChild(includePersistent: true);
            Parent?.RemoveChild(this);

            DisposeUI();
            _disposed = true;
        }

        public virtual void DisposeUI() { }
        public bool IsDisposed => _disposed;

        #endregion

        #region Position Utilities

        /// <summary>Get absolute position by summing ancestors</summary>
        public Vector2F GetCurrentAbsolutePosition()
        {
            Vector2F accumulated = LocalPosition.Current;
#nullable enable
            UIElement? current = this.Parent;
#nullable disable

            while (current != null)
            {
                if (current.Physics != null)
                {
                    accumulated += current.Physics.Position.Current;
                    break;
                }
                accumulated += current.LocalPosition.Current;
                current = current.Parent;
            }

            return accumulated;
        }

        /// <summary>Get absolute bounds</summary>
        public LayoutF GetCurrentAbsoluteBounds()
        {
            return new LayoutF(GetCurrentAbsolutePosition(), Size);
        }

        /// <summary>
        /// Recalculates element position and size based on parent bounds and layout rules.
        /// </summary>
        public virtual void UpdateLayout()
        {
            if (Parent == null || LayoutRules.IgnoreParentLayout)
                return;

            var parentBounds = Parent.GetCurrentAbsoluteBounds();
            var newPos = LocalPosition.Current;
            var newSize = Size;

            var margin = LayoutRules.Margin;
            var anchor = LayoutRules.Anchor;

            // Compute size if percentage set
            if (LayoutRules.SizePercent != null)
            {
                var percent = LayoutRules.SizePercent;
                newSize = new Vector2F(parentBounds.Size.X * percent.X, parentBounds.Size.Y * percent.Y);
            }

            // Compute position
            float x = newPos.X, y = newPos.Y;

            if (anchor.HasFlag(Anchor.Left))
                x = margin.Left;
            if (anchor.HasFlag(Anchor.Right))
                x = parentBounds.Size.X - newSize.X - margin.Right;
            if (anchor.HasFlag(Anchor.CenterX))
                x = (parentBounds.Size.X - newSize.X) / 2f;

            if (anchor.HasFlag(Anchor.Top))
                y = margin.Top;
            if (anchor.HasFlag(Anchor.Bottom))
                y = parentBounds.Size.Y - newSize.Y - margin.Bottom;
            if (anchor.HasFlag(Anchor.CenterY))
                y = (parentBounds.Size.Y - newSize.Y) / 2f;

            LocalPosition = new UIPosition(new Vector2F(x, y));
            Size = newSize;
            _layoutDirty = false;
            Bounds = new LayoutF(GetCurrentAbsolutePosition(), Size);

            // Apply recursively
            foreach (var child in Children)
                if (child.LayoutRules.AutoUpdate)
                    child.UpdateLayout();
        }

        #endregion


        #region Child Management

        public virtual void AddChild(UIElement child)
        {
            child.Parent = this;
            Children.Add(child);
            child.OnAddedToParent();
            _isChildrenSortedDirty = true;
        }

        protected virtual void OnAddedToParent()
        {
            if (Physics != null)
                Physics.Position.Current = GetCurrentAbsolutePosition();
        }

        public virtual void RemoveChild(UIElement child)
        {
            if (Children.Remove(child))
            {
                child.Parent = null;
                _isChildrenSortedDirty = true;
            }
        }

        public void NotifyChildOrderChanged() => _isChildrenSortedDirty = true;

        public IReadOnlyList<UIElement> GetSortedChildrenByZIndex(bool descending = false)
        {
            if (_isChildrenSortedDirty || _sortedChildrenAsc == null || _sortedChildrenDesc == null)
            {
                _sortedChildrenAsc = Children.OrderBy(c => c.ZIndex).ToList();
                _sortedChildrenDesc = Children.OrderByDescending(c => c.ZIndex).ToList();
                _isChildrenSortedDirty = false;
            }

            return descending ? _sortedChildrenDesc : _sortedChildrenAsc;
        }

        public virtual void RemoveAllChild(
            bool includePersistent = false,
            List<UIElementType> onlyTypes = null,
            List<UIElementType> excludeTypes = null)
        {
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                var child = Children[i];
                if (!includePersistent && child.IsPersistent) continue;
                if (onlyTypes != null && !onlyTypes.Contains(child.ElementType)) continue;
                if (excludeTypes != null && excludeTypes.Contains(child.ElementType)) continue;

                child.Parent = null;
                Children.RemoveAt(i);
            }
        }

        #endregion

        #region HitTest / Input

        /// <summary>
        /// Checks if a screen-space point is within the bounds of this UI element.
        /// </summary>
        /// <param name="screenPoint">The point in global/screen coordinates.</param>
        /// <returns>True if the point is inside this element's bounds.</returns>
        public virtual bool ContainsScreenPoint(Vector2F screenPoint)
        {
            var absPos = this.GetCurrentAbsolutePosition();  // Full resolved screen-space position
            return screenPoint.X >= absPos.X &&
                screenPoint.X <= absPos.X + Size.X &&
                screenPoint.Y >= absPos.Y &&
                screenPoint.Y <= absPos.Y + Size.Y;
        }

        public virtual bool HitTest(PointF point)
        {
            if (!IsEnabled) return false;
            if (!IsVisible && !AllowHitWhenInvisible) return false;
            return GetCurrentAbsoluteBounds().Contains(point);
        }

        public UIElement GetRoot()
        {
            UIElement node = this;
            while (node.Parent != null)
                node = node.Parent;
            return node;
        }

        /// <summary>
        /// Starting from this element, search sub-elements depth-first to find the top-level UIElement that hits the point
        /// </summary>
        #nullable enable
        public UIElement? HitTestDeep(PointF point, bool isRootCall = true)
        #nullable disable
        {
            // Check root
            if (isRootCall && this.GetRoot().ElementType != UIElementType.Root)
                return null;

            // Check ancestors visibility
            var current = this.Parent;
            while (current != null)
            {
                if (!current.IsVisible && !current.AllowHitWhenInvisible)
                    return null;
                current = current.Parent;
            }

            // Check children last to first (higher ZIndex first)
            foreach (var child in GetSortedChildrenByZIndex(descending: true))
            {
                var hit = child.HitTestDeep(point);
                if (hit != null && hit.IsInteractable)
                    return hit;
            }

            // Check self
            if (HitTest(point))
                return this;

            return null;
        }

        // Mouse event handling
        protected bool PropagateMouseEvent(MouseEventArgs e, UIEventType eventName)
        {
            bool isInside = IsInteractable && GetCurrentAbsoluteBounds().Contains(e.Location);

            // Propagate to child
            foreach (var child in GetSortedChildrenByZIndex(descending: true))
            {
                bool handled = eventName switch
                {
                    UIEventType.MouseClick => child.OnMouseClick(e),
                    UIEventType.MouseDown => child.OnMouseDown(e),
                    UIEventType.MouseMove => child.OnMouseMove(e),
                    UIEventType.MouseUp => child.OnMouseUp(e),
                    UIEventType.MouseWheel => child.OnMouseWheel(e),
                    _ => false
                };

                if (handled)
                    return true;
            }

            switch (eventName)
            {
                // These events must be within the area
                case UIEventType.MouseDown:
                case UIEventType.MouseWheel:
                case UIEventType.MouseClick:
                    if (!isInside) return false;
                    break;

                // No need to detect the area
                case UIEventType.MouseMove:
                case UIEventType.MouseUp:
                    break;
            }

            // Self handling
            return eventName switch
            {
                UIEventType.MouseClick => HandleMouseClick(e),
                UIEventType.MouseDown => HandleMouseDown(e),
                UIEventType.MouseMove => HandleMouseMove(e),
                UIEventType.MouseUp => HandleMouseUp(e),
                UIEventType.MouseWheel => HandleMouseWheel(e),
                _ => false
            };
        }

        public virtual bool OnMouseDown(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseDown);
        }
        protected virtual bool HandleMouseDown(MouseEventArgs e)
        {
            return false; // The default is not to process, and the subclass can return true to indicate successful processing
        }

        public virtual bool OnMouseMove(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseMove);
        }
        protected virtual bool HandleMouseMove(MouseEventArgs e)
        {
            return false;
        }

        public virtual bool OnMouseUp(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseUp);
        }
        protected virtual bool HandleMouseUp(MouseEventArgs e)
        {
            return false;
        }

        public virtual bool OnMouseWheel(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseWheel);
        }
        protected virtual bool HandleMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        public virtual bool OnMouseClick(MouseEventArgs e)
        {
            var uIElement = this;
            return PropagateMouseEvent(e, UIEventType.MouseClick);
        }
        public virtual bool HandleMouseClick(MouseEventArgs e)
        {
            var uIElement = this;
            var root = this.GetRoot();
            return false;
        }

        #endregion

        #region Update / Draw

        public void Init()
        {
            if (IsInitialized)
                return;
            IsInitialized = true;
            OnInit();
        }

        protected virtual void OnInit() { }

        public virtual void Update()
        {
            Physics?.SmoothUpdate();
            OnUpdate();
            foreach (var child in Children) child.Update();
        }

        protected virtual void OnUpdate() { }

        public virtual void Draw(Graphics g)
        {
            if (_layoutDirty)
                UpdateLayout();

            if (DisableRender || IsDisposed)
                return;

            OnDraw(g);

            foreach (var child in GetSortedChildrenByZIndex(descending: true)
                .Where(c => !c.DisableRender && c.IsVisible))
                child.Draw(g);
        }

        protected virtual void OnDraw(Graphics g) { }

        public virtual void EndFrame()
        {
            foreach (var child in Children)
                if (child.IsVisible)
                    child.EndFrame();
        }

        #endregion
    }
}
