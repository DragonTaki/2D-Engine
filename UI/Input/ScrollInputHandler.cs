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
using System.Diagnostics;
using System.Drawing;
using System.Linq;

using Engine.Mathematics;
using Engine.Physics;
using Engine.Platform;
using Engine.UI.Core.Bases;

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
        /// Registration counter: the tie-breaker among targets of equal z-index (later = on top).
        /// </summary>
        private int _nextTargetOrder;

        // Recent drag steps, for the release velocity. Only the last ReleaseVelocityWindowSeconds
        // counts: if the pointer stopped before being released, there's no fling.
        private readonly List<(long Timestamp, Vector2F Delta)> _dragSamples = new();
        private const double ReleaseVelocityWindowSeconds = 0.08;

        /// <summary>
        /// Drag helper managing threshold, delta, and movement state.
        /// </summary>
        private readonly DragHandler _dragHandler;

        /// <summary>
        /// Whether the user is currently performing a drag gesture (delegates to the drag helper).
        /// </summary>
        public bool IsDragging => _dragHandler.IsDragging;

        /// <summary>
        /// Handler-wide Z-index required by <see cref="IScrollInputHandler"/>. Not read by this
        /// class: the priority among overlapping targets is the per-target zIndex given to
        /// <see cref="RegisterScrollTarget"/>.
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

            // Ensure velocity is reset on active target (if any). Note: _activeTarget is always
            // null while constructing, so this currently never does anything.
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

            // zIndex is this target's priority among overlapping targets. It used to be
            // written to the handler-wide ZIndex (overwritten by every registration) and
            // never read, so target priority was just reverse registration order.
            _scrollTargets.Add(new ScrollTarget
            {
                Element = element,
                Physics = physics,
                ViewportGetter = viewportGetter,
                Behavior = behavior ?? new ScrollBehavior(),
                ZIndex = zIndex,
                Order = _nextTargetOrder++
            });
        }

        /// <summary>
        /// Removes a scroll target; clears it as the active target if it was one.
        /// </summary>
        public void UnregisterScrollTarget(UIElementBase element)
        {
            if (_activeTarget?.Element == element)
            {
                _dragHandler.Cancel();
                _activeTarget = null;
            }

            _scrollTargets.RemoveAll(t => t.Element == element);
        }

        #endregion

        #region Mouse Event Handlers

        /// <summary>
        /// Determines if the mouse is currently dragging within the active scroll target's viewport.
        /// Used to optionally suppress UI events.
        /// </summary>
        /// <param name="location">Mouse location in screen coordinates.</param>
        /// <returns>True if dragging within the active target; otherwise false.</returns>
        public bool IsDraggingWithinActiveTarget(Vector2F location)
        {
            return IsDragging &&
                _activeTarget?.ViewportGetter().Contains(location) == true;
        }

        /// <summary>
        /// Handles MouseDown: begins drag detection for the scroll target under the cursor.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if a scroll target was activated; otherwise false.</returns>
        public bool OnMouseDown(IMouseEvent e)
        {
            // Cancel any previous drag if active
            if (IsDragging)
            {
                _dragHandler.OnMouseUp(e);
                _activeTarget = null;
            }

            var target = FindTargetAt(e.Location);
            if (target == null)
                return false;

            _activeTarget = target;
            _dragSamples.Clear();
            target.Behavior?.OnPress?.Invoke();
            _dragHandler.OnMouseDown(e);
            return true;
        }

        private static bool IsSelfOrDescendant(UIElementBase element, UIElementBase ancestor)
        {
            for (var e = element; e != null; e = e.Parent)
            {
                if (e == ancestor)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the top-most interactable scroll target whose viewport contains the point, or null.
        /// </summary>
        private ScrollTarget FindTargetAt(Vector2F location)
        {
            // Top-most first: higher ZIndex wins, then the later-registered target.
            foreach (var target in _scrollTargets
                .OrderByDescending(t => t.ZIndex)
                .ThenByDescending(t => t.Order))
            {
                // Skip targets that aren't interactable (hidden, disabled or not displayed) or aren't attached to a parent
                if (!target.Element.IsInteractable || target.Element.Parent == null)
                    continue;

                var bounds = target.ViewportGetter();
                if (bounds == RectangleF.Empty || !bounds.Contains(location))
                    continue;

                // Hit test from the root, and accept only if the top-most element under the
                // point belongs to this target. Testing just the target's own subtree ignored
                // anything drawn above it (e.g. an open dialog and its mask), so a press there
                // started a drag-scroll underneath and swallowed the press.
                var hit = target.Element.GetRoot().HitTestDeep(location);
                if (hit != null && IsSelfOrDescendant(hit, target.Element))
                    return target;
            }

            return null;
        }

        /// <summary>
        /// Handles MouseMove: updates drag and applies delta if threshold exceeded.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if drag moved enough to update the target; otherwise false.</returns>
        public bool OnMouseMove(IMouseEvent e)
        {
            bool handled = _dragHandler.OnMouseMove(e);
            return handled;
        }

        /// <summary>
        /// Handles MouseUp: ends drag and computes inertial velocity.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>Always true, as <see cref="DragHandler.OnMouseUp"/> always reports the release as handled.</returns>
        public bool OnMouseUp(IMouseEvent e)
        {
            var target = _activeTarget;
            bool wasDragging = _dragHandler.IsDragging && _dragHandler.HasMovedEnoughToDrag;

            bool handled = _dragHandler.OnMouseUp(e);

            if (wasDragging && target != null)
                target.Behavior?.OnRelease?.Invoke(ComputeReleaseVelocity());
            _dragSamples.Clear();

            return handled;
        }

        /// <summary>
        /// Average drag velocity (units/second) over the last ReleaseVelocityWindowSeconds.
        /// </summary>
        private Vector2F ComputeReleaseVelocity()
        {
            long now = Stopwatch.GetTimestamp();
            long windowStart = now - (long)(ReleaseVelocityWindowSeconds * Stopwatch.Frequency);

            float sumX = 0f, sumY = 0f;
            long oldest = now;
            foreach (var (timestamp, delta) in _dragSamples)
            {
                if (timestamp < windowStart)
                    continue;
                sumX += delta.X;
                sumY += delta.Y;
                oldest = Math.Min(oldest, timestamp);
            }

            // At least one 60 Hz frame of span, so a single last-moment step can't yield
            // an absurd speed.
            double span = Math.Max((now - oldest) / (double)Stopwatch.Frequency, 1.0 / 60.0);
            return new Vector2F((float)(sumX / span), (float)(sumY / span));
        }

        /// <summary>
        /// Handles mouse wheel input for the scroll target under the cursor.
        /// </summary>
        /// <param name="e">Mouse wheel event arguments.</param>
        /// <returns>True if the event affected a target; otherwise false.</returns>
        public bool OnMouseWheel(IMouseEvent e)
        {
            // The wheel scrolls what's under the cursor, not whichever target was last pressed.
            var target = FindTargetAt(e.Location);
            if (target?.Physics == null || target.Behavior?.AllowWheel != true)
                return false;

            target.Behavior.OnPress?.Invoke();

            // One event scrolls at most one notch. macOS accelerates wheel input (a fast
            // spin reports several notches per event), which made the wheel scroll far
            // faster than dragging; WinForms reports one notch (120) per event anyway.
            float notches = Math.Clamp(e.Delta / (float)WheelNotchDelta, -1f, 1f);
            target.Physics.Position.Current += new Vector2F(0, -notches * WheelStep);
            return true;
        }

        /// <summary>Wheel delta of one notch (the WinForms convention, which the backends follow).</summary>
        private const int WheelNotchDelta = 120;

        /// <summary>
        /// Scroll distance per wheel notch, in UI design units. 30 is the engine's own
        /// default; a host may set it at registration (the launchers set it from the
        /// player settings, docs/SETTINGS.md).
        /// </summary>
        public float WheelStep { get; set; } = 30f;

        /// <summary>
        /// Handles MouseClick. This handler does not process clicks directly.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>Always returns false; clicks handled elsewhere.</returns>
        public bool OnMouseClick(IMouseEvent e)
        {
            return false;
        }

        #endregion

        #region Drag Handling

        /// <summary>
        /// Internal callback when drag occurs, updates scroll target position.
        /// </summary>
        /// <param name="delta">The drag delta vector since the previous mouse move event.</param>
        private void HandleDrag(Vector2F delta)
        {
            if (_activeTarget.Physics == null) return;

            var b = _activeTarget.Behavior;

            float dx = b.AllowDragX ? delta.X : 0;
            float dy = b.AllowDragY ? delta.Y : 0;

            // Apply delta directly to Physics position
            _activeTarget.Physics.Position.Current += new Vector2F(dx, dy);

            long now = Stopwatch.GetTimestamp();
            _dragSamples.Add((now, new Vector2F(dx, dy)));
            long windowStart = now - (long)(ReleaseVelocityWindowSeconds * Stopwatch.Frequency);
            _dragSamples.RemoveAll(s => s.Timestamp < windowStart);

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

            // Only reset the velocity if not dragging
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
        /// Abandons any drag in progress (see <see cref="DragHandler.Cancel"/>).
        /// </summary>
        public void CancelDrag()
        {
            _dragHandler.Cancel();
            _dragSamples.Clear();
        }

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
            public int ZIndex;
            public int Order;
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

            /// <summary>
            /// Called when a drag on this target is released, with the drag velocity at that
            /// moment (units per second) - e.g. to start a short inertia animation.
            /// </summary>
            public Action<Vector2F> OnRelease { get; set; }

            /// <summary>
            /// Called when this target is pressed or wheel-scrolled - e.g. to stop inertia.
            /// </summary>
            public Action OnPress { get; set; }
        }

        #endregion
    }
}
