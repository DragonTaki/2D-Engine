/* ----- ----- ----- ----- */
// IBrushFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2025/05/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Geometry;

namespace Engine.Styles
{
    public interface IBrushFactory
    {
        Brush Create(LayoutF bounds);
    }
}
