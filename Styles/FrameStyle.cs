/* ----- ----- ----- ----- */
// FrameStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Configs;
using Engine.Platform;

namespace Engine.Styles
{
    /// <summary>
    /// A way to draw a panel's frame in one colour, optionally with a speech-bubble
    /// <see cref="FrameTail"/> on the left edge. Everything is drawn inside the given bounds (the style
    /// insets itself by <see cref="Inset"/>) except the tail, which sticks out to the left. The known
    /// styles are listed by <see cref="FrameStyles"/>.
    /// </summary>
    public abstract class FrameStyle
    {
        protected FrameStyle(FrameStyleKind kind, string name)
        {
            Kind = kind;
            Name = name;
        }

        /// <summary>The style's identity (also what a setting stores).</summary>
        public FrameStyleKind Kind { get; }

        /// <summary>The style's human-readable name (English; unique among the styles).</summary>
        public string Name { get; }

        /// <summary>How far inside the bounds the frame's outline is drawn, so strokes and glows stay within them.</summary>
        public abstract float Inset { get; }

        /// <summary>Draws the frame.</summary>
        /// <param name="g">Where to draw.</param>
        /// <param name="bounds">The panel's bounds (absolute).</param>
        /// <param name="color">The frame's colour (its alpha is scaled by the style's own).</param>
        /// <param name="tail">The tail on the left edge, or null for none (the tail is in the same coordinates as <paramref name="bounds"/>).</param>
        public abstract void Draw(IGraphics g, RectangleF bounds, Color color, FrameTail? tail);

        /// <summary>How far in from the right edge a <see cref="FrameGap"/> cuts (more than any style's outermost line and glow).</summary>
        public const float GapReach = 18f;

        /// <summary>Draws the frame with an opening on its right edge (see <see cref="FrameGeometry.GapClipRects"/>); the rest is as <see cref="Draw(IGraphics, RectangleF, Color, FrameTail?)"/>.</summary>
        /// <param name="g">Where to draw.</param>
        /// <param name="bounds">The panel's bounds (absolute).</param>
        /// <param name="color">The frame's colour.</param>
        /// <param name="tail">The tail on the left edge, or null for none.</param>
        /// <param name="gap">The opening on the right edge, or null for none.</param>
        public void Draw(IGraphics g, RectangleF bounds, Color color, FrameTail? tail, FrameGap? gap)
        {
            if (gap is not FrameGap opening)
            {
                Draw(g, bounds, color, tail);
                return;
            }

            foreach (var rect in FrameGeometry.GapClipRects(bounds, opening, GapReach))
            {
                g.SetClip(rect);
                try
                {
                    Draw(g, bounds, color, tail);
                }
                finally
                {
                    g.ResetClip();
                }
            }
        }

        /// <summary>The tail as this style draws it: its middle moved to where <see cref="FrameGeometry.ClampTailCentre"/> puts it.</summary>
        /// <param name="bounds">The panel's bounds (absolute).</param>
        /// <param name="tail">The wanted tail.</param>
        public FrameTail ClampedTail(RectangleF bounds, FrameTail tail) =>
            tail with { CentreY = ClampTailCentre(bounds, tail) };

        /// <summary>Where this style puts the middle of <paramref name="tail"/> (default: within the straight part of the left edge, between the rounded corners).</summary>
        protected virtual float ClampTailCentre(RectangleF bounds, FrameTail tail)
        {
            var inner = Deflate(bounds, Inset);
            float r = FrameGeometry.ClampRadius(CornerRadius, inner.Width, inner.Height);
            return FrameGeometry.ClampTailCentre(inner.Y, inner.Height, r, tail.HalfHeight, tail.CentreY);
        }

        /// <summary>The outline's corner radius (0 for a style with none).</summary>
        protected virtual float CornerRadius => 0f;

        /// <summary><paramref name="bounds"/> moved in by <paramref name="amount"/> on every side.</summary>
        protected static RectangleF Deflate(RectangleF bounds, float amount) =>
            new(bounds.X + amount, bounds.Y + amount, bounds.Width - amount * 2f, bounds.Height - amount * 2f);

        /// <summary><paramref name="color"/> with its alpha multiplied by <paramref name="factor"/>.</summary>
        protected static Color Scaled(Color color, float factor) =>
            Color.FromArgb(System.Math.Clamp((int)(color.A * factor), 0, 255), color);

        /// <summary>Strokes <paramref name="path"/>.</summary>
        protected static void Stroke(IGraphics g, IGraphicsPath path, Color color, float width)
        {
            using var pen = GraphicsBackend.Factory.CreatePen(color, width);
            g.DrawPath(pen, path);
        }

        /// <summary>Fills <paramref name="path"/>.</summary>
        protected static void Fill(IGraphics g, IGraphicsPath path, Color color)
        {
            using var brush = GraphicsBackend.Factory.CreateSolidBrush(color);
            g.FillPath(brush, path);
        }

        /// <summary>The tail moved in by <paramref name="gap"/>: narrower and shorter, for an inner line that follows an outer one.</summary>
        protected static FrameTail? Shrunk(FrameTail? tail, float gap) =>
            tail is FrameTail t ? new FrameTail(t.CentreY, System.Math.Max(0f, t.Depth - gap * 2f), System.Math.Max(0f, t.HalfHeight - gap)) : null;
    }
}
