/* ----- ----- ----- ----- */
// UINumberFieldRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.Platform;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UINumberField"/>: the box and text of
    /// <see cref="UITextInputRenderer{TElement, THandler, TRenderer}"/>, with the box narrowed
    /// to leave room for the unit, which is drawn after it (vertically centered).
    /// </summary>
    public class UINumberFieldRenderer : UITextInputRenderer<UINumberField, UINumberFieldHandler, UINumberFieldRenderer>
    {
        public UINumberFieldRenderer() { }

        /// <summary>The element's width without the unit and its gap.</summary>
        protected override RectangleF BoxBounds(IGraphics g, UINumberField element, RectangleF bounds)
        {
            float reserved = UnitWidth(g, element);
            if (reserved > 0f)
                reserved += element.UnitGap;
            return new RectangleF(bounds.X, bounds.Y, Math.Max(0f, bounds.Width - reserved), bounds.Height);
        }

        protected override void OnAfterRender(IGraphics g, UINumberField element, RectangleF bounds)
        {
            if (string.IsNullOrEmpty(element.Unit) || element.Font == null)
                return;

            var style = element.Style ?? TextFieldDefaults.Style;
            float width = UnitWidth(g, element);
            float height = g.MeasureString(element.Unit, element.Font).Height;
            float x = bounds.Right - width;
            float y = bounds.Y + (bounds.Height - height) / 2f;
            if (element.UnitBrush != null)
            {
                g.DrawString(element.Unit, element.Font, element.UnitBrush, x, y);
                return;
            }
            float opacity = element.IsEnabled ? 1f : Math.Clamp(style.DisabledOpacity, 0f, 1f);
            using var brush = GraphicsBackend.Factory.CreateSolidBrush(Fade(style.TextColor, opacity));
            g.DrawString(element.Unit, element.Font, brush, x, y);
        }

        private static float UnitWidth(IGraphics g, UINumberField element) =>
            string.IsNullOrEmpty(element.Unit) || element.Font == null ? 0f : g.MeasureString(element.Unit, element.Font).Width;
    }
}
