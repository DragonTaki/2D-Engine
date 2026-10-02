/* ----- ----- ----- ----- */
// DropdownLayout.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace Engine.UI.Utils
{
    /// <summary>
    /// Where an open dropdown's list goes and how it scrolls (pure geometry, no drawing; used by
    /// <c>UIDropdownList</c>): below the closed box when the whole list fits there, else above it
    /// when it fits there, else on the side with more room, cut to the items that fit (at least
    /// one) and scrolled. The list is as wide as the box.
    /// </summary>
    public readonly struct DropdownLayout
    {
        /// <summary>The list's box (absolute, padding included).</summary>
        public RectangleF ListRect { get; }

        /// <summary>The part of the list the items are drawn in (the box minus the vertical padding).</summary>
        public RectangleF ItemsRect { get; }

        /// <summary>Whether the list opens above the closed box.</summary>
        public bool OpensUp { get; }

        /// <summary>How many items are visible at once (at least 1 when there are any).</summary>
        public int VisibleCount { get; }

        /// <summary>Height of one item.</summary>
        public float ItemHeight { get; }

        /// <summary>Number of items.</summary>
        public int ItemCount { get; }

        /// <summary>Largest scroll offset (0 when every item is visible).</summary>
        public float MaxScroll => Math.Max(0f, ItemCount * ItemHeight - ItemsRect.Height);

        private DropdownLayout(RectangleF listRect, RectangleF itemsRect, bool opensUp, int visibleCount, float itemHeight, int itemCount)
        {
            ListRect = listRect;
            ItemsRect = itemsRect;
            OpensUp = opensUp;
            VisibleCount = visibleCount;
            ItemHeight = itemHeight;
            ItemCount = itemCount;
        }

        /// <summary>
        /// Places the list of <paramref name="itemCount"/> items for a closed box at
        /// <paramref name="box"/> inside <paramref name="viewport"/> (the area the list must stay in).
        /// </summary>
        /// <param name="box">The closed dropdown's absolute bounds.</param>
        /// <param name="viewport">The area the list must fit in (e.g. the window's UI area).</param>
        /// <param name="itemCount">Number of options.</param>
        /// <param name="itemHeight">Height of one option (positive).</param>
        /// <param name="maxVisibleItems">Most options shown at once (at least 1); more scroll.</param>
        /// <param name="gap">Space between the box and the list.</param>
        /// <param name="padding">Space above the first and below the last visible option inside the list.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="itemHeight"/> is not positive.</exception>
        public static DropdownLayout Compute(RectangleF box, RectangleF viewport, int itemCount, float itemHeight,
            int maxVisibleItems, float gap, float padding)
        {
            if (!(itemHeight > 0f))
                throw new ArgumentOutOfRangeException(nameof(itemHeight), itemHeight, "The item height must be positive.");

            itemCount = Math.Max(0, itemCount);
            gap = Math.Max(0f, gap);
            padding = Math.Max(0f, padding);
            int wanted = Math.Min(itemCount, Math.Max(1, maxVisibleItems));

            float below = viewport.Bottom - (box.Bottom + gap);
            float above = (box.Top - gap) - viewport.Top;
            float wantedHeight = wanted * itemHeight + padding * 2f;

            bool up;
            int visible;
            if (wantedHeight <= below)
            {
                up = false;
                visible = wanted;
            }
            else if (wantedHeight <= above)
            {
                up = true;
                visible = wanted;
            }
            else
            {
                // Neither side takes the whole list: the roomier side, as many items as fit there.
                up = above > below;
                float room = (up ? above : below) - padding * 2f;
                visible = Math.Clamp((int)MathF.Floor(room / itemHeight), Math.Min(1, wanted), wanted);
            }

            float itemsHeight = visible * itemHeight;
            float listHeight = itemsHeight + padding * 2f;
            float top = up ? box.Top - gap - listHeight : box.Bottom + gap;
            var list = new RectangleF(box.X, top, box.Width, listHeight);
            var items = new RectangleF(box.X, top + padding, box.Width, itemsHeight);
            return new DropdownLayout(list, items, up, visible, itemHeight, itemCount);
        }

        /// <summary><paramref name="scroll"/> limited to 0..<see cref="MaxScroll"/>.</summary>
        public float ClampScroll(float scroll) => Math.Clamp(scroll, 0f, MaxScroll);

        /// <summary>
        /// The scroll offset closest to <paramref name="scroll"/> that shows item
        /// <paramref name="index"/> whole (e.g. the chosen option when the list opens).
        /// </summary>
        public float ScrollToShow(int index, float scroll)
        {
            if (index < 0 || index >= ItemCount)
                return ClampScroll(scroll);
            float itemTop = index * ItemHeight;
            float itemBottom = itemTop + ItemHeight;
            if (itemTop < scroll)
                scroll = itemTop;
            else if (itemBottom > scroll + ItemsRect.Height)
                scroll = itemBottom - ItemsRect.Height;
            return ClampScroll(scroll);
        }

        /// <summary>The item under the absolute point (<paramref name="x"/>, <paramref name="y"/>) at <paramref name="scroll"/>, or -1.</summary>
        public int ItemIndexAt(float x, float y, float scroll)
        {
            if (x < ItemsRect.Left || x >= ItemsRect.Right || y < ItemsRect.Top || y >= ItemsRect.Bottom)
                return -1;
            int index = (int)MathF.Floor((y - ItemsRect.Top + scroll) / ItemHeight);
            return index >= 0 && index < ItemCount ? index : -1;
        }

        /// <summary>The absolute top of item <paramref name="index"/> at <paramref name="scroll"/> (may be outside <see cref="ItemsRect"/>).</summary>
        public float ItemTop(int index, float scroll) => ItemsRect.Top + index * ItemHeight - scroll;
    }
}
