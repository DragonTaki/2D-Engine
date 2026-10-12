/* ----- ----- ----- ----- */
// UnsupportedClipboard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Platform
{
    /// <summary>
    /// The <see cref="IClipboard"/> used when the launcher registers none (e.g. a headless host):
    /// always empty, so pasting does nothing.
    /// </summary>
    public sealed class UnsupportedClipboard : IClipboard
    {
        /// <inheritdoc/>
        public string GetText() => string.Empty;
    }
}
