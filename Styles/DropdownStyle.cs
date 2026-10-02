/* ----- ----- ----- ----- */
// DropdownStyle.cs
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
    /// Looks of a dropdown (<c>UIDropdown</c>): the closed box (another one while its list is
    /// open) with the chosen option and a ▼ arrow at its right, and the open list (its box, one
    /// row per option, the hovered row's highlight, a scroll bar when not every option fits).
    /// </summary>
    public class DropdownStyle
    {
        /// <summary>The closed box; null = no box.</summary>
        public IBoxDrawStyle Box { get; set; }

        /// <summary>The closed box while the list is open; null = <see cref="Box"/>.</summary>
        public IBoxDrawStyle OpenBox { get; set; }

        /// <summary>The open list's box; null = no box.</summary>
        public IBoxDrawStyle ListBox { get; set; }

        /// <summary>Colour of the chosen option's text in the closed box, and of the options in the list.</summary>
        public Color TextColor { get; set; }

        /// <summary>Colour of the chosen option's text in the open list.</summary>
        public Color SelectedTextColor { get; set; }

        /// <summary>Fill behind the option under the mouse in the open list.</summary>
        public Color HoverColor { get; set; }

        /// <summary>Colour of the ▼ arrow.</summary>
        public Color ArrowColor { get; set; }

        /// <summary>Width of the ▼ arrow (its height is half of it; design units).</summary>
        public float ArrowWidth { get; set; }

        /// <summary>Space between the arrow and the closed box's right edge (design units).</summary>
        public float ArrowInset { get; set; }

        /// <summary>Space between the box's left edge and the text, in the closed box and the list's rows (design units).</summary>
        public float TextInset { get; set; }

        /// <summary>Height of one option in the open list (design units, positive).</summary>
        public float ItemHeight { get; set; }

        /// <summary>Most options shown at once; more scroll (at least 1).</summary>
        public int MaxVisibleItems { get; set; }

        /// <summary>Space between the closed box and the open list (design units).</summary>
        public float ListGap { get; set; }

        /// <summary>Space above the first and below the last visible option inside the list (design units).</summary>
        public float ListPadding { get; set; }

        /// <summary>Colour of the list's scroll bar (drawn only when not every option fits).</summary>
        public Color ScrollBarColor { get; set; }

        /// <summary>Width of the list's scroll bar (design units; 0 = none).</summary>
        public float ScrollBarWidth { get; set; }

        /// <summary>Opacity (0 to 1) the closed box's text and arrow are multiplied by while the dropdown is disabled.</summary>
        public float DisabledOpacity { get; set; }
    }
}
