/* ----- ----- ----- ----- */
// GraphicsBackend.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Platform
{
    /// <summary>
    /// The single active <see cref="IGraphicsFactory"/> for the app's
    /// lifetime, pushed in once by the composition root
    /// (<c>Launcher/Program.cs</c>). Style factories and helpers that need to
    /// create backend drawing objects read this instead of taking a
    /// constructor dependency, so their existing call sites (e.g.
    /// <c>new SolidBrushFactory(color)</c>) don't need to change.
    /// </summary>
    public static class GraphicsBackend
    {
        public static IGraphicsFactory Factory { get; set; }
    }
}
