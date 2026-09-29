/* ----- ----- ----- ----- */
// UITextBoxHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/25
// Update Date: 2025/10/25
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Engine.UI.Core.Renderers;
using Engine.UI.Elements;

namespace Engine.UI.Core.Handlers
{
    public abstract class UITextBoxHandler<TElement, THandler, TRenderer> : UIContainerHandler<TElement, THandler, TRenderer>
        where TElement : UITextBox<TElement, THandler, TRenderer>
        where THandler : UITextBoxHandler<TElement, THandler, TRenderer>
        where TRenderer : UITextBoxRenderer<TElement, THandler, TRenderer>
    {
        private UITextBox<TElement, THandler, TRenderer> TextBox => (UITextBox<TElement, THandler, TRenderer>)Element;
        public event Action<string> OnMessageAdded;
        
        /// <summary>
        /// 新增一則訊息，使用預設文字顏色與樣式
        /// </summary>
        public void AddMessage(string msg)
        {
            AddMessage(msg, TextBox.TextColor, bold: false, italic: false);
        }

        /// <summary>
        /// 新增一則訊息，可自訂文字顏色與樣式
        /// </summary>
        public void AddMessage(string msg, Color color, bool bold = false, bool italic = false)
        {
            // 將訊息包成 TextFragment 並加入 UITextBox
            TextBox.AppendLine(msg, color, bold, italic);

            // 保留原本事件通知
            OnMessageAdded?.Invoke(msg);
        }

        /// <summary>
        /// 新增一則多色訊息
        /// </summary>
        public void AddMessage(IEnumerable<TextFragment> fragments)
        {
            // Materialize once: the sequence used to be enumerated twice (a one-shot
            // iterator would log an empty message on the second pass).
            var list = fragments?.ToList() ?? new List<TextFragment>();

            // One message = one line of inline runs (it used to become one line per fragment).
            TextBox.AppendFragmentLine(list);

            // 可以傳送事件，也可改為傳 fragments
            OnMessageAdded?.Invoke(string.Concat(list.Select(f => f.Text)));
        }
    }
}
