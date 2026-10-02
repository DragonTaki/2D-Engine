using System;

using Engine.Platform;
using Engine.Styles;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Shared state of the one-line editable fields (<see cref="UITextField"/>, <see cref="UINumberField"/>):
    /// the text, caret, font, style, focus and scroll, and the text measuring. Their handler
    /// (<see cref="UITextInputHandler{TElement, THandler, TRenderer}"/>) edits it and their renderer
    /// (<see cref="UITextInputRenderer{TElement, THandler, TRenderer}"/>) draws it.
    ///
    /// A one-line editable text field. Pressing the mouse on it gives it the keyboard focus
    /// (<see cref="Input.KeyboardFocus"/>, through its handler, an
    /// <see cref="Input.IKeyboardInputTarget"/>): typed characters are inserted at the caret,
    /// Backspace / Delete / Left / Right / Home / End edit and move it, Enter confirms and
    /// Escape puts back the text the edit started with (both end the focus); a click places
    /// the caret. The handler reports every change (TextChanged)
    /// and the confirmed text when the focus ends (TextCommitted).
    /// Drawn by the renderer with <see cref="Style"/>; text wider than the
    /// field scrolls so the caret stays visible.
    /// </summary>
    public abstract class UITextInputElement<TElement, THandler, TRenderer> : UIElement<TElement, THandler, TRenderer>
        where TElement : UITextInputElement<TElement, THandler, TRenderer>
        where THandler : UITextInputHandler<TElement, THandler, TRenderer>
        where TRenderer : UITextInputRenderer<TElement, THandler, TRenderer>
    {
        #region Fields

        private string _text = string.Empty;
        private int _maxLength = TextFieldDefaults.MaxLength;
        private int _caretIndex;

        #endregion

        #region Properties

        /// <summary>
        /// The text (never null; cut to <see cref="MaxLength"/>). Setting it puts the caret at
        /// the end and does not run the handler's callbacks.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = Limit(value);
                CaretIndex = _text.Length;
            }
        }

        /// <summary>Longest text (characters, at least 0); a longer current text is cut.</summary>
        public int MaxLength
        {
            get => _maxLength;
            set
            {
                _maxLength = Math.Max(0, value);
                if (_text.Length > _maxLength)
                    Text = _text;
            }
        }

        /// <summary>Text shown dimmed while the field is empty and not focused.</summary>
        public string Placeholder { get; set; } = string.Empty;

        /// <summary>Font of the text (owned by whoever assigns it; never disposed here). Nothing is drawn without one.</summary>
        public IFont Font { get; set; }

        /// <summary>How the field is drawn; null = <see cref="TextFieldDefaults.Style"/>.</summary>
        public TextFieldStyle Style { get; set; }

        /// <summary>Caret position: the number of characters before it (0 to <see cref="Text"/>'s length).</summary>
        public int CaretIndex
        {
            get => _caretIndex;
            internal set
            {
                _caretIndex = Math.Clamp(value, 0, _text.Length);
                CaretMovedAt = Environment.TickCount64;
            }
        }

        /// <summary>Whether the field has the keyboard focus (set by its handler).</summary>
        public bool IsFocused { get; internal set; }

        /// <summary>When the caret last moved (<see cref="Environment.TickCount64"/>): the blink restarts there, so it is visible while typing.</summary>
        internal long CaretMovedAt { get; private set; }

        /// <summary>How far the text is scrolled left (design units) to keep the caret visible; kept by the renderer.</summary>
        internal float TextScroll { get; set; }

        #endregion

        #region Constructors

        /// <summary>Creates an empty field.</summary>
        protected UITextInputElement()
            : base(zIndex: 0, isPersistent: false, type: UIElementType.Generic)
        {
        }

        #endregion

        /// <summary>Applies the default size (a layout style replaces it).</summary>
        protected override void OnInit()
        {
            Size = TextFieldDefaults.Size;
        }

        #region Methods

        /// <summary>Replaces the text and the caret together (the handler's edits).</summary>
        internal void SetTextAndCaret(string text, int caretIndex)
        {
            _text = Limit(text);
            CaretIndex = caretIndex;
        }

        /// <summary>
        /// The caret position closest to the absolute x coordinate <paramref name="x"/> (where a
        /// click puts the caret), given the current scroll; 0 without a font.
        /// </summary>
        public int CaretIndexAt(float x)
        {
            if (Font == null || _text.Length == 0)
                return 0;

            var style = Style ?? TextFieldDefaults.Style;
            float local = x - (GetCurrentAbsolutePosition().X + style.TextInset) + TextScroll;

            using var g = GraphicsBackend.Factory.CreateMeasurementContext();
            float previous = 0f;
            for (int i = 1; i <= _text.Length; i++)
            {
                float width = MeasurePrefix(g, _text, i, Font);
                if (local < (previous + width) / 2f)
                    return i - 1;
                previous = width;
            }
            return _text.Length;
        }

        /// <summary>
        /// Width of the first <paramref name="length"/> characters of <paramref name="text"/>,
        /// trailing spaces included (measured against a following marker character, because
        /// GDI+ leaves trailing spaces out of a plain measurement).
        /// </summary>
        public static float MeasurePrefix(IGraphics g, string text, int length, IFont font)
        {
            if (length <= 0 || string.IsNullOrEmpty(text) || font == null)
                return 0f;

            const string Marker = "|";
            string prefix = text.Substring(0, Math.Min(length, text.Length));
            return Math.Max(0f, g.MeasureString(prefix + Marker, font).Width - g.MeasureString(Marker, font).Width);
        }

        /// <summary><paramref name="text"/> (null as empty) cut to <see cref="MaxLength"/>.</summary>
        private string Limit(string text)
        {
            text ??= string.Empty;
            return text.Length > _maxLength ? text.Substring(0, _maxLength) : text;
        }

        #endregion
    }
}
