/* ----- ----- ----- ----- */
// FlexEnums.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/29
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Constants.Core
{
    /// <summary>
    /// How a container arranges its in-flow (<see cref="PositionMode.Flow"/>) children.
    /// </summary>
    public enum LayoutContainer
    {
        /// <summary>
        /// Not a layout container: each in-flow child is sized and aligned on its own
        /// inside the content box (children overlap, like a single-cell grid).
        /// </summary>
        None,

        /// <summary>Flexbox container (CSS <c>display: flex</c>).</summary>
        Flex
    }

    /// <summary>Main axis of a flex container (CSS <c>flex-direction</c>).</summary>
    public enum FlexDirection
    {
        /// <summary>Main axis is X (left to right).</summary>
        Row,

        /// <summary>Main axis is Y (top to bottom).</summary>
        Column
    }

    /// <summary>Whether flex items may wrap onto multiple lines (CSS <c>flex-wrap</c>).</summary>
    public enum FlexWrap
    {
        NoWrap,
        Wrap
    }

    /// <summary>Main-axis distribution of free space in a flex line (CSS <c>justify-content</c>).</summary>
    public enum JustifyContent
    {
        Start,
        Center,
        End,
        SpaceBetween,
        SpaceAround,
        SpaceEvenly
    }

    /// <summary>
    /// Cross-axis alignment of flex items inside their line (CSS <c>align-items</c> /
    /// <c>align-self</c>).
    /// </summary>
    public enum FlexAlign
    {
        Start,
        Center,
        End,

        /// <summary>
        /// Items whose cross size is <see cref="SizeMode.Auto"/> fill the line's cross
        /// size (minus margins); others are placed at Start.
        /// </summary>
        Stretch
    }

    /// <summary>Cross-axis distribution of wrapped flex lines (CSS <c>align-content</c>).</summary>
    public enum AlignContent
    {
        Start,
        Center,
        End,
        Stretch,
        SpaceBetween,
        SpaceAround
    }
}
