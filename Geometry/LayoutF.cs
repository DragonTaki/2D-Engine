/* ----- ----- ----- ----- */
// LayoutF.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/10/24
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using Engine.Globals;
using Engine.Mathematics;

namespace Engine.Geometry
{
    /// <summary>
    /// Represents a layout rectangle with floating-point position and size.
    /// </summary>
    public class LayoutF
    {
        public Vector2F Position { get; set; }
        public Vector2F Size { get; set; }
        public float X => Position.X;
        public float Y => Position.Y;
        public float Width => Size.X;
        public float Height => Size.Y;

        public LayoutF(Vector2F position, Vector2F size)
        {
            // Copies: callers pass live vectors (e.g. an element's own Size), which would
            // otherwise alias this layout.
            Position = new Vector2F(position.X, position.Y);
            Size = new Vector2F(size.X, size.Y);
        }

        public LayoutF(float x, float y, float width, float height)
            : this(new Vector2F(x, y), new Vector2F(width, height)) { }

        public LayoutF(float x, float y, Vector2F size)
            : this(new Vector2F(x, y), size) { }

        public LayoutF(Vector2F position, float width, float height)
            : this(position, new Vector2F(width, height)) { }

        public LayoutF(PointF position, Vector2F size)
            : this((Vector2F)position, size) { }

        public LayoutF(Vector2F position, PointF size)
            : this(position, (Vector2F)size) { }

        public LayoutF(PointF position, PointF size)
            : this((Vector2F)position, (Vector2F)size) { }

        public LayoutF(SizeF position, Vector2F size)
            : this((Vector2F)position, size) { }

        public LayoutF(Vector2F position, SizeF size)
            : this(position, (Vector2F)size) { }

        public LayoutF(SizeF position, PointF size)
            : this((Vector2F)position, (Vector2F)size) { }

        public LayoutF(PointF position, SizeF size)
            : this((Vector2F)position, (Vector2F)size) { }

        public LayoutF(SizeF position, SizeF size)
            : this((Vector2F)position, (Vector2F)size) { }

        /// <summary>
        /// Represents an empty layout at (0,0) with zero size.
        /// A new instance per access (like <see cref="Vector2F.Zero"/>): LayoutF and its
        /// vectors are mutable, and this is every UIElementBase's default Bounds.
        /// </summary>
        public static LayoutF Zero => new LayoutF(Vector2F.Zero, Vector2F.Zero);

        // Optionally, helper properties or methods
        public Vector2F Center => new Vector2F(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f);

        public static LayoutF FromSizeCentered(Vector2F size) =>
            new LayoutF(GlobalViewport.Center - new Vector2F(size.Width, size.Height) / 2f, size);

        // Half-open on the far edges (like RectangleF.Contains), so two adjacent layouts
        // sharing an edge don't both contain a point on it.
        public bool Contains(Vector2F point) =>
            point.X >= Position.X && point.X < Position.X + Size.X &&
            point.Y >= Position.Y && point.Y < Position.Y + Size.Y;

        /// <summary>
        /// Implicit conversion from RectangleF to LayoutF.
        /// Allows automatic conversion when assigning RectangleF to LayoutF.
        /// </summary>
        /// <param name="rect">The RectangleF to convert.</param>
        public static implicit operator LayoutF(RectangleF rect)
        {
            return new LayoutF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public RectangleF ToRectangleF()
        {
            return new RectangleF(Position.X, Position.Y, Size.X, Size.Y);
        }

        public static implicit operator RectangleF(LayoutF layout)
        {
            return layout.ToRectangleF();
        }

        /// <summary>
        /// Returns a new LayoutF inset (or expanded) by the specified amount from all sides,
        /// keeping the center position unchanged.
        /// </summary>
        /// <param name="amount">Inset amount in X and Y directions (positive = shrink, negative = expand).</param>
        public LayoutF Inset(Vector2F amount)
        {
            // Insetting past the center collapses to a zero size at the center instead of
            // producing a negative size (which yields inverted/self-intersecting paths).
            float width = Math.Max(0f, Size.X - amount.X * 2);
            float height = Math.Max(0f, Size.Y - amount.Y * 2);
            var center = Center;
            return new LayoutF(
                new Vector2F(center.X - width / 2f, center.Y - height / 2f),
                new Vector2F(width, height)
            );
        }

        /// <summary>
        /// Returns a new LayoutF inset (or expanded) by the specified amount from all sides,
        /// keeping the center position unchanged.
        /// </summary>
        /// <param name="x">Inset amount in X direction.</param>
        /// <param name="y">Inset amount in Y direction.</param>
        public LayoutF Inset(float x, float y) => Inset(new Vector2F(x, y));
    }
}