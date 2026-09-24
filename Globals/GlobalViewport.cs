/* ----- ----- ----- ----- */
// GlobalViewport.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.Mathematics;

namespace Engine.Globals
{
    /// <summary>
    /// Maps the fixed-aspect-ratio content coordinate space every renderer
    /// already draws in (<see cref="DesignSize"/> — the UI's own authored
    /// size, pushed in once by <c>Game</c>, e.g. via
    /// <c>UILayoutConstants.DesignSize</c>) onto whatever actual window/
    /// framebuffer size the OS window happens to be right now.
    /// <para>
    /// The window itself is free to be resized to any size or aspect ratio;
    /// content is scaled uniformly (never stretched — <see cref="Scale"/> is
    /// a single factor, not independent X/Y factors) to fit ("contain") and
    /// centered, leaving letterbox/pillarbox bars on whichever axis doesn't
    /// match. This is distinct from <see cref="GlobalWindow"/>, which still
    /// reports the actual, unscaled window size — full-bleed background
    /// effects (e.g. StarAnimation) intentionally keep using that instead of
    /// this class, so they cover the whole physical window including the
    /// letterbox bars.
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
        /// The letterbox/pillarbox offset (in actual window pixels) where
        /// scaled content begins, so it stays centered.
        /// </summary>
        public static Vector2F Offset { get; private set; } = Vector2F.Zero;

        /// <summary>The content coordinate space's size — always <see cref="DesignSize"/>.</summary>
        public static Vector2F Size => DesignSize;

        /// <summary>The center point of the content coordinate space.</summary>
        public static Vector2F Center => DesignSize / 2f;

        /// <summary>The content coordinate space's bounds, as a <see cref="LayoutF"/>.</summary>
        public static LayoutF Bounds => new LayoutF(Vector2F.Zero, DesignSize);

        /// <summary>
        /// Recomputes <see cref="Scale"/> and <see cref="Offset"/> for the
        /// actual window/framebuffer size. Call once at startup and again
        /// whenever the window is resized.
        /// </summary>
        public static void Recalculate(float actualWidth, float actualHeight)
        {
            if (DesignSize.X <= 0 || DesignSize.Y <= 0 || actualWidth <= 0 || actualHeight <= 0)
            {
                Scale = 1f;
                Offset = Vector2F.Zero;
                return;
            }

            Scale = System.Math.Min(actualWidth / DesignSize.X, actualHeight / DesignSize.Y);

            float contentWidth = DesignSize.X * Scale;
            float contentHeight = DesignSize.Y * Scale;
            Offset = new Vector2F((actualWidth - contentWidth) / 2f, (actualHeight - contentHeight) / 2f);
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
