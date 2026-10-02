/* ----- ----- ----- ----- */
// TabBarDefaults.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Mathematics;
using Engine.Styles;

namespace Engine.UI.Constants.Components
{
    /// <summary>
    /// Defaults of <c>UITabBar</c>: its size before any layout, and the style used when the
    /// bar has none of its own (tabs in <see cref="DefaultStyles.DefaultButtonStyle"/>, the
    /// selected one marked only by the indicator bar). Neutral placeholders.
    /// </summary>
    public static class TabBarDefaults
    {
        /// <summary>Size of a new tab bar.</summary>
        public static readonly Vector2F Size = new Vector2F(400.0f, 48.0f);

        /// <summary>The style of a tab bar whose <c>Style</c> is null.</summary>
        public static readonly TabBarStyle Style = new TabBarStyle
        {
            TabStyle = null,
            SelectedTabStyle = null,
            TabGap = 8.0f,
            IndicatorColor = Color.FromArgb(255, 33, 150, 243),  // #2196F3
            IndicatorHeight = 4.0f,
            IndicatorInset = 8.0f,
        };
    }
}
