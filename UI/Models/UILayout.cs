/* ----- ----- ----- ----- */
// UILayout.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Mathematics;
using Engine.UI.Constants.Core;

namespace Engine.UI.Models
{
    /// <summary>
    /// Represents layout configuration parameters that define how a UI element is positioned
    /// and sized relative to its parent container.  
    /// <para>
    /// This class provides options for anchoring, alignment, margins, and proportional sizing.
    /// It is designed to be used by the layout engine to determine the element’s final
    /// position and dimensions within its parent.
    /// </para>
    /// </summary>
    public class UILayout
    {
        #region Properties

        /// <summary>
        /// Gets or sets the anchor rule that determines which edges of the element
        /// remain fixed relative to the parent container.  
        /// <para>
        /// For example, <see cref="Anchor.TopLeft"/> keeps the element attached to
        /// the parent’s top-left corner even if the parent resizes.
        /// </para>
        /// </summary>
        public Anchor Anchor { get; set; } = Anchor.None;

        /// <summary>
        /// Gets or sets the margin offset from the parent’s edges, measured in pixels.  
        /// <para>
        /// This margin acts as spacing between the element’s anchored position
        /// and its parent boundaries.
        /// </para>
        /// </summary>
        public PaddingF Margin { get; set; } = PaddingF.Zero;

        /// <summary>
        /// Gets or sets the alignment rule applied when the element is centered within the parent.  
        /// <para>
        /// This property only affects layout when <see cref="Anchor.Center"/> is used,
        /// allowing horizontal and/or vertical centering control.
        /// </para>
        /// </summary>
        public Alignment Alignment { get; set; } = Alignment.None;

        /// <summary>
        /// Gets or sets the proportional size of the element relative to its parent container.
        /// <para>
        /// The value range is between 0 and 1 (e.g., <c>(1, 0.5)</c> makes the element as wide as
        /// its parent, and half its height).
        /// </para>
        /// <para>
        /// If set to <see langword="null"/>, absolute pixel sizing is used instead.
        /// </para>
        /// </summary>
        public Vector2F SizePercent { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether this layout should automatically
        /// recalculate when the parent container changes size.  
        /// <para>
        /// When <see langword="true"/>, the layout system updates every frame
        /// if the parent dimensions change.
        /// </para>
        /// </summary>
        public bool AutoUpdate { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this layout should ignore
        /// the parent’s layout rules (e.g., for scrollable content or overlay layers).  
        /// <para>
        /// When <see langword="true"/>, the parent container will not reposition or resize
        /// this element automatically.
        /// </para>
        /// </summary>
        public bool IgnoreParentLayout { get; set; } = false;

        #endregion
    }
}
