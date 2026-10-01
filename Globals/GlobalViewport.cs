/* ----- ----- ----- ----- */
// GlobalViewport.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/30
// Version: v1.2
/* ----- ----- ----- ----- */

using System;

using Engine.Geometry;
using Engine.Mathematics;

namespace Engine.Globals
{
    /// <summary>
    /// Maps the UI's design coordinate space onto whatever actual window/
    /// framebuffer size the OS window happens to be right now.
    /// <see cref="DesignSize"/> is the UI's own authored size, pushed in once
    /// by <c>Game</c> (e.g. via <c>UILayoutConstants.DesignSize</c>).
    /// <para>
    /// The window itself is free to be resized to any size or aspect ratio.
    /// Content is scaled uniformly (never stretched — <see cref="Scale"/> is
    /// a single factor, not independent X/Y factors) by the factor that
    /// would fit <see cref="DesignSize"/> ("contain"), so text and buttons
    /// keep one scale. There is no letterbox: the UI area starts at the
    /// window's top-left corner (<see cref="Offset"/> is always zero) and
    /// covers the whole window — <see cref="Size"/> is the window size in
    /// design units, equal to <see cref="DesignSize"/> on the axis that
    /// limits the scale and larger on the other one. The layout system
    /// distributes that extra space.
    /// </para>
    /// <para>
    /// <see cref="GlobalWindow"/> still reports the actual, unscaled window
    /// size; full-bleed background effects (e.g. StarAnimation) keep using
    /// that instead of this class.
    /// </para>
    /// </summary>
    public static class GlobalViewport
    {
        /// <summary>
        /// The content's own authored size — the coordinate space every
        /// layout constant and renderer already assumes. Set once at
        /// startup by Game (see <c>UILayoutConstants.DesignSize</c>);
        /// never changes afterward.
        /// </summary>
        public static Vector2F DesignSize { get; set; } = new Vector2F(1, 1);

        /// <summary>
        /// The uniform factor design-space content is currently scaled by
        /// to fit the actual window. Recomputed by <see cref="Recalculate"/>.
        /// </summary>
        public static float Scale { get; private set; } = 1f;

        /// <summary>
        /// Where scaled content begins, in actual window pixels. Always zero
        /// now that the UI covers the whole window (it used to be the
        /// letterbox/pillarbox offset that centered <see cref="DesignSize"/>);
        /// kept so the launchers' transform and the pixel snapping stay
        /// written against the general <c>device = v * Scale + Offset</c> mapping.
        /// </summary>
        public static Vector2F Offset { get; private set; } = Vector2F.Zero;

        // null until Recalculate has a valid window and design size.
        private static Vector2F _size;

        /// <summary>
        /// The UI area's size in design units: the whole window divided by
        /// <see cref="Scale"/>. At least <see cref="DesignSize"/> on both axes
        /// (equal on the axis that limits the scale). Before the first
        /// <see cref="Recalculate"/> it is <see cref="DesignSize"/>.
        /// A copy per access (<see cref="Vector2F"/> is mutable).
        /// </summary>
        public static Vector2F Size => _size != null ? new Vector2F(_size.X, _size.Y) : DesignSize;

        /// <summary>The center point of the UI area (<see cref="Size"/>).</summary>
        public static Vector2F Center => Size / 2f;

        /// <summary>The UI area's bounds (origin, <see cref="Size"/>), as a <see cref="LayoutF"/>.</summary>
        public static LayoutF Bounds => new LayoutF(Vector2F.Zero, Size);

        /// <summary>
        /// Raised by <see cref="Recalculate"/> after <see cref="Scale"/>,
        /// <see cref="Offset"/>, <see cref="Size"/> or <see cref="DesignSize"/>
        /// changed (window resize). The UI root listens to resize itself to
        /// <see cref="Size"/> and invalidate its layout (pixel snapping also
        /// depends on the scale).
        /// </summary>
        public static event Action Changed;

        private static Vector2F _lastDesignSize = new Vector2F(0, 0);

        /// <summary>
        /// Recomputes <see cref="Scale"/>, <see cref="Offset"/> and
        /// <see cref="Size"/> for the actual window/framebuffer size. Call once at startup and again
        /// whenever the window is resized.
        /// </summary>
        public static void Recalculate(float actualWidth, float actualHeight)
        {
            float oldScale = Scale;
            var oldOffset = Offset;
            var oldSize = Size;

            Offset = Vector2F.Zero;
            if (DesignSize.X <= 0 || DesignSize.Y <= 0 || actualWidth <= 0 || actualHeight <= 0)
            {
                Scale = 1f;
                _size = null;
            }
            else
            {
                // One uniform scale, the one that fits the design size; the UI area is then
                // the whole window in design units - DesignSize on the limiting axis, larger
                // on the other.
                Scale = System.Math.Min(actualWidth / DesignSize.X, actualHeight / DesignSize.Y);
                _size = new Vector2F(actualWidth / Scale, actualHeight / Scale);
            }

            bool designChanged = DesignSize.X != _lastDesignSize.X || DesignSize.Y != _lastDesignSize.Y;
            _lastDesignSize = new Vector2F(DesignSize.X, DesignSize.Y);

            var size = Size;
            if (designChanged || Scale != oldScale || Offset.X != oldOffset.X || Offset.Y != oldOffset.Y
                || size.X != oldSize.X || size.Y != oldSize.Y)
                Changed?.Invoke();
        }

        /// <summary>
        /// Rounds a design-space coordinate to the nearest device (framebuffer)
        /// pixel boundary under the current transform: device = v * Scale + offset,
        /// where <paramref name="deviceOffset"/> is <see cref="Offset"/>.X or .Y.
        /// </summary>
        public static float SnapToDevicePixel(float value, float deviceOffset)
        {
            if (Scale <= 0f || float.IsNaN(value) || float.IsInfinity(value))
                return value;
            return (MathF.Round(value * Scale + deviceOffset) - deviceOffset) / Scale;
        }

        /// <summary>
        /// Converts a point in actual window/framebuffer pixels (e.g. a raw
        /// mouse position) into this content coordinate space, inverting the
        /// transform <see cref="Recalculate"/> computed.
        /// </summary>
        public static Vector2F ScreenToDesign(Vector2F screenPoint) =>
            new Vector2F(
                (screenPoint.X - Offset.X) / Scale,
                (screenPoint.Y - Offset.Y) / Scale);
    }
}
