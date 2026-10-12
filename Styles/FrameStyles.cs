/* ----- ----- ----- ----- */
// FrameStyles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Engine.Configs;
using Engine.Platform;

namespace Engine.Styles
{
    /// <summary>
    /// The registry of the known <see cref="FrameStyle"/>s, one per <see cref="FrameStyleKind"/>, in
    /// the order they are listed to the player (and to a style gallery). Each can be drawn in any
    /// rectangle, with or without a tail.
    /// </summary>
    public static class FrameStyles
    {
        /// <summary>The style used when nothing (or something unknown) is chosen.</summary>
        public const FrameStyleKind DefaultKind = FrameStyleKind.FilledPanel;

        private static readonly IReadOnlyList<FrameStyle> Styles = new FrameStyle[]
        {
            new LineFrame(FrameStyleKind.ThinLine, "Thin line", radius: 4f, width: 1.5f, alpha: 0.9f),
            new DoubleLineFrame(),
            new LineFrame(FrameStyleKind.Rounded, "Rounded", radius: 22f, width: 3f, alpha: 0.9f),
            new CornerBracketsFrame(),
            new SoftGlowFrame(),
            new FilledPanelFrame(),
            new NoFrame(),
        };

        /// <summary>Every style, in the order they are listed.</summary>
        public static IReadOnlyList<FrameStyle> All => Styles;

        /// <summary>The style of <see cref="DefaultKind"/>.</summary>
        public static FrameStyle Default => Of(DefaultKind);

        /// <summary>The style of <paramref name="kind"/>; the default for a value that is not a style.</summary>
        public static FrameStyle Of(FrameStyleKind kind) => Styles.FirstOrDefault(s => s.Kind == kind) ?? Styles.First(s => s.Kind == DefaultKind);

        /// <summary>The style named <paramref name="name"/> (its kind's name or its human-readable name, any case); the default when there is none.</summary>
        public static FrameStyle Find(string name) =>
            Styles.FirstOrDefault(s => string.Equals(s.Kind.ToString(), name, StringComparison.OrdinalIgnoreCase)
                                    || string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)) ?? Default;

        /// <summary>The position of <paramref name="kind"/>'s style in <see cref="All"/> (the default's for an unknown value).</summary>
        public static int IndexOf(FrameStyleKind kind) => Math.Max(0, Styles.ToList().IndexOf(Of(kind)));

        /// <summary>The style at <paramref name="index"/> in <see cref="All"/> (kept within the list).</summary>
        public static FrameStyle At(int index) => Styles[Math.Clamp(index, 0, Styles.Count - 1)];

        #region Styles

        /// <summary>One solid line along the outline.</summary>
        private sealed class LineFrame : FrameStyle
        {
            private readonly float _radius, _width, _alpha;

            public LineFrame(FrameStyleKind kind, string name, float radius, float width, float alpha) : base(kind, name)
            {
                _radius = radius;
                _width = width;
                _alpha = alpha;
            }

            public override float Inset => _width / 2f + 1f;

            public override void Draw(IGraphics g, RectangleF bounds, Color color, FrameTail? tail)
            {
                using var path = FrameGeometry.CreateOutline(Deflate(bounds, Inset), _radius, tail);
                Stroke(g, path, Scaled(color, _alpha), _width);
            }
        }

        /// <summary>Two lines: the outline and a thinner one a little inside it.</summary>
        private sealed class DoubleLineFrame : FrameStyle
        {
            private const float OuterWidth = 2.5f, InnerWidth = 1.2f, Gap = 6f, Radius = 10f;

            public DoubleLineFrame() : base(FrameStyleKind.DoubleLine, "Double line") { }

            public override float Inset => OuterWidth / 2f + 1f;

            public override void Draw(IGraphics g, RectangleF bounds, Color color, FrameTail? tail)
            {
                var outer = Deflate(bounds, Inset);
                using (var path = FrameGeometry.CreateOutline(outer, Radius, tail))
                    Stroke(g, path, Scaled(color, 0.9f), OuterWidth);
                using (var path = FrameGeometry.CreateOutline(Deflate(outer, Gap), Radius - Gap / 2f, Shrunk(tail, Gap)))
                    Stroke(g, path, Scaled(color, 0.6f), InnerWidth);
            }
        }

        /// <summary>Only the four corners are drawn (brackets); a tail is drawn as a point with a short piece of the edge on each side.</summary>
        private sealed class CornerBracketsFrame : FrameStyle
        {
            private const float Width = 3f, MaxArm = 36f, Stub = 14f;

            public CornerBracketsFrame() : base(FrameStyleKind.CornerBrackets, "Corner brackets") { }

            public override float Inset => Width / 2f + 1f;

            public override void Draw(IGraphics g, RectangleF bounds, Color color, FrameTail? tail)
            {
                var r = Deflate(bounds, Inset);
                float arm = Math.Min(MaxArm, Math.Min(r.Width, r.Height) / 3f);
                using var pen = GraphicsBackend.Factory.CreatePen(Scaled(color, 0.9f), Width);

                g.DrawLine(pen, r.Left, r.Top, r.Left + arm, r.Top);
                g.DrawLine(pen, r.Left, r.Top, r.Left, r.Top + arm);
                g.DrawLine(pen, r.Right, r.Top, r.Right - arm, r.Top);
                g.DrawLine(pen, r.Right, r.Top, r.Right, r.Top + arm);
                g.DrawLine(pen, r.Left, r.Bottom, r.Left + arm, r.Bottom);
                g.DrawLine(pen, r.Left, r.Bottom, r.Left, r.Bottom - arm);
                g.DrawLine(pen, r.Right, r.Bottom, r.Right - arm, r.Bottom);
                g.DrawLine(pen, r.Right, r.Bottom, r.Right, r.Bottom - arm);

                if (tail is FrameTail t)
                {
                    float cy = FrameGeometry.ClampTailCentre(r.Top, r.Height, arm, t.HalfHeight + Stub, t.CentreY);
                    float top = cy - t.HalfHeight, bottom = cy + t.HalfHeight;
                    g.DrawLine(pen, r.Left, top - Stub, r.Left, bottom + Stub);
                    g.DrawLine(pen, r.Left, top, r.Left - t.Depth, cy);
                    g.DrawLine(pen, r.Left - t.Depth, cy, r.Left, bottom);
                }
            }
        }

        /// <summary>A thin solid line with a soft halo of fading wide strokes around it.</summary>
        private sealed class SoftGlowFrame : FrameStyle
        {
            private const float CoreWidth = 2f, Radius = 16f;
            private static readonly (float width, float alpha)[] Halo = { (12f, 0.07f), (8f, 0.12f), (5f, 0.2f) };

            public SoftGlowFrame() : base(FrameStyleKind.SoftGlow, "Soft glow") { }

            public override float Inset => Halo[0].width / 2f + 1f;

            public override void Draw(IGraphics g, RectangleF bounds, Color color, FrameTail? tail)
            {
                using var path = FrameGeometry.CreateOutline(Deflate(bounds, Inset), Radius, tail);
                foreach (var (width, alpha) in Halo)
                    Stroke(g, path, Scaled(color, alpha), width);
                Stroke(g, path, Scaled(color, 0.95f), CoreWidth);
            }
        }

        /// <summary>A translucent fill of the colour inside a solid border.</summary>
        private sealed class FilledPanelFrame : FrameStyle
        {
            private const float Width = 2.5f, Radius = 14f;

            public FilledPanelFrame() : base(FrameStyleKind.FilledPanel, "Filled panel") { }

            public override float Inset => Width / 2f + 1f;

            public override void Draw(IGraphics g, RectangleF bounds, Color color, FrameTail? tail)
            {
                using var path = FrameGeometry.CreateOutline(Deflate(bounds, Inset), Radius, tail);
                Fill(g, path, Scaled(color, 0.14f));
                Stroke(g, path, Scaled(color, 0.9f), Width);
            }
        }

        /// <summary>No frame; a tail is only a filled point.</summary>
        private sealed class NoFrame : FrameStyle
        {
            public NoFrame() : base(FrameStyleKind.None, "None") { }

            public override float Inset => 0f;

            public override void Draw(IGraphics g, RectangleF bounds, Color color, FrameTail? tail)
            {
                if (tail is not FrameTail t)
                    return;

                float cy = FrameGeometry.ClampTailCentre(bounds.Y, bounds.Height, 0f, t.HalfHeight, t.CentreY);
                using var path = GraphicsBackend.Factory.CreatePath();
                path.StartFigure();
                path.AddLine(bounds.X, cy - t.HalfHeight, bounds.X - t.Depth, cy);
                path.AddLine(bounds.X - t.Depth, cy, bounds.X, cy + t.HalfHeight);
                path.AddLine(bounds.X, cy + t.HalfHeight, bounds.X, cy - t.HalfHeight);
                path.CloseFigure();
                Fill(g, path, Scaled(color, 0.9f));
            }
        }

        #endregion
    }
}
