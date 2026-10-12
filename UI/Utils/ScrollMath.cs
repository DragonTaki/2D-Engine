/* ----- ----- ----- ----- */
// ScrollMath.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.UI.Utils
{
    /// <summary>
    /// Wheel / scroll-offset arithmetic (pure, no drawing; used by the scroll input handler and
    /// the dropdown list). Every backend delivers the same sign: a positive wheel delta means
    /// the wheel was turned up / away (WinForms <c>MouseEventArgs.Delta</c>, GLFW's scroll Y
    /// offset - which on macOS already follows the player's natural-scroll setting), i.e. the
    /// content follows it downward and the scroll offset (0 at the top, negative once scrolled
    /// down) increases.
    /// </summary>
    public static class ScrollMath
    {
        /// <summary>Wheel delta of one notch (the WinForms convention the backends are scaled to).</summary>
        public const int NotchDelta = 120;

        /// <summary>
        /// A backend's scroll amount in notches (GLFW reports +/-1 per notch, fractions for a
        /// trackpad) as the engine's wheel delta.
        /// </summary>
        public static int ToWheelDelta(float notches) => (int)(notches * NotchDelta);

        /// <summary>
        /// A wheel delta in notches, at most one notch per event: macOS accelerates a fast spin
        /// into several notches per event, which scrolled far faster than dragging.
        /// </summary>
        public static float Notches(int wheelDelta) => Math.Clamp(wheelDelta / (float)NotchDelta, -1f, 1f);

        /// <summary>
        /// The change of a scroll offset (0 = top, negative = scrolled down) one wheel event
        /// makes: wheel up (positive delta) moves the content down.
        /// </summary>
        public static float OffsetChange(int wheelDelta, float step) => Notches(wheelDelta) * step;

        /// <summary>
        /// Keeps a scroll offset within <c>viewportHeight - contentHeight</c>..0 (just 0 when
        /// the content fits). Applied after every change, including a content that shrank.
        /// </summary>
        public static float ClampOffset(float offset, float contentHeight, float viewportHeight)
        {
            float min = Math.Min(0f, viewportHeight - contentHeight);
            if (float.IsNaN(offset))
                return 0f;
            return Math.Clamp(offset, min, 0f);
        }
    }
}
