/* ----- ----- ----- ----- */
// UITextField.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// A one-line editable text field. Pressing the mouse on it gives it the keyboard focus
    /// (<see cref="Input.KeyboardFocus"/>, through its handler, an
    /// <see cref="Input.IKeyboardInputTarget"/>): typed characters are inserted at the caret,
    /// Backspace / Delete / Left / Right / Home / End edit and move it, Enter confirms and
    /// Escape puts back the text the edit started with (both end the focus); a click places
    /// the caret. The handler reports every change (<see cref="UITextFieldHandler.TextChanged"/>)
    /// and the confirmed text when the focus ends (<see cref="UITextFieldHandler.TextCommitted"/>).
    /// Drawn by <see cref="UITextFieldRenderer"/> with <c>Style</c>; text wider than the
    /// field scrolls so the caret stays visible. The editing state is shared with
    /// <see cref="UINumberField"/> (<see cref="UITextInputElement{TElement, THandler, TRenderer}"/>).
    /// </summary>
    public class UITextField : UITextInputElement<UITextField, UITextFieldHandler, UITextFieldRenderer>
    {
    }
}
