/* ----- ----- ----- ----- */
// UITextBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2026/10/05
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Engine.Diagnostics;
using Engine.Platform;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Pure Engine text box (used as a log box): renders lines of text inside a scroll container, with no dependency on WinForms controls.
    /// </summary>
    public abstract class UITextBox<TElement, THandler, TRenderer> : UIContainer<TElement, THandler, TRenderer>
        where TElement : UITextBox<TElement, THandler, TRenderer>
        where THandler : UITextBoxHandler<TElement, THandler, TRenderer>
        where TRenderer : UITextBoxRenderer<TElement, THandler, TRenderer>
    {
        #region Fields / Properties

        public UIScrollContainer ScrollContainer { get; private set; }

        /// <summary>
        /// The switch that draws the red debug background behind each line appended with
        /// <see cref="AppendLine"/> (<see cref="UILabel.DebugBackgroundSwitch"/>); null for the
        /// labels' default, <see cref="DebugOptions.LabelBackgrounds"/>. Applies to lines appended after it is set.
        /// </summary>
        public Func<bool> LineDebugBackgroundSwitch { get; set; }

        /// <remarks>Same ownership rule as <see cref="UILabel.Font"/>.</remarks>
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

        // Per-style fonts for log lines, shared by every line label and owned by this
        // text box (creating one per AppendLine leaked a native font per log line).
        private readonly List<IFont> _lineFonts = new();
        public float LineHeight { get; set; }
        public Color BackgroundColor { get; set; } = Color.Black;
        public Color TextColor { get; set; } = Color.White;
        public float LineSpacing { get; set; } = 4f;
        public float ParagraphSpacing { get; set; } = 5f;

        #endregion

        #region Constructor

        public UITextBox() { }

        #endregion

        #region Initialization

        public override void Init(IUiFactory factory, THandler handler, TRenderer renderer)
        {
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"[UITextBox]Init 3 generic Current type: {this?.GetType().FullName ?? "null"}, IsInitialized: {IsInitialized}");
            if (IsInitialized) return;
            IsInitialized = true;
            _factory = factory;

            OnBeforeInit(factory);

            // Bind Handler
            Handler = handler;
            Handler.Element = (TElement)(object)this;
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"[UITextBox]Handler type: {Handler?.GetType().FullName ?? "null"}");

            // Bind Renderer
            Renderer = renderer;
            Renderer.Element = (TElement)(object)this;
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"[UITextBox]Renderer type: {Renderer?.GetType().FullName ?? "null"}");
            BuildScrollContainer();

            RunInitHooks();

            OnInit(factory);
            OnAfterInit(factory);
        }

        protected override void OnInit(IUiFactory factory)
        {
            LocalPosition = TextBoxDefaults.Position;
            Size = TextBoxDefaults.Size;
        }

        protected virtual void BuildScrollContainer()
        {
            ScrollContainer = _factory.CreateScrollContainer();
            ScrollContainer.Layout = TextBoxDefaults.Scroll.Layout;
            ScrollContainer.VerticalAlignment = ScrollAlignment.Bottom;
            AddChild(ScrollContainer);
        }

        #endregion

        #region Text Operations

        /// <summary>
        /// Appends text to the box, one label per line; lines are split on '\n' and the whole text forms one paragraph.
        /// </summary>
        public void AppendLine(string text, Color? color = null, bool bold = false, bool italic = false)
        {
            using var g = GraphicsBackend.Factory.CreateMeasurementContext();

            float y = ScrollContainer.ContentHeight;
            bool isFirstParagraph = y == 0;

            var lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                var style = FontStyleFlags.Regular;
                if (bold) style |= FontStyleFlags.Bold;
                if (italic) style |= FontStyleFlags.Italic;
                IFont font = GetLineFont(style);

                SizeF size = g.MeasureString(line, font);

                // Add paragraph spacing before a paragraph's first line (not for the first paragraph)
                if (i == 0 && !isFirstParagraph)
                    y += ParagraphSpacing;

                // Add line spacing before later lines within a paragraph (not for a paragraph's first line)
                if (i > 0)
                    y += LineSpacing;

                var label = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();

                label.Text = line;
                label.Font = font;
                label.ForeColor = color ?? TextColor;
                label.Layout = new Geometry.LayoutF(0, y, Size.X, size.Height);
                label.WordWrap = false; // One Label per line
                label.TextAlign = ContentAlign.MiddleLeft;
                label.DebugBackgroundSwitch = LineDebugBackgroundSwitch;

                label.LocalPosition.Current.Y = y;

                ScrollContainer.AddChild(label);

                y += size.Height;
            }

            // Automatic content size (the last label's bottom edge, = y); refreshed now so
            // the next append in the same frame starts below this line.
            ScrollContainer.RefreshContentSize(forceAlignment: true);
        }

        /// <summary>
        /// Appends one line made of inline text fragments (each with its own color and
        /// bold/italic), as a single label, e.g. a multi-colored log message.
        /// </summary>
        public void AppendFragmentLine(IReadOnlyList<TextFragment> fragments)
        {
            if (fragments == null || fragments.Count == 0)
                return;

            using var g = GraphicsBackend.Factory.CreateMeasurementContext();

            float y = ScrollContainer.ContentHeight;
            if (y != 0)
                y += ParagraphSpacing;  // Same spacing as a new AppendLine paragraph

            // Line height = the tallest fragment (bold/italic variants can differ slightly).
            float height = 0f;
            foreach (var frag in fragments)
            {
                var style = FontStyleFlags.Regular;
                if (frag.Bold) style |= FontStyleFlags.Bold;
                if (frag.Italic) style |= FontStyleFlags.Italic;
                height = Math.Max(height, g.MeasureString(string.IsNullOrEmpty(frag.Text) ? " " : frag.Text, GetLineFont(style)).Height);
            }

            var label = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();
            label.Font = GetLineFont(FontStyleFlags.Regular);
            label.ForeColor = TextColor;
            label.Layout = new Geometry.LayoutF(0, y, Size.X, height);
            label.WordWrap = false;
            label.TextAlign = ContentAlign.MiddleLeft;
            label.Handler.SetTextFragments(new List<TextFragment>(fragments));

            ScrollContainer.AddChild(label);
            ScrollContainer.RefreshContentSize(forceAlignment: true);  // = y + height
        }

        #endregion

        private IFont GetLineFont(FontStyleFlags style)
        {
            foreach (var f in _lineFonts)
            {
                if (f.Style == style && f.Size == Font.Size &&
                    (ReferenceEquals(f.FontFamily, Font.FontFamily) || f.FontFamily.Name == Font.FontFamily.Name))
                    return f;
            }

            var font = GraphicsBackend.Factory.CreateFont(Font.FontFamily, Font.Size, style);
            _lineFonts.Add(font);
            return font;
        }

        protected override void DisposeUI()
        {
            base.DisposeUI();

            // Labels are children and get disposed first; they don't own these.
            foreach (var f in _lineFonts)
                f.Dispose();
            _lineFonts.Clear();

            if (_ownsFont)
                _font?.Dispose();
            _font = null;
            _ownsFont = false;
        }

        #region Draw

        public RectangleF GetAbsClipRect() => ScrollContainer.GetAbsClippingRect();

        #endregion

        /// <summary>
        /// Clears all log entries and optionally disposes their resources.
        /// </summary>
        /// <param name="disposeChildren">
        /// Whether to call Dispose() on each UILabel before removal.
        /// </param>
        public void ClearLogs(bool disposeChildren = true)
        {
            var labels = ScrollContainer.Children.OfType<UILabel>().ToList();
            foreach (var label in labels)
            {
                if (disposeChildren)
                    label.Dispose();

                ScrollContainer.RemoveChild(label);
            }

            // Reset scroll, layout, or cached text metrics
            ResetState();
        }

        /// <summary>
        /// Resets internal state after clearing.
        /// </summary>
        private void ResetState()
        {
            // Example: reset scroll position or internal log buffer
            ScrollContainer.RefreshContentSize(forceAlignment: true);  // No labels left: 0
            // if you have an internal string buffer, clear it too
            // _logBuffer.Clear();
        }
    }

    /// <summary>
    /// Single-line text display element.
    /// </summary>
    public class UITextLine : UIElement
    {
        public string Text { get; }
        public Color Color { get; }
        public IFont Font { get; }

        public UITextLine(Geometry.LayoutF layout, string text, Color color, IFont font)
        {
            Layout = layout;
            Text = text;
            Color = color;
            Font = font;
            RendererBase = new UITextLineRenderer(this);
        }
    }

    public class UITextLineRenderer : UIRenderer<UITextLine>
    {
        private readonly UITextLine _element;

        public UITextLineRenderer(UITextLine element)
        {
            _element = element;
        }

        protected override void OnRender(IGraphics g, UITextLine element)
        {
            var rect = element.GetCurrentAbsoluteBounds();
            using var brush = GraphicsBackend.Factory.CreateSolidBrush(element.Color);
            g.DrawString(element.Text, element.Font, brush, rect.X, rect.Y);
        }
    }

    /// <summary>
    /// A run of text with its own color and bold/italic style (data only).
    /// </summary>
    public struct TextFragment
    {
        public string Text;
        public Color Color;
        public bool Bold;
        public bool Italic;
    }
}
