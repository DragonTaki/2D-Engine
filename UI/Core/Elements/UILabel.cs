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
        public Font Font { get; set; } = SystemFonts.DefaultFont;

        /// <summary>
        /// Color of the text.
        /// </summary>
        public Color ForeColor { get; set; } = Color.Black;

        /// <summary>
        /// Text alignment within the label bounds.
        /// </summary>
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleCenter;

        /// <summary>
        /// Indicates whether text wrapping is enabled.
        /// </summary>
        public bool WordWrap { get; set; } = true;

#nullable enable
        public SolidBrush? _cachedBrush;

        public StringFormat? _cachedFormat;
#nullable disable

        public ContentAlignment _lastAlign;

        public bool _lastWrap;

        public bool IsSelectable { get; set; } = false;

        public int SelectionStart { get; private set; }

        public int SelectionEnd { get; private set; }

        #endregion

        public UILabel() : base(type: UIElementType.Label)
        {

        }
    }
}
