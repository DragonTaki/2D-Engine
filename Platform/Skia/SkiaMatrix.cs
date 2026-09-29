/* ----- ----- ----- ----- */
// SkiaMatrix.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>
    /// SkiaSharp-backed <see cref="IMatrix"/>. <see cref="SKMatrix"/> is a
    /// value type, so this class holds and mutates one in place; each call
    /// pre-concatenates the new transform onto whatever was accumulated so
    /// far, matching GDI+'s <c>Matrix.Translate</c>/<c>Shear</c> default
    /// <c>MatrixOrder.Prepend</c>: the most recently added transform is the
    /// first one applied to a point.
    /// </summary>
    internal sealed class SkiaMatrix : IMatrix
    {
        public SKMatrix Native { get; private set; } = SKMatrix.CreateIdentity();

        public void Translate(float dx, float dy) =>
            Native = Native.PreConcat(SKMatrix.CreateTranslation(dx, dy));

        public void Shear(float shearX, float shearY) =>
            Native = Native.PreConcat(new SKMatrix
            {
                ScaleX = 1,
                ScaleY = 1,
                SkewX = shearX,
                SkewY = shearY,
                Persp2 = 1
            });

        public void Dispose() { }
    }
}
