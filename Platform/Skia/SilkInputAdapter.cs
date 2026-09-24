/* ----- ----- ----- ----- */
// SilkInputAdapter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Numerics;

using Silk.NET.Input;

using Engine.Globals;
using Engine.UI.Input;

using SilkWindowInterface = Silk.NET.Windowing.IWindow;

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
        private readonly SilkWindowInterface _window;

        /// <summary>
        /// GLFW reports mouse position in the window's logical (point) space.
        /// Getting that into a coordinate a renderer or handler can actually
        /// use takes two conversions: logical points → physical framebuffer
        /// pixels (HiDPI/Retina displays scale those by the display's scale
        /// factor), then framebuffer pixels → the fixed-aspect content
        /// coordinate space every layout constant assumes (see
        /// <c>GlobalViewport</c>, which OnRender's <c>PushTransform</c>
        /// applies on the drawing side). Skipping either step means clicks
        /// land on the wrong element.
        /// </summary>
        public SilkInputAdapter(UIInputManager inputManager, IMouse mouse, SilkWindowInterface window)
        {
            _inputManager = inputManager;
            _mouse = mouse;
            _window = window;

            _mouse.MouseDown += OnMouseDown;
            _mouse.MouseUp += OnMouseUp;
            _mouse.MouseMove += OnMouseMove;
            _mouse.Click += OnClick;
            _mouse.Scroll += OnScroll;
        }

        private Vector2 ToContentSpace(Vector2 windowPosition)
        {
            var windowSize = _window.Size;
            var framebufferSize = _window.FramebufferSize;
            if (windowSize.X <= 0 || windowSize.Y <= 0)
                return windowPosition;

            var framebufferPosition = new Vector2(
                windowPosition.X * framebufferSize.X / windowSize.X,
                windowPosition.Y * framebufferSize.Y / windowSize.Y);

            var designPoint = GlobalViewport.ScreenToDesign(
                new Engine.Mathematics.Vector2F(framebufferPosition.X, framebufferPosition.Y));

            return new Vector2(designPoint.X, designPoint.Y);
        }

        private void OnMouseDown(IMouse mouse, MouseButton button) =>
            _inputManager.OnMouseDown(new SilkMouseEvent(ToContentSpace(mouse.Position)));

        private void OnMouseUp(IMouse mouse, MouseButton button) =>
            _inputManager.OnMouseUp(new SilkMouseEvent(ToContentSpace(mouse.Position)));

        private void OnMouseMove(IMouse mouse, Vector2 position) =>
            _inputManager.OnMouseMove(new SilkMouseEvent(ToContentSpace(position)));

        private void OnClick(IMouse mouse, MouseButton button, Vector2 position) =>
            _inputManager.OnMouseClick(new SilkMouseEvent(ToContentSpace(position)));

        private void OnScroll(IMouse mouse, ScrollWheel wheel) =>
            _inputManager.OnMouseWheel(new SilkMouseEvent(ToContentSpace(mouse.Position), (int)(wheel.Y * WheelNotchScale)));
    }
}
