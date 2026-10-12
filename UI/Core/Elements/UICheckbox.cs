/* ----- ----- ----- ----- */
// UICheckbox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.Mathematics;
using Engine.Platform;
using Engine.Styles;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>A square checkbox with a right-hand label; its entire row accepts input.</summary>
    public class UICheckbox : UIElement<UICheckbox, UICheckboxHandler, UICheckboxRenderer>
    {
        /// <summary>Checked state. Assignment synchronizes the display without raising Changed.</summary>
        public bool IsChecked { get; set; }

        /// <summary>Raised after enabled user input changes the checked state.</summary>
        public event Action<bool> Changed;

        /// <summary>Right-hand label.</summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>Drawing style; null uses the engine defaults.</summary>
        public CheckboxStyle Style { get; set; }

        /// <summary>Current keyboard focus, maintained by the handler.</summary>
        public bool IsFocused { get; internal set; }

        /// <summary>Next keyboard target when Tab is pressed.</summary>
        public UIElementBase NextFocus { get; set; }

        private IFont _font;
        private bool _ownsFont;

        /// <summary>Label font. Assigned fonts remain owned by their caller.</summary>
        public IFont Font
        {
            get
            {
                if (_font == null)
                {
                    _font = GraphicsBackend.Factory.CreateFont(GraphicsBackend.Factory.GenericSansSerifFontFamily, 10f);
                    _ownsFont = true;
                }
                return _font;
            }
            set
            {
                if (ReferenceEquals(value, _font))
                    return;
                if (_ownsFont)
                    _font?.Dispose();
                _font = value;
                _ownsFont = false;
                InvalidateLayout();
            }
        }

        public UICheckbox() : base(type: UIElementType.Generic) { }

        protected override void OnInit() => Size = new Vector2F(CheckboxDefaults.Size.X, CheckboxDefaults.Size.Y);

        internal void NotifyChanged() => Changed?.Invoke(IsChecked);

        protected override void DisposeUI()
        {
            base.DisposeUI();
            Changed = null;
            if (_ownsFont)
                _font?.Dispose();
            _font = null;
            _ownsFont = false;
        }
    }
}
