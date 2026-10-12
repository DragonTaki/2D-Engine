/* ----- ----- ----- ----- */
// FrameGeometry.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using Engine.Platform;

namespace Engine.Styles
{
    /// <summary>
    /// A speech-bubble tail on a frame's left edge: a "&lt;" shaped point sticking out to the left.
    /// </summary>
    /// <param name="CentreY">Where the tail's middle is, in the same coordinates as the frame's bounds.</param>
    /// <param name="Depth">How far the tail's point sticks out of the left edge.</param>
    /// <param name="HalfHeight">Half of the tail's height along the left edge.</param>
    public readonly record struct FrameTail(float CentreY, float Depth, float HalfHeight);

    /// <summary>
    /// The shapes frames are drawn along: a rounded rectangle, optionally with a <see cref="FrameTail"/>
    /// on its left edge, as one closed outline so a stroke or a fill has no seam at the tail.
    /// </summary>
    public static class FrameGeometry
    {
        /// <summary>The corner radius a frame of this size can really have (never more than half of a side).</summary>
        public static float ClampRadius(float radius, float width, float height) =>
            MathF.Max(0f, MathF.Min(radius, MathF.Min(width, height) / 2f));

        /// <summary>
        /// Moves the tail's middle so the whole tail stays on the left edge's straight part (between the
        /// rounded corners); a frame too short for that gets the tail in the middle.
        /// </summary>
        /// <param name="top">The frame's top edge.</param>
        /// <param name="height">The frame's height.</param>
        /// <param name="radius">The frame's corner radius.</param>
        /// <param name="halfHeight">Half of the tail's height.</param>
        /// <param name="centreY">Where the tail's middle wants to be.</param>
        /// <returns>The tail's middle, within the straight part.</returns>
        public static float ClampTailCentre(float top, float height, float radius, float halfHeight, float centreY)
        {
            float min = top + radius + halfHeight;
            float max = top + height - radius - halfHeight;
            if (min > max)
                return top + height / 2f;
            return Math.Clamp(centreY, min, max);
        }

        /// <summary>
        /// The closed outline of <paramref name="bounds"/> with rounded corners and, when
        /// <paramref name="tail"/> is given, the tail on the left edge (its middle clamped by
        /// <see cref="ClampTailCentre"/>). The caller disposes the path.
        /// </summary>
        public static IGraphicsPath CreateOutline(RectangleF bounds, float radius, FrameTail? tail)
        {
            float x = bounds.X, y = bounds.Y, w = MathF.Max(0f, bounds.Width), h = MathF.Max(0f, bounds.Height);
            float r = ClampRadius(radius, w, h);
            float d = r * 2f;

            var path = GraphicsBackend.Factory.CreatePath();
            path.StartFigure();
            // Clockwise from the top edge: top, top-right corner, right, bottom-right, bottom, bottom-left, left (with the tail), top-left.
            path.AddLine(x + r, y, x + w - r, y);
            if (r > 0f) path.AddArc(x + w - d, y, d, d, 270f, 90f);
            path.AddLine(x + w, y + r, x + w, y + h - r);
            if (r > 0f) path.AddArc(x + w - d, y + h - d, d, d, 0f, 90f);
            path.AddLine(x + w - r, y + h, x + r, y + h);
            if (r > 0f) path.AddArc(x, y + h - d, d, d, 90f, 90f);

            if (tail is FrameTail t)
            {
                float cy = ClampTailCentre(y, h, r, t.HalfHeight, t.CentreY);
                path.AddLine(x, y + h - r, x, cy + t.HalfHeight);
                path.AddLine(x, cy + t.HalfHeight, x - t.Depth, cy);
                path.AddLine(x - t.Depth, cy, x, cy - t.HalfHeight);
                path.AddLine(x, cy - t.HalfHeight, x, y + r);
            }
            else
            {
                path.AddLine(x, y + h - r, x, y + r);
            }

            if (r > 0f) path.AddArc(x, y, d, d, 180f, 90f);
            path.CloseFigure();
            return path;
        }
    }
}
