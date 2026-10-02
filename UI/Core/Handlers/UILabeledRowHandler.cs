/* ----- ----- ----- ----- */
// UILabeledRowHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    /// <summary>
    /// Handler for <see cref="UILabeledRow"/>: the row itself takes no input (its control,
    /// a child, handles its own).
    /// </summary>
    public class UILabeledRowHandler : UIContainerHandler<UILabeledRow, UILabeledRowHandler, UILabeledRowRenderer>
    {
        public UILabeledRowHandler() { }
    }
}
