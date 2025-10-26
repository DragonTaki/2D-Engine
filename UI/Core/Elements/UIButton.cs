/* ----- ----- ----- ----- */
// UIButton.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/10/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Windows.Forms;

using Engine.Styles;
using Engine.UI.Constants.Components;
using Engine.UI.Widgets;

namespace Engine.UI.Core.Elements
{
    public class UIButton : UIElement
    {
        #region Properties

        // 文字可讀寫
        public string Text { get; set; }

        // 按鈕點擊 Action
#nullable enable
        public Action? Action { get; set; }
#nullable disable

        // 高亮狀態
        public bool IsHighlighted { get; set; } = false;

        public IButtonDrawStyle Style { get; set; } = null;

        #endregion

        #region Constructors

        public UIButton() 
            : base(zIndex: 0, isPersistent: false, type: UIElementType.Button)
        {
        }

#nullable enable
        public UIButton(string text, Action? action = null)
            : this()
#nullable disable
        {
            Text = text;
            Action = action;
        }

        #endregion

        protected override void OnInit()
        {
            LocalPosition = ButtonDefaults.Position;
            Size = ButtonDefaults.Size;
        }

        #region Mouse Handling

        public override bool HandleMouseClick(MouseEventArgs e)
        {
            if (!IsEnabled) return false; // 不可用時不觸發
            Action?.Invoke();
            return true;
        }

        #endregion
    }
    
    public class UIButton<TEnum> : UIButton where TEnum : Enum
    {
        public TEnum Type { get; }

#nullable enable
        public UIButton(string text, TEnum type, Action? action = null)
            : base(text, action)
#nullable disable
        {
            Type = type;
        }

        public UIButton(ButtonEntry<TEnum> button)
            : this(button.Label, button.Type, button.OnClick) { }

        public UIButton(string text)
            : this(text, default, null) { }

    }
}