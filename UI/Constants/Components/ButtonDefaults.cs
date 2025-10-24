/* ----- ----- ----- ----- */
// ButtonDefaults.cs
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
    public static class ButtonDefaults
    {
        public static readonly Vector2F Size = BaseDefaults.Size;

        public static readonly Vector2F Position = BaseDefaults.Position;

        public static readonly LayoutF Layout = new LayoutF(Size, Position);

        public const float Margin = BaseDefaults.Margin;

        public static readonly Anchor Anchor = BaseDefaults.Anchor;

        public static readonly PaddingF Padding = BaseDefaults.Padding;
    }
}
