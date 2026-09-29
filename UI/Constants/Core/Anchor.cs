/* ----- ----- ----- ----- */
// Anchor.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2026/09/29
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

namespace Engine.UI.Constants.Core
{
    /// <summary>
    /// Defines which sides of the parent an element is anchored to (legacy layout mode).
    /// <para>
    /// Anchoring both opposite sides (<see cref="StretchX"/>, <see cref="StretchY"/>) stretches
    /// the element between them, minus its margins. Center flags override edge flags on
    /// their axis.
    /// </para>
    /// </summary>
    [Flags]
    public enum Anchor
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3,
        CenterX = 1 << 4,
        CenterY = 1 << 5,
        StretchX = Left | Right,
        StretchY = Top | Bottom,
        Center = CenterX | CenterY,
        TopLeft = Top | Left
    }
}
