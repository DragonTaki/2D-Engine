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
    /// A generic node element that does not render itself and always participates in hit testing.
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
        /// Always returns true to indicate this element participates in hit testing.
        /// </summary>
        /// <param name="point">The point in absolute coordinates to test against this element.</param>
        /// <returns>Always returns <c>true</c>.</returns>
        public override bool HitTest(PointF point) => true;

        #endregion
    }
}
