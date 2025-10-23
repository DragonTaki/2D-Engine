/* ----- ----- ----- ----- */
// IDrawable.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.UI.Core.Interfaces
{
    /// <summary>
    /// Interface for UI elements that require periodic drawing per frame.
    /// </summary>
    /// <remarks>
    /// Classes implementing IDrawable should encapsulate their own drawing logic.
    /// Typically, the Draw method is invoked once per frame by a rendering system
    /// such as a game loop or UI update cycle.
    /// </remarks>
    public interface IDrawable
    {
        /// <summary>
        /// Called every frame to render the object.
        /// </summary>
        /// <param name="g">
        /// The <see cref="Graphics"/> object to draw on. This provides the rendering context,
        /// including clipping, transform, and drawing surface.
        /// </param>
        /// <remarks>
        /// Implementations should only render visual content and avoid side effects.
        /// Drawing order should be controlled by the caller or UI hierarchy to ensure
        /// proper layering of elements.
        /// </remarks>
        void Draw(Graphics g);
    }
}