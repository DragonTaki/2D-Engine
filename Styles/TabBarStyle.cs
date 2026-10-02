/* ----- ----- ----- ----- */
// TabBarStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Styles
{
    /// <summary>
    /// Looks of a tab bar (<c>UITabBar</c>): every tab is a button box drawn with
    /// <see cref="TabStyle"/>, the selected one with <see cref="SelectedTabStyle"/> and an
    /// indicator bar along its bottom edge.
    /// </summary>
    public class TabBarStyle
    {
        /// <summary>A tab that is not selected; null = <see cref="DefaultStyles.DefaultButtonStyle"/>.</summary>
        public IButtonDrawStyle TabStyle { get; set; }

        /// <summary>The selected tab; null = <see cref="TabStyle"/> (the indicator bar still marks it).</summary>
        public IButtonDrawStyle SelectedTabStyle { get; set; }

        /// <summary>Horizontal space between two tabs (design units).</summary>
        public float TabGap { get; set; }

        /// <summary>Colour of the selected tab's indicator bar.</summary>
        public Color IndicatorColor { get; set; }

        /// <summary>Height of the indicator bar (design units; 0 = no bar).</summary>
        public float IndicatorHeight { get; set; }

        /// <summary>Horizontal inset of the indicator bar from the tab's sides (design units).</summary>
        public float IndicatorInset { get; set; }
    }
}
