/* ----- ----- ----- ----- */
// UILabelHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;
using Engine.UI.Elements;

namespace Engine.UI.Core.Handlers
{
    public class UILabelHandler : UIHandler<UILabel, UILabelHandler, UILabelRenderer>
    {
        private UILabel Label => (UILabel)Element;

        public void SetTextFragments(List<TextFragment> fragments)
        {
            Label._fragments = fragments;
        }

        /*protected override bool HandleMouseDown(MouseEventArgs e)
        {
            if (!IsSelectable) return false;

            SelectionStart = GetCharIndexAtPoint(e.Location);
            SelectionEnd = SelectionStart;
            return true; // 表示事件已處理，不再往上傳遞
        }*/
    }
}
