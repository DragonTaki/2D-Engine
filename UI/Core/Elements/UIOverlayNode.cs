/* ----- ----- ----- ----- */
// UIOverlayNode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2026/09/30
// Version: v1.1
/* ----- ----- ----- ----- */

using Engine.UI.Constants.Components;
using Engine.UI.Constants.Core;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Represents a persistent overlay layer in the UI hierarchy.
    /// Typically used for modals, dialogs, or temporary notifications.
    /// Always rendered above regular UI elements due to its high Z-index.
    /// </summary>
    public sealed class UIOverlayNode : UINode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIOverlayNode"/> class.
        /// </summary>
        /// <param name="zIndex">
        /// The Z-order index of the overlay node.
        /// Defaults to <see cref="int.MaxValue"/> to ensure it appears above other UI elements.
        /// </param>
        /// <param name="isPersistent">
        /// Indicates whether the overlay node should persist across different UI screens.
        /// Default is true.
        /// </param>
        /// <param name="type">
        /// The type of this UI element.
        /// Should be <see cref="UIElementType.Overlay"/> for overlay nodes.
        /// </param>
        public UIOverlayNode(int zIndex = int.MaxValue, bool isPersistent = true, UIElementType type = UIElementType.Overlay)
            : base(zIndex, isPersistent, type)
        {
            // Spans its parent (the root, i.e. the whole viewport), so layout-managed
            // overlays such as dialogs can be centered in it. Hit testing is unaffected
            // (UINode always hits); before this its size was 0 and nothing read it.
            LayoutRules.PositionMode = PositionMode.Absolute;
            LayoutRules.Left = 0f;
            LayoutRules.Top = 0f;
            LayoutRules.Right = 0f;
            LayoutRules.Bottom = 0f;
        }
    }
}
