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
    /// <summary>
    /// Represents the position state of a UI element.
    /// It contains both the base layout position (defined by UI design)
    /// and the current position (used for rendering or dynamic offset effects).
    /// </summary>
    public class UIPosition
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base position of the UI element in its parent coordinate space.
        /// This position is typically determined during layout initialization.
        /// </summary>
        public Vector2F Base { get; set; }

        /// <summary>
        /// Gets or sets the actual position used for rendering and hit detection.
        /// This may differ from <see cref="Base"/> due to runtime transformations,
        /// scrolling, or animations.
        /// </summary>
        public Vector2F Current { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UIPosition"/> class with the specified base position.
        /// The <see cref="Current"/> position is initially set to the same value as <see cref="Base"/>.
        /// </summary>
        /// <param name="basePosition">
        /// The base position vector of the UI element, usually determined by layout or design.
        /// </param>
        public UIPosition(Vector2F basePosition)
        {
            Base = basePosition;
            Current = basePosition;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Implicitly converts a <see cref="Vector2F"/> to a <see cref="UIPosition"/>.
        /// This allows creating a <see cref="UIPosition"/> object directly from a position vector.
        /// </summary>
        /// <param name="v">The vector to convert into a <see cref="UIPosition"/>.</param>
        /// <returns>
        /// A new <see cref="UIPosition"/> instance with both <see cref="Base"/> and <see cref="Current"/>
        /// initialized to the same coordinates as <paramref name="v"/>.
        /// </returns>
        public static implicit operator UIPosition(Vector2F v)
        {
            return new UIPosition(v);
        }

        #endregion
    }
}
