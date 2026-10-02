/* ----- ----- ----- ----- */
// UITabBarRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.Styles;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;

namespace Engine.UI.Core.Renderers
{
    /// <summary>
    /// Renderer for <see cref="UITabBar"/>: each tab as a button box with its text (the
    /// selected tab in the selected style), then the indicator bar along the bottom of the
    /// selected tab.
    /// </summary>
    public class UITabBarRenderer : UIRenderer<UITabBar, UITabBarHandler, UITabBarRenderer>
    {
        public UITabBarRenderer() { }

        public override void OnRender(IGraphics g, UITabBar element)
        {
            var style = element.Style ?? TabBarDefaults.Style;
            IButtonDrawStyle tabStyle = style.TabStyle ?? DefaultStyles.DefaultButtonStyle;
            IButtonDrawStyle selectedStyle = style.SelectedTabStyle ?? tabStyle;

            for (int i = 0; i < element.Tabs.Count; i++)
            {
                var bounds = element.GetTabBounds(i);
                if (bounds.Width <= 0f || bounds.Height <= 0f)
                    continue;

                (i == element.SelectedIndex ? selectedStyle : tabStyle)?.Draw(g, element.Tabs[i], bounds);
            }

            if (element.SelectedIndex < 0 || style.IndicatorHeight <= 0f)
                return;

            var selected = element.GetTabBounds(element.SelectedIndex);
            var indicator = selected.Inset(style.IndicatorInset, 0f);
            if (indicator.Width <= 0f)
                return;

            using var brush = GraphicsBackend.Factory.CreateSolidBrush(style.IndicatorColor);
            g.FillRectangle(brush, indicator.X, selected.Y + selected.Height - style.IndicatorHeight,
                indicator.Width, style.IndicatorHeight);
        }
    }
}
