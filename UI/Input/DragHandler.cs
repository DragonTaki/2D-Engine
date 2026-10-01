/* ----- ----- ----- ----- */
// DragHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Diagnostics;
using System.Drawing;
using Engine.Platform;

using Engine.Mathematics;

namespace Engine.UI.Input
{
    /// <summary>
    /// Handles mouse-based drag input including:
    /// - Detecting drag gesture from click-and-move
    /// - Applying movement threshold filtering to prevent false triggers
    /// - Emitting drag deltas via <see cref="OnDrag"/>
    /// - Detecting click if movement and time thresholds are not exceeded
    /// This class does not directly modify or update any physics system.
    /// </summary>
    public class DragHandler : IInputHandler
    {
        #region Fields : Internal States

        /// <summary>Indicates whether the user is currently holding down the mouse and dragging.</summary>
        public bool IsDragging { get; private set; } = false;

        /// <summary>Indicates whether the current drag distance has exceeded the configured threshold.</summary>
        public bool HasMovedEnoughToDrag = false;

        /// <summary>Stores the initial point where the mouse was pressed down.</summary>
        private Vector2F _dragStartPoint = Vector2F.Zero;

        /// <summary>Stores the last recorded mouse position during dragging.</summary>
        private Vector2F _dragLastPoint = Vector2F.Zero;

        /// <summary>Records when the mouse button was first pressed.</summary>
        // Stopwatch timestamps: monotonic, unlike DateTime.Now (wall clock), which can jump
        // with clock/DST adjustments and has coarse resolution on some platforms.
        private long _dragStartTime;

        /// <summary>Records the timestamp of the last movement or release event.</summary>
        private long _dragLastTime;

        /// <summary>Tracks the total accumulated movement distance since drag start.</summary>
        private float _totalDragDistance = 0.0f;

        #endregion

        #region Properties : Computed Threshold Checks

        /// <summary>
        /// Checks if the horizontal drag distance has exceeded the defined threshold.
        /// </summary>
        public bool DragDistXOverThreshold => Math.Abs(_dragLastPoint.X - _dragStartPoint.X) > DragThreshold;

        /// <summary>
        /// Checks if the vertical drag distance has exceeded the defined threshold.
        /// </summary>
        public bool DragDistYOverThreshold => Math.Abs(_dragLastPoint.Y - _dragStartPoint.Y) > DragThreshold;

        /// <summary>
        /// Checks if the total drag duration has exceeded the time threshold.
        /// </summary>
        public bool DragTimeOverThreshold => Stopwatch.GetElapsedTime(_dragStartTime, _dragLastTime).TotalMilliseconds > DragTimeThreshold;

        #endregion

        #region Configuration : Thresholds

        /// <summary>
        /// Minimum movement distance (in pixels) required before a drag is recognized.
        /// Prevents small mouse jitters from being treated as drag input.
        /// </summary>
        public float DragThreshold { get; set; } = 5.0f;

        /// <summary>
        /// Maximum duration (in milliseconds) for a press that has already moved past
        /// <see cref="DragThreshold"/> to still count as a click (together with a vertical
        /// distance within the threshold, see <see cref="OnMouseUp"/>). A press that never moved
        /// past the threshold is a click regardless of how long it was held.
        /// </summary>
        public float DragTimeThreshold { get; set; } = 160.0f;

        #endregion

        #region Events : Drag & Click

        /// <summary>
        /// Triggered continuously when dragging occurs and movement exceeds the threshold.
        /// Provides a <see cref="Vector2F"/> delta representing incremental motion.
        /// </summary>
        public event Action<Vector2F> OnDrag;

        /// <summary>
        /// Triggered when a mouse press and release is detected as a click (no drag occurred).
        /// Provides the click location as a <see cref="Vector2F"/>.
        /// </summary>
        public event Action<Vector2F> OnClick;

        #endregion

        #region Methods : Input Handlers

        /// <summary>
        /// Called when the mouse button is pressed down.
        /// Initializes internal states for drag detection.
        /// </summary>
        /// <param name="e">Mouse event arguments containing position and button information.</param>
        /// <returns>Always returns <c>true</c> to indicate event was handled.</returns>
        public bool OnMouseDown(IMouseEvent e)
        {
            IsDragging = true;
            HasMovedEnoughToDrag = false;

            // Copy: Vector2F is a mutable reference type.
            _dragStartPoint = new Vector2F(e.X, e.Y);
            _dragLastPoint = new Vector2F(e.X, e.Y);
            _dragStartTime = Stopwatch.GetTimestamp();
            _dragLastTime = _dragStartTime;
            
            _totalDragDistance = 0.0f;

            return true;
        }

        /// <summary>
        /// Called when the mouse moves. If dragging, this method calculates movement delta and triggers <see cref="OnDrag"/> when thresholds are met.
        /// </summary>
        /// <param name="e">Mouse event arguments containing the new cursor position.</param>
        /// <returns>
        /// <c>true</c> if drag movement is active and processed;  
        /// <c>false</c> if movement is below threshold and not yet considered as a drag.
        /// </returns>
        public bool OnMouseMove(IMouseEvent e)
        {
            if (!IsDragging)
                return false;  // No active drag; ignore move

            float deltaX = e.X - _dragLastPoint.X;
            float deltaY = e.Y - _dragLastPoint.Y;

            // Compute length of this movement step
            Vector2F delta = new Vector2F(deltaX, deltaY);
            float deltaLength = MathF.Sqrt(MathF.Pow(deltaX, 2) + MathF.Pow(deltaY, 2));
            _totalDragDistance += deltaLength;  // accumulate distance

            // If move too small, don't give movement yet. The threshold is measured as
            // displacement from the press point, not as a sum of per-event step lengths
            // (_dragLastPoint doesn't advance before the threshold, so summing those
            // would count the same displacement again on every event).
            if (!HasMovedEnoughToDrag)
            {
                float dispX = e.X - _dragStartPoint.X;
                float dispY = e.Y - _dragStartPoint.Y;
                if (MathF.Sqrt(dispX * dispX + dispY * dispY) >= DragThreshold)
                {
                    HasMovedEnoughToDrag = true;
                }
                else
                {
                    return false;  // Do not start scrolling yet
                }
            }

            // Emit drag delta event
            OnDrag?.Invoke(delta);

            // Update last position
            _dragLastPoint = new Vector2F(e.X, e.Y);

            return true;
        }

        /// <summary>
        /// Called when the mouse button is released.
        /// Determines whether the action was a click or a completed drag.
        /// </summary>
        /// <param name="e">Mouse event arguments containing the release position.</param>
        /// <returns>Always returns <c>true</c> to indicate event was handled.</returns>
        public bool OnMouseUp(IMouseEvent e)
        {
            if (!IsDragging)
                return true;  // Nothing to end

            IsDragging = false;
            
            _dragLastPoint = new Vector2F(e.X, e.Y);
            _dragLastTime = Stopwatch.GetTimestamp();

            // Determine if this was a click instead of a drag
            bool isClick = !HasMovedEnoughToDrag ||
                (!DragDistYOverThreshold && !DragTimeOverThreshold);

            HasMovedEnoughToDrag = false;

            if (isClick)
            {
                // Treat as click: no velocity or drag event
                OnClick?.Invoke(e.Location);
            }

            return true;
        }

        /// <summary>
        /// Abandons any drag in progress without emitting a click, e.g. when the window loses
        /// focus mid-drag and the matching MouseUp will never arrive (otherwise IsDragging
        /// stays true and plain mouse moves keep scrolling until the next MouseDown).
        /// </summary>
        public void Cancel()
        {
            IsDragging = false;
            HasMovedEnoughToDrag = false;
        }

        /// <summary>
        /// Called when the mouse wheel is scrolled.
        /// Currently not handled in this class.
        /// </summary>
        /// <param name="e">Mouse wheel event arguments.</param>
        /// <returns>Always returns <c>false</c> since this class does not process wheel input.</returns>
        public bool OnMouseWheel(IMouseEvent e)
        {
            return false;
        }

        /// <summary>
        /// Called when a mouse click event occurs directly.
        /// Currently not used; click is handled through <see cref="OnMouseUp"/>.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>Always returns <c>false</c> since direct click event is not processed.</returns>
        public bool OnMouseClick(IMouseEvent e)
        {
            return false;
        }

        /// <summary>
        /// Called at the end of each frame.
        /// This is a placeholder for potential state cleanup or extension by subclass.
        /// </summary>
        public void EndFrame()
        {
            // Optionally override in subclass
        }

        #endregion
    }
}
