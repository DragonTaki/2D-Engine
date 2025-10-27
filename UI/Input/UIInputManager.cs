/* ----- ----- ----- ----- */
// UIInputManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Windows.Forms;

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
            // Create the centralized mouse router
            MouseRouter = new MouseInputRouter(root, scroll);

            // Register mouse router itself to receive general event processing
            RegisterHandler(MouseRouter);  // Add mouse router itself to the general processor

            // Optionally register scroll handler as general input
            if (scroll != null)
                RegisterHandler(scroll);
        }

        #endregion

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
        /// Processes MouseDown events by forwarding to the mouse router.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled by the router.</returns>
        public bool OnMouseDown(MouseEventArgs e)
        {
            return MouseRouter.OnMouseDown(e);
        }

        /// <summary>
        /// Processes MouseMove events by forwarding to the mouse router.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled by the router.</returns>
        public bool OnMouseMove(MouseEventArgs e)
        {
            return MouseRouter.OnMouseMove(e);
        }

        /// <summary>
        /// Processes MouseUp events by forwarding to the mouse router.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled by the router.</returns>
        public bool OnMouseUp(MouseEventArgs e)
        {
            return MouseRouter.OnMouseUp(e);
        }

        /// <summary>
        /// Processes MouseClick events by forwarding to the mouse router.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if the event was handled by the router.</returns>
        public bool OnMouseClick(MouseEventArgs e)
        {
            return MouseRouter.OnMouseClick(e);
        }

        /// <summary>
        /// Processes MouseWheel events by forwarding to the mouse router.
        /// </summary>
        /// <param name="e">Mouse wheel event arguments.</param>
        /// <returns>True if the event was handled by the router.</returns>
        public bool OnMouseWheel(MouseEventArgs e)
        {
            return MouseRouter.OnMouseWheel(e);
        }

        #endregion

        #region Event Handler Wrappers (for WinForms)

        /// <summary>
        /// Wrapper for MouseDown event from WinForms.
        /// </summary>
        public void ProcessMouseDown (object s, MouseEventArgs e)=> OnMouseDown(e);

        /// <summary>
        /// Wrapper for MouseMove event from WinForms.
        /// </summary>
        public void ProcessMouseMove(object s, MouseEventArgs e) => OnMouseMove(e);

        /// <summary>
        /// Wrapper for MouseUp event from WinForms.
        /// </summary>
        public void ProcessMouseUp(object s, MouseEventArgs e) => OnMouseUp(e);

        /// <summary>
        /// Wrapper for MouseClick event from WinForms.
        /// </summary>
        public void ProcessMouseClick(object s, MouseEventArgs e) => OnMouseClick(e);

        /// <summary>
        /// Wrapper for MouseWheel event from WinForms.
        /// </summary>
        public void ProcessMouseWheel(object s, MouseEventArgs e) => OnMouseWheel(e);

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
