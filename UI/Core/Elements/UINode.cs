/* ----- ----- ----- ----- */
// UINode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.UI.Constants.Components;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// A generic node element that does not render itself. Its <see cref="HitTest"/> always
    /// succeeds, but it is never <see cref="IsInteractable"/>: a parent's deep hit test
    /// (<c>HitTestDeep</c>) skips it and returns only its interactable descendants, and it
    /// ignores mouse events itself. A top-level <c>HitTestDeep</c> called on the node (e.g.
    /// <c>Root.HitTestDeep</c> in the input router) does return the node when no descendant
    /// is hit - the "nothing else here" result, whose mouse handlers then do nothing.
    /// Serves as a structural container for other UI elements in the UI hierarchy.
    /// </summary>
    public abstract class UINode : UIElement
    {
        #region Properties

        /// <summary>
        /// Indicates that this element is not interactable and cannot receive input.
        /// </summary>
        public override bool IsInteractable => false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UINode"/> class.
        /// </summary>
        /// <param name="zIndex">Z-order of this node in the UI hierarchy.</param>
        /// <param name="isPersistent">Whether this node persists across screens.</param>
        /// <param name="type">Type of the UI element, default is Generic.</param>
        public UINode(int zIndex = 0, bool isPersistent = false, UIElementType type = UIElementType.Generic)
            : base(zIndex, isPersistent, type)
        {
            /* no-op */
        }

        #endregion

        #region Methods

        /// <summary>
        /// Always returns true: the node's own bounds test passes for any point. A parent's
        /// <c>HitTestDeep</c> still skips the node because <see cref="IsInteractable"/> is false
        /// (only a top-level call on the node itself can return it, see the class remarks).
        /// </summary>
        /// <param name="point">The point in absolute coordinates to test against this element.</param>
        /// <returns>Always returns <c>true</c>.</returns>
        public override bool HitTest(PointF point) => true;

        #endregion
    }
}
