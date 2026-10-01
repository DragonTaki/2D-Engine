/* ----- ----- ----- ----- */
// UIRendererBase.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;

namespace Engine.UI.Core.Bases
{
    /// <summary>
    /// Non-generic base of every UI renderer (the drawing half of an element):
    /// lets <see cref="UIElementBase"/> draw itself without knowing the renderer's type.
    /// </summary>
    public abstract class UIRendererBase
    {
        /// <summary>Reference back to the element (non-generic).</summary>
        public UIElementBase Element { get; internal set; }

        /// <summary>
        /// Binds the renderer to its element; called by the UI factory before the element's own Init.
        /// </summary>
        /// <param name="element">The element this renderer draws.</param>
        public virtual void Init(UIElementBase element) { }

        /// <summary>
        /// Draws <paramref name="element"/> (not its children). The base does nothing.
        /// </summary>
        /// <param name="g">Graphics context to draw on.</param>
        /// <param name="element">The element to draw.</param>
        public virtual void Render(IGraphics g, UIElementBase element) { }
    }
}
