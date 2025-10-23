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
    /// Handles mouse-based scroll input, including drag detection, threshold filtering,
    /// and inertial velocity output. This class does not directly update any physics system.
    /// </summary>
    public class DragHandler : IInputHandler
    {
        /// <summary>Indicates whether the user is currently dragging.</summary>
        public bool IsDragging { get; private set; } = false;

        /// <summary>Indicates whether movement passed drag threshold.</summary>
        public bool HasMovedEnoughToDrag = false;
        private Point _dragStartPoint;
        private Point _dragLastPoint;
        public bool DragDistXOverThreshold => Math.Abs(_dragLastPoint.X - _dragStartPoint.X) > DragThreshold;
        public bool DragDistYOverThreshold => Math.Abs(_dragLastPoint.Y - _dragStartPoint.Y) > DragThreshold;
        private DateTime _dragStartTime;
        private DateTime _dragLastTime;
        public bool DragTimeOverThreshold => (_dragLastTime - _dragStartTime).TotalMilliseconds > DragTimeThreshold;
        private float _totalDragDistance = 0.0f;

        /// <summary>Drag distance threshold (in pixels) to begin scrolling.</summary>
        public float DragThreshold { get; set; } = 5.0f;

        /// <summary>Maximum time (ms) under which tiny movement is treated as click.</summary>
        public float DragTimeThreshold { get; set; } = 160.0f;

        public event Action<Vector2F> OnDrag;     // (start, current)
        public event Action<Point> OnClick;           // if determined as click

        /// <summary>
        /// Call when mouse button is pressed to begin scroll detection.
        /// </summary>
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
        /// Call on mouse move. ScrollDelta is updated only after threshold exceeded.
        /// </summary>
        public bool OnMouseMove(MouseEventArgs e)
        {
            if (!IsDragging)
                return true;

            float deltaX = e.X - _dragLastPoint.X;
            float deltaY = e.Y - _dragLastPoint.Y;
            Vector2F delta = new Vector2F(deltaX, deltaY);
            float deltaLength = MathF.Sqrt(MathF.Pow(deltaX, 2) + MathF.Pow(deltaY, 2));
            _totalDragDistance += deltaLength;

            // If move too small, don't give movement yet
            if (!HasMovedEnoughToDrag)
            {
                if (_totalDragDistance >= DragThreshold)
                {
                    HasMovedEnoughToDrag = true;
                }
                else
                {
                    return false; // Do not start scrolling yet
                }
            }

            OnDrag?.Invoke(delta);
            _dragLastPoint = e.Location;

            return true;
        }

        /// <summary>
        /// Call on mouse release. Computes inertial velocity if drag was valid.
        /// </summary>
        public bool OnMouseUp(MouseEventArgs e)
        {
            if (!IsDragging)
                return true;

            IsDragging = false;
            
            _dragLastPoint = e.Location;
            _dragLastTime = DateTime.Now;

            bool isClick = !HasMovedEnoughToDrag ||
                (!DragDistYOverThreshold && !DragTimeOverThreshold);

            HasMovedEnoughToDrag = false;

            if (isClick)
            {
                // Treat as click: no velocity
                OnClick?.Invoke(e.Location);
            }

            return true;
        }

        public bool OnMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        public bool OnMouseClick(MouseEventArgs e)
        {
            return false;
        }

        public void EndFrame()
        {
            // Optionally override in subclass
        }
    }
}
