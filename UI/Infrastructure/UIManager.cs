/* ----- ----- ----- ----- */
// UIManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Windows.Forms;

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
        /// Updates the UI logic (e.g., animation, layout, scroll).
        /// </summary>
        public void Update()
        {
            Root?.Update();
        }

        /// <summary>
        /// Renders the UI to the screen.
        /// </summary>
        public void Draw(Graphics g)
        {
            Root?.Draw(g);
        }

        /// <summary>
        /// Forwards mouse down events to the root element.
        /// </summary>
        public void OnMouseDown(MouseEventArgs e)
        {
            Root?.OnMouseDown(e);
        }

        /// <summary>
        /// Forwards mouse move events to the root element.
        /// </summary>
        public void OnMouseMove(MouseEventArgs e)
        {
            Root?.OnMouseMove(e);
        }

        /// <summary>
        /// Forwards mouse up events to the root element.
        /// </summary>
        public void OnMouseUp(MouseEventArgs e)
        {
            Root?.OnMouseUp(e);
        }

        /// <summary>
        /// Forwards mouse wheel events to the root element.
        /// </summary>
        public void OnMouseWheel(MouseEventArgs e)
        {
            Root?.OnMouseWheel(e);
        }

        /// <summary>
        /// Forwards mouse click events to the root element.
        /// </summary>
        public void OnMouseClick(MouseEventArgs e)
        {
            Root?.OnMouseClick(e);
        }

        // 可以擴充 Focus、Hover、Keyboard 等功能
    }
}