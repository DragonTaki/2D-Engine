/* ----- ----- ----- ----- */
// UITextBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

using Engine.Geometry;
using Engine.UI.Core.Elements;

namespace Engine.UI.Elements
{
    /// <summary>
    /// Pure Engine LoggerBox: 渲染文字、支援滾動，不依賴 WinForms 控件
    /// </summary>
    public class UITextBox : UIElement
    {
        private ScrollTextBox _scrollBox;
        private readonly List<TextFragment> _fragments = new();

        // 可動態設定字型、顏色、行高、背景
        public Font Font
        {
            get => _scrollBox.Font;
            set => _scrollBox.Font = value;
        }

        public float LineHeight
        {
            get => _scrollBox.LineHeight;
            set => _scrollBox.LineHeight = value;
        }

        public Color BackgroundColor
        {
            get => _scrollBox.BackgroundColor;
            set => _scrollBox.BackgroundColor = value;
        }

        public Color TextColor
        {
            get => _scrollBox.TextColor;
            set => _scrollBox.TextColor = value;
        }

        public UITextBox(LayoutF bounds)
        {
            _scrollBox = new ScrollTextBox(bounds);
            AddChild(_scrollBox); // 組合到 UI 層級
        }

        /// <summary>
        /// 新增一行文字
        /// </summary>
        public void AppendLine(string text, Color? color = null)
        {
            _fragments.Add(new TextFragment
            {
                Text = text,
                Color = color ?? TextColor,
                Bold = false,
                Italic = false
            });

            _scrollBox.SetFragments(_fragments);
        }

        /// <summary>
        /// 清空文字
        /// </summary>
        public void Clear()
        {
            _fragments.Clear();
            _scrollBox.SetFragments(_fragments);
        }

        /// <summary>
        /// 滾動，delta: 正數向下，負數向上
        /// </summary>
        public void Scroll(int delta)
        {
            _scrollBox.Scroll(delta);
        }

        protected override void OnDraw(Graphics g)
        {
            _scrollBox.Draw(g);
        }
    }
}
