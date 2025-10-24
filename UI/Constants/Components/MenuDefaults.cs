/* ----- ----- ----- ----- */
// MenuDefaults.cs
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
    public static class MenuDefaults
    {
        // Menu 整體大小與位置
        public static readonly Vector2F Size = BaseDefaults.Size;

        public static readonly Vector2F Position = BaseDefaults.Position;

        public static readonly LayoutF Layout = new LayoutF(Size, Position);

        public const float Margin = ScrollContainerDefaults.Margin;

        public static readonly Anchor Anchor = ScrollContainerDefaults.Anchor;

        public static readonly PaddingF Padding = ScrollContainerDefaults.Padding;

        // ScrollContainer 預設值
        public static class Scroll
        {
            public static readonly Vector2F Size = ScrollContainerDefaults.Size;

            public static readonly Vector2F Position = ScrollContainerDefaults.Position;

            public static readonly LayoutF Layout = new LayoutF(Size, Position);

            public const float Margin = ScrollContainerDefaults.Margin;

            public static readonly Anchor Anchor = ScrollContainerDefaults.Anchor;

            public static readonly PaddingF Padding = ScrollContainerDefaults.Padding;

            public const float OverscrollLimit = ScrollContainerDefaults.DefaultOverscrollLimit;
        }

        // 按鈕預設值
        public static class Button
        {
            public static readonly Vector2F Size = ButtonDefaults.Size;

            public static readonly Vector2F Position = ButtonDefaults.Position;

            public static readonly LayoutF Layout = new LayoutF(Size, Position);

            public const float Margin = ButtonDefaults.Margin;

            public static readonly Anchor Anchor = ButtonDefaults.Anchor;

            public static readonly PaddingF Padding = ButtonDefaults.Padding;
        }
    }
}
