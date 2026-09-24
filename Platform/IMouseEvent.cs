/* ----- ----- ----- ----- */
// IMouseEvent.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Mathematics;

namespace Engine.Platform
{
    /// <summary>
    /// A mouse input event, mirroring the subset of WinForms' <c>MouseEventArgs</c>
    /// actually used here. Created by the active windowing backend and passed
    /// down through <c>Engine.UI.Input</c>/<c>Engine.UI.Core.Handlers</c>.
    /// </summary>
    public interface IMouseEvent
    {
        float X { get; }
        float Y { get; }
        Vector2F Location { get; }

        /// <summary>Mouse wheel delta for this event, if any (0 otherwise).</summary>
        int Delta { get; }
    }
}
