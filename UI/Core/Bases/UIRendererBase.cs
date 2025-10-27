/* ----- ----- ----- ----- */
// UIRendererBase.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.UI.Core.Bases
{
    public abstract class UIRendererBase
    {
        /// <summary>Reference back to the element (non-generic)</summary>
        public UIElementBase Element { get; internal set; }

        public virtual void Init(UIElementBase element) { }

        public virtual void Render(Graphics g, UIElementBase element) { }
    }
}
