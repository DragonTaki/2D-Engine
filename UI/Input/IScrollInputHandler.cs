/* ----- ----- ----- ----- */
// IScrollInputHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.Physics;
using Engine.UI.Core.Elements;
using static Engine.UI.Input.ScrollInputHandler;

namespace Engine.UI.Input
{
    /// <summary>
    /// Interface for scroll input handler used by the UI system.
    /// </summary>
    public interface IScrollInputHandler : IInputHandler
    {
        bool IsDragging { get; }
        int ZIndex { get; set; }

        void RegisterScrollTarget(
            UIElement element,
            Physics2D physics,
            Func<RectangleF> viewportGetter,
            ScrollBehavior behavior = null,
            int zIndex = 0);

        bool IsDraggingWithinActiveTarget(Point location);

        bool HasMovedEnoughToDrag();

        float DragThreshold();
    }
}