/* ----- ----- ----- ----- */
// BaseDefaults.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.Mathematics;
using Engine.UI.Constants.Core;

namespace Engine.UI.Constants.Components
{
    public static class BaseDefaults
    {
        public static readonly Vector2F Size = new Vector2F(500, 500);

        public static readonly Vector2F Position = new Vector2F(0, 0);

        public static readonly LayoutF Layout = new LayoutF(Size, Position);

        public const float Margin = 5.0f;

        public static readonly Anchor Anchor = Anchor.TopLeft | Anchor.StretchX;

        public static readonly PaddingF Padding = new PaddingF(10);
    }
}
