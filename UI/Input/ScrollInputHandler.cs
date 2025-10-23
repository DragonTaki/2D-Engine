/* ----- ----- ----- ----- */
// ScrollInputHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Engine.Mathematics;
using Engine.Physics;
using Engine.UI.Core.Elements;

namespace Engine.UI.Input
{
    /// <summary>
    /// Handles mouse-based scroll input, including drag detection, threshold filtering,
    /// and inertial velocity output. This class does not directly update any physics system.
    /// </summary>
    public sealed class ScrollInputHandler : IScrollInputHandler
    {
        /// <summary>
        /// [DEPRECATED] Singleton
        /// Replaced by DI
        /// </summary>

        /*
        private static readonly Lazy<ScrollInputHandler> _instance =
            new Lazy<ScrollInputHandler>(() => new ScrollInputHandler());

        public static ScrollInputHandler Instance => _instance.Value;
        */

        private readonly List<ScrollTarget> _scrollTargets = new();

#nullable enable
        private ScrollTarget? _activeTarget = null;
#nullable disable

        private readonly DragHandler _dragHandler;

        public bool IsDragging => _dragHandler.IsDragging;
        public int ZIndex { get; set; } = 0;

        public ScrollInputHandler()
        {
            _dragHandler = new DragHandler();
            _dragHandler.OnDrag += HandleDrag;
            {
                if (_activeTarget?.Physics != null)
                    _activeTarget.Physics.Velocity.Reset();
            }
            ;
        }

        public void RegisterScrollTarget(
            UIElement element,
            Physics2D physics,
            Func<RectangleF> viewportGetter,
            ScrollBehavior behavior = null,
            int zIndex = 0)
        {
            if (_scrollTargets.Exists(t => t.Element == element)) return;

            _scrollTargets.Add(new ScrollTarget
            {
                Element = element,
                Physics = physics,
                ViewportGetter = viewportGetter,
                Behavior = behavior ?? new ScrollBehavior()
            });
            ZIndex = zIndex;
        }

        /// <summary>
        /// Returns true if currently dragging and mouse is inside the active scroll target.
        /// Used to optionally suppress UI interaction while dragging.
        /// </summary>
        public bool IsDraggingWithinActiveTarget(Point location)
        {
            return IsDragging &&
                _activeTarget?.ViewportGetter().Contains(location) == true;
        }

        /// <summary>
        /// Call when mouse button is pressed to begin scroll detection.
        /// </summary>
        public bool OnMouseDown(MouseEventArgs e)
        {
            if (IsDragging)
            {
                _dragHandler.OnMouseUp(e);
                _activeTarget = null;
            }
            for (int i = _scrollTargets.Count - 1; i >= 0; i--)
            {
                var target = _scrollTargets[i];

                // If not IsVisible or not IsEnabled
                if (!target.Element.IsInteractable || target.Element.Parent == null)
                {
                    continue;
                }

                var bounds = target.ViewportGetter();
                if (bounds == RectangleF.Empty || !bounds.Contains(e.Location))
                    continue;

                var hitElement = target.Element.HitTestDeep(e.Location);
                if (hitElement != null)
                {
                    _activeTarget = target;
                    _dragHandler.OnMouseDown(e);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Call on mouse move. ScrollDelta is updated only after threshold exceeded.
        /// </summary>
        public bool OnMouseMove(MouseEventArgs e)
        {
            bool handled = _dragHandler.OnMouseMove(e);
            return handled;
        }

        /// <summary>
        /// Call on mouse release. Computes inertial velocity if drag was valid.
        /// </summary>
        public bool OnMouseUp(MouseEventArgs e)
        {
            bool handled = _dragHandler.OnMouseUp(e);
            return handled;
        }

        private void HandleDrag(Vector2F delta)
        {
            if (_activeTarget.Physics == null) return;

            var b = _activeTarget.Behavior;
            float dx = b.AllowDragX ? delta.X : 0;
            float dy = b.AllowDragY ? delta.Y : 0;

            _activeTarget.Physics.Position.Current += new Vector2F(dx, dy);
            _activeTarget.Physics.Velocity.Current = Vector2F.Zero;
        }

        /// <summary>
        /// Call on mouse wheels.
        /// </summary>
        public bool OnMouseWheel(MouseEventArgs e)
        {
            if (_activeTarget?.Physics == null || _activeTarget?.Behavior?.AllowWheel != true)
                return false;

            _activeTarget.Physics.Position.Current += new Vector2F(0, -e.Delta * 0.25f);
            return true;
        }

        /// <summary>
        /// Call on mouse clicks.
        /// </summary>
        public bool OnMouseClick(MouseEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Call at the end of frame to reset scroll delta after applying.
        /// </summary>
        public void ResetDelta()
        {
            if (_activeTarget?.Physics == null) return;

            // Only reset delta if not dragging
            if (!IsDragging)
            {
                _activeTarget.Physics.Velocity.Current = Vector2F.Zero;
            }
        }
        public void EndFrame() => ResetDelta();
        private class ScrollTarget
        {
            public UIElement Element;
            public Physics2D Physics;
            public Func<RectangleF> ViewportGetter;
            public ScrollBehavior Behavior;
        }

        public class ScrollBehavior
        {
            public bool AllowDragX { get; set; } = false;
            public bool AllowDragY { get; set; } = true;
            public bool AllowWheel { get; set; } = true;
        }

        public bool HasMovedEnoughToDrag() => _dragHandler.HasMovedEnoughToDrag;
        public float DragThreshold() => _dragHandler.DragThreshold;
    }
}
