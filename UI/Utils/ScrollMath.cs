/* ----- ----- ----- ----- */
// ScrollMath.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.3
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

        /// <summary>
        /// Applies a wheel <paramref name="change"/> to a scroll offset with rubber-band resistance:
        /// inside the content range it moves freely, past an end the further out the offset is the
        /// less a change that pushes it further moves it (nothing at <paramref name="overscrollLimit"/>),
        /// and a change back toward the range moves it in full. Never leaves the overscroll range.
        /// </summary>
        public static float ApplyWheelChange(float offset, float change, float contentHeight, float viewportHeight, float overscrollLimit)
        {
            float limit = Math.Max(0f, overscrollLimit);
            if (float.IsNaN(offset) || float.IsNaN(change))
                return ClampOffsetWithOverscroll(offset, contentHeight, viewportHeight, limit);

            float min = Math.Min(0f, viewportHeight - contentHeight);
            offset = ClampOffsetWithOverscroll(offset, contentHeight, viewportHeight, limit);
            float result = offset;
            float remaining = change;

            // Part of the change that moves within the range (or back into it) goes in full.
            if (remaining > 0f)
            {
                if (result < 0f)
                {
                    float room = Math.Min(remaining, -result);
                    result += room;
                    remaining -= room;
                }
                result += Resist(result, remaining, 0f, limit, +1f);
            }
            else if (remaining < 0f)
            {
                if (result > min)
                {
                    float room = Math.Min(-remaining, result - min);
                    result -= room;
                    remaining += room;
                }
                result += Resist(result, remaining, min, limit, -1f);
            }
            return ClampOffsetWithOverscroll(result, contentHeight, viewportHeight, limit);
        }

        /// <summary>
        /// One frame of the rebound of an overscrolled offset toward the nearest end of
        /// <paramref name="minOffset"/>..0: exponential (time constant
        /// <see cref="WheelOverscroll.ReboundTimeConstant"/>), so it is monotonic, frame-rate
        /// independent and never crosses the end; an offset already inside the range is kept.
        /// </summary>
        public static float ReboundStep(float offset, float minOffset, float deltaSeconds)
        {
            float edge = Math.Clamp(offset, Math.Min(minOffset, 0f), 0f);
            float distance = offset - edge;
            if (Math.Abs(distance) <= WheelOverscroll.ReboundSnapDistance)
                return edge;
            return edge + distance * MathF.Exp(-Math.Max(0f, deltaSeconds) / WheelOverscroll.ReboundTimeConstant);
        }

        /// <summary>
        /// The part of <paramref name="change"/> that is applied beyond an edge: scaled by the room
        /// left before the limit at the starting offset, a change toward the range excluded
        /// (<paramref name="side"/> is +1 for the top edge, -1 for the bottom one).
        /// </summary>
        private static float Resist(float offset, float change, float edge, float limit, float side)
        {
            if (change == 0f || limit <= 0f)
                return 0f;
            float over = Math.Max(0f, (offset - edge) * side);
            if (change * side < 0f)
                return change;
            return change * Math.Max(0f, 1f - over / limit);
        }

        /// <summary>
        /// Like <see cref="ClampOffset"/>, but lets the offset go <paramref name="overscrollLimit"/>
        /// past either end (the edge spring pulls it back once the input stops), so a fast
        /// spin never carries the content further than that.
        /// </summary>
        public static float ClampOffsetWithOverscroll(float offset, float contentHeight, float viewportHeight, float overscrollLimit)
        {
            float limit = Math.Max(0f, overscrollLimit);
            float min = Math.Min(0f, viewportHeight - contentHeight) - limit;
            if (float.IsNaN(offset))
                return 0f;
            return Math.Clamp(offset, min, limit);
        }
    }
}
