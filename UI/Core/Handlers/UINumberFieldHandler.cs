/* ----- ----- ----- ----- */
// UINumberFieldHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;
using Engine.UI.Utils;

namespace Engine.UI.Core.Handlers
{
    /// <summary>
    /// Handler for <see cref="UINumberField"/>: the editing of
    /// <see cref="UITextInputHandler{TElement, THandler, TRenderer}"/>, with only ASCII digits
    /// accepted and every edit corrected at once (<see cref="NumberInput.Correct"/>). When the
    /// focus ends the value is corrected into <c>Min</c>..<c>Max</c> and, if it differs from
    /// the value before the edit, reported by <see cref="ValueCommitted"/>.
    /// </summary>
    public class UINumberFieldHandler : UITextInputHandler<UINumberField, UINumberFieldHandler, UINumberFieldRenderer>
    {
        private int _valueAtFocus;

#nullable enable
        /// <summary>Invoked with the legal, corrected value when an edit ends and the value changed.</summary>
        public Action<int>? ValueCommitted { get; set; }
#nullable disable

        public UINumberFieldHandler() { }

        protected override bool AcceptsChar(char c) => NumberInput.IsDigit(c);

        protected override (string Text, int Caret) Correct(string text, int caret) =>
            NumberInput.Correct(text, caret, Element.Max);

        /// <summary>Also remember the value (what Escape's restored text commits to).</summary>
        protected override void OnFocusStarted() => _valueAtFocus = Element.Value;

        protected override void OnFocusEnded()
        {
            int value = Element.CommitText();
            if (value != _valueAtFocus)
                ValueCommitted?.Invoke(value);
        }
    }
}
