/* ----- ----- ----- ----- */
// UIDropdownList.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.UI.Constants.Components;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;
using Engine.UI.Utils;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// The open list of a <see cref="UIDropdown"/>, created and removed by its handler
    /// (<see cref="UIDropdownHandler.Open"/> / <see cref="UIDropdownHandler.Close"/>). It lives on
    /// the UI root's overlay layer and covers all of it, like a dialog's mask, so while it is open
    /// it takes every press (one outside the list closes it), hover and wheel event - nothing
    /// underneath reacts, and the page under it does not scroll. Only the list itself is drawn
    /// (<see cref="UIDropdownListRenderer"/>), placed next to the closed box every frame
    /// (<see cref="ComputeLayout"/>), so it follows the box when the window is resized.
    /// </summary>
    public class UIDropdownList : UIElement<UIDropdownList, UIDropdownListHandler, UIDropdownListRenderer>
    {
        #region Properties

        /// <summary>The dropdown this list belongs to (null once closed).</summary>
        public UIDropdown Owner { get; internal set; }

        /// <summary>The option under the mouse, or -1.</summary>
        public int HoverIndex { get; internal set; } = -1;

        /// <summary>How far the options are scrolled (design units, 0 = the first option at the top).</summary>
        public float Scroll { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>Creates a list that covers its parent (the overlay layer) on all four edges.</summary>
        public UIDropdownList()
            : base(zIndex: DropdownDefaults.ListZIndex, isPersistent: false, type: UIElementType.Generic)
        {
            LayoutRules.PositionMode = PositionMode.Absolute;
            LayoutRules.Left = 0f;
            LayoutRules.Top = 0f;
            LayoutRules.Right = 0f;
            LayoutRules.Bottom = 0f;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Where the list is now: next to the owner's current absolute bounds, inside this
        /// element's bounds (the whole overlay layer, i.e. the UI area; the parent's before the
        /// first layout pass), with the owner's style.
        /// </summary>
        public DropdownLayout ComputeLayout()
        {
            var style = Owner?.Style ?? DropdownDefaults.Style;
            var viewport = GetCurrentAbsoluteBounds();
            if ((viewport.Width <= 0f || viewport.Height <= 0f) && Parent is UIElement parent)
                viewport = parent.GetCurrentAbsoluteBounds();

            RectangleF box = Owner != null ? Owner.GetCurrentAbsoluteBounds() : RectangleF.Empty;
            float itemHeight = style.ItemHeight > 0f ? style.ItemHeight : DropdownDefaults.Style.ItemHeight;
            return DropdownLayout.Compute(box, viewport, Owner?.Options.Count ?? 0, itemHeight,
                style.MaxVisibleItems, style.ListGap, style.ListPadding);
        }

        #endregion
    }
}
