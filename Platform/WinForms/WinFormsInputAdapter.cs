/* ----- ----- ----- ----- */
// WinFormsInputAdapter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Windows.Forms;

using Engine.UI.Input;

namespace Engine.Platform.WinForms
{
    /// <summary>
    /// Wraps a <see cref="UIInputManager"/> with the exact delegate shape
    /// WinForms' <c>Control</c> mouse events expect, translating each
    /// <see cref="MouseEventArgs"/> into an <see cref="IMouseEvent"/> before
    /// forwarding it. Wire this adapter's methods to <c>Control.MouseDown</c>
    /// etc. instead of the input manager directly.
    /// </summary>
    public sealed class WinFormsInputAdapter
    {
        private readonly UIInputManager _inputManager;

        public WinFormsInputAdapter(UIInputManager inputManager)
        {
            _inputManager = inputManager;
        }

        public void ProcessMouseDown(object sender, MouseEventArgs e) =>
            _inputManager.OnMouseDown(new WinFormsMouseEvent(e));

        public void ProcessMouseMove(object sender, MouseEventArgs e) =>
            _inputManager.OnMouseMove(new WinFormsMouseEvent(e));

        public void ProcessMouseUp(object sender, MouseEventArgs e) =>
            _inputManager.OnMouseUp(new WinFormsMouseEvent(e));

        public void ProcessMouseClick(object sender, MouseEventArgs e) =>
            _inputManager.OnMouseClick(new WinFormsMouseEvent(e));

        public void ProcessMouseWheel(object sender, MouseEventArgs e) =>
            _inputManager.OnMouseWheel(new WinFormsMouseEvent(e));
    }
}
