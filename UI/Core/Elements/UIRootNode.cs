/* ----- ----- ----- ----- */
// UIRootNode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Constants.Components;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Represents the root container of the UI hierarchy.
    /// This is typically the topmost element that holds all other UI elements,
    /// such as screens, panels, dialogs, or overlays.
    /// </summary>
    public class UIRootNode : UINode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIRootNode"/> class.
        /// </summary>
        /// <param name="zIndex">
        /// The Z-order index of the root node.
        /// Elements with higher <paramref name="zIndex"/> are rendered on top.
        /// Default is 0.
        /// </param>
        /// <param name="isPersistent">
        /// Indicates whether this root node should persist across different UI screens.
        /// Default is true.
        /// </param>
        /// <param name="type">
        /// The type of this UI element.
        /// Should be <see cref="UIElementType.Root"/> for root nodes.
        /// </param>
        public UIRootNode(int zIndex = 0, bool isPersistent = true, UIElementType type = UIElementType.Root)
            : base(zIndex, isPersistent, type)
        {
            /* no-op */
        }
    }
}
