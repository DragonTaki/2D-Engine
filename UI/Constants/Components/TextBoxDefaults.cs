/* ----- ----- ----- ----- */
// TextBoxDefaults.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/25
// Update Date: 2025/10/25
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.Mathematics;
using Engine.UI.Constants.Core;

namespace Engine.UI.Constants.Components
{
    public static class TextBoxDefaults
    {
        // Overall size and position of the TextBox
        public static readonly Vector2F Position = BaseDefaults.Position;

        public static readonly Vector2F Size = BaseDefaults.Size;

        public static readonly LayoutF Layout = new LayoutF(Position, Size);

        public const float Margin = ScrollContainerDefaults.Margin;

        public static readonly Anchor Anchor = ScrollContainerDefaults.Anchor;

        public static readonly PaddingF Padding = ScrollContainerDefaults.Padding;

        // Defaults for the inner ScrollContainer
        public static class Scroll
        {
            public static readonly Vector2F Position = ScrollContainerDefaults.Position;

            public static readonly Vector2F Size = ScrollContainerDefaults.Size;

            public static readonly LayoutF Layout = new LayoutF(Position, Size);

            public const float Margin = ScrollContainerDefaults.Margin;

            public static readonly Anchor Anchor = ScrollContainerDefaults.Anchor;

            public static readonly PaddingF Padding = ScrollContainerDefaults.Padding;

            public const float OverscrollLimit = ScrollContainerDefaults.DefaultOverscrollLimit;
        }
    }
}
