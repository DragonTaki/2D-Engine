/* ----- ----- ----- ----- */
// IBrush.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Platform
{
    /// <summary>
    /// A fill style (solid color or gradient) used by <see cref="IGraphics"/>
    /// fill operations. Backend-specific; created via <see cref="IGraphics"/>
    /// factory methods, never constructed directly by callers.
    /// </summary>
    public interface IBrush : IDisposable
    {
    }
}
