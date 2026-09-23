/* ----- ----- ----- ----- */
// IMatrix.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Platform
{
    /// <summary>
    /// A 2D affine transform used to reshape an <see cref="IGraphicsPath"/>.
    /// Created via <see cref="IGraphics.CreateMatrix"/>.
    /// </summary>
    public interface IMatrix : IDisposable
    {
        void Translate(float dx, float dy);
        void Shear(float shearX, float shearY);
    }
}
