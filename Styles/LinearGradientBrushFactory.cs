/* ----- ----- ----- ----- */
// LinearGradientBrushFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

using Engine.Geometry;

namespace Engine.Styles
{
    /// <summary>
    /// A factory that creates vertical or directional LinearGradientBrush instances.
    /// </summary>
    public class LinearGradientBrushFactory : IBrushFactory
    {
        public Color TopColor { get; set; }
        public Color BottomColor { get; set; }
        public LinearGradientMode Mode { get; set; }

        public LinearGradientBrushFactory(Color topColor, Color bottomColor, LinearGradientMode mode = LinearGradientMode.Vertical)
        {
            TopColor = topColor;
            BottomColor = bottomColor;
            Mode = mode;
        }
        
        /// <summary>
        /// Creates a LinearGradientBrush from a RectangleF.
        /// </summary>
        public Brush Create(RectangleF bounds)
        {
            return new LinearGradientBrush(bounds, TopColor, BottomColor, Mode);
        }

        /// <summary>
        /// Creates a LinearGradientBrush from a LayoutF by converting it to RectangleF.
        /// </summary>
        public Brush Create(LayoutF layout) => Create(layout.ToRectangleF());
    }
}
