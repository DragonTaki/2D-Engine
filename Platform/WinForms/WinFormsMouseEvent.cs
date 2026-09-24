/* ----- ----- ----- ----- */
// WinFormsMouseEvent.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Windows.Forms;

using Engine.Globals;
using Engine.Mathematics;

namespace Engine.Platform.WinForms
{
    /// <summary>
    /// GDI+/WinForms-backed <see cref="IMouseEvent"/>. <c>MouseEventArgs</c>
    /// reports position in the form's client-area pixels; that's rescaled
    /// into the fixed-aspect content coordinate space every layout constant
    /// assumes (see <c>GlobalViewport</c>, which <c>MainForm.OnPaint</c>'s
    /// <c>PushTransform</c> applies on the drawing side) before exposing it.
    /// </summary>
    internal sealed class WinFormsMouseEvent : IMouseEvent
    {
        private readonly Vector2F _designPosition;

        public WinFormsMouseEvent(MouseEventArgs native)
        {
            _designPosition = GlobalViewport.ScreenToDesign(new Vector2F(native.X, native.Y));
            Delta = native.Delta;
        }

        public float X => _designPosition.X;
        public float Y => _designPosition.Y;
        public Vector2F Location => _designPosition;
        public int Delta { get; }
    }
}
