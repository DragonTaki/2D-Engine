/* ----- ----- ----- ----- */
// DefaultStyles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Styles
{
    /// <summary>
    /// Engine-level default styles, pushed in by the app's composition root
    /// (Launcher/Program.cs) at startup. Engine renderers fall back to these
    /// when an element has no style of its own; Engine must not read Game's
    /// style constants directly.
    /// </summary>
    public static class DefaultStyles
    {
        public static IButtonDrawStyle DefaultButtonStyle { get; set; }
    }
}
