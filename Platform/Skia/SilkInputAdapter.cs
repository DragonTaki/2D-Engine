/* ----- ----- ----- ----- */
// SilkInputAdapter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Silk.NET.Input;

using Engine.UI.Input;

namespace Engine.Platform.Skia
{
    /// <summary>
    /// Wraps a <see cref="UIInputManager"/> with the delegate shape Silk.NET's
    /// <see cref="IMouse"/> events expect, translating each callback into an
    /// <see cref="IMouseEvent"/> before forwarding it. Wire this adapter's
    /// methods to <c>IMouse</c> events instead of the input manager directly.
    /// </summary>
    public sealed class SilkInputAdapter
    {
        // A GLFW scroll notch reports as +/-1 on ScrollWheel.Y; WinForms'
        // MouseEventArgs.Delta reports the same notch as +/-120. Scaling here
        // keeps ScrollInputHandler's tuning (which assumes the WinForms scale)
        // correct regardless of which backend is active.
        private const int WheelNotchScale = 120;

        private readonly UIInputManager _inputManager;
        private readonly IMouse _mouse;

        public SilkInputAdapter(UIInputManager inputManager, IMouse mouse)
        {
            _inputManager = inputManager;
            _mouse = mouse;

            _mouse.MouseDown += OnMouseDown;
            _mouse.MouseUp += OnMouseUp;
            _mouse.MouseMove += OnMouseMove;
            _mouse.Click += OnClick;
            _mouse.Scroll += OnScroll;
        }

        private void OnMouseDown(IMouse mouse, MouseButton button) =>
            _inputManager.OnMouseDown(new SilkMouseEvent(mouse.Position));

        private void OnMouseUp(IMouse mouse, MouseButton button) =>
            _inputManager.OnMouseUp(new SilkMouseEvent(mouse.Position));

        private void OnMouseMove(IMouse mouse, System.Numerics.Vector2 position) =>
            _inputManager.OnMouseMove(new SilkMouseEvent(position));

        private void OnClick(IMouse mouse, MouseButton button, System.Numerics.Vector2 position) =>
            _inputManager.OnMouseClick(new SilkMouseEvent(position));

        private void OnScroll(IMouse mouse, ScrollWheel wheel) =>
            _inputManager.OnMouseWheel(new SilkMouseEvent(mouse.Position, (int)(wheel.Y * WheelNotchScale)));
    }
}
