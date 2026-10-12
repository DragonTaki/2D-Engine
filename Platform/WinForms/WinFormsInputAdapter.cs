/* ----- ----- ----- ----- */
// WinFormsInputAdapter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/10/12
// Version: v1.3
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
    /// etc. instead of the input manager directly. The keyboard: <c>KeyPress</c>
    /// characters become text input, <c>KeyDown</c> editing keys become <see cref="UIKey"/>s and
    /// Ctrl+V a paste (<see cref="UIInputManager.OnPaste"/>).
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

        // The keyboard members below are Windows-only like the rest of this WinForms adapter;
        // CA1416 is suppressed here so they don't add to the build's platform warnings.
#pragma warning disable CA1416
        /// <summary>A typed character (control characters such as Backspace arrive as KeyDown instead).</summary>
        public void ProcessKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && _inputManager.OnTextInput(e.KeyChar))
                e.Handled = true;
        }

        /// <summary>An editing key (see <see cref="UIKey"/>) or Ctrl+V (paste); other keys are left alone.</summary>
        public void ProcessKeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+V without Alt (Ctrl+Alt is AltGr on many layouts, which types characters).
            if (e.KeyCode == Keys.V && e.Control && !e.Alt)
            {
                if (_inputManager.OnPaste())
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                return;
            }

            UIKey? key = e.KeyCode switch
            {
                Keys.Back => UIKey.Backspace,
                Keys.Delete => UIKey.Delete,
                Keys.Enter => UIKey.Enter,
                Keys.Escape => UIKey.Escape,
                Keys.Left => UIKey.Left,
                Keys.Right => UIKey.Right,
                Keys.Home => UIKey.Home,
                Keys.End => UIKey.End,
                // Plain Tab only: Shift+Tab (backward) has no focus order to go back through.
                // The form must report Tab as an input key, or WinForms uses it for its own
                // control navigation and KeyDown never sees it (Launcher.MainForm.IsInputKey).
                Keys.Tab when !e.Shift => UIKey.Tab,
                _ => null,
            };
            if (key is UIKey k && _inputManager.OnKeyDown(k))
                e.Handled = true;
        }
#pragma warning restore CA1416
    }
}
