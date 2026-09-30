/* ----- ----- ----- ----- */
// UIElement.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2026/09/30
// Version: v1.3
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

using Engine.Geometry;
using Engine.Mathematics;
using Engine.Physics;
using Engine.Platform;
using Engine.UI.Constants.Components;
using Engine.UI.Constants.Core;
using Engine.UI.Constants.Events;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Layout;
using Engine.UI.Models;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Base class representing a generic UI element.
    /// Provides foundational logic for layout, hierarchy, input, rendering, and lifecycle management.
    /// </summary>
    public abstract class UIElement : UIElementBase
    {
        //public UIHandlerBase Handler => base.HandlerBase;

        //public UIRendererBase Renderer => base.RendererBase;

        #region Constructor / Initialization

        /// <summary>
        /// Creates a new UIElement with optional ZIndex, persistence, and type.
        /// </summary>
        /// <param name="zIndex">Render/input order in parent container.</param>
        /// <param name="isPersistent">Whether element persists when parent clears children.</param>
        /// <param name="type">Optional type classification.</param>
        public UIElement(int zIndex = 0, bool isPersistent = false, UIElementType type = UIElementType.Generic)
        {
            InstanceId = Interlocked.Increment(ref s_nextId);
            _zIndex = zIndex;
            IsPersistent = isPersistent;
            ElementType = type;
        }

        /// <summary>
        /// Initialize element. Calls pre-init, init, and post-init hooks.
        /// </summary>
        public virtual void Init()
        {
            //Console.WriteLine($"[UIElement]Init no generic Current type: {this?.GetType().FullName ?? "null"}, IsInitialized: {IsInitialized}");
            if (IsInitialized)
                return;
            IsInitialized = true;

            RunInitHooks();
        }

        /// <summary>
        /// Runs the parameterless init hooks without touching <see cref="UIElementBase.IsInitialized"/>,
        /// so Init overloads that already set the flag can still invoke them.
        /// </summary>
        protected void RunInitHooks()
        {
            OnBeforeInit();
            OnInit();
            OnAfterInit();
        }

        /// <summary>
        /// Called before Init for setup tasks.
        /// </summary>
        protected virtual void OnBeforeInit() { }

        /// <summary>
        /// Core initialization logic for this element.
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// Called after Init for post-processing (e.g., registering events).
        /// </summary>
        protected virtual void OnAfterInit() { }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose element and all children recursively.
        /// </summary>
        public override void Dispose()
        {
            if (_disposed)
                return;
            OnBeforeDispose();

            // Dispose children
            foreach (var child in Children.ToList())
                child.Dispose();

            RemoveAllChild(includePersistent: true);
            Parent?.RemoveChild(this);

            OnDispose();
            OnAfterDispose();
            _disposed = true;
        }

        /// <summary>
        /// Hook called before disposal begins.
        /// </summary>
        protected virtual void OnBeforeDispose() { }

        /// <summary>
        /// Core disposal logic for this element.
        /// </summary>
        protected virtual void OnDispose()
        {
            DisposeUI();
        }

        /// <summary>
        /// Hook called after disposal completes.
        /// </summary>
        protected virtual void OnAfterDispose() { }

        /// <summary>
        /// Dispose element-specific resources.
        /// </summary>
        protected virtual void DisposeUI() { }

        #endregion

        #region Position & Layout Utilities

        /// <summary>
        /// Get absolute position by summing all ancestor positions.
        /// </summary>
        public Vector2F GetCurrentAbsolutePosition()
        {
            Vector2F accumulated = LocalPosition.Current;
#nullable enable
            UIElementBase? current = this.Parent;
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

        /// <summary>
        /// Returns absolute bounds (position + size).
        /// </summary>
        public override LayoutF GetCurrentAbsoluteBounds()
        {
            return new LayoutF(GetCurrentAbsolutePosition(), Size);
        }

        /// <summary>
        /// Absolute content box: the bounds minus <see cref="UILayout.Padding"/> - where
        /// layout-managed in-flow children are placed and what <see cref="UILayout.Overflow"/> clips to.
        /// </summary>
        public RectangleF GetCurrentAbsoluteContentBox()
        {
            var position = GetCurrentAbsolutePosition();
            var padding = LayoutRules.Padding;
            return new RectangleF(
                position.X + padding.Left,
                position.Y + padding.Top,
                Math.Max(0f, Size.X - padding.Horizontal),
                Math.Max(0f, Size.Y - padding.Vertical));
        }

        /// <summary>
        /// True when <see cref="UILayout.Overflow"/> clips children (Hidden or Scroll).
        /// </summary>
        public bool ClipsChildren => LayoutRules.Overflow != OverflowMode.Visible;

        /// <summary>
        /// Whether a point may reach this element's children: always, unless children are
        /// clipped and the point is outside the content box (clipped content isn't hittable).
        /// </summary>
        protected bool ChildrenCanReceive(PointF point)
        {
            if (!ClipsChildren)
                return true;
            var box = GetCurrentAbsoluteContentBox();
            return point.X >= box.Left && point.X < box.Right && point.Y >= box.Top && point.Y < box.Bottom;
        }

        /// <summary>
        /// The layout pass for this element, run by <see cref="Draw"/> whenever
        /// <see cref="UIElementBase.LayoutDirty"/> or <see cref="UIElementBase.LegacyLayoutPending"/>
        /// is set (and callable directly, see also <see cref="PerformLayout"/>):
        /// <list type="number">
        /// <item>Legacy elements: the one-shot legacy rules (<see cref="ApplyLegacyLayout"/>),
        /// unchanged from before the layout system.</item>
        /// <item>If dirty: arranges this element's layout-managed children and recurses into
        /// the dirty ones (<see cref="UpdateManagedLayout"/>).</item>
        /// </list>
        /// </summary>
        public override void UpdateLayout()
        {
            if (LegacyLayoutPending && !IsLayoutManaged)
                ApplyLegacyLayout();

            if (_layoutDirty)
                UpdateManagedLayout();
        }

        /// <summary>
        /// Runs the layout pass for this element and then for every descendant, instead of
        /// waiting for each one's next draw. Use when positions are needed before drawing
        /// (e.g. to hit-test or measure right after building a subtree).
        /// </summary>
        public void PerformLayout()
        {
            UpdateLayout();
            foreach (var child in Children.ToArray())
                if (child is UIElement element)
                    element.PerformLayout();
        }

        /// <summary>
        /// Clears the dirty flag, (re)arranges the layout-managed children, then recurses into
        /// every child that is (still or newly) dirty. Never re-applies legacy rules, so
        /// legacy elements keep their positions exactly.
        /// </summary>
        protected void UpdateManagedLayout()
        {
            _layoutDirty = false;
            LayoutEngine.ArrangeChildren(this);
            OnChildrenArranged();

            // Snapshot: arranging may run measure hooks, which must not see a list mid-change.
            foreach (var child in Children.ToArray())
                if (child.LayoutDirty && child is UIElement element)
                    element.UpdateManagedLayout();
        }

        /// <summary>
        /// Called by the layout pass right after this element's layout-managed children were
        /// arranged (legacy children keep their own positions), before recursing into them.
        /// Containers that derive state from their children's rectangles (e.g. a scroll
        /// container's content size) update it here.
        /// </summary>
        protected virtual void OnChildrenArranged() { }

        /// <summary>
        /// Updates element's layout based on parent bounds and legacy layout rules
        /// (<see cref="UILayout.Anchor"/>, <see cref="UILayout.Margin"/>, <see cref="UILayout.SizePercent"/>).
        /// Also recursively updates children if AutoUpdate is enabled.
        /// <para>
        /// One-shot, as it always was: it runs on the element's first layout pass and then
        /// never again (<see cref="UIElementBase.LegacyLayoutPending"/>). The root, and elements
        /// with <see cref="UILayout.IgnoreParentLayout"/>, return early and stay pending, which
        /// is harmless.
        /// </para>
        /// </summary>
        protected void ApplyLegacyLayout()
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

            // Anchoring both opposite edges (Anchor.StretchX / StretchY) stretches between
            // them, minus the margins. Without this the Right/Bottom rule below simply
            // overrode Left/Top and the element kept its size. (A new vector: newSize may
            // still be this element's own Size instance.)
            bool stretchX = anchor.HasFlag(Anchor.Left) && anchor.HasFlag(Anchor.Right);
            bool stretchY = anchor.HasFlag(Anchor.Top) && anchor.HasFlag(Anchor.Bottom);
            if (stretchX || stretchY)
            {
                newSize = new Vector2F(
                    stretchX ? Math.Max(0f, parentBounds.Size.X - margin.Left - margin.Right) : newSize.X,
                    stretchY ? Math.Max(0f, parentBounds.Size.Y - margin.Top - margin.Bottom) : newSize.Y);
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
            LegacyLayoutPending = false;
            Bounds = new LayoutF(GetCurrentAbsolutePosition(), Size);

            // Apply recursively
            foreach (var child in Children)
                if (child.LayoutRules.AutoUpdate && child.LegacyLayoutPending)
                    child.UpdateLayout();
        }

        #endregion

        #region Child Management

        public override void AddChild(UIElementBase child)
        {
            child.Parent = this;
            Children.Add(child);
            child.OnAddedToParent();
            _isChildrenSortedDirty = true;

            // New context for the child's own children (snapping), new item for this layout.
            child.InvalidateLayout();
            InvalidateLayout();

            // The child's own OnAddedToParent resynced it; its descendants' absolute
            // positions changed too (e.g. a scroll container inside an attached submenu).
            NotifyDescendantsMoved(child);
        }

        public override void OnAddedToParent()
        {
            if (Physics != null)
                Physics.Position.Current = GetCurrentAbsolutePosition();
        }

        public override void RemoveChild(UIElementBase child)
        {
            if (Children.Remove(child))
            {
                child.Parent = null;
                _isChildrenSortedDirty = true;
                InvalidateLayout();
            }
        }

        public override void NotifyChildOrderChanged() => _isChildrenSortedDirty = true;

        /// <summary>
        /// Returns children sorted by ZIndex.
        /// </summary>
        public IReadOnlyList<UIElementBase> GetSortedChildrenByZIndex(bool descending = false)
        {
            if (_isChildrenSortedDirty || _sortedChildrenAsc == null || _sortedChildrenDesc == null)
            {
                _sortedChildrenAsc = Children.OrderBy(c => c.ZIndex).ToList();
                _sortedChildrenDesc = Children.OrderByDescending(c => c.ZIndex).ToList();
                _isChildrenSortedDirty = false;
            }

            return descending ? _sortedChildrenDesc : _sortedChildrenAsc;
        }

        /// <summary>
        /// Removes all children optionally filtering by persistence or type.
        /// </summary>
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
                InvalidateLayout();
            }
        }

        #endregion

        #region HitTest / Input Handling

        /// <summary>
        /// Checks if a screen-space point is within the bounds of this UI element.
        /// </summary>
        /// <param name="screenPoint">The point in global/screen coordinates.</param>
        /// <returns>True if the point is inside this element's bounds.</returns>
        public virtual bool ContainsScreenPoint(Vector2F screenPoint)
        {
            // Same (half-open) test as HitTest, via the absolute bounds.
            return GetCurrentAbsoluteBounds().Contains(screenPoint);
        }

        /// <summary>
        /// Check if point lies within bounds, taking visibility and enabled flags into account.
        /// </summary>
        public virtual bool HitTest(PointF point)
        {
            if (!IsEnabled || !IsDisplayed) return false;
            if (!IsVisible && !AllowHitWhenInvisible) return false;
            return GetCurrentAbsoluteBounds().Contains(point);
        }

        /// <summary>
        /// Returns root element in hierarchy.
        /// </summary>
        public override UIElementBase GetRoot()
        {
            UIElementBase node = this;
            while (node.Parent != null)
                node = node.Parent;
            return (UIElementBase)node;
        }

        /// <summary>
        /// Starting from this element, search sub-elements depth-first to find the top-level UIElement that hits the point
        /// </summary>
#nullable enable
        public override UIElementBase? HitTestDeep(PointF point, bool isRootCall = true)
#nullable disable
        {
            // Check root
            if (isRootCall && this.GetRoot().ElementType != UIElementType.Root)
                return null;

            // Check ancestors visibility
            var current = this.Parent;
            while (current != null)
            {
                if (!current.IsDisplayed)
                    return null;
                if (!current.IsVisible && !current.AllowHitWhenInvisible)
                    return null;
                current = current.Parent;
            }

            // Check children last to first (higher ZIndex first)
            if (ChildrenCanReceive(point))
            {
                foreach (var child in GetSortedChildrenByZIndex(descending: true))
                {
                    var hit = child.HitTestDeep(point);
                    if (hit != null && hit.IsInteractable)
                        return hit;
                }
            }

            // Check self
            if (HitTest(point))
                return this;

            return null;
        }

        /// <summary>
        /// Propagates mouse events to children and self.
        /// </summary>
        protected bool PropagateMouseEvent(IMouseEvent e, UIEventType eventName)
        {
            // A hidden or disabled element doesn't take input, and neither does its subtree.
            if (!IsInteractable)
                return false;

            bool isInside = IsInteractable && GetCurrentAbsoluteBounds().Contains(e.Location);

            // Clipped children can't be pressed/clicked/scrolled outside the content box
            // (moves and releases still reach them, e.g. to end a drag).
            bool areaEvent = eventName == UIEventType.MouseDown
                || eventName == UIEventType.MouseWheel
                || eventName == UIEventType.MouseClick;
            bool childrenReachable = !areaEvent || ChildrenCanReceive(e.Location);

            // Propagate to child
            foreach (var child in GetSortedChildrenByZIndex(descending: true))
            {
                if (!childrenReachable)
                    break;

                bool handled = eventName switch
                {
                    UIEventType.MouseDown => child.OnMouseDown(e),
                    UIEventType.MouseMove => child.OnMouseMove(e),
                    UIEventType.MouseUp => child.OnMouseUp(e),
                    UIEventType.MouseWheel => child.OnMouseWheel(e),
                    UIEventType.MouseClick => child.OnMouseClick(e),
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
                UIEventType.MouseDown => HandlerBase?.HandleMouseDown(e) ?? false,
                UIEventType.MouseMove => HandlerBase?.HandleMouseMove(e) ?? false,
                UIEventType.MouseUp => HandlerBase?.HandleMouseUp(e) ?? false,
                UIEventType.MouseWheel => HandlerBase?.HandleMouseWheel(e) ?? false,
                UIEventType.MouseClick => HandlerBase?.HandleMouseClick(e) ?? false,
                _ => false
            };
        }

        public override bool OnMouseDown(IMouseEvent e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseDown);
        }

        public override bool OnMouseMove(IMouseEvent e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseMove);
        }

        public override bool OnMouseUp(IMouseEvent e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseUp);
        }

        public override bool OnMouseWheel(IMouseEvent e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseWheel);
        }

        public override bool OnMouseClick(IMouseEvent e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseClick);
        }

        #endregion

        #region Update / Draw / Reset

        /// <summary>
        /// Update element and children (handler updates).
        /// </summary>
        /// <remarks>
        /// <see cref="Physics"/> is not integrated here: every <see cref="Physics2D"/>
        /// registers itself with <see cref="PhysicsRegistry"/>, and the frame loop's
        /// <see cref="PhysicsRegistry.UpdateAll"/> already steps it once per frame.
        /// </remarks>
        public override void Update()
        {
            HandlerBase?.OnUpdate();

            // Snapshot: a handler's OnUpdate (e.g. posted actions) may add/remove children,
            // which would otherwise throw InvalidOperationException mid-enumeration.
            foreach (var child in Children.ToArray())
                child.Update();
        }

        /// <summary>
        /// Draw element and children.
        /// </summary>
        public override void Draw(IGraphics g)
        {
            if (_layoutDirty || LegacyLayoutPending)
                UpdateLayout();

            if (DisableRender || IsDisposed)
                return;

            //Console.WriteLine($"OnDraw called: {this.GetType().Name}");
            RendererBase?.Render(g, this);

            // Overflow Hidden/Scroll: clip the children to the content box. Balanced with
            // try/finally - an unpaired SetClip leaks on GDI+ and unbalances Skia's save stack.
            bool clip = ClipsChildren;
            if (clip)
                g.SetClip(GetCurrentAbsoluteContentBox());
            try
            {
                // Painter's algorithm: lowest ZIndex first so higher ZIndex ends up on top
                // (hit testing iterates the opposite way, highest first).
                foreach (var child in GetSortedChildrenByZIndex(descending: false)
                    .Where(c => !c.DisableRender && c.IsVisible))
                    child.Draw(g);
            }
            finally
            {
                if (clip)
                    g.ResetClip();
            }
        }

        public override void RequestRedraw()
        {
            Parent?.RequestRedraw();  // 遞迴向上通知
            // 或者直接觸發 Invalidate() / Refresh() 在畫布上
        }

        /// <summary>
        /// Reset element and all children recursively.
        /// </summary>
        public override void Reset()
        {
            OnBeforeReset();    // Preliminary stage: pause state, cancel task, etc.

            if (this is IResettable)
                OnReset();      // Main reset: clear the content and reset the attributes

            foreach (var child in Children.ToArray())
                child.Reset();  // Reset child elements

            OnAfterReset();     // Post-stage: restart animation, rebind data, etc.
        }

        /// <summary>
        /// Called before the reset process begins.
        /// </summary>
        protected virtual void OnBeforeReset() { }

        /// <summary>
        /// Performs the actual reset logic for this element.
        /// </summary>
        protected virtual void OnReset() { }

        /// <summary>
        /// Called after the reset process has finished.
        /// </summary>
        protected virtual void OnAfterReset() { }

        /// <summary>
        /// Called at end of frame for element and visible children.
        /// </summary>
        public override void EndFrame()
        {
            HandlerBase?.OnEndFrame();

            foreach (var child in Children.ToArray())
                if (child.IsVisible && child.IsDisplayed)
                    child.EndFrame();
        }

        #endregion
    }

    #region Generic Class

    /// <summary>
    /// Strongly-typed generic UIElement binding handler and renderer.
    /// </summary>
    public abstract class UIElement<TElement, THandler, TRenderer> : UIElement
        where TElement : UIElement<TElement, THandler, TRenderer>
        where THandler : UIHandler<TElement, THandler, TRenderer>
        where TRenderer : UIRenderer<TElement, THandler, TRenderer>
    {
        /// <summary>
        /// Strongly-typed handler reference.
        /// </summary>
        public THandler Handler
        {
            get => (THandler)HandlerBase;
            protected set => HandlerBase = value;
        }

        /// <summary>
        /// Strongly-typed renderer reference.
        /// </summary>
        public TRenderer Renderer
        {
            get => (TRenderer)RendererBase;
            protected set => RendererBase = value;
        }

        public UIElement(int zIndex = 0, bool isPersistent = false, UIElementType type = UIElementType.Generic)
            : base(zIndex, isPersistent, type)
        {
        }

        /// <summary>
        /// Initializes element with factory, handler, and renderer.
        /// </summary>
        /// <param name="factory">UI factory for creating dependencies.</param>
        /// <param name="handler">Handler instance to bind.</param>
        /// <param name="renderer">Renderer instance to bind.</param>
        public virtual void Init(IUiFactory factory, THandler handler, TRenderer renderer)
        {
            Console.WriteLine($"[UIElement]Init 3 generic Current type: {this?.GetType().FullName ?? "null"}, IsInitialized: {IsInitialized}");
            if (IsInitialized) return;
            IsInitialized = true;
            _factory = factory;

            OnBeforeInit(factory);

            // Bind Handler
            Handler = handler;
            Handler.Element = (TElement)(object)this;
            Console.WriteLine($"[UIElement]Handler type: {Handler?.GetType().FullName ?? "null"}");

            // Bind Renderer
            Renderer = renderer;
            Renderer.Element = (TElement)(object)this;
            Console.WriteLine($"[UIElement]Renderer type: {Renderer?.GetType().FullName ?? "null"}");

            RunInitHooks();

            OnInit(factory);
            OnAfterInit(factory);
        }

        /// <summary>
        /// Called before Init for setup tasks.
        /// </summary>
        protected virtual void OnBeforeInit(IUiFactory factory) { }

        /// <summary>
        /// Core initialization logic for this element.
        /// </summary>
        protected virtual void OnInit(IUiFactory factory) { }

        /// <summary>
        /// Called after Init for post-processing (e.g., registering events).
        /// </summary>
        protected virtual void OnAfterInit(IUiFactory factory) { }

        /// <summary>
        /// Creates renderer using factory function and binds it to this element.
        /// </summary>
        protected virtual TRenderer CreateRenderer(Func<TRenderer> factory)
        {
            var renderer = factory();
            renderer.Element = (TElement)(object)this;  // this 是 UIElementBase
            return renderer;
        }
    }

    #endregion
}
