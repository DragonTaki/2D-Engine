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
    public abstract class UIHandlerBase
    {
        /// <summary>Reference back to the element (non-generic)</summary>
        public UIElementBase Element { get; internal set; }

        internal abstract bool HandleMouseDown(IMouseEvent e);

        internal abstract bool HandleMouseMove(IMouseEvent e);

        internal abstract bool HandleMouseUp(IMouseEvent e);

        internal abstract bool HandleMouseWheel(IMouseEvent e);

        internal abstract bool HandleMouseClick(IMouseEvent e);

        public virtual void Init(IUiFactory factory, UIElementBase element) { }

        internal abstract void OnUpdate();

        internal abstract void OnEndFrame();
    }
}
