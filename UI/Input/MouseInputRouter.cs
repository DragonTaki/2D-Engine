/* ----- ----- ----- ----- */
// MouseInputRouter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2026/10/02
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Engine.Platform;
using Engine.UI.Core.Elements;

namespace Engine.UI.Input
{
    /// <summary>
    /// Centralized mouse input router. Delegates mouse events to the UI root element
    /// and optional scroll input handlers. Supports unified control of drag, scroll,
    /// and click interactions.
    /// </summary>
    public class MouseInputRouter : IInputHandler
    {
        #region Fields and Properties

        /// <summary>
        /// The root UI element that receives mouse events. May be null (e.g. before the UI
        /// tree is built): events then reach only the input handlers, like in <c>UIManager</c>.
        /// </summary>
        public UIElement Root { get; set; }

        /// <summary>
        /// List of additional input handlers to forward mouse events.
        /// Typically includes scroll input handlers.
        /// </summary>
        private readonly List<IInputHandler> _handlers = new();

        /// <summary>
        /// Optional scroll handler controlling vertical scroll interactions.
        /// </summary>
        private readonly IScrollInputHandler _scrollHandler;

        /// <summary>
        /// When true, suppresses further UI mouse event delivery during drag.
        /// </summary>
        public bool SuppressUIWhenDragging { get; set; } = true;

        /// <summary>
        /// Internal flag: drag process has started.
        /// </summary>
        private bool _dragStarted = false;

        /// <summary>
        /// Internal flag: user has moved enough to be considered a drag.
        /// </summary>
        private bool _hasDragged = false;

        /// <summary>
        /// Public flag indicating whether a drag is active or in progress.
        /// </summary>
        public bool IsDragging => _dragStarted || _hasDragged;

        /// <summary>
        /// The UI element that was pressed during MouseDown.
        /// Used to forward mouse up and click events correctly.
        /// </summary>
        private UIElement _pressedElement = null;

        /// <summary>
        /// The pressed element's handler while it has captured the pointer (a press-drag-release
        /// it takes for itself, see <see cref="IPointerCaptureTarget"/>), or null.
        /// </summary>
        private IPointerCaptureTarget _capture = null;

        /// <summary>Whether an element has captured the pointer (from its press until the release).</summary>
        public bool HasCapture => _capture != null;

        #endregion

        #region Constructor

#nullable enable
        /// <summary>
        /// Initializes a new instance of MouseInputRouter.
        /// </summary>
        /// <param name="root">The root UI element to forward mouse events to.</param>
        /// <param name="scroll">Optional scroll input handler for drag/scroll support.</param>
        public MouseInputRouter(UIElement root, IScrollInputHandler? scroll = null)
#nullable disable
        {
            Root = root;
            _scrollHandler = scroll;

            if (_scrollHandler != null)
                _handlers.Add(_scrollHandler);
        }

        #endregion

        #region Handler Management

        /// <summary>
        /// Adds an additional input handler to receive forwarded mouse events.
        /// </summary>
        /// <param name="handler">The input handler to add.</param>
        public void AddHandler(IInputHandler handler) => _handlers.Add(handler);

        #endregion

        #region Mouse Event Handlers

        /// <summary>
        /// Drops all in-progress press/drag state without delivering MouseUp or a click.
        /// Call when the window loses focus: the matching MouseUp won't arrive.
        /// </summary>
        public void CancelInput()
        {
            ReleaseCapture();
            _scrollHandler?.CancelDrag();
            _pressedElement = null;
            _dragStarted = false;
            _hasDragged = false;
        }

        /// <summary>
        /// Handles the MouseDown event: triggers scroll/drag detection and forwards to UI.
        /// </summary>
        /// <param name="e">Mouse event arguments containing location and button info.</param>
        /// <returns>True if any handler or UI element processed the event; otherwise false.</returns>
        public bool OnMouseDown(IMouseEvent e)
        {
            _dragStarted = false;
            _hasDragged = false;
            //Console.WriteLine($"[MouseDown] MouseDown start");

            bool handled = false;
            ReleaseCapture();

            // A press on an element that captures the pointer (e.g. a slider) goes to that element
            // only: the other handlers - the drag-scroll - never see it, so the page doesn't scroll.
            var hit = (UIElement)Root?.HitTestDeep(e.Location);
            if (hit?.HandlerBase is IPointerCaptureTarget capture && capture.CapturesPress(e.Location))
            {
                _capture = capture;
                _pressedElement = hit;
                _pressedElement.OnMouseDown(e);
                return true;
            }

            // Forward MouseDown to other registered input handlers first
            foreach (var h in _handlers)
            {
                if (h.OnMouseDown(e))
                {
                    handled = true;
                    break;
                }
            }

            // The element under the press (hit-tested above, before any handler ran)
            _pressedElement = hit;
            //Console.WriteLine($"[MouseDown] _pressedElement = {_pressedElement?.GetType().Name}");

            // Forward MouseDown to the pressed element if not handled by other handlers
            if (!handled && _pressedElement != null)
            {
                _pressedElement.OnMouseDown(e);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the MouseMove event: updates scroll physics and forwards to UI.
        /// Suppresses UI events when dragging if configured.
        /// </summary>
        /// <param name="e">Mouse event arguments with location.</param>
        /// <returns>True if the event was handled by scroll or UI; otherwise false.</returns>
        public bool OnMouseMove(IMouseEvent e)
        {
            // While captured, every move goes straight to the capturing element (wherever the
            // mouse is), and nothing else sees it.
            if (_capture != null)
            {
                _pressedElement?.OnMouseMove(e);
                return true;
            }

            //Console.WriteLine($"[MouseMove] _dragStarted = {_dragStarted}, _hasDragged = {_hasDragged}");
            // Event _handlers first, then we know if is dragging or not
            foreach (var h in _handlers)
            {
                if (h.OnMouseMove(e))
                    break;
            }

            // If configured, suppress UI interaction while dragging. Decided from the
            // scroll handler's drag state rather than from `handled`: a drag past the
            // threshold is exactly the case the handlers report as handled.
            if (SuppressUIWhenDragging)
            {
                if (!_dragStarted && _scrollHandler?.IsDragging == true && _scrollHandler.HasMovedEnoughToDrag())
                {
                    _hasDragged = true;
                    _dragStarted = true;
                    return true;  // suppress UI
                }
                if (_dragStarted && _scrollHandler.IsDraggingWithinActiveTarget(e.Location))
                {
                    return true;  // still dragging within target
                }
            }

            // Then process the UI mouse event
            if (Root?.OnMouseMove(e) == true)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the MouseUp event: releases scroll drag and forwards to UI.
        /// Determines whether a click should be processed based on drag state.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>Always returns true as the event is consumed.</returns>
        public bool OnMouseUp(IMouseEvent e)
        {
            // Release the input handlers regardless of what was pressed
            foreach (var h in _handlers)
                h.OnMouseUp(e);

            // Determine if click should be blocked due to drag
            bool blockClick = SuppressUIWhenDragging && _dragStarted && _hasDragged;
            //Console.WriteLine($"[MouseUp] _pressedElement = {_pressedElement?.GetType().Name}, blockClick = {blockClick}");

            if (_pressedElement != null)
            {
                // Forward MouseUp to pressed element
                _pressedElement.OnMouseUp(e);

                // Only trigger click if not blocked and cursor still over element
                if (!blockClick && _pressedElement.HitTest(e.Location))
                {
                    _pressedElement.HandlerBase?.HandleMouseClick(e);
                }

                _pressedElement = null;  // reset
            }

            // The release was delivered: the capture (if any) is over.
            ReleaseCapture();

            // Reset drag state (both flags: IsDragging stayed true after the release until
            // the next MouseDown when only _dragStarted was cleared here)
            _dragStarted = false;
            _hasDragged = false;
            return true;
        }

        /// <summary>
        /// Handles the MouseWheel event: forwards to UI first, then other handlers.
        /// </summary>
        /// <param name="e">Mouse event arguments containing wheel delta.</param>
        /// <returns>True if the event was handled by any handler; otherwise false.</returns>
        public bool OnMouseWheel(IMouseEvent e)
        {
            // No wheel scrolling in the middle of a captured drag (e.g. under a slider's knob).
            if (_capture != null)
                return true;

            // Process UI mouse event first
            if (Root?.OnMouseWheel(e) == true)
                return true;

            // Then the other mouse event _handlers
            foreach (var h in _handlers)
                if (h.OnMouseWheel(e)) return true;

            return false;
        }

        /// <summary>
        /// Handles the MouseClick event from Windows.
        /// Click is managed manually via MouseUp to differentiate click vs drag.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>Always returns false; handled manually in OnMouseUp.</returns>
        public bool OnMouseClick(IMouseEvent e)
        {
            // Windows mouse event
            // If [press -> drag -> release]
            // Windows will give [mouse down -> mouse move -> mouse click + mouse up]
            // If [click]
            // Windows will give [mouse click + mouse up]

            // Thus manual disable mouse click action
            // All handling by mouse up, it will decided user is drag or click

            return false;
        }

        /// <summary>Ends the pointer capture (if any) and tells the capturing handler.</summary>
        private void ReleaseCapture()
        {
            var capture = _capture;
            _capture = null;
            capture?.OnCaptureLost();
        }

        #endregion

        #region Frame Management

        /// <summary>
        /// Called every frame after input processing to reset per-frame scroll or mouse state.
        /// </summary>
        public void EndFrame()
        {
            // End the frame for the UI tree first
            Root?.EndFrame();

            // Reset additional handlers
            foreach (var h in _handlers) h?.EndFrame();
        }

        #endregion
    }
}
