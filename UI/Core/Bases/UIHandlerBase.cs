/* ----- ----- ----- ----- */
// UIHandlerBase.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.UI.Core.Interfaces;

namespace Engine.UI.Core.Bases
{
    /// <summary>
    /// Non-generic base of every UI handler (the input/logic half of an element):
    /// lets <see cref="UIElementBase"/> route mouse events and frame callbacks to it.
    /// </summary>
    public abstract class UIHandlerBase
    {
        /// <summary>Reference back to the element (non-generic).</summary>
        public UIElementBase Element { get; internal set; }

        internal abstract bool HandleMouseDown(IMouseEvent e);

        internal abstract bool HandleMouseMove(IMouseEvent e);

        internal abstract bool HandleMouseUp(IMouseEvent e);

        internal abstract bool HandleMouseWheel(IMouseEvent e);

        internal abstract bool HandleMouseClick(IMouseEvent e);

        /// <summary>
        /// Binds the handler to its element; called by the UI factory before the element's own Init.
        /// </summary>
        /// <param name="factory">The UI factory, for resolving services.</param>
        /// <param name="element">The element this handler serves.</param>
        public virtual void Init(IUiFactory factory, UIElementBase element) { }

        internal abstract void OnUpdate();

        internal abstract void OnEndFrame();
    }
}
