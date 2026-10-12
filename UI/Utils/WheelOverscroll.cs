/* ----- ----- ----- ----- */
// WheelOverscroll.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v2.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.UI.Utils
{
    /// <summary>
    /// A scroll container's wheel / trackpad scrolling (pure, no physics, no drawing). Wheel events move a
    /// target offset (<see cref="Push"/>: within the content range freely, past an end with rubber-band
    /// resistance and never beyond the overscroll limit); every frame <see cref="Step"/> eases the content
    /// toward that target exponentially, so scrolling is smooth and never overshoots. Once the wheel has been
    /// idle for <see cref="IdleSeconds"/> the target returns to the nearest end of the range and the content
    /// follows it, a bit slower, back into the range. Then the container is handed back to the drag logic.
    /// </summary>
    public sealed class WheelOverscroll
    {
        /// <summary>How long after a wheel event the wheel still counts as active (seconds); trackpad inertia sends events a few frames apart.</summary>
        public const float IdleSeconds = 0.15f;

        /// <summary>Time constant (seconds) of the content following the target while the wheel is active.</summary>
        public const float FollowTimeConstant = 0.05f;

        /// <summary>Time constant (seconds) of the rebound into the range (<see cref="ScrollMath.ReboundStep"/> and the idle follow).</summary>
        public const float ReboundTimeConstant = 0.11f;

        /// <summary>The easing stops here: a gap this small (design units) is closed at once.</summary>
        public const float ReboundSnapDistance = 0.5f;

        private bool _owns;
        private float _sinceWheel;
        private float _target;

        /// <summary>Whether wheel scrolling currently owns the position (from a wheel event until the content has come to rest in range).</summary>
        public bool IsOwner => _owns;

        /// <summary>Whether wheel events are still arriving.</summary>
        public bool IsActive => _owns && _sinceWheel < IdleSeconds;

        /// <summary>The offset the content is easing toward.</summary>
        public float Target => _target;

        /// <summary>Applies a wheel event: moves the target by <paramref name="change"/> (see <see cref="ScrollMath.ApplyWheelChange"/>).</summary>
        /// <param name="currentOffset">The content's offset now (the target starts from it when the wheel did not own the position yet).</param>
        /// <param name="change">The offset change of the event.</param>
        public void Push(float currentOffset, float change, float contentHeight, float viewportHeight, float overscrollLimit)
        {
            float from = _owns ? _target : currentOffset;
            _target = ScrollMath.ApplyWheelChange(from, change, contentHeight, viewportHeight, overscrollLimit);
            _owns = true;
            _sinceWheel = 0f;
        }

        /// <summary>A press (drag, click) takes the position back from the wheel.</summary>
        public void Release() => _owns = false;

        /// <summary>
        /// Advances one frame: eases <paramref name="offset"/> toward the target (and the target back into
        /// the range once the wheel is idle).
        /// </summary>
        /// <param name="deltaSeconds">The frame's delta time.</param>
        /// <param name="offset">The scroll offset (0 = top, negative = scrolled down); updated.</param>
        /// <param name="minOffset">The lowest normal offset (viewport - content, 0 when the content fits).</param>
        /// <returns>True while wheel scrolling owns the position this frame (the caller keeps the spring off).</returns>
        public bool Step(float deltaSeconds, ref float offset, float minOffset)
        {
            if (!_owns)
                return false;

            float dt = Math.Max(0f, deltaSeconds);
            _sinceWheel += dt;
            bool idle = _sinceWheel >= IdleSeconds;
            float min = Math.Min(minOffset, 0f);
            if (idle)
                _target = Math.Clamp(_target, min, 0f);

            float gap = _target - offset;
            if (Math.Abs(gap) <= ReboundSnapDistance)
                offset = _target;
            else
                offset += gap * (1f - MathF.Exp(-dt / (idle ? ReboundTimeConstant : FollowTimeConstant)));

            if (idle && offset == _target)
                _owns = false;
            return true;
        }
    }
}
