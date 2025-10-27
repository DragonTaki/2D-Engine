/* ----- ----- ----- ----- */
// UIScrollContainerHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Windows.Forms;

using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    public class UIScrollContainerHandler : UIContainerHandler<UIScrollContainer, UIScrollContainerHandler, UIScrollContainerRenderer>
    {
        private UIScrollContainer ScrollContainer => Element as UIScrollContainer;
        public UIScrollContainerHandler() { }

        /// <summary>
        /// Applies a physics target offset based on current scrolling velocity for rebound/inertia.
        /// </summary>
        /// <param name="velocity">Current vertical scrolling velocity.</param>
        private void ApplyVelocityBasedTarget(float velocity)
        {
            // For fast scroll, set the target position farther (e.g., 60px based on velocity)
            float targetOffset = Math.Sign(velocity) * Math.Min(Math.Abs(velocity), 60); // You can adjust this multiplier (e.g., 60) for higher velocities.

            // Apply target offset depending on scroll direction
            Element.Physics.Position.Target = Element.Physics.Position.Current + targetOffset;
            Element.Physics.Position.HasTarget = true;
        }

        /// <summary>
        /// Applies current alignment mode (Top or Bottom) when content size changes.
        /// </summary>
        public void ApplyAlignment()
        {
            if (!ScrollContainer.OverContent)
            {
                // Content smaller than viewport: Reset to top
                ScrollContainer.ScrollY = 0;
                return;
            }

            switch (ScrollContainer.VerticalAlignment)
            {
                case ScrollAlignment.Top:
                    ScrollContainer.ScrollY = 0;

                    Element.Physics.Position.Target = new Vector2F(Element.Physics.Position.Base.X, Element.GetCurrentAbsolutePosition().Y);
                    break;

                case ScrollAlignment.Bottom:
                    float absTopY = Element.GetCurrentAbsolutePosition().Y;
                    float gapY = -(ScrollContainer.ContentHeight - Element.Size.Y);
                    float absGapY = absTopY + gapY;
                    ScrollContainer.ScrollY = gapY;

                    Element.Physics.Position.Target = new Vector2F(Element.Physics.Position.Base.X, absGapY);
                    break;
            }
            Element.Physics.Position.HasTarget = false;
        }

        /// <summary>
        /// Handles mouse down events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if event was handled, otherwise false.</returns>
        internal override bool HandleMouseDown(MouseEventArgs e)
        {
            return ScrollContainer.InputHandler.OnMouseDown(e);
        }

        /// <summary>
        /// Handles mouse move events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if event was handled, otherwise false.</returns>
        internal override bool HandleMouseMove(MouseEventArgs e)
        {
            return ScrollContainer.InputHandler.OnMouseMove(e);
        }

        /// <summary>
        /// Handles mouse up events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if event was handled, otherwise false.</returns>
        internal override bool HandleMouseUp(MouseEventArgs e)
        {
            return ScrollContainer.InputHandler.OnMouseUp(e);
        }

        /// <summary>
        /// Handles mouse wheel events.
        /// </summary>
        /// <param name="e">Mouse event arguments.</param>
        /// <returns>True if event was handled, otherwise false.</returns>
        internal override bool HandleMouseWheel(MouseEventArgs e)
        {
            return ScrollContainer.InputHandler.OnMouseWheel(e);
        }

        /// <summary>
        /// Updates scroll container every frame. Handles overscroll, inertia, and rebound behavior.
        /// </summary>
        internal override void OnUpdate()
        {
            //Console.WriteLine($"ScrollY: {ScrollY}, gap: {-(ContentHeight - Size.Y)}, Physics.Position.Base: {Physics.Position.Base}, Physics.Position.Current: {Physics.Position.Current}, Physics.Position.Target: {Physics.Position.Target}");
            // If content fits within viewport, return to base position
            if (!ScrollContainer.OverContent)
            {
                // Moved
                if (ScrollContainer.ScrollY != 0)
                {
                    Element.Physics.Position.Target = Element.Physics.Position.Base;
                    Element.Physics.Position.HasTarget = true;
                }
                // Already back to base position
                else
                {
                    Element.Physics.Position.HasTarget = false;
                }
            }
            // Content is bigger than viewpoint
            else
            {
                //Console.WriteLine($"ScrollY: {ScrollY}, gap: {-(ContentHeight - Size.Y)}");
                // Moved
                if (ScrollContainer.ScrollY != 0)
                {
                    if (ScrollContainer.ScrollY > 0)
                    {
                        Element.Physics.Position.Target = Element.Physics.Position.Base;
                        Element.Physics.Position.HasTarget = true;
                    }
                    else if (ScrollContainer.ScrollY < -(ScrollContainer.ContentHeight - Element.Size.Y))
                    {
                        Element.Physics.Position.Target = Element.Physics.Position.Base - new Vector2F(0, ScrollContainer.ContentHeight - Element.Size.Y);
                        Element.Physics.Position.HasTarget = true;
                    }
                    else
                    {
                        Element.Physics.Position.HasTarget = false;
                    }
                }
                // Already back to base position
                else
                {
                    Element.Physics.Position.HasTarget = false;
                }
            }
        }

        /// <summary>
        /// Resets input delta after processing input each frame.
        /// </summary>
        internal override void OnEndFrame()
        {
            ScrollContainer.InputHandler.EndFrame();
        }

    }
}
