/* ----- ----- ----- ----- */
// UITabBar.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Engine.Geometry;
using Engine.Mathematics;
using Engine.Styles;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// A row of tabs, exactly one of them selected (none while there are no tabs). The tabs
    /// share the bar's width equally, with <see cref="TabBarStyle.TabGap"/> between them; the
    /// bar draws them itself (<see cref="UITabBarRenderer"/>) with <see cref="Style"/>, the
    /// selected tab in its own style and with an indicator bar. A click on a tab selects it
    /// and runs the handler's <see cref="UITabBarHandler.SelectionChanged"/>.
    /// </summary>
    public class UITabBar : UIElement<UITabBar, UITabBarHandler, UITabBarRenderer>
    {
        #region Fields / Properties

        private readonly List<string> _tabs = new();
        private int _selectedIndex = -1;

        /// <summary>The tab texts, left to right.</summary>
        public IReadOnlyList<string> Tabs => _tabs;

        /// <summary>
        /// Index of the selected tab (-1 only while there are no tabs). Setting it only
        /// changes what is shown (no <see cref="UITabBarHandler.SelectionChanged"/>); a click
        /// goes through <see cref="UITabBarHandler.Select"/>, which does notify.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The index is not a tab's.</exception>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value < 0 || value >= _tabs.Count)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"No tab {value} (tab count {_tabs.Count}).");
                _selectedIndex = value;
            }
        }

        /// <summary>How the bar is drawn; null = <see cref="TabBarDefaults.Style"/>.</summary>
        public TabBarStyle Style { get; set; }

        #endregion

        #region Constructors

        /// <summary>Creates a bar without tabs.</summary>
        public UITabBar()
            : base(zIndex: 0, isPersistent: false, type: UIElementType.Generic)
        {
        }

        #endregion

        /// <summary>Applies the default size (a layout style replaces it).</summary>
        protected override void OnInit()
        {
            Size = TabBarDefaults.Size;
        }

        #region Methods

        /// <summary>
        /// Replaces the tabs and selects the first one (none when <paramref name="tabs"/> is
        /// empty). Does not run <see cref="UITabBarHandler.SelectionChanged"/>.
        /// </summary>
        /// <param name="tabs">The tab texts, left to right.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tabs"/> is null.</exception>
        public void SetTabs(IEnumerable<string> tabs)
        {
            ArgumentNullException.ThrowIfNull(tabs);

            _tabs.Clear();
            _tabs.AddRange(tabs.Select(t => t ?? string.Empty));
            _selectedIndex = _tabs.Count > 0 ? 0 : -1;
        }

        /// <summary>
        /// Absolute bounds of tab <paramref name="index"/>: the bar's height, an equal share
        /// of its width after the gaps.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The index is not a tab's.</exception>
        public LayoutF GetTabBounds(int index)
        {
            if (index < 0 || index >= _tabs.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"No tab {index} (tab count {_tabs.Count}).");

            var bounds = GetCurrentAbsoluteBounds();
            float gap = (Style ?? TabBarDefaults.Style).TabGap;
            float width = Math.Max(0f, (bounds.Width - gap * (_tabs.Count - 1)) / _tabs.Count);
            return new LayoutF(bounds.X + index * (width + gap), bounds.Y, width, bounds.Height);
        }

        /// <summary>Index of the tab under the absolute point <paramref name="point"/>, or -1 (outside, or in a gap).</summary>
        public int TabIndexAt(Vector2F point)
        {
            for (int i = 0; i < _tabs.Count; i++)
                if (GetTabBounds(i).Contains(point))
                    return i;
            return -1;
        }

        #endregion
    }
}
