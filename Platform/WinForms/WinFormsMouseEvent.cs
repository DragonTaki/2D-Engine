/* ----- ----- ----- ----- */
// WinFormsMouseEvent.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Windows.Forms;

using Engine.Mathematics;

namespace Engine.Platform.WinForms
{
    /// <summary>GDI+/WinForms-backed <see cref="IMouseEvent"/>.</summary>
    internal sealed class WinFormsMouseEvent : IMouseEvent
    {
        private readonly MouseEventArgs _native;

        public WinFormsMouseEvent(MouseEventArgs native)
        {
            _native = native;
        }

        public float X => _native.X;
        public float Y => _native.Y;
        public Vector2F Location => new Vector2F(_native.Location);
        public int Delta => _native.Delta;
    }
}
