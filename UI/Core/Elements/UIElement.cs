/* ----- ----- ----- ----- */
// UIElement.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Engine.Geometry;
using Engine.Mathematics;
using Engine.Physics;
using Engine.UI.Constants;
using Engine.UI.Core.Interfaces;
using Engine.UI.Input;
using Engine.UI.Models;

namespace Engine.UI.Core.Elements
{
    public class UIElement : IUpdatable, IDrawable, IInputHandler
    {
        private static long s_nextId = 0;

        /// <summary>
        /// Unique entity ID for each UI element, auto-incremented.
        /// Can be used to track or identify specific UI elements.
        /// </summary>
        public long InstanceId { get; }

        /// <summary>
        /// Parent node: if null, it means the top node of the UI hierarchy.
        /// </summary>
#nullable enable
        public UIElement? Parent { get; set; }
#nullable disable

        /// <summary>
        /// The collection of all child nodes. The logical children of this UI element.
        /// </summary>
        public List<UIElement> Children { get; } = new();

        /// <summary>
        /// Whether the child nodes need to be reordered.
        /// </summary>
        private bool _isChildrenSortedDirty = true;

        /// <summary>
        /// Cache sorted child nodes (by ZIndex).
        /// </summary>
        private List<UIElement> _sortedChildrenAsc;
        private List<UIElement> _sortedChildrenDesc;

        /// <summary>
        /// The position of the component relative to the parent node.
        /// </summary>
        public virtual UIPosition LocalPosition { get; set; } = new UIPosition(Vector2F.Zero);

        /// <summary>
        /// The width and height of the UI element.
        /// </summary>
        public virtual Vector2F Size { get; set; } = Vector2F.Zero;

        /// <summary>
        /// The actual display area (position + size) of this element.
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
        /// Controls the priority of drawing and input event handling of components in the parent node.
        /// The higher value means the higher layer, which will be drawn and receive input first.
        /// </summary>
        private int _zIndex = 0;

        /// <summary>
        /// Controls the priority of drawing and input event handling of components in the parent node.
        /// The higher value means the higher layer, which will be drawn and receive input first.
        /// When modified, parent is notified to reorder its child nodes.
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

        /// <summary>
        /// Whether this element should be kept when its parent's children are cleared.
        /// Useful for long-lived elements like overlay layers, backgrounds, or root HUDs.
        /// </summary>
        public bool IsPersistent { get; set; } = false;

        /// <summary>
        /// Optional UI type classification, useful for filtering or managing cleanup logic.
        /// </summary>
        public UIElementType ElementType { get; set; } = UIElementType.Generic;

        /// <summary>
        /// Is it logically visible (use for UI display and HitTest).
        /// `IsVisible = false` means that the object is not drawn and cannot be clicked.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Whether it can interact with the mouse (click, drag, etc.) or keybord.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Whether it can interact with the mouse (click, drag, etc.) or keybord.
        /// </summary>
        public virtual bool IsInteractable => IsVisible && IsEnabled;

        /// <summary>
        /// Is it still possible to HitTest even if `IsVisible == false` (e.g. transparent mask).
        /// </summary>
        public virtual bool AllowHitWhenInvisible => false;

        /// <summary>
        /// Only controls whether the element is rendered (can be overridden).
        /// When `IsVisible = false`, it will not be drawn automatically, but some objects can be forced not to be drawn.
        /// </summary>
        public virtual bool DisableRender => !IsVisible;

        /// <summary>
        /// Optional physics animation controller for animating position, velocity, elasticity, and other effects.
        /// </summary>
#nullable enable
        public virtual Physics2D? Physics { get; set; }
#nullable disable


        /// <summary>
        /// Tracks whether this element has been disposed.
        /// </summary>
        private bool _disposed = false;

        public UIElement(int zIndex = 0, bool isPersistent = false, UIElementType type = UIElementType.Generic)
        {
            InstanceId = Interlocked.Increment(ref s_nextId);
            _zIndex = zIndex;
            IsPersistent = isPersistent;
            ElementType = type;
        }

        /// <summary>
        /// Calculate absolute coordinates: Base + offsets of all ancestors
        /// </summary>
        public Vector2F GetBaseAbsolutePosition()
        {
            Vector2F pos = LocalPosition.Base;
            while (Parent != null)
            {
                pos += Parent.LocalPosition.Base;
                Parent = Parent.Parent;
            }
            return pos;
        }

        /// <summary>
        /// Get absolute position of this UI element.
        /// </summary>
        /// <returns>Absolute position of this UI element.</returns>
        public Vector2F GetCurrentAbsolutePosition()
        {
            Vector2F accumulated = LocalPosition.Current;

#nullable enable
            UIElement? current = this.Parent;
#nullable disable

            while (current != null)
            {
                // Once a parent object with Physics2D is encountered, the position is the absolute anchor point and recursion stops
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

        /// <summary>
        /// Get absolute bounds of this UI element.
        /// </summary>
        /// <returns>Absolute bounds of this UI element.</returns>
        public LayoutF GetCurrentAbsoluteBounds()
        {
            return new LayoutF(GetCurrentAbsolutePosition(), Size);
        }

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

        public UIElement GetRoot()
        {
            UIElement node = this;
            while (node.Parent != null)
            {
                node = node.Parent;
            }
            return node;
        }

        /// <summary>
        /// Gets a collection of child nodes sorted by ZIndex, re-sorting only when data changes.
        /// </summary>
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

        public virtual void AddChild(UIElement child)
        {
            child.Parent = this;
            Children.Add(child);
            // Update child's phisics2D absolute position
            child.OnAddedToParent();

            _isChildrenSortedDirty = true;
        }

        protected virtual void OnAddedToParent()
        {
            if (Physics != null)
                Physics.Position = GetCurrentAbsolutePosition();
        }

        public virtual void RemoveChild(UIElement child)
        {
            if (Children.Remove(child))
            {
                child.Parent = null;
                _isChildrenSortedDirty = true;
            }
        }

        /// <summary>
        /// When the ZIndex changes, this method should be called manually to invalidate the sort cache.
        /// </summary>
        public void NotifyChildOrderChanged()
        {
            _isChildrenSortedDirty = true;
        }

        /// <summary>
        /// Removes all children based on optional filtering rules.
        /// </summary>
        /// <param name="includePersistent">If true, persistent elements will also be removed.</param>
        /// <param name="onlyTypes">Optional: only remove children with specific element types.</param>
        /// <param name="excludeTypes">Optional: skip removal for children with these element types.</param>
        public virtual void RemoveAllChild(
            bool includePersistent = false,
            List<UIElementType> onlyTypes = null,
            List<UIElementType> excludeTypes = null)
        {
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                var child = Children[i];

                if (!includePersistent && child.IsPersistent)
                    continue;

                if (onlyTypes != null && !onlyTypes.Contains(child.ElementType))
                    continue;

                if (excludeTypes != null && excludeTypes.Contains(child.ElementType))
                    continue;

                child.Parent = null;
                Children.RemoveAt(i);
            }
        }

        /// <summary>
        /// Determines whether the given point is within the visible area of ​​this UI element.
        /// </summary>
        public virtual bool HitTest(PointF point)
        {
            if (!IsEnabled)
                return false;

            // Skip if it is not visible and transparent hits are not allowed
            if (!IsVisible && !AllowHitWhenInvisible)
                return false;

            return GetCurrentAbsoluteBounds().Contains(point);
        }

        /// <summary>
        /// Starting from this element, search the sub-elements depth-first to find the top-level UIElement that hits the point
        /// </summary>
#nullable enable
        public UIElement? HitTestDeep(PointF point, bool isRootCall = true)
#nullable disable
        {
            //Console.WriteLine($"[HitTestDeep] Checking: {this.GetType().Name}");
            // If no hit, return `null`

            // Check root
            if (isRootCall && this.GetRoot().ElementType != UIElementType.Root)
                return null;

            // Check ancestors
            var current = this.Parent;
            while (current != null)
            {
                if (!current.IsVisible && !current.AllowHitWhenInvisible)
                    return null;
                current = current.Parent;
            }
            var root = this.GetRoot();

            // Search from the last child element forward (elements with higher ZIndex are at the back)
            foreach (var child in GetSortedChildrenByZIndex(descending: true))
            {
                //Console.WriteLine($"[HitTestDeep] Checking child deep: {child.GetType().Name}");
                var hit = child.HitTestDeep(point);
                if (hit != null && hit.IsInteractable)
                {
                    //Console.WriteLine($"[HitTestDeep] Hit child: {hit.GetType().Name}");
                    return hit;
                }
            }

            // Check self in the end
            if (HitTest(point))
            {
                return this;
            }

            // If none of the child elements are hit, return self
            //Console.WriteLine($"[HitTestDeep] Child and this no hit: {this.GetType().Name}");
            return null;
        }

        public virtual void Update()
        {
            Physics?.SmoothUpdate();
            OnUpdate();

            foreach (var child in Children)
                child.Update();
        }

        protected virtual void OnUpdate() { }

        public virtual void Draw(Graphics g)
        {
            if (DisableRender)
                return;

            if (IsDisposed)
                return;
            
            OnDraw(g);

            foreach (var child in GetSortedChildrenByZIndex()
                .Where(c => !c.DisableRender))
                if (child.IsVisible)
                    child.Draw(g);
        }

        protected virtual void OnDraw(Graphics g) { }

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

        public virtual void EndFrame()
        {
            // Optionally override in subclass
            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    child.EndFrame();
                }
            }
        }

        /// <summary>
        /// Releases resources used by this element.
        /// Removes all child elements and clears internal state.
        /// </summary>
        public virtual void Dispose()
        {
            if (_disposed)
                return;  // Already disposed

            // Dispose all children first
            foreach (var child in Children)
            {
                child.Dispose();
            }

            // Remove all child elements, including persistent ones
            RemoveAllChild(includePersistent: true);

            Parent?.RemoveChild(this);

            DisposeUI();

            // Mark as disposed
            _disposed = true;
        }

        /// <summary>
        /// 供子類覆寫，用於釋放自定義資源（非子 UI 元素）
        /// </summary>
        public virtual void DisposeUI() { }
        public bool IsDisposed => _disposed;
    }
}
