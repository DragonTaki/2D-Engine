/* ----- ----- ----- ----- */
// WheelOverscroll.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v2.1
/* ----- ----- ----- ----- */

using System;

namespace Engine.UI.Utils
{
    /// <summary>
    /// Eases wheel motion inside the normal range. Outside it, the container's shared spring
    /// owns the position, including frames without new wheel input.
    /// </summary>
    public sealed class WheelOverscroll
    {
        /// <summary>Time constant (seconds) of the content following the target while the wheel is active.</summary>
        public const float FollowTimeConstant = 0.05f;
        /// <summary>Time constant (seconds) of the released drag rebound (<see cref="ScrollMath.ReboundStep"/>).</summary>
        public const float ReboundTimeConstant = 0.11f;
        /// <summary>The easing stops here: a gap this small (design units) is closed at once.</summary>
        public const float ReboundSnapDistance = 0.5f;

        private bool _owns;
        private bool _overscrolled;
        private float _target;

        /// <summary>Whether wheel easing or its shared spring motion is still in progress.</summary>
        public bool IsOwner => _owns;
        /// <summary>The offset the content is easing toward.</summary>
        public float Target => _target;

        /// <summary>Returns the displacement to apply now, preserving easing only inside the normal range.</summary>
        public float Push(float currentOffset, float change, float contentHeight, float viewportHeight)
        {
            float min = Math.Min(0f, viewportHeight - contentHeight);
            float from = _owns && !_overscrolled ? _target : currentOffset;
            float next = Math.Clamp(from + change, -contentHeight, viewportHeight);
            _owns = true;
            if (currentOffset > 0f || currentOffset < min || next > 0f || next < min)
            {
                _overscrolled = true;
                _target = next;
                return _target;
            }
            _overscrolled = false;
            _target = next;
            return currentOffset;
        }

        /// <summary>A press takes the position back from wheel scrolling.</summary>
        public void Release()
        {
            _owns = false;
            _overscrolled = false;
        }

        /// <summary>Eases in-range motion; returns false while the shared spring handles overscroll.</summary>
        public bool Step(float deltaSeconds, ref float offset, float minOffset)
        {
            if (!_owns)
                return false;
            if (_overscrolled || offset > 0f || offset < minOffset)
            {
                _overscrolled = true;
                if (offset >= minOffset && offset <= 0f)
                    Release();
                return false;
            }
            _target = Math.Clamp(_target, minOffset, 0f);
            float gap = _target - offset;
            if (Math.Abs(gap) <= ReboundSnapDistance)
            {
                offset = _target;
                Release();
            }
            else
                offset += gap * (1f - MathF.Exp(-Math.Max(0f, deltaSeconds) / FollowTimeConstant));
            return true;
        }
    }
}
