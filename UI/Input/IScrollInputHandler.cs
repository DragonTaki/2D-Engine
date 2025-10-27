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
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using static Engine.UI.Input.ScrollInputHandler;

namespace Engine.UI.Input
{
    /// <summary>
    /// Defines the contract for all scroll input handler modules used in the UI system.
    /// Implementations are responsible for processing drag-based scroll gestures, 
    /// managing multiple scrollable targets, and coordinating interactions between UI layers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface extends <see cref="IInputHandler"/> to handle advanced scroll input logic,
    /// such as inertia, overscroll, viewport clipping, and prioritization of overlapping scroll zones.
    /// </para>
    /// <para>
    /// Each scrollable region is registered with its own <see cref="Physics2D"/> instance and viewport,
    /// allowing multiple independent scroll targets (e.g., nested menus or text boxes).
    /// </para>
    /// </remarks>
    public interface IScrollInputHandler : IInputHandler
    {
        #region Properties : State and Priority

        /// <summary>
        /// Gets a value indicating whether the user is currently performing a drag gesture.
        /// </summary>
        bool IsDragging { get; }

        /// <summary>
        /// Gets or sets the Z-index priority for this scroll handler.
        /// A higher value indicates that this handler should receive input before others when overlapping.
        /// </summary>
        int ZIndex { get; set; }

        #endregion

        #region Scroll Target Management

        /// <summary>
        /// Registers a scrollable UI target with its associated <see cref="Physics2D"/> instance and viewport region.
        /// </summary>
        /// <param name="element">The UI element that acts as the scrollable container.</param>
        /// <param name="physics">The physics instance controlling scroll position and velocity.</param>
        /// <param name="viewportGetter">
        /// A delegate that returns the visible region of the scrollable area.
        /// This allows for dynamic viewport updates (e.g., when the container resizes).
        /// </param>
        /// <param name="behavior">
        /// Optional scroll behavior configuration (inertia, damping, edge bounce, etc.).
        /// Defaults to <c>null</c> if not specified.
        /// </param>
        /// <param name="zIndex">
        /// Optional z-index priority override for the registered scroll target.
        /// Defaults to <c>0</c>.
        /// </param>
        /// <remarks>
        /// Typically called once during UI initialization or layout construction.
        /// </remarks>
        void RegisterScrollTarget(
            UIElementBase element,
            Physics2D physics,
            Func<RectangleF> viewportGetter,
            ScrollBehavior behavior = null,
            int zIndex = 0);

        #endregion

        #region Dragging Detection and Threshold

        /// <summary>
        /// Determines whether the user is currently dragging inside the active scroll target's area.
        /// </summary>
        /// <param name="location">The current mouse cursor position (in screen coordinates).</param>
        /// <returns>
        /// <c>true</c> if the drag is occurring within the active scrollable region;  
        /// otherwise, <c>false</c>.
        /// </returns>
        bool IsDraggingWithinActiveTarget(Point location);

        /// <summary>
        /// Checks if the accumulated movement has exceeded the configured drag threshold, 
        /// allowing scroll motion to begin.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the drag distance exceeds the motion threshold;  
        /// otherwise, <c>false</c>.
        /// </returns>
        bool HasMovedEnoughToDrag();

        /// <summary>
        /// Returns the minimum distance (in pixels) required for a drag gesture
        /// to be recognized as scroll input rather than a simple click.
        /// </summary>
        /// <returns>
        /// The drag threshold value (in pixels).
        /// </returns>
        float DragThreshold();

        #endregion
    }
}
