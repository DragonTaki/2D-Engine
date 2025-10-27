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
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;

namespace Engine.UI.Input
{
    /// <summary>
    /// Handles mouse-based scroll input, including drag detection, threshold filtering,
    /// and inertial velocity output. This class does not directly update any physics system
    /// other than the assigned ScrollTarget.Physics.
    /// </summary>
    public sealed class ScrollInputHandler : IScrollInputHandler
    {
        #region Fields and Properties

        /// <summary>
        /// List of registered scroll targets that this handler can manipulate.
        /// </summary>
        private readonly List<ScrollTarget> _scrollTargets = new();

#nullable enable
        /// <summary>
        /// The currently active scroll target under drag.
        /// </summary>
        private ScrollTarget? _activeTarget = null;
#nullable disable

        /// <summary>
        /// Drag helper managing threshold, delta, and movement state.
        /// </summary>
        private readonly DragHandler _dragHandler;

        /// <summary>
        /// Drag helper managing threshold, delta, and movement state.
        /// </summary>
        public bool IsDragging => _dragHandler.IsDragging;

        /// <summary>
        /// Optional Z-order for overlapping scroll targets.
        /// </summary>
        public int ZIndex { get; set; } = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of ScrollInputHandler.
        /// </summary>
        public ScrollInputHandler()
        {
            _dragHandler = new DragHandler();
            _dragHandler.OnDrag += HandleDrag;

            // Ensure velocity is reset on active target (if any)
            if (_activeTarget?.Physics != null)
                _activeTarget.Physics.Velocity.Reset();
        }

        #endregion

        #region Scroll Target Management

        /// <summary>
        /// Registers a scroll target to this handler.
        /// </summary>
        /// <param name="element">The UI element that represents the scrollable area.</param>
        /// <param name="physics">The Physics2D object controlling position and velocity.</param>
        /// <param name="viewportGetter">Function that returns the visible viewport rectangle.</param>
        /// <param name="behavior">Optional ScrollBehavior controlling drag/wheel permissions.</param>
        /// <param name="zIndex">Optional z-order priority for overlapping targets.</param>
        public void RegisterScrollTarget(
            UIElementBase element,
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

        #endregion

        #region Mouse Event Handlers

        /// <summary>
        /// Determines if the mouse is currently dragging within the active scroll target's viewport.
        /// Used to optionally suppress UI events.
        /// </summary>
        /// <param name="location">Mouse location in screen coordinates.</param>
        /// <returns>True if dragging within the active target; otherwise false.</returns>
        public bool IsDraggingWithinActiveTarget(Point location)
        {
            return IsDragging &&
                _activeTarget?.ViewportGetter().Contains(location) == true;
        }

        /// <summary>
        /// Handles MouseDown: begins drag detection for the scroll target under the cursor.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if a scroll target was activated; otherwise false.</returns>
        public bool OnMouseDown(MouseEventArgs e)
        {
            // Cancel any previous drag if active
            if (IsDragging)
            {
                _dragHandler.OnMouseUp(e);
                _activeTarget = null;
            }

            // Iterate from top-most target (higher index) to bottom
            for (int i = _scrollTargets.Count - 1; i >= 0; i--)
            {
                var target = _scrollTargets[i];

                // If not IsVisible or not IsEnabled
                if (!target.Element.IsInteractable || target.Element.Parent == null)
                    continue;

                var bounds = target.ViewportGetter();
                if (bounds == RectangleF.Empty || !bounds.Contains(e.Location))
                    continue;

                // Hit test for child elements
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
        /// Handles MouseMove: updates drag and applies delta if threshold exceeded.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if drag moved enough to update the target; otherwise false.</returns>
        public bool OnMouseMove(MouseEventArgs e)
        {
            bool handled = _dragHandler.OnMouseMove(e);
            return handled;
        }

        /// <summary>
        /// Handles MouseUp: ends drag and computes inertial velocity.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if a drag was active and released; otherwise false.</returns>
        public bool OnMouseUp(MouseEventArgs e)
        {
            bool handled = _dragHandler.OnMouseUp(e);
            return handled;
        }

        /// <summary>
        /// Handles mouse wheel input for the active scroll target.
        /// </summary>
        /// <param name="e">Mouse wheel event arguments.</param>
        /// <returns>True if the event affected the active target; otherwise false.</returns>
        public bool OnMouseWheel(MouseEventArgs e)
        {
            if (_activeTarget?.Physics == null || _activeTarget?.Behavior?.AllowWheel != true)
                return false;

            _activeTarget.Physics.Position.Current += new Vector2F(0, -e.Delta * 0.25f);
            return true;
        }

        /// <summary>
        /// Handles MouseClick. This handler does not process clicks directly.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>Always returns false; clicks handled elsewhere.</returns>
        public bool OnMouseClick(MouseEventArgs e)
        {
            return false;
        }

        #endregion

        #region Drag Handling

        /// <summary>
        /// Internal callback when drag occurs, updates scroll target position.
        /// </summary>
        /// <param name="delta">The drag delta vector since last frame.</param>
        private void HandleDrag(Vector2F delta)
        {
            if (_activeTarget.Physics == null) return;

            var b = _activeTarget.Behavior;

            float dx = b.AllowDragX ? delta.X : 0;
            float dy = b.AllowDragY ? delta.Y : 0;

            // Apply delta directly to Physics position
            _activeTarget.Physics.Position.Current += new Vector2F(dx, dy);

            // Reset instantaneous velocity to zero while dragging
            _activeTarget.Physics.Velocity.Current = Vector2F.Zero;
        }

        #endregion

        #region Frame Management

        /// <summary>
        /// Resets velocity/delta at the end of the frame.
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

        /// <summary>
        /// Called every frame after input processing to reset per-frame scroll state.
        /// </summary>
        public void EndFrame() => ResetDelta();

        #endregion

        #region Utility

        /// <summary>
        /// Returns true if the drag has exceeded the configured threshold.
        /// </summary>
        public bool HasMovedEnoughToDrag() => _dragHandler.HasMovedEnoughToDrag;

        /// <summary>
        /// Returns the configured drag threshold for detection.
        /// </summary>
        public float DragThreshold() => _dragHandler.DragThreshold;

        #endregion

        #region Nested Types

        /// <summary>
        /// Represents a scrollable target tracked by this handler.
        /// </summary>
        private class ScrollTarget
        {
            public UIElementBase Element;
            public Physics2D Physics;
            public Func<RectangleF> ViewportGetter;
            public ScrollBehavior Behavior;
        }

        /// <summary>
        /// Behavior settings for a scroll target.
        /// </summary>

        public class ScrollBehavior
        {
            /// <summary>If true, allows horizontal drag movement.</summary>
            public bool AllowDragX { get; set; } = false;

            /// <summary>If true, allows vertical drag movement.</summary>
            public bool AllowDragY { get; set; } = true;

            /// <summary>If true, allows scrolling with mouse wheel.</summary>
            public bool AllowWheel { get; set; } = true;
        }

        #endregion
    }
}
