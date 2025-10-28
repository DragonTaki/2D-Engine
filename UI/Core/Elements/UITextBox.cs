/* ----- ----- ----- ----- */
// UITextBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Engine.Mathematics;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Utils;

namespace Engine.UI.Elements
{
    /// <summary>
    /// Pure Engine LoggerBox: 渲染文字、支援滾動，不依賴 WinForms 控件
    /// </summary>
    public abstract class UITextBox<TElement, THandler, TRenderer> : UIContainer<TElement, THandler, TRenderer>
        where TElement : UITextBox<TElement, THandler, TRenderer>
        where THandler : UITextBoxHandler<TElement, THandler, TRenderer>
        where TRenderer : UITextBoxRenderer<TElement, THandler, TRenderer>
    {
        #region Fields / Properties
        public UIScrollContainer ScrollContainer { get; private set; }

        // 儲存所有要顯示的文字段落
        protected List<TextFragment> _fragments = new();
        public List<UILabel> _labels = new();
        // 可動態調整屬性
        public Font Font { get; set; } = SystemFonts.DefaultFont;
        public float LineHeight { get; set; }
        public Color BackgroundColor { get; set; } = Color.Black;
        public Color TextColor { get; set; } = Color.White;
        public float LineSpacing { get; set; } = 4f;

        #endregion

        #region Constructor

        public UITextBox() { }

        #endregion

        #region Initialization

        public override void Init(IUiFactory factory, THandler handler, TRenderer renderer)
        {
            Console.WriteLine($"[UITextBox]Init 3 generic Current type: {this?.GetType().FullName ?? "null"}, IsInitialized: {IsInitialized}");
            if (IsInitialized) return;
            IsInitialized = true;
            _factory = factory;

            BeforeInit(factory);

            // 綁定 Handler
            Handler = handler;
            Handler.Element = (TElement)(object)this;
            Console.WriteLine($"[UITextBox]Handler type: {Handler?.GetType().FullName ?? "null"}");

            // 綁定 Renderer
            Renderer = renderer;
            Renderer.Element = (TElement)(object)this;
            Console.WriteLine($"[UITextBox]Renderer type: {Renderer?.GetType().FullName ?? "null"}");
            BuildScrollContainer();

            base.Init();

            OnInit(factory);
            AfterInit(factory);
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
        /// 新增一行文字
        /// </summary>
        public void AppendLine(string text, Color? color = null, bool bold = false, bool italic = false)
        {
            var frag = new TextFragment
            {
                Text = text,
                Color = color ?? TextColor,
                Bold = bold,
                Italic = italic
            };

            _fragments.Add(frag);
            RefreshTextContent();
        }

        /// <summary>
        /// 清空文字內容
        /// </summary>
        public void Clear()
        {
            _fragments.Clear();
            ScrollContainer.RemoveAllChild();
            ScrollContainer.ContentHeight = 0f;
        }

        protected void UpdateScrollContentHeight()
        {
            if (_labels.Count == 0) return;
            var buttonHeight = _labels[0].Size.Y;
            ScrollContainer.ContentHeight = _labels.Count * (buttonHeight + LineSpacing);
        }

        public List<UILabel> GetVisibleLines()
        {
            UIElementUtils.UpdateVisibleState(_labels, ScrollContainer.GetAbsClippingRect());
            return _labels.Where(b => b.IsEnabled).ToList();
        }

        /// <summary>
        /// 重新建立所有文字行元素
        /// </summary>
        protected void RefreshTextContent()
        {
            ScrollContainer.RemoveAllChild();
            _labels.Clear();

            float y = 0f;

            using var bmp = new Bitmap(1, 1);
            using var g = Graphics.FromImage(bmp);

            foreach (var frag in _fragments)
            {
                var lines = frag.Text.Split('\n');
                foreach (var line in lines)
                {
                    Font font = new Font(Font,
                        (frag.Bold ? FontStyle.Bold : FontStyle.Regular) |
                        (frag.Italic ? FontStyle.Italic : FontStyle.Regular));

                    SizeF size = g.MeasureString(line, font);
        
                    var label = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();

                    label.Text = line;
                    label.Font = font;
                    label.ForeColor = frag.Color;
                    label.Layout = new Geometry.LayoutF(0, y, Size.X, size.Height);
                    label.WordWrap = false; // 一行一個 Label
                    label.TextAlign = ContentAlignment.MiddleLeft;

                    label.LocalPosition.Current.Y = y;

                    ScrollContainer.AddChild(label);
                    _labels.Add(label);

                    y += size.Height + LineSpacing;
                }
            }

            ScrollContainer.ContentHeight = Math.Max(0, y - LineSpacing);
        }

        #endregion

        #region Draw

        public RectangleF GetAbsClipRect() => ScrollContainer.GetAbsClippingRect();

        #endregion
    }

    /// <summary>
    /// 單行文字顯示元素
    /// </summary>
    public class UITextLine : UIElement
    {
        public string Text { get; }
        public Color Color { get; }
        public Font Font { get; }

        public UITextLine(Geometry.LayoutF layout, string text, Color color, Font font)
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

        protected override void OnRender(Graphics g, UITextLine element)
        {
            var rect = element.GetCurrentAbsoluteBounds();
            using var brush = new SolidBrush(element.Color);
            g.DrawString(element.Text, element.Font, brush, rect.X, rect.Y);
        }
    }

    /// <summary>
    /// 文字資料結構
    /// </summary>
    public struct TextFragment
    {
        public string Text;
        public Color Color;
        public bool Bold;
        public bool Italic;
    }
}
