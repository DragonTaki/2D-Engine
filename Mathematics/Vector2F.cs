/* ----- ----- ----- ----- */
// Vector2F.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/11
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace Engine.Mathematics
{
    /// <summary>
    /// Represents a 2D vector with floating-point values (X, Y).
    /// Also provides aliases Width and Height for layout-style access.
    /// </summary>
    public class Vector2F
    {
        /// <summary>
        /// Gets or sets the X component of the vector.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y component of the vector.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the Width, which is an alias for X.
        /// </summary>
        public float Width
        {
            get => X;
            set => X = value;
        }

        /// <summary>
        /// Gets or sets the Height, which is an alias for Y.
        /// </summary>
        public float Height
        {
            get => Y;
            set => Y = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2F"/> class with default values (0, 0).
        /// </summary>
        public Vector2F() : this(0f, 0f) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2F"/> class with the specified X and Y values.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2F"/> class using a <see cref="PointF"/>.
        /// </summary>
        /// <param name="pt">The point providing X and Y values.</param>
        public Vector2F(PointF pt) : this(pt.X, pt.Y) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2F"/> class using a <see cref="SizeF"/>.
        /// </summary>
        /// <param name="sz">The size providing Width and Height as X and Y values.</param>
        public Vector2F(SizeF sz) : this(sz.Width, sz.Height) { }

        /// <summary>
        /// Gets a new vector with zero values (0, 0).
        /// </summary>
        public static Vector2F Zero => new Vector2F(0, 0);

        /// <summary>
        /// Converts this vector to a <see cref="Point"/>.
        /// </summary>
        /// <returns>A <see cref="Point"/> representing this vector.</returns>
        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

        /// <summary>
        /// Converts the specified vector to a <see cref="Point"/>.
        /// </summary>
        /// <param name="vector">The vector to convert.</param>
        /// <returns>A <see cref="Point"/> representation.</returns>
        public static Point ToPoint(Vector2F vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Vector2F"/> to a <see cref="Point"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vector2F"/> instance to convert.</param>
        /// <returns>A <see cref="Point"/> with the same X and Y values.</returns>
        public static implicit operator Point(Vector2F v) => new Point((int)v.X, (int)v.Y);

        /// <summary>
        /// Implicitly converts a <see cref="Point"/> to a <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="pt">The <see cref="Point"/> to convert.</param>
        /// <returns>A new <see cref="Vector2F"/> with the same X and Y values.</returns>
        public static implicit operator Vector2F(Point pt) => new Vector2F(pt);

        /// <summary>
        /// Converts this vector to a <see cref="PointF"/>.
        /// </summary>
        /// <returns>A <see cref="PointF"/> representing this vector.</returns>
        public PointF ToPointF()
        {
            return new PointF(X, Y);
        }

        /// <summary>
        /// Converts the specified vector to a <see cref="PointF"/>.
        /// </summary>
        /// <param name="vector">The vector to convert.</param>
        /// <returns>A <see cref="PointF"/> representation.</returns>
        public static PointF ToPointF(Vector2F vector)
        {
            return new PointF(vector.X, vector.Y);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Vector2F"/> to a <see cref="PointF"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vector2F"/> instance to convert.</param>
        /// <returns>A <see cref="PointF"/> with the same X and Y values.</returns>
        public static implicit operator PointF(Vector2F v) => new PointF(v.X, v.Y);

        /// <summary>
        /// Implicitly converts a <see cref="PointF"/> to a <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="pt">The <see cref="PointF"/> to convert.</param>
        /// <returns>A new <see cref="Vector2F"/> with the same X and Y values.</returns>
        public static implicit operator Vector2F(PointF pt) => new Vector2F(pt);

        /// <summary>
        /// Converts this vector to a <see cref="Size"/> by casting X and Y to integers.
        /// </summary>
        /// <returns>A <see cref="Size"/> representing this vector.</returns>
        public Size ToSize()
        {
            return new Size((int)X, (int)Y);
        }

        /// <summary>
        /// Converts the specified vector to a <see cref="Size"/> by casting X and Y to integers.
        /// </summary>
        /// <param name="vector">The vector to convert.</param>
        /// <returns>A <see cref="Size"/> representation.</returns>
        public static Size ToSize(Vector2F vector)
        {
            return new Size((int)vector.X, (int)vector.Y);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Vector2F"/> to a <see cref="Size"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vector2F"/> instance to convert.</param>
        /// <returns>A <see cref="Size"/> with Width = X and Height = Y.</returns>
        public static implicit operator Size(Vector2F v) => new Size((int)v.X, (int)v.Y);

        /// <summary>
        /// Implicitly converts a <see cref="Size"/> to a <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="sz">The <see cref="Size"/> to convert.</param>
        /// <returns>A new <see cref="Vector2F"/> with X = Width and Y = Height.</returns>
        public static implicit operator Vector2F(Size sz) => new Vector2F(sz);

        /// <summary>
        /// Converts this vector to a <see cref="SizeF"/>.
        /// </summary>
        /// <returns>A <see cref="SizeF"/> representing this vector.</returns>
        public SizeF ToSizeF()
        {
            return new SizeF(X, Y);
        }

        /// <summary>
        /// Converts the specified vector to a <see cref="SizeF"/>.
        /// </summary>
        /// <param name="vector">The vector to convert.</param>
        /// <returns>A <see cref="SizeF"/> representation.</returns>
        public static SizeF ToSizeF(Vector2F vector)
        {
            return new SizeF(vector.X, vector.Y);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Vector2F"/> to a <see cref="SizeF"/>.
        /// </summary>
        /// <param name="v">The <see cref="Vector2F"/> instance to convert.</param>
        /// <returns>A <see cref="SizeF"/> with Width = X and Height = Y.</returns>
        public static implicit operator SizeF(Vector2F v) => new SizeF(v.X, v.Y);

        /// <summary>
        /// Implicitly converts a <see cref="SizeF"/> to a <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="sz">The <see cref="SizeF"/> to convert.</param>
        /// <returns>A new <see cref="Vector2F"/> with X = Width and Y = Height.</returns>
        public static implicit operator Vector2F(SizeF sz) => new Vector2F(sz);

        /// <summary>
        /// Implicitly converts a <see cref="float"/> to a <see cref="Vector2F"/>.
        /// </summary>
        /// <param name="value">The <see cref="float"/> to convert.</param>
        /// <returns>A new <see cref="Vector2F"/> with X = value and Y = value.</returns>
        public static implicit operator Vector2F(float value) => new Vector2F(value, value);

        /// <summary>
        /// Converts two vectors to a <see cref="RectangleF"/> using one as the position and the other as the size.
        /// </summary>
        /// <param name="position">The top-left position of the rectangle.</param>
        /// <param name="size">The size of the rectangle.</param>
        /// <returns>A <see cref="RectangleF"/> structure.</returns>
        public static RectangleF ToRectangleF(Vector2F position, Vector2F size)
        {
            return new RectangleF(position.X, position.Y, size.X, size.Y);
        }

        /// <summary>
        /// Returns a string representation of the vector in the format "(X, Y)".
        /// </summary>
        /// <returns>A string representing this vector.</returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        // Subtraction (Unary negation)
        /// <summary>
        /// Returns a new vector with the X and Y components negated.
        /// </summary>
        /// <param name="v">The vector to negate.</param>
        /// <returns>A vector with negated X and Y values.</returns>
        public static Vector2F operator -(Vector2F v)
        {
            return new Vector2F(-v.X, -v.Y);
        }

        /// <summary>
        /// Adds two vectors component-wise.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Sum of the two vectors.</returns>
        public static Vector2F operator +(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Adds a scalar value to both components of the vector.
        /// </summary>
        /// <param name="v">Vector operand.</param>
        /// <param name="scalar">Scalar value to add.</param>
        /// <returns>A new vector with scalar added to both components.</returns>
        public static Vector2F operator +(Vector2F v, float scalar)
        {
            return new Vector2F(v.X + scalar, v.Y + scalar);
        }

        /// <summary>
        /// Adds a scalar value to both components of the vector.
        /// </summary>
        /// <param name="scalar">Scalar value to add.</param>
        /// <param name="v">Vector operand.</param>
        /// <returns>A new vector with scalar added to both components.</returns>
        public static Vector2F operator +(float scalar, Vector2F v)
        {
            return new Vector2F(v.X + scalar, v.Y + scalar);
        }

        /// <summary>
        /// Subtracts two vectors component-wise.
        /// </summary>
        /// <param name="a">First vector (minuend).</param>
        /// <param name="b">Second vector (subtrahend).</param>
        /// <returns>Resulting vector after subtraction.</returns>
        public static Vector2F operator -(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X - b.X, a.Y - b.Y);
        }

        /// <summary>
        /// Subtracts a scalar from both components of a vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scalar">Scalar value to subtract.</param>
        /// <returns>A new vector with scalar subtracted from both components.</returns>
        public static Vector2F operator -(Vector2F v, float scalar)
        {
            return new Vector2F(v.X - scalar, v.Y - scalar);
        }

        /// <summary>
        /// Subtracts a vector's components from a scalar.
        /// </summary>
        /// <param name="scalar">Scalar value.</param>
        /// <param name="v">The vector to subtract.</param>
        /// <returns>A new vector where scalar minus each component is performed.</returns>
        public static Vector2F operator -(float scalar, Vector2F v)
        {
            return new Vector2F(scalar - v.X, scalar - v.Y);
        }

        /// <summary>
        /// Multiplies two vectors component-wise.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Product of both vectors component-wise.</returns>
        public static Vector2F operator *(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X * b.X, a.Y * b.Y);
        }

        /// <summary>
        /// Multiplies both components of the vector by a scalar.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scalar">The scalar multiplier.</param>
        /// <returns>Scaled vector.</returns>
        public static Vector2F operator *(Vector2F v, float scalar)
        {
            return new Vector2F(v.X * scalar, v.Y * scalar);
        }

        /// <summary>
        /// Multiplies both components of the vector by a scalar.
        /// </summary>
        /// <param name="scalar">The scalar multiplier.</param>
        /// <param name="v">The vector.</param>
        /// <returns>Scaled vector.</returns>
        public static Vector2F operator *(float scalar, Vector2F v)
        {
            return new Vector2F(v.X * scalar, v.Y * scalar);
        }

        /// <summary>
        /// Divides one vector by another component-wise.
        /// </summary>
        /// <param name="a">Numerator vector.</param>
        /// <param name="b">Denominator vector.</param>
        /// <returns>Resulting vector after division.</returns>
        public static Vector2F operator /(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X / b.X, a.Y / b.Y);
        }

        /// <summary>
        /// Divides the vector by a scalar.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scalar">The scalar divisor.</param>
        /// <returns>A new vector with components divided by scalar.</returns>
        public static Vector2F operator /(Vector2F v, float scalar)
        {
            return new Vector2F(v.X / scalar, v.Y / scalar);
        }

        /// <summary>
        /// Divides a scalar by each component of the vector.
        /// </summary>
        /// <param name="scalar">Scalar numerator.</param>
        /// <param name="v">Vector denominator.</param>
        /// <returns>A new vector with scalar divided by each component.</returns>
        public static Vector2F operator /(float scalar, Vector2F v)
        {
            return new Vector2F(v.X / scalar, v.Y / scalar);
        }

        /// <summary>
        /// Computes the dot product of two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>The dot product (a.X * b.X + a.Y * b.Y).</returns>
        public static float DotProduct(Vector2F a, Vector2F b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// Returns the magnitude (length) of the vector.
        /// </summary>
        /// <returns>Length as a float.</returns>
        public float Length()
        {
            return MathF.Sqrt(X * X + Y * Y);
        }

        /// <summary>
        /// Returns a normalized (unit length) version of this vector.
        /// </summary>
        /// <returns>The normalized vector, or (0, 0) if length is too small.</returns>
        public Vector2F Normalize()
        {
            float length = Length();
            return length > 0.001f ?
                new Vector2F(X / length, Y / length) :
                new Vector2F(0, 0);
        }

        /// <summary>
        /// Linearly interpolates between two vectors by the factor t.
        /// </summary>
        /// <param name="a">Start vector.</param>
        /// <param name="b">End vector.</param>
        /// <param name="t">Interpolation factor between 0 and 1.</param>
        /// <returns>Interpolated vector.</returns>
        public static Vector2F Lerp(Vector2F a, Vector2F b, float t)
        {
            return new Vector2F(
                MathUtil.Lerp(a.X, b.X, t),
                MathUtil.Lerp(a.Y, b.Y, t)
            );
        }

        /// <summary>
        /// Calculates the angle (in radians) between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Angle in radians between the two vectors.</returns>
        public static float AngleBetween(Vector2F a, Vector2F b)
        {
            float dot = a.X * b.X + a.Y * b.Y;
            float magProduct = a.Length() * b.Length();
            if (magProduct == 0f) return 0f;

            float cosTheta = MathUtil.ClampF(dot / magProduct, -1f, 1f);
            return MathF.Acos(cosTheta);  // Unit: radians
        }

        /// <summary>
        /// Rotates the vector counterclockwise by the given angle (in radians).
        /// </summary>
        /// <param name="angleRadians">Angle to rotate, in radians.</param>
        /// <returns>New rotated vector.</returns>
        public Vector2F Rotate(float angleRadians)
        {
            float cos = MathF.Cos(angleRadians);
            float sin = MathF.Sin(angleRadians);

            return new Vector2F(
                X * cos - Y * sin,
                X * sin + Y * cos
            );
        }
    }
}
