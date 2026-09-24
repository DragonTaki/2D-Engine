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
        /// Apply vertical distortion (e.g., top-narrow, bottom-wide) to a given path.
        /// </summary>
        public static IGraphicsPath Apply(IGraphicsPath originalPath, float topScale, float bottomScale, float height)
        {
            // Create a custom skew matrix
            IGraphicsPath transformed = originalPath.Clone();

            using (IMatrix matrix = GraphicsBackend.Factory.CreateMatrix())
            {
                matrix.Translate(0, -height / 2.0f); // Center to origin
                matrix.Shear((bottomScale - topScale) / height, 0); // Shear horizontally based on difference
                matrix.Translate(0, height / 2.0f); // Translate back
                transformed.Transform(matrix);
            }

            return transformed;
        }
    }
}