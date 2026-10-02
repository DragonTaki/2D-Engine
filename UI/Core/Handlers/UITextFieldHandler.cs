/* ----- ----- ----- ----- */
// UITextFieldHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.Platform;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;
using Engine.UI.Input;

namespace Engine.UI.Core.Handlers
{
    /// <summary>
    /// Handler for <see cref="UITextField"/>: the keyboard input target while the field has the
    /// focus (typing, editing keys), and a click places the caret. A disabled field takes no input.
    /// </summary>
    public class UITextFieldHandler : UIHandler<UITextField, UITextFieldHandler, UITextFieldRenderer>, IKeyboardInputTarget
    {
        /// <summary>The text when the field got the focus (what Escape puts back, and what a commit is compared with).</summary>
        private string _textAtFocus = string.Empty;

#nullable enable
        /// <summary>Invoked with the new text after every edit (typing, deleting, Escape putting the old text back).</summary>
        public Action<string>? TextChanged { get; set; }

        /// <summary>Invoked with the text when the focus ends (Enter, Escape, a press elsewhere) and the text differs from when it began.</summary>
        public Action<string>? TextCommitted { get; set; }
#nullable disable

        public UITextFieldHandler() { }

        #region IKeyboardInputTarget

        UIElementBase IKeyboardInputTarget.TargetElement => Element;

        /// <summary>Inserts <paramref name="c"/> at the caret (a full field takes it without change).</summary>
        public bool HandleTextInput(char c)
        {
            if (!Element.IsEnabled)
                return false;

            var text = Element.Text;
            if (text.Length >= Element.MaxLength)
                return true;

            int caret = Element.CaretIndex;
            SetText(text.Insert(caret, c.ToString()), caret + 1);
            return true;
        }

        /// <summary>Backspace / Delete remove a character, the arrows / Home / End move the caret, Escape puts back the starting text; Enter only confirms.</summary>
        public bool HandleKey(UIKey key)
        {
            if (!Element.IsEnabled)
                return false;

            var text = Element.Text;
            int caret = Element.CaretIndex;
            switch (key)
            {
                case UIKey.Backspace:
                    if (caret > 0)
                        SetText(text.Remove(caret - 1, 1), caret - 1);
                    return true;
                case UIKey.Delete:
                    if (caret < text.Length)
                        SetText(text.Remove(caret, 1), caret);
                    return true;
                case UIKey.Left:
                    Element.CaretIndex = caret - 1;
                    return true;
                case UIKey.Right:
                    Element.CaretIndex = caret + 1;
                    return true;
                case UIKey.Home:
                    Element.CaretIndex = 0;
                    return true;
                case UIKey.End:
                    Element.CaretIndex = text.Length;
                    return true;
                case UIKey.Enter:
                    return true;
                case UIKey.Escape:
                    if (text != _textAtFocus)
                        SetText(_textAtFocus, _textAtFocus.Length);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>Focus gained: remember the text, caret to the end. Focus lost: commit when the text changed.</summary>
        public void OnFocusChanged(bool focused)
        {
            Element.IsFocused = focused;
            if (focused)
            {
                _textAtFocus = Element.Text;
                Element.CaretIndex = Element.Text.Length;
                return;
            }

            if (Element.Text != _textAtFocus)
                TextCommitted?.Invoke(Element.Text);
        }

        #endregion

        #region Mouse Handling

        /// <summary>A click puts the caret at the clicked character boundary (the press already gave the focus).</summary>
        internal override bool HandleMouseClick(IMouseEvent e)
        {
            if (!Element.IsEnabled)
                return false;

            Element.CaretIndex = Element.CaretIndexAt(e.X);
            return true;
        }

        #endregion

        /// <summary>Applies an edit and reports it.</summary>
        private void SetText(string text, int caretIndex)
        {
            Element.SetTextAndCaret(text, caretIndex);
            TextChanged?.Invoke(Element.Text);
        }
    }
}
