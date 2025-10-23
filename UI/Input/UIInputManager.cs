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
    public class UIInputManager : IInputHandler
    {
        /// <summary>
        /// Collect all IInputHandler handlers and dispatch them to mouse events in order.
        /// </summary>
        public MouseInputRouter MouseRouter { get; }
        private readonly List<IInputHandler> _generalHandlers = new();

#nullable enable
        public UIInputManager(UIElement root, IScrollInputHandler? scroll = null)
#nullable disable
        {
            MouseRouter = new MouseInputRouter(root, scroll);
            RegisterHandler(MouseRouter); // Add mouse router itself to the general processor
            if (scroll != null)
                RegisterHandler(scroll);
        }

        public void RegisterHandler(IInputHandler handler)
        {
            if (!_generalHandlers.Contains(handler))
                _generalHandlers.Add(handler);
        }

        public void UnregisterHandler(IInputHandler handler)
        {
            _generalHandlers.Remove(handler);
        }
        public bool OnMouseDown(MouseEventArgs e)
        {
            return MouseRouter.OnMouseDown(e);
        }

        public bool OnMouseMove(MouseEventArgs e)
        {
            return MouseRouter.OnMouseMove(e);
        }

        public bool OnMouseUp(MouseEventArgs e)
        {
            return MouseRouter.OnMouseUp(e);
        }

        public bool OnMouseClick(MouseEventArgs e)
        {
            return MouseRouter.OnMouseClick(e);
        }

        public bool OnMouseWheel(MouseEventArgs e)
        {
            return MouseRouter.OnMouseWheel(e);
        }

        public void ProcessMouseDown (object s, MouseEventArgs e)=> OnMouseDown(e);
        public void ProcessMouseMove (object s, MouseEventArgs e)=> OnMouseMove(e);
        public void ProcessMouseUp   (object s, MouseEventArgs e)=> OnMouseUp(e);
        public void ProcessMouseClick(object s, MouseEventArgs e)=> OnMouseClick(e);
        public void ProcessMouseWheel(object s, MouseEventArgs e)=> OnMouseWheel(e);

        public void EndFrame()
        {
            MouseRouter.EndFrame();
            foreach (var h in _generalHandlers)
                h.EndFrame();
        }
    }
}
