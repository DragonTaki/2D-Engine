/* ----- ----- ----- ----- */
// UIDropdownListRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.Geometry;
using Engine.Platform;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UIDropdownList"/>: only the list (the rest of the overlay it
    /// covers stays transparent) - the list box, then, clipped to the items area, one row per
    /// option scrolled by <see cref="UIDropdownList.Scroll"/> (the hovered row filled with the
    /// hover colour, the chosen option's text in the selected colour), and a scroll bar at the
    /// right when not every option fits.
    /// </summary>
    public class UIDropdownListRenderer : UIRenderer<UIDropdownList, UIDropdownListHandler, UIDropdownListRenderer>
    {
        /// <summary>Character measured for the line height (so every row's text is centered the same).</summary>
        private const string LineMeasure = "|";

        /// <summary>Shortest scroll bar thumb, as a fraction of one row's height.</summary>
        private const float MinThumbFraction = 0.5f;

        public UIDropdownListRenderer() { }

        public override void OnRender(IGraphics g, UIDropdownList element)
        {
            var owner = element.Owner;
            if (owner == null || owner.Options.Count == 0)
                return;

            var style = owner.Style ?? DropdownDefaults.Style;
            var layout = element.ComputeLayout();
            var listRect = layout.ListRect;
            if (listRect.Width <= 0f || listRect.Height <= 0f)
                return;

            style.ListBox?.Draw(g, (LayoutF)listRect);

            var items = layout.ItemsRect;
            float scroll = element.Scroll;
            var font = owner.Font;

            g.SetClip(items);
            try
            {
                int first = Math.Max(0, (int)MathF.Floor(scroll / layout.ItemHeight));
                int last = Math.Min(layout.ItemCount - 1, (int)MathF.Floor((scroll + items.Height) / layout.ItemHeight));
                float lineHeight = font != null ? g.MeasureString(LineMeasure, font).Height : 0f;

                for (int i = first; i <= last; i++)
                {
                    float top = layout.ItemTop(i, scroll);
                    if (i == element.HoverIndex)
                    {
                        using var hoverBrush = GraphicsBackend.Factory.CreateSolidBrush(style.HoverColor);
                        g.FillRectangle(hoverBrush, items.X, top, items.Width, layout.ItemHeight);
                    }

                    if (font == null)
                        continue;
                    var color = i == owner.SelectedIndex ? style.SelectedTextColor : style.TextColor;
                    using var textBrush = GraphicsBackend.Factory.CreateSolidBrush(color);
                    g.DrawString(owner.Options[i], font, textBrush, items.X + style.TextInset, top + (layout.ItemHeight - lineHeight) / 2f);
                }
            }
            finally
            {
                g.ResetClip();
            }

            // Scroll bar: a thumb whose length is the visible share of the list and whose
            // position is the scroll's share of its range.
            float maxScroll = layout.MaxScroll;
            if (maxScroll > 0f && style.ScrollBarWidth > 0f)
            {
                float contentHeight = layout.ItemCount * layout.ItemHeight;
                float thumbHeight = Math.Max(items.Height * items.Height / contentHeight, layout.ItemHeight * MinThumbFraction);
                thumbHeight = Math.Min(thumbHeight, items.Height);
                float thumbTop = items.Y + (items.Height - thumbHeight) * Math.Clamp(scroll / maxScroll, 0f, 1f);
                float thumbLeft = items.Right - style.ScrollBarWidth - style.ScrollBarWidth / 2f;
                using var barBrush = GraphicsBackend.Factory.CreateSolidBrush(style.ScrollBarColor);
                g.FillRectangle(barBrush, thumbLeft, thumbTop, style.ScrollBarWidth, thumbHeight);
            }
        }
    }
}
