/* ----- ----- ----- ----- */
// UINumberField.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.Platform;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;
using Engine.UI.Utils;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// A one-line whole-number field (typed, never a slider): it shares the caret, scrolling,
    /// focus and keyboard handling of <see cref="UITextField"/>
    /// (<see cref="UITextInputElement{TElement, THandler, TRenderer}"/>) but takes only the ASCII
    /// digits (<see cref="NumberInput"/>). A value above <see cref="Max"/> becomes it at once, leading
    /// zeros are removed as you type, and a value below <see cref="Min"/> (or an empty text) is
    /// corrected when the edit ends (Enter or focus loss; the handler then reports it as
    /// <see cref="UINumberFieldHandler.ValueCommitted"/>); Escape puts back the value from before the edit.
    /// An optional <see cref="Unit"/> is drawn after the box, inside the element's bounds.
    /// </summary>
    public class UINumberField : UITextInputElement<UINumberField, UINumberFieldHandler, UINumberFieldRenderer>
    {
        #region Fields

        private int _min;
        private int _max = int.MaxValue;
        private int _value;

        #endregion

        #region Properties

        /// <summary>Smallest legal value (at least 0). Setting it keeps <see cref="Max"/> at or above it and the value within the range.</summary>
        public int Min
        {
            get => _min;
            set
            {
                _min = Math.Max(0, value);
                if (_max < _min)
                    Max = _min;
                else
                    Value = _value;
            }
        }

        /// <summary>Largest legal value; also fixes the text's longest length (its digits). Never below <see cref="Min"/>.</summary>
        public int Max
        {
            get => _max;
            set
            {
                _max = Math.Max(_min, value);
                MaxLength = NumberInput.MaxLength(_max);
                Value = _value;
            }
        }

        /// <summary>
        /// The legal value (the last one set or committed; while typing, the text may differ until
        /// the edit ends). Setting it clamps it into the range and shows it, without the handler's callback.
        /// </summary>
        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Clamp(value, _min, _max);
                Text = _value.ToString();
            }
        }

        /// <summary>Text drawn after the box (e.g. "分鐘"); empty = none.</summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>Brush of the <see cref="Unit"/> (owned by whoever assigns it; never disposed here); null = the style's text colour.</summary>
        public IBrush UnitBrush { get; set; }

        /// <summary>Space between the box and the unit (design units).</summary>
        public float UnitGap { get; set; } = 10f;

        #endregion

        /// <summary>Creates a field for 0 to <see cref="int.MaxValue"/>; set <see cref="Min"/> / <see cref="Max"/> before use.</summary>
        public UINumberField()
        {
            MaxLength = NumberInput.MaxLength(_max);
            Text = "0";
        }

        #region Methods

        /// <summary>Corrects the text to the legal value (<see cref="NumberInput.Commit"/>), stores and shows it, and returns it.</summary>
        internal int CommitText()
        {
            _value = NumberInput.Commit(Text, _min, _max);
            Text = _value.ToString();
            return _value;
        }

        #endregion
    }
}
