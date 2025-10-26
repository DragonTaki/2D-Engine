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

using Engine.UI.Core.Elements;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;
using Engine.UI.Elements;

namespace Engine.UI.Core.Handlers
{
    public abstract class UITextBoxHandler<THandler> : UIContainerHandler<THandler>
        where THandler : UITextBoxHandler<THandler>
    {
        protected UITextBox<THandler> _textbox;
        public event Action<string> OnMessageAdded;

        protected override void OnInit((IUiFactory, UIContainer<THandler>) arg)
        {
            _factory = arg.Item1;

            if (arg.Item2 is UITextBox<THandler> textbox)
            {
                _textbox = textbox;
                OnMenuInit((_factory, textbox));
            }

            _navigationManager = _factory.Resolve<NavigationManager>();

            OnMenuInit(arg);
        }

        protected virtual void OnMenuInit((IUiFactory, UIContainer<THandler>) arg) { }
        

        /// <summary>
        /// 新增一則訊息，使用預設文字顏色與樣式
        /// </summary>
        public void AddMessage(string msg)
        {
            AddMessage(msg, _textbox.TextColor, bold: false, italic: false);
        }

        /// <summary>
        /// 新增一則訊息，可自訂文字顏色與樣式
        /// </summary>
        public void AddMessage(string msg, Color color, bool bold = false, bool italic = false)
        {
            // 將訊息包成 TextFragment 並加入 UITextBox
            _textbox.AppendLine(msg, color, bold, italic);

            // 保留原本事件通知
            OnMessageAdded?.Invoke(msg);
        }

        /// <summary>
        /// 新增一則多色訊息
        /// </summary>
        public void AddMessage(IEnumerable<TextFragment> fragments)
        {
            foreach (var frag in fragments)
            {
                _textbox.AppendLine(frag.Text, frag.Color, frag.Bold, frag.Italic);
            }

            // 可以傳送事件，也可改為傳 fragments
            OnMessageAdded?.Invoke(string.Join("\n", fragments.Select(f => f.Text)));
        }
    }
}
