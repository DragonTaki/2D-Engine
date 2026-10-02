/* ----- ----- ----- ----- */
// UITextFieldHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    /// <summary>
    /// Handler for <see cref="UITextField"/>: the keyboard input target while the field has the
    /// focus (typing, editing keys, see <see cref="UITextInputHandler{TElement, THandler, TRenderer}"/>),
    /// and a click places the caret. A disabled field takes no input.
    /// </summary>
    public class UITextFieldHandler : UITextInputHandler<UITextField, UITextFieldHandler, UITextFieldRenderer>
    {
#nullable enable
        /// <summary>Invoked with the new text after every edit (typing, deleting, Escape putting the old text back).</summary>
        public Action<string>? TextChanged { get; set; }

        /// <summary>Invoked with the text when the focus ends (Enter, Escape, a press elsewhere) and the text differs from when it began.</summary>
        public Action<string>? TextCommitted { get; set; }
#nullable disable

        public UITextFieldHandler() { }

        protected override void OnEdited() => TextChanged?.Invoke(Element.Text);

        protected override void OnFocusEnded()
        {
            if (Element.Text != _textAtFocus)
                TextCommitted?.Invoke(Element.Text);
        }
    }
}
