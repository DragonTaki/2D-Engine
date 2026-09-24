/* ----- ----- ----- ----- */
// IButtonDrawStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.Mathematics;
using Engine.Platform;

namespace Engine.Styles
{
    public interface IButtonDrawStyle : IBoxDrawStyle
    {
        void Draw(IGraphics g, string text, LayoutF bounds);
        void Draw(IGraphics g, string text, Vector2F position, Vector2F size);
    }
}
