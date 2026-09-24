/* ----- ----- ----- ----- */
// SilkMouseEvent.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Numerics;

using Engine.Mathematics;

namespace Engine.Platform.Skia
{
    /// <summary>Silk.NET-backed <see cref="IMouseEvent"/>.</summary>
    internal sealed class SilkMouseEvent : IMouseEvent
    {
        public float X { get; }
        public float Y { get; }
        public Vector2F Location { get; }
        public int Delta { get; }

        public SilkMouseEvent(Vector2 position, int delta = 0)
        {
            X = position.X;
            Y = position.Y;
            Location = new Vector2F(position.X, position.Y);
            Delta = delta;
        }
    }
}
