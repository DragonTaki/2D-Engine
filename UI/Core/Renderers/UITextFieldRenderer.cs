/* ----- ----- ----- ----- */
// UITextFieldRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UITextField"/>: the box (the focused box while focused), the text or
    /// placeholder scrolled to keep the caret visible, and the blinking caret
    /// (<see cref="UITextInputRenderer{TElement, THandler, TRenderer}"/>).
    /// </summary>
    public class UITextFieldRenderer : UITextInputRenderer<UITextField, UITextFieldHandler, UITextFieldRenderer>
    {
        public UITextFieldRenderer() { }
    }
}
