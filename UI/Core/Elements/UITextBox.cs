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
    public abstract class UITextBox<THandler> : UIContainer<THandler>
        where THandler : UITextBoxHandler<THandler>
    {
        #region Fields / Properties

        protected UITextBoxRenderer<THandler> _textBoxRenderer;
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

        public UITextBox()
        {
            LocalPosition = Vector2F.Zero;
            Size = Vector2F.Zero;
        }

        public UITextBox(Vector2F position, Vector2F size)
        {
            LocalPosition.Base = position;
            Size = size;
        }

        #endregion

        #region Initialization

        protected override void OnInit((IUiFactory, THandler) arg)
        {
            _factory = arg.Item1;
            _handler = arg.Item2;

            LocalPosition = TextBoxDefaults.Position;
            Size = TextBoxDefaults.Size;
            LineHeight = Font.Height;

            BuildScrollContainer();

            UpdateScrollContentHeight();

            _textBoxRenderer = CreateTextBoxRenderer();
            if (_textBoxRenderer == null) throw new Exception("_menuRenderer is null!");
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

            foreach (var frag in _fragments)
            {
                var lines = frag.Text.Split('\n');
                foreach (var line in lines)
                {
                    var label = new UILabel
                    {
                        Text = line,
                        Font = new Font(Font,
                            (frag.Bold ? FontStyle.Bold : FontStyle.Regular) |
                            (frag.Italic ? FontStyle.Italic : FontStyle.Regular)),
                        ForeColor = frag.Color,
                        Layout = new Geometry.LayoutF(0, y, Size.X, LineHeight),
                        WordWrap = false, // 一行一個 Label
                        TextAlign = ContentAlignment.MiddleLeft,
                    };

                    label.LocalPosition.Current.Y = y;
                    ScrollContainer.AddChild(label);
                    _labels.Add(label);

                    y += LineHeight + LineSpacing;
                }
            }

            ScrollContainer.ContentHeight = Math.Max(0, y - LineSpacing);
        }

        #endregion

        #region Draw

        public RectangleF GetAbsClipRect() => ScrollContainer.GetAbsClippingRect();

        protected virtual UITextBoxRenderer<THandler> CreateTextBoxRenderer()
        {
            return new UITextBoxRenderer<THandler>(this);
        }
        protected override void OnDraw(Graphics g)
        {
            if (_textBoxRenderer == null) throw new Exception("_textBoxRenderer is null!");

            _textBoxRenderer.Render(g, this); 
        }

        #endregion
    }

    /// <summary>
    /// 單行文字顯示元素
    /// </summary>
    public class UITextLine : UIElement
    {
        private readonly string _text;
        private readonly Color _color;
        private readonly Font _font;

        public UITextLine(Geometry.LayoutF layout, string text, Color color, Font font)
        {
            Layout = layout;
            _text = text;
            _color = color;
            _font = font;
        }

        protected override void OnDraw(Graphics g)
        {
            var rect = GetCurrentAbsoluteBounds();
            using var brush = new SolidBrush(_color);
            g.DrawString(_text, _font, brush, rect.X, rect.Y);
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
