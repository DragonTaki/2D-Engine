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

namespace Engine.UI.Core.Handlers
{
    public class UILabelHandler : UIHandler<UILabel, UILabelHandler, UILabelRenderer>
    {
        private UILabel Label => (UILabel)Element;

        /// <summary>
        /// Replaces the label's inline text fragments (null or empty: plain Text is drawn).
        /// Invalidates the layout like setting Text does, since Auto sizes measure the fragments.
        /// </summary>
        /// <param name="fragments">The runs to draw as one line.</param>
        public void SetTextFragments(List<TextFragment> fragments)
        {
            Label._fragments = fragments;
            Label.InvalidateLayout();
        }

        /*protected override bool HandleMouseDown(MouseEventArgs e)
        {
            if (!IsSelectable) return false;

            SelectionStart = GetCharIndexAtPoint(e.Location);
            SelectionEnd = SelectionStart;
            return true; // Means the event is handled and is not propagated further
        }*/
    }
}
