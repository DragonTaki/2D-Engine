/* ----- ----- ----- ----- */
// UIHandlerBase.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2026/10/05
// Version: v1.1
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

        protected internal abstract bool HandleMouseDown(IMouseEvent e);

        protected internal abstract bool HandleMouseMove(IMouseEvent e);

        protected internal abstract bool HandleMouseUp(IMouseEvent e);

        protected internal abstract bool HandleMouseWheel(IMouseEvent e);

        protected internal abstract bool HandleMouseClick(IMouseEvent e);

        /// <summary>
        /// Binds the handler to its element; called by the UI factory before the element's own Init.
        /// </summary>
        /// <param name="factory">The UI factory, for resolving services.</param>
        /// <param name="element">The element this handler serves.</param>
        public virtual void Init(IUiFactory factory, UIElementBase element) { }

        protected internal abstract void OnUpdate();

        protected internal abstract void OnEndFrame();
    }
}
