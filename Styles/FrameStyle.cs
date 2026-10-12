/* ----- ----- ----- ----- */
// FrameStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.4
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

        /// <summary>
        /// The empty margin between a panel's bounds (or the screen's edge) and every style's outline
        /// (author 2026-10-12: frames were drawn too close to the edge). The one place to change it; each
        /// style's <see cref="Inset"/> starts with it, and a tail's point stays that far in from where
        /// its caller wants it unless the caller adds the margin to the tail's depth.
        /// </summary>
        public const float EdgeMargin = 10f;

        /// <summary>How far inside the bounds the frame's outline is drawn (<see cref="EdgeMargin"/> plus what the strokes and glows need), so they stay within them.</summary>
        public abstract float Inset { get; }

        /// <summary>Draws the frame: <see cref="DrawBackdrop"/> (fill and glow), then <see cref="DrawLines"/>.</summary>
        /// <param name="g">Where to draw.</param>
        /// <param name="bounds">The panel's bounds (absolute).</param>
        /// <param name="color">The frame's colour (its alpha is scaled by the style's own).</param>
        /// <param name="tail">The tail on the left edge, or null for none (the tail is in the same coordinates as <paramref name="bounds"/>).</param>
        public void Draw(IGraphics g, RectangleF bounds, Color color, FrameTail? tail)
        {
            DrawBackdrop(g, bounds, color, tail);
            DrawLines(g, bounds, color, tail);
        }

        /// <summary>The soft parts of the frame: the translucent fill and the glow (nothing for a style with neither). Never cut by an opening.</summary>
        public virtual void DrawBackdrop(IGraphics g, RectangleF bounds, Color color, FrameTail? tail) { }

        /// <summary>The crisp lines of the frame (and a tail's point). The only part an opening cuts.</summary>
        public abstract void DrawLines(IGraphics g, RectangleF bounds, Color color, FrameTail? tail);

        /// <summary>
        /// The area this frame covers when drawn with <paramref name="tail"/>: the same outline the style
        /// strokes (its bounds moved in by <see cref="Inset"/>, its corners and its tail), grown by
        /// <paramref name="grow"/> on every side so no anti-aliased seam is left at its edge. The caller disposes the path.
        /// </summary>
        /// <param name="bounds">The panel's bounds (absolute).</param>
        /// <param name="tail">The tail on the left edge, or null for none.</param>
        /// <param name="grow">How far to grow the outline.</param>
        public virtual IGraphicsPath CreateShapePath(RectangleF bounds, FrameTail? tail, float grow)
        {
            var inner = Deflate(bounds, Inset - grow);
            FrameTail? shaped = tail is FrameTail t
                ? new FrameTail(ClampedTail(bounds, t).CentreY, t.Depth + grow, t.HalfHeight + grow)
                : null;
            return FrameGeometry.CreateOutline(inner, CornerRadius + grow, shaped);
        }

        /// <summary>
        /// Draws the frame (fill, glow and lines, as one picture) with the area of <paramref name="exclude"/>
        /// left out: nothing of the frame is drawn there, whatever is already under it shows.
        /// </summary>
        /// <param name="g">Where to draw.</param>
        /// <param name="bounds">The panel's bounds (absolute).</param>
        /// <param name="color">The frame's colour.</param>
        /// <param name="tail">The tail on the left edge, or null for none.</param>
        /// <param name="exclude">The area to leave out, or null for none (see <see cref="CreateShapePath"/> for another frame's shape).</param>
        public void Draw(IGraphics g, RectangleF bounds, Color color, FrameTail? tail, IGraphicsPath exclude)
        {
            if (exclude == null)
            {
                Draw(g, bounds, color, tail);
                return;
            }

            g.ExcludeClip(exclude);
            try
            {
                Draw(g, bounds, color, tail);
            }
            finally
            {
                g.ResetClip();
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
