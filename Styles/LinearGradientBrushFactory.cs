/* ----- ----- ----- ----- */
// LinearGradientBrushFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Geometry;
using Engine.Platform;

namespace Engine.Styles
{
    /// <summary>
    /// A factory that creates vertical or directional linear gradient brushes.
    /// </summary>
    public class LinearGradientBrushFactory : IBrushFactory
    {
        public Color TopColor { get; set; }
        public Color BottomColor { get; set; }
        public GradientDirection Direction { get; set; }

        public LinearGradientBrushFactory(Color topColor, Color bottomColor, GradientDirection direction = GradientDirection.Vertical)
        {
            TopColor = topColor;
            BottomColor = bottomColor;
            Direction = direction;
        }

        /// <summary>
        /// Creates a linear gradient brush from a RectangleF.
        /// </summary>
        public IBrush Create(RectangleF bounds)
        {
            return GraphicsBackend.Factory.CreateLinearGradientBrush(bounds, TopColor, BottomColor, Direction);
        }

        /// <summary>
        /// Creates a linear gradient brush from a LayoutF by converting it to RectangleF.
        /// </summary>
        public IBrush Create(LayoutF layout) => Create(layout.ToRectangleF());
    }
}
