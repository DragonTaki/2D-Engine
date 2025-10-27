/* ----- ----- ----- ----- */
// DragHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Windows.Forms;

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
        private Point _dragStartPoint;

        /// <summary>Stores the last recorded mouse position during dragging.</summary>
        private Point _dragLastPoint;

        /// <summary>Records when the mouse button was first pressed.</summary>
        private DateTime _dragStartTime;

        /// <summary>Records the timestamp of the last movement or release event.</summary>
        private DateTime _dragLastTime;

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
        public bool DragTimeOverThreshold => (_dragLastTime - _dragStartTime).TotalMilliseconds > DragTimeThreshold;

        #endregion

        #region Configuration : Thresholds

        /// <summary>
        /// Minimum movement distance (in pixels) required before a drag is recognized.
        /// Prevents small mouse jitters from being treated as drag input.
        /// </summary>
        public float DragThreshold { get; set; } = 5.0f;

        /// <summary>
        /// Maximum duration (in milliseconds) allowed for a click action.
        /// If the user releases the mouse within this time and without exceeding the distance threshold, it counts as a click.
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
        /// Provides the click location as a <see cref="Point"/>.
        /// </summary>
        public event Action<Point> OnClick;

        #endregion

        #region Methods : Input Handlers

        /// <summary>
        /// Called when the mouse button is pressed down.
        /// Initializes internal states for drag detection.
        /// </summary>
        /// <param name="e">Mouse event arguments containing position and button information.</param>
        /// <returns>Always returns <c>true</c> to indicate event was handled.</returns>
        public bool OnMouseDown(MouseEventArgs e)
        {
            IsDragging = true;
            HasMovedEnoughToDrag = false;

            _dragStartPoint = e.Location;
            _dragLastPoint = e.Location;
            _dragStartTime = DateTime.Now;
            _dragLastTime = DateTime.Now;
            
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
        public bool OnMouseMove(MouseEventArgs e)
        {
            if (!IsDragging)
                return true;  // No active drag; ignore move

            float deltaX = e.X - _dragLastPoint.X;
            float deltaY = e.Y - _dragLastPoint.Y;

            // Compute length of this movement step
            Vector2F delta = new Vector2F(deltaX, deltaY);
            float deltaLength = MathF.Sqrt(MathF.Pow(deltaX, 2) + MathF.Pow(deltaY, 2));
            _totalDragDistance += deltaLength;  // accumulate distance

            // If move too small, don't give movement yet
            if (!HasMovedEnoughToDrag)
            {
                if (_totalDragDistance >= DragThreshold)
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
            _dragLastPoint = e.Location;

            return true;
        }

        /// <summary>
        /// Called when the mouse button is released.
        /// Determines whether the action was a click or a completed drag.
        /// </summary>
        /// <param name="e">Mouse event arguments containing the release position.</param>
        /// <returns>Always returns <c>true</c> to indicate event was handled.</returns>
        public bool OnMouseUp(MouseEventArgs e)
        {
            if (!IsDragging)
                return true;  // Nothing to end

            IsDragging = false;
            
            _dragLastPoint = e.Location;
            _dragLastTime = DateTime.Now;

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
        /// Called when the mouse wheel is scrolled.
        /// Currently not handled in this class.
        /// </summary>
        /// <param name="e">Mouse wheel event arguments.</param>
        /// <returns>Always returns <c>false</c> since this class does not process wheel input.</returns>
        public bool OnMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Called when a mouse click event occurs directly.
        /// Currently not used; click is handled through <see cref="OnMouseUp"/>.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>Always returns <c>false</c> since direct click event is not processed.</returns>
        public bool OnMouseClick(MouseEventArgs e)
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
