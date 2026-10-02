/* ----- ----- ----- ----- */
// UILabeledRow.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// A form row: a name drawn at the left of the row, and the row's control (a switch, a
    /// button, ...) as a child, placed by the layout - typically a flex row with
    /// <c>JustifyContent = End</c> and <c>AlignItems = Center</c>, which puts the control at
    /// the right edge, vertically centered. The row draws its optional box <c>Style</c>, then
    /// the name, single line, vertically centered, from the left edge of its content box
    /// (inside the layout padding); children are drawn by the render pipeline.
    /// <para>
    /// A row without a control is a plain line of text (e.g. a "not available" note).
    /// </para>
    /// </summary>
    public class UILabeledRow : UIContainer<UILabeledRow, UILabeledRowHandler, UILabeledRowRenderer>
    {
        #region Properties

        /// <summary>The name shown at the left.</summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>Font of the name (owned by whoever assigns it; never disposed here). Nothing is drawn without one.</summary>
        public IFont Font { get; set; }

        /// <summary>Brush of the name (owned by whoever assigns it; never disposed here). Nothing is drawn without one.</summary>
        public IBrush TextBrush { get; set; }

        #endregion

        #region Constructor

        /// <summary>Creates a row without a name or control.</summary>
        public UILabeledRow() { }

        #endregion
    }
}
