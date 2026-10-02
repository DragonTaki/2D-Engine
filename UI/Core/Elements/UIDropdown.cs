/* ----- ----- ----- ----- */
// UIDropdown.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Engine.Platform;
using Engine.Styles;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// A dropdown: a closed box showing the chosen option and a ▼ arrow. A click opens the list
    /// of every option (<see cref="UIDropdownList"/>) on the UI root's overlay layer, so it is
    /// drawn above everything and never clipped by a scrolling container; it opens below the box,
    /// or above it when there is no room below, and scrolls when it is long. Clicking an option
    /// chooses it and closes the list (the handler's <see cref="UIDropdownHandler.SelectionChanged"/>
    /// runs when the choice changed); a press outside the list, or Escape, closes it without a
    /// change. Drawn by <see cref="UIDropdownRenderer"/> with <see cref="Style"/>.
    /// <para>
    /// Disabled (<see cref="Bases.UIElementBase.IsEnabled"/> false): still drawn, its text and
    /// arrow faded by <see cref="DropdownStyle.DisabledOpacity"/>, and ignores clicks (it is not
    /// hit-tested, so a click falls through to its parent).
    /// </para>
    /// </summary>
    public class UIDropdown : UIElement<UIDropdown, UIDropdownHandler, UIDropdownRenderer>
    {
        #region Fields

        private readonly List<string> _options = new();
        private int _selectedIndex = -1;

        #endregion

        #region Properties

        /// <summary>The options' texts, in order (set with <see cref="SetOptions"/>).</summary>
        public IReadOnlyList<string> Options => _options;

        /// <summary>
        /// The chosen option's index: -1 without options, otherwise kept within the options.
        /// Setting it only changes what is shown (no <see cref="UIDropdownHandler.SelectionChanged"/>);
        /// a click on an option goes through <see cref="UIDropdownHandler.Select"/>, which does notify.
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => _selectedIndex = _options.Count == 0 ? -1 : Math.Clamp(value, 0, _options.Count - 1);
        }

        /// <summary>The chosen option's text (empty without options).</summary>
        public string SelectedText => _selectedIndex >= 0 ? _options[_selectedIndex] : string.Empty;

        /// <summary>Font of the texts (owned by whoever assigns it; never disposed here). No text is drawn without one.</summary>
        public IFont Font { get; set; }

        /// <summary>How the box and its list are drawn; null = <see cref="DropdownDefaults.Style"/>.</summary>
        public DropdownStyle Style { get; set; }

        /// <summary>Whether the list is open.</summary>
        public bool IsOpen => List != null;

        /// <summary>The open list (on the overlay layer), or null while closed; managed by the handler.</summary>
        internal UIDropdownList List { get; set; }

        #endregion

        #region Constructors

        /// <summary>Creates a dropdown without options.</summary>
        public UIDropdown()
            : base(zIndex: 0, isPersistent: false, type: UIElementType.Generic)
        {
        }

        #endregion

        /// <summary>Applies the default size (a layout style replaces it).</summary>
        protected override void OnInit()
        {
            Size = DropdownDefaults.Size;
        }

        #region Methods

        /// <summary>
        /// Replaces the options (null as none; null texts as empty). The chosen index is kept
        /// within the new options; an open list is closed.
        /// </summary>
        public void SetOptions(IEnumerable<string> options)
        {
            Handler?.Close();
            _options.Clear();
            if (options != null)
                _options.AddRange(options.Select(o => o ?? string.Empty));
            SelectedIndex = _selectedIndex < 0 ? 0 : _selectedIndex;
        }

        /// <summary>Closes the open list with the dropdown (it lives on the overlay layer, not under this element).</summary>
        protected override void DisposeUI()
        {
            Handler?.Close();
            base.DisposeUI();
        }

        #endregion
    }
}
