/* ----- ----- ----- ----- */
// WheelOverscroll.cs
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
    /// A scroll container's wheel / trackpad overscroll (pure, no physics, no drawing). While wheel
    /// events keep arriving the wheel owns the position (see <see cref="ScrollMath.ApplyWheelChange"/>);
    /// once they stop for <see cref="IdleSeconds"/>, the position glides back to the nearest end of
    /// the content range - exponentially, so it never overshoots or oscillates - and the container
    /// is handed back to the edge spring and drag logic. The state is advanced once per frame by
    /// <see cref="Step"/>, with the frame's delta time.
    /// </summary>
    public sealed class WheelOverscroll
    {
        /// <summary>How long after a wheel event the wheel still counts as active (seconds); trackpad inertia sends events a few frames apart.</summary>
        public const float IdleSeconds = 0.15f;

        /// <summary>Time constant of the rebound (seconds): the overscroll shrinks by 1/e in this time.</summary>
        public const float ReboundTimeConstant = 0.2f;

        /// <summary>The rebound stops here: an overscroll this small (design units) is closed at once.</summary>
        public const float ReboundSnapDistance = 0.5f;

        private bool _owns;
        private float _sinceWheel;

        /// <summary>Whether the wheel currently owns the position (a wheel event came and the offset has not yet come back into range).</summary>
        public bool IsOwner => _owns;

        /// <summary>Whether wheel events are still arriving.</summary>
        public bool IsActive => _owns && _sinceWheel < IdleSeconds;

        /// <summary>A wheel event was applied: the wheel owns the position, and is active again.</summary>
        public void Touch()
        {
            _owns = true;
            _sinceWheel = 0f;
        }

        /// <summary>A press (drag, click) takes the position back from the wheel.</summary>
        public void Release() => _owns = false;

        /// <summary>
        /// Advances one frame. While the wheel is active the offset is left alone; afterwards an
        /// offset outside <paramref name="minOffset"/>..0 glides back toward it.
        /// </summary>
        /// <param name="deltaSeconds">The frame's delta time.</param>
        /// <param name="offset">The scroll offset (0 = top, negative = scrolled down); updated.</param>
        /// <param name="minOffset">The lowest normal offset (viewport - content, 0 when the content fits).</param>
        /// <returns>True while the wheel owns the position this frame (the caller keeps the spring off).</returns>
        public bool Step(float deltaSeconds, ref float offset, float minOffset)
        {
            if (!_owns)
                return false;

            _sinceWheel += Math.Max(0f, deltaSeconds);
            if (_sinceWheel < IdleSeconds)
                return true;

            offset = ScrollMath.ReboundStep(offset, minOffset, deltaSeconds);
            if (offset >= minOffset && offset <= 0f)
                _owns = false;
            return true;
        }
    }
}
