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
    /// Describes layout behavior relative to parent container.
    /// </summary>
    public class UILayout
    {
        /// <summary>Anchor defines which edges are locked to parent.</summary>
        public Anchor Anchor { get; set; } = Anchor.None;

        /// <summary>Optional margin space between element and parent edges.</summary>
        public PaddingF Margin { get; set; } = PaddingF.Zero;

        /// <summary>Optional alignment for flexible centering (used when Anchor.Center is set).</summary>
        public Alignment Alignment { get; set; } = Alignment.None;

        /// <summary>Optional size as a percentage of parent size (0~1 range).</summary>
        public Vector2F SizePercent { get; set; } = null;

        /// <summary>If true, layout will automatically update each frame when parent resizes.</summary>
        public bool AutoUpdate { get; set; } = true;

        /// <summary>Used by custom UI containers (like scroll panels) to override normal layout.</summary>
        public bool IgnoreParentLayout { get; set; } = false;
    }
}
