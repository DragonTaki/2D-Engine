/* ----- ----- ----- ----- */
// UILabel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/10/27
// Version: v1.2
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

using Engine.Platform;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;
using Engine.UI.Elements;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Represents a basic label UI element for displaying text.
    /// </summary>
    public class UILabel : UIElement<UILabel, UILabelHandler, UILabelRenderer>
    {
        #region Fields

        /// <summary>
        /// Text content to be displayed.
        /// </summary>
        public string Text { get; set; } = string.Empty;

#nullable enable
        public List<TextFragment>? _fragments;
#nullable disable

        /// <summary>
        /// Font used to render the text.
        /// </summary>
        /// <remarks>
        /// The default font is created lazily and owned (disposed) by this label; a font
        /// assigned from outside is owned by whoever created it and is never disposed here.
        /// </remarks>
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
            }
        }

        private IFont _font;
        private bool _ownsFont;

        /// <summary>
        /// Color of the text.
        /// </summary>
        public Color ForeColor { get; set; } = Color.Black;

        /// <summary>
        /// Text alignment within the label bounds.
        /// </summary>
        public ContentAlign TextAlign { get; set; } = ContentAlign.MiddleCenter;

        /// <summary>
        /// Indicates whether text wrapping is enabled.
        /// </summary>
        public bool WordWrap { get; set; } = true;

        public IBrush _cachedBrush;
        public Color _lastForeColor;

        public IStringFormat _cachedFormat;

        public ContentAlign _lastAlign;

        public bool _lastWrap;

        public bool IsSelectable { get; set; } = false;

        public int SelectionStart { get; private set; }

        public int SelectionEnd { get; private set; }

        #endregion

        public UILabel() : base(type: UIElementType.Label)
        {

        }

        protected override void DisposeUI()
        {
            base.DisposeUI();

            // Render caches filled by UILabelRenderer.
            _cachedBrush?.Dispose();
            _cachedBrush = null;
            _cachedFormat?.Dispose();
            _cachedFormat = null;

            if (_ownsFont)
                _font?.Dispose();
            _font = null;
            _ownsFont = false;
        }
    }
}
