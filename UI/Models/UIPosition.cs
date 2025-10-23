/* ----- ----- ----- ----- */
// UIPosition.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Mathematics;

namespace Engine.UI.Models
{
    public class UIPosition
    {
        /// <summary>
        /// The base position of the UI element in the parent layer. (arranged by the UI designer)
        /// </summary>
        public Vector2F Base { get; set; }

        /// <summary>
        /// The actual position after adding offsets. (used for drawing and event detection)
        /// </summary>
        public Vector2F Current { get; set; }

        public UIPosition(Vector2F basePosition)
        {
            Base = basePosition;
            Current = basePosition;
        }

        public static implicit operator UIPosition(Vector2F v)
        {
            return new UIPosition(v);
        }
    }
}
