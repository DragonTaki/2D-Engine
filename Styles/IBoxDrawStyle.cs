/* ----- ----- ----- ----- */
// IBoxDrawStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.Mathematics;
using Engine.Platform;

namespace Engine.Styles
{
    public interface IBoxDrawStyle
    {
        void Draw(IGraphics g, LayoutF bounds);
        void Draw(IGraphics g, Vector2F position, Vector2F size);
    }
}
