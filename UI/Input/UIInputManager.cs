/* ----- ----- ----- ----- */
// UIInputManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Engine.Platform;
using Engine.UI.Core.Elements;

namespace Engine.UI.Input
{
    /// <summary>
    /// Centralized manager for UI input handling. 
    /// Aggregates multiple IInputHandler instances and routes mouse events to them in order.
    /// Typically uses a MouseInputRouter for unified drag, scroll, and click processing.
    /// </summary>
    public class UIInputManager : IInputHandler
    {
        #region Fields and Properties

        /// <summary>
        /// The primary mouse input router responsible for delegating mouse events
        /// to UI root elements and scroll handlers.
        /// </summary>
        public MouseInputRouter MouseRouter { get; }

        /// <summary>
        /// Additional registered general input handlers to receive forwarded events.
        /// </summary>
        private readonly List<IInputHandler> _generalHandlers = new();

        #endregion

        #region Constructor

#nullable enable
        /// <summary>
        /// Initializes a new UIInputManager with a UI root element and optional scroll handler.
        /// </summary>
        /// <param name="root">The root UIElement to route mouse events to.</param>
        /// <param name="scroll">Optional scroll input handler for drag/scroll support.</param>
        public UIInputManager(UIElement root, IScrollInputHandler? scroll = null)
#nullable disable
        {
            // Create the centralized mouse router. It is called directly below and owns the
            // scroll handler, so neither is registered as a general handler as well: that made
            // Root.EndFrame() run twice and the scroll handler's EndFrame three times a frame.
            MouseRouter = new MouseInputRouter(root, scroll);
        }

        #endregion

        /// <summary>
        /// Drops all in-progress press/drag state (see <see cref="MouseInputRouter.CancelInput"/>).
        /// </summary>
        public void CancelInput() => MouseRouter.CancelInput();

        #region Handler Management

        /// <summary>
        /// Registers an input handler to receive mouse events.
        /// </summary>
        /// <param name="handler">The input handler to register.</param>
        public void RegisterHandler(IInputHandler handler)
        {
            if (!_generalHandlers.Contains(handler))
                _generalHandlers.Add(handler);
        }

        /// <summary>
        /// Unregisters a previously registered input handler.
        /// </summary>
        /// <param name="handler">The input handler to remove.</param>
        public void UnregisterHandler(IInputHandler handler)
        {
            _generalHandlers.Remove(handler);
        }

        #endregion

        #region Mouse Event Routing

        /// <summary>
        /// Processes MouseDown events: forwards to the mouse router, then to every registered general handler.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the router or any general handler handled the event.</returns>
        public bool OnMouseDown(IMouseEvent e)
        {
            bool handled = MouseRouter.OnMouseDown(e);

            // Every registered general handler observes every event (a handler that missed
            // e.g. MouseUp because the router consumed it would be left mid-gesture).
            foreach (var h in _generalHandlers.ToArray())
                handled |= h.OnMouseDown(e);

            return handled;
        }

        /// <summary>
        /// Processes MouseMove events: forwards to the mouse router, then to every registered general handler.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the router or any general handler handled the event.</returns>
        public bool OnMouseMove(IMouseEvent e)
        {
            bool handled = MouseRouter.OnMouseMove(e);

            // Every registered general handler observes every event (a handler that missed
            // e.g. MouseUp because the router consumed it would be left mid-gesture).
            foreach (var h in _generalHandlers.ToArray())
                handled |= h.OnMouseMove(e);

            return handled;
        }

        /// <summary>
        /// Processes MouseUp events: forwards to the mouse router, then to every registered general handler.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the router or any general handler handled the event.</returns>
        public bool OnMouseUp(IMouseEvent e)
        {
            bool handled = MouseRouter.OnMouseUp(e);

            // Every registered general handler observes every event (a handler that missed
            // e.g. MouseUp because the router consumed it would be left mid-gesture).
            foreach (var h in _generalHandlers.ToArray())
                handled |= h.OnMouseUp(e);

            return handled;
        }

        /// <summary>
        /// Processes MouseClick events: forwards to the mouse router, then to every registered general handler.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the router or any general handler handled the event.</returns>
        public bool OnMouseClick(IMouseEvent e)
        {
            bool handled = MouseRouter.OnMouseClick(e);

            // Every registered general handler observes every event (a handler that missed
            // e.g. MouseUp because the router consumed it would be left mid-gesture).
            foreach (var h in _generalHandlers.ToArray())
                handled |= h.OnMouseClick(e);

            return handled;
        }

        /// <summary>
        /// Processes MouseWheel events: forwards to the mouse router, then to every registered general handler.
        /// </summary>
        /// <param name="e">Mouse wheel event arguments.</param>
        /// <returns>True if the router or any general handler handled the event.</returns>
        public bool OnMouseWheel(IMouseEvent e)
        {
            bool handled = MouseRouter.OnMouseWheel(e);

            // Every registered general handler observes every event (a handler that missed
            // e.g. MouseUp because the router consumed it would be left mid-gesture).
            foreach (var h in _generalHandlers.ToArray())
                handled |= h.OnMouseWheel(e);

            return handled;
        }

        #endregion

        #region Frame Management

        /// <summary>
        /// Called every frame to reset input state and allow per-frame updates in handlers.
        /// </summary>
        public void EndFrame()
        {
            MouseRouter.EndFrame();

            // Update all general handlers per-frame
            foreach (var h in _generalHandlers)
                h.EndFrame();
        }

        #endregion
    }
}
