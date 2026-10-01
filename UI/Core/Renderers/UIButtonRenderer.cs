/* ----- ----- ----- ----- */
// UIButtonRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UIButton"/>. It adds no drawing of its own (the inherited
    /// <c>OnRender</c> is a no-op): buttons inside a menu are drawn by the menu renderer
    /// through their <c>IButtonDrawStyle</c>.
    /// </summary>
    public class UIButtonRenderer : UIRenderer<UIButton, UIButtonHandler, UIButtonRenderer>
    {
        public UIButtonRenderer() { }
    }

    public class UIButtonRenderer<TEnum> : UIButtonRenderer
        where TEnum : Enum
    {
        //
    }
}
