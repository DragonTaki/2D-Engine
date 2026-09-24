/* ----- ----- ----- ----- */
// WinFormsWindow.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Windows.Forms;

namespace Engine.Platform.WinForms
{
    /// <summary>WinForms-backed <see cref="IWindow"/>.</summary>
    public sealed class WinFormsWindow : IWindow
    {
        private readonly Form _form;

        public WinFormsWindow(Form form)
        {
            _form = form;
        }

        public void Invalidate() => _form.Invalidate();
    }
}
