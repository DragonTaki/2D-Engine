/* ----- ----- ----- ----- */
// UILabel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2026/10/05
// Version: v1.4
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using Engine.Globals;
using Engine.Mathematics;
using Engine.Platform;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Represents a basic label UI element for displaying text.
    /// </summary>
    public class UILabel : UIElement<UILabel, UILabelHandler, UILabelRenderer>
    {
        #region Fields

        /// <summary>
        /// Text content to be displayed. Changing it invalidates the layout (Auto sizes
        /// depend on it).
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (_text == value)
                    return;
                _text = value;
                InvalidateLayout();
            }
        }

        private string _text = string.Empty;

#nullable enable
        public List<TextFragment>? Fragments;
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
                InvalidateLayout();
            }
        }

        private IFont _font;
        private bool _ownsFont;

        // Bold/italic variants of Font for text fragments, owned by this label.
        private readonly Dictionary<FontStyleFlags, IFont> _fragmentFonts = new();
        private IFont _fragmentFontsBase;

        /// <summary>
        /// Font for a text fragment: <see cref="Font"/> itself when the fragment is plain,
        /// otherwise a cached bold/italic variant of it.
        /// </summary>
        public IFont GetFragmentFont(bool bold, bool italic)
        {
            var style = FontStyleFlags.Regular;
            if (bold) style |= FontStyleFlags.Bold;
            if (italic) style |= FontStyleFlags.Italic;
            if (style == FontStyleFlags.Regular)
                return Font;

            // Variants are derived from the current Font; rebuild them if it was replaced.
            if (!ReferenceEquals(_fragmentFontsBase, Font))
            {
                DisposeFragmentFonts();
                _fragmentFontsBase = Font;
            }

            if (!_fragmentFonts.TryGetValue(style, out var font))
            {
                font = GraphicsBackend.Factory.CreateFont(Font.FontFamily, Font.Size, style);
                _fragmentFonts[style] = font;
            }
            return font;
        }

        private void DisposeFragmentFonts()
        {
            foreach (var f in _fragmentFonts.Values)
                f.Dispose();
            _fragmentFonts.Clear();
        }

        /// <summary>
        /// Color of the text.
        /// </summary>
        public Color ForeColor { get; set; } = Color.Black;

        /// <summary>
        /// The switch that draws the semi-transparent red debug background behind the text
        /// (plain-text mode); null for <see cref="Engine.Diagnostics.DebugOptions.LabelBackgrounds"/>.
        /// A text box can give its lines another switch (<c>UITextBox.LineDebugBackgroundSwitch</c>).
        /// </summary>
        public Func<bool> DebugBackgroundSwitch { get; set; }

        /// <summary>
        /// Text alignment within the label bounds.
        /// </summary>
        public ContentAlign TextAlign { get; set; } = ContentAlign.MiddleCenter;

        /// <summary>
        /// Indicates whether text wrapping is enabled. Changing it invalidates the layout.
        /// </summary>
        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                if (_wordWrap == value)
                    return;
                _wordWrap = value;
                InvalidateLayout();
            }
        }

        private bool _wordWrap = true;

        public IBrush CachedBrush;
        public Color LastForeColor;

        public IStringFormat CachedFormat;

        public ContentAlign LastAlign;

        public bool LastWrap;

        public bool IsSelectable { get; set; } = false;

        public int SelectionStart { get; private set; }

        public int SelectionEnd { get; private set; }

        #endregion

        public UILabel() : base(type: UIElementType.Label)
        {

        }

        /// <summary>
        /// Intrinsic size for Auto Width/Height: the text measured with <see cref="Font"/>
        /// (wrapped to the available width when <see cref="WordWrap"/> is on and that width is
        /// finite), plus <see cref="Models.UILayout.Padding"/>. Fragments measure as one line of
        /// runs, as <c>UILabelRenderer</c> draws them. Empty text keeps one line's height.
        /// <para>
        /// One device pixel of slack is added on each axis: pixel snapping can shrink the final
        /// box by up to a device pixel, which would otherwise make the drawn text wrap or be
        /// trimmed differently from what was measured.
        /// </para>
        /// <para>
        /// The renderer still draws the text across the whole bounds (padding is not inset
        /// yet), so padding only adds space around the measured text.
        /// </para>
        /// </summary>
        public override Vector2F MeasureIntrinsicSize(Vector2F available)
        {
            var padding = LayoutRules.Padding;
            float availableWidth = available.X - padding.Horizontal;

            using var g = GraphicsBackend.Factory.CreateMeasurementContext();

            float width = 0f, height = 0f;
            if (Fragments != null && Fragments.Count > 0)
            {
                foreach (var fragment in Fragments)
                {
                    var size = g.MeasureString(fragment.Text ?? string.Empty, GetFragmentFont(fragment.Bold, fragment.Italic));
                    width += size.Width;
                    height = Math.Max(height, size.Height);
                }
            }
            else if (string.IsNullOrEmpty(Text))
            {
                height = g.MeasureString(" ", Font).Height;
            }
            else
            {
                bool wrap = WordWrap && !float.IsInfinity(availableWidth) && !float.IsNaN(availableWidth) && availableWidth >= 1f;
                var size = wrap
                    ? g.MeasureString(Text, Font, (int)MathF.Floor(availableWidth))
                    : g.MeasureString(Text, Font);
                width = size.Width;
                height = size.Height;
            }

            float slack = GlobalViewport.Scale > 0f ? 1f / GlobalViewport.Scale : 1f;
            return new Vector2F(
                width + slack + padding.Horizontal,
                height + slack + padding.Vertical);
        }

        protected override void DisposeUI()
        {
            base.DisposeUI();

            // Render caches filled by UILabelRenderer.
            CachedBrush?.Dispose();
            CachedBrush = null;
            CachedFormat?.Dispose();
            CachedFormat = null;

            DisposeFragmentFonts();

            if (_ownsFont)
                _font?.Dispose();
            _font = null;
            _ownsFont = false;
        }
    }
}
