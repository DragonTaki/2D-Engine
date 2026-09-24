/* ----- ----- ----- ----- */
// SilkWindow.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Platform.Skia
{
    /// <summary>
    /// Silk.NET-backed <see cref="IWindow"/>. Silk.NET's <c>IView.Render</c>
    /// fires every frame on its own (there is no WinForms-style "only repaint
    /// when invalidated" mode here), so <see cref="Invalidate"/> is a no-op —
    /// the next frame redraws unconditionally regardless of whether this was
    /// called.
    /// </summary>
    public sealed class SilkWindow : IWindow
    {
        public void Invalidate() { }
    }
}
