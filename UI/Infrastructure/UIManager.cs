/* ----- ----- ----- ----- */
// UIManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */


using Engine.Platform;
using Engine.UI.Core.Elements;

namespace Engine.UI.Core.Infrastructure
{
    /// <summary>
    /// Manages UI root, dispatching updates, rendering, and input events.
    /// </summary>
    public class UIManager
    {
        public UIElement Root { get; private set; }

        public UIManager(UIElement root)
        {
            Root = root;
        }

        /// <summary>
        /// Updates the UI logic (e.g., handler updates such as scroll inertia and rebound; the layout pass runs during Draw).
        /// </summary>
        public void Update()
        {
            Root?.Update();
        }

        /// <summary>
        /// Renders the UI to the screen.
        /// </summary>
        public void Draw(IGraphics g)
        {
            Root?.Draw(g);
        }

        /// <summary>
        /// Runs per-frame cleanup (handler OnEndFrame, e.g. scroll delta reset) after input
        /// and drawing - the third step of the Update/Draw/EndFrame frame cycle that the
        /// element tree expects and the frame loops perform.
        /// </summary>
        public void EndFrame()
        {
            Root?.EndFrame();
        }

        /// <summary>
        /// Forwards mouse down events to the root element.
        /// </summary>
        public void OnMouseDown(IMouseEvent e)
        {
            Root?.OnMouseDown(e);
        }

        /// <summary>
        /// Forwards mouse move events to the root element.
        /// </summary>
        public void OnMouseMove(IMouseEvent e)
        {
            Root?.OnMouseMove(e);
        }

        /// <summary>
        /// Forwards mouse up events to the root element.
        /// </summary>
        public void OnMouseUp(IMouseEvent e)
        {
            Root?.OnMouseUp(e);
        }

        /// <summary>
        /// Forwards mouse wheel events to the root element.
        /// </summary>
        public void OnMouseWheel(IMouseEvent e)
        {
            Root?.OnMouseWheel(e);
        }

        /// <summary>
        /// Forwards mouse click events to the root element.
        /// </summary>
        public void OnMouseClick(IMouseEvent e)
        {
            Root?.OnMouseClick(e);
        }

        // TODO: Extend with Focus, Hover, Keyboard, etc.
    }
}