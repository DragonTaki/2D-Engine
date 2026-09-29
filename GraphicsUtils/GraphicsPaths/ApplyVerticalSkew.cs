/* ----- ----- ----- ----- */
// ApplyVerticalSkew.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2025/05/22
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;

namespace Engine.GraphicsUtils.GraphicsPaths
{
    public static class ApplyVerticalSkew
    {
        /// <summary>
        /// Shears a path horizontally in proportion to vertical position, pivoting on its
        /// vertical center: rows below the center shift by +(bottomScale - topScale)/2 per
        /// unit of height, rows above it the opposite way. (An affine shear yields a
        /// parallelogram; it cannot narrow the top and widen the bottom at the same time.)
        /// </summary>
        public static IGraphicsPath Apply(IGraphicsPath originalPath, float topScale, float bottomScale, float height)
        {
            // Create a custom skew matrix
            IGraphicsPath transformed = originalPath.Clone();

            // A zero/negative height has no vertical extent to shear over (and would divide by zero).
            if (height <= 0f)
                return transformed;

            using (IMatrix matrix = GraphicsBackend.Factory.CreateMatrix())
            {
                // IMatrix composes in prepend order (last call applies first), so these are
                // listed in reverse: move the center to the origin, shear, move back.
                matrix.Translate(0, height / 2.0f); // Translate back
                matrix.Shear((bottomScale - topScale) / height, 0); // Shear horizontally based on difference
                matrix.Translate(0, -height / 2.0f); // Center to origin
                transformed.Transform(matrix);
            }

            return transformed;
        }
    }
}