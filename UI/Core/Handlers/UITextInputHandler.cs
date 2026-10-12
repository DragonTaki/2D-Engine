/* ----- ----- ----- ----- */
// UITextInputHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/12
// Version: v1.2
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
    /// Shared handler of <see cref="UITextInputElement{TElement, THandler, TRenderer}"/> fields
    /// (<see cref="UITextFieldHandler"/>, <see cref="UINumberFieldHandler"/>): the keyboard input target while the field has the
    /// focus (typing, editing keys), and a click places the caret. A disabled field takes no input.
    /// </summary>
    public abstract class UITextInputHandler<TElement, THandler, TRenderer> : UIHandler<TElement, THandler, TRenderer>, IKeyboardInputTarget
        where TElement : UITextInputElement<TElement, THandler, TRenderer>
        where THandler : UITextInputHandler<TElement, THandler, TRenderer>
        where TRenderer : UITextInputRenderer<TElement, THandler, TRenderer>
    {
        /// <summary>The text when the field got the focus (what Escape puts back, and what a commit is compared with).</summary>
        protected string _textAtFocus = string.Empty;

        /// <summary>Whether Enter ended the current focus (<see cref="OnSubmitted"/> runs once the focus is gone).</summary>
        private bool _submitPending;

        protected UITextInputHandler() { }

        #region IKeyboardInputTarget

        UIElementBase IKeyboardInputTarget.TargetElement => Element;

        /// <summary>The handler of the element's <c>NextFocus</c>, when that handler takes keyboard input.</summary>
        IKeyboardInputTarget IKeyboardInputTarget.NextTarget => Element.NextFocus?.HandlerBase as IKeyboardInputTarget;

        /// <summary>Inserts <paramref name="c"/> at the caret (a full field takes it without change).</summary>
        public bool HandleTextInput(char c)
        {
            if (!Element.IsEnabled)
                return false;

            if (!AcceptsChar(c))
                return true;

            var text = Element.Text;
            if (text.Length >= Element.MaxLength)
                return true;

            int caret = Element.CaretIndex;
            SetText(text.Insert(caret, c.ToString()), caret + 1);
            return true;
        }

        /// <summary>
        /// Backspace / Delete remove a character, the arrows / Home / End move the caret, Escape
        /// puts back the starting text; Enter confirms (and submits, see <see cref="OnSubmitted"/>);
        /// Tab is used (nothing typed) so the focus can move to the next field.
        /// </summary>
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
                    // The focus ends right after this; the submit follows the commit (OnFocusChanged).
                    _submitPending = true;
                    return true;
                case UIKey.Tab:
                    return true;
                case UIKey.Escape:
                    if (text != _textAtFocus)
                        SetText(_textAtFocus, _textAtFocus.Length);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>Whether typed character <paramref name="c"/> may enter the text (anything by default; ignored otherwise).</summary>
        protected virtual bool AcceptsChar(char c) => true;

        /// <summary>
        /// Corrects an edit before it is applied (called with the edited text and its caret;
        /// returns the text and caret to keep). Nothing by default.
        /// </summary>
        protected virtual (string Text, int Caret) Correct(string text, int caret) => (text, caret);

        /// <summary>Called after every applied edit (the corrected text is in the element).</summary>
        protected virtual void OnEdited() { }

        /// <summary>Called when the focus is gained (after the starting text is remembered).</summary>
        protected virtual void OnFocusStarted() { }

        /// <summary>Called when the focus ends (after <see cref="UITextInputElement{TElement, THandler, TRenderer}.IsFocused"/> turned false).</summary>
        protected virtual void OnFocusEnded() { }

        /// <summary>
        /// Called when Enter ended the focus, after <see cref="OnFocusEnded"/> (so a commit is
        /// reported before the submit, and the field no longer has the focus). Nothing by default.
        /// </summary>
        protected virtual void OnSubmitted() { }

        /// <summary>
        /// Focus gained: remember the text, caret to the end. Focus lost: <see cref="OnFocusEnded"/>,
        /// then <see cref="OnSubmitted"/> when Enter ended it.
        /// </summary>
        public void OnFocusChanged(bool focused)
        {
            Element.IsFocused = focused;
            if (focused)
            {
                _submitPending = false;
                _textAtFocus = Element.Text;
                Element.CaretIndex = Element.Text.Length;
                OnFocusStarted();
                return;
            }

            OnFocusEnded();
            if (_submitPending)
            {
                _submitPending = false;
                OnSubmitted();
            }
        }

        #endregion

        #region Mouse Handling

        /// <summary>A click puts the caret at the clicked character boundary (the press already gave the focus).</summary>
        protected internal override bool HandleMouseClick(IMouseEvent e)
        {
            if (!Element.IsEnabled)
                return false;

            Element.CaretIndex = Element.CaretIndexAt(e.X);
            return true;
        }

        #endregion

        /// <summary>Applies an edit (corrected by <see cref="Correct"/>) and reports it.</summary>
        private void SetText(string text, int caretIndex)
        {
            var (corrected, caret) = Correct(text, caretIndex);
            Element.SetTextAndCaret(corrected, caret);
            OnEdited();
        }
    }
}
