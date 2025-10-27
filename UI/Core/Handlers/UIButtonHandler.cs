/* ----- ----- ----- ----- */
// UIButtonHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Windows.Forms;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    public class UIButtonHandler : UIHandler<UIButton, UIButtonHandler, UIButtonRenderer>
    {

        // 按鈕點擊 Action
#nullable enable
        public Action? Action { get; set; }
#nullable disable

        // 高亮狀態
        public bool IsHighlighted { get; set; } = false;
        public UIButtonHandler() { }

        #region Mouse Handling

        internal override bool HandleMouseClick(MouseEventArgs e)
        {
            if (!Element.IsEnabled) return false; // 不可用時不觸發
            Action?.Invoke();
            return true;
        }

        #endregion
    }

    public class UIButtonHandler<TEnum> : UIButtonHandler
        where TEnum : Enum
    {
#nullable enable
        public new Action<TEnum>? Action { get; set; }
#nullable disable

        internal void OnClick(TEnum type)
        {
            Action?.Invoke(type);
        }
    }
}
