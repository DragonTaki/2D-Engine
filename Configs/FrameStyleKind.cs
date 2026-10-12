/* ----- ----- ----- ----- */
// FrameStyleKind.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.1
/* ----- ----- ----- ----- */

namespace Engine.Configs
{
    /// <summary>
    /// The frame styles a panel can be drawn with (the drawing lives in <c>Engine.Styles.FrameStyles</c>).
    /// Kept here, in the layer that stores settings, so a setting can name one; written to
    /// settings.ini by name.
    /// </summary>
    public enum FrameStyleKind
    {
        ThinLine,
        DoubleLine,
        Rounded,
        CornerBrackets,
        SoftGlow,
        FilledPanel,
        GlowPanel,
        None,
    }
}
