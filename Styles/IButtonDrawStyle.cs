/* ----- ----- ----- ----- */
// IButtonDrawStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Geometry;
using Engine.Mathematics;

namespace Engine.Styles
{
    public interface IButtonDrawStyle : IBoxDrawStyle
    {
        void Draw(Graphics g, string text, LayoutF bounds);
        void Draw(Graphics g, string text, Vector2F position, Vector2F size);
    }
}
