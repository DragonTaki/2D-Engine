/* ----- ----- ----- ----- */
// IPointerCaptureTarget.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Mathematics;

namespace Engine.UI.Input
{
    /// <summary>
    /// An element's handler that takes a whole press-drag-release for itself (e.g. a slider's
    /// knob). When a press lands on its element and <see cref="CapturesPress"/> says yes,
    /// <see cref="MouseInputRouter"/> captures the pointer: the press goes only to that element
    /// (the other input handlers - the drag-scroll of a scroll container - never see it, so the
    /// page does not scroll), every move goes straight to it until the release (wherever the
    /// mouse is, so a drag past the element's edge keeps working), and the wheel does nothing
    /// meanwhile.
    /// </summary>
    public interface IPointerCaptureTarget
    {
        /// <summary>Whether a press at <paramref name="location"/> (absolute, on the element) starts a capture.</summary>
        bool CapturesPress(Vector2F location);

        /// <summary>The capture ended: after the release was delivered, or when input was cancelled (the release will not arrive).</summary>
        void OnCaptureLost();
    }
}
