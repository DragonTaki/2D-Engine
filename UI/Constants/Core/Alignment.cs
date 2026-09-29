/* ----- ----- ----- ----- */
// Alignment.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2026/09/29
// Version: v1.1
/* ----- ----- ----- ----- */

namespace Engine.UI.Constants.Core
{
    /// <summary>
    /// Horizontal/vertical alignment modes.
    /// <para>
    /// Used per axis by the layout system (<c>UILayout.AlignX</c>/<c>AlignY</c>): where an
    /// element sits when it is smaller than the space available to it on that axis.
    /// <see cref="None"/> means "unset" and behaves as <see cref="Start"/>.
    /// </para>
    /// </summary>
    public enum Alignment
    {
        None,
        Start,
        Center,
        End
    }
}
