/* ----- ----- ----- ----- */
// WinFormsMatrix.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing.Drawing2D;

namespace Engine.Platform.WinForms
{
    /// <summary>GDI+-backed <see cref="IMatrix"/>.</summary>
    internal sealed class WinFormsMatrix : IMatrix
    {
        public Matrix Native { get; }

        public WinFormsMatrix(Matrix native)
        {
            Native = native;
        }

        public void Translate(float dx, float dy) => Native.Translate(dx, dy);
        public void Shear(float shearX, float shearY) => Native.Shear(shearX, shearY);

        public void Dispose() => Native.Dispose();
    }
}
