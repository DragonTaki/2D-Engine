/* ----- ----- ----- ----- */
// LayoutEngine.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/29
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

using Engine.Globals;
using Engine.Mathematics;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Bases;

namespace Engine.UI.Layout
{
    /// <summary>
    /// Connects UI elements to the pure solvers (<see cref="BoxSolver"/>, <see cref="FlexSolver"/>):
    /// collects a container's layout-managed children, runs the arrangement in the
    /// container's content/padding box, applies <see cref="Models.UILayout.Offset"/>, snaps the
    /// resulting absolute rectangles to device pixels and writes them back.
    /// <para>
    /// Driven by <c>UIElement.UpdateLayout</c> (top-down: a container is arranged before its
    /// children arrange theirs).
    /// </para>
    /// </summary>
    public static class LayoutEngine
    {
        #region Arrange

        /// <summary>
        /// Arranges <paramref name="container"/>'s layout-managed children (Flow and Absolute,
        /// displayed, not <see cref="Models.UILayout.IgnoreParentLayout"/>). Legacy children are
        /// left untouched. Children whose box changed are marked dirty so their own pass runs.
        /// </summary>
        public static void ArrangeChildren(UIElementBase container)
        {
            var flow = new List<LayoutItem>();
            var absolute = new List<LayoutItem>();
            CollectItems(container, flow, absolute);
            if (flow.Count == 0 && absolute.Count == 0)
                return;

            var rules = container.LayoutRules;
            var padding = rules.Padding;
            float width = container.Size.X;
            float height = container.Size.Y;
            float contentW = System.Math.Max(0f, width - padding.Horizontal);
            float contentH = System.Math.Max(0f, height - padding.Vertical);

            if (flow.Count > 0)
            {
                if (rules.Container == LayoutContainer.Flex)
                {
                    FlexSolver.Solve(rules, contentW, contentH, flow);
                    foreach (var item in flow)
                    {
                        item.X += padding.Left;
                        item.Y += padding.Top;
                    }
                }
                else
                {
                    foreach (var item in flow)
                        BoxSolver.PlaceFlow(item, padding.Left, padding.Top, contentW, contentH);
                }
            }

            foreach (var item in absolute)
                BoxSolver.PlaceAbsolute(item, width, height);

            var origin = GetChildOrigin(container);
            foreach (var item in flow)
                Apply(item, origin);
            foreach (var item in absolute)
                Apply(item, origin);
        }

        private static void CollectItems(UIElementBase container, List<LayoutItem> flow, List<LayoutItem> absolute)
        {
            int index = 0;
            foreach (var child in container.Children)
            {
                if (!child.IsLayoutManaged || !child.IsDisplayed)
                    continue;

                var item = CreateItem(child, index++);
                if (child.LayoutRules.PositionMode == PositionMode.Absolute)
                    absolute.Add(item);
                else
                    flow.Add(item);
            }
        }

        private static LayoutItem CreateItem(UIElementBase element, int index)
        {
            var declared = element.DeclaredSize ?? Vector2F.Zero;
            return new LayoutItem(element.LayoutRules, declared.X, declared.Y, index,
                element.MeasureIntrinsicSize, element);
        }

        /// <summary>
        /// Adds the offset, snaps the absolute edges to device pixels and writes the result.
        /// </summary>
        private static void Apply(LayoutItem item, Vector2F origin)
        {
            var element = (UIElementBase)item.Tag;
            var offset = element.LayoutRules.Offset ?? Vector2F.Zero;

            float left = origin.X + item.X + offset.X;
            float top = origin.Y + item.Y + offset.Y;
            var snapped = SnapRect(left, top, item.Width, item.Height);

            element.ApplyLayoutResult(
                new Vector2F(snapped.X - origin.X, snapped.Y - origin.Y),
                new Vector2F(snapped.Width, snapped.Height));
        }

        #endregion

        #region Measure

        /// <summary>
        /// Default <c>MeasureIntrinsicSize</c>: the content size of the element's in-flow
        /// children (flex or not) plus its padding; the <see cref="UIElementBase.DeclaredSize"/>
        /// when it has no in-flow children.
        /// </summary>
        public static Vector2F MeasureContent(UIElementBase element, Vector2F available)
        {
            var flow = new List<LayoutItem>();
            var absolute = new List<LayoutItem>();
            CollectItems(element, flow, absolute);

            if (flow.Count == 0)
            {
                var declared = element.DeclaredSize ?? Vector2F.Zero;
                return new Vector2F(declared.X, declared.Y);
            }

            var rules = element.LayoutRules;
            var padding = rules.Padding;
            float spaceW = available.X - padding.Horizontal;
            float spaceH = available.Y - padding.Vertical;

            var content = rules.Container == LayoutContainer.Flex
                ? FlexSolver.Solve(rules, spaceW, spaceH, flow, measuring: true)
                : BoxSolver.MeasureFlow(flow, spaceW, spaceH);

            return new Vector2F(content.X + padding.Horizontal, content.Y + padding.Vertical);
        }

        #endregion

        #region Coordinates / Snapping

        /// <summary>
        /// The absolute position a container's children are placed relative to, ignoring
        /// scroll offsets: the same walk as <c>UIElement.GetCurrentAbsolutePosition</c>, but
        /// through a physics ancestor's <c>Position.Base</c> (its unscrolled position) instead
        /// of <c>Current</c>, so snapping stays stable while content scrolls.
        /// </summary>
        public static Vector2F GetChildOrigin(UIElementBase container)
        {
            if (container.Physics != null)
                return container.Physics.Position.Base;

            float x = container.LocalPosition.Current.X;
            float y = container.LocalPosition.Current.Y;
            var current = container.Parent;
            while (current != null)
            {
                if (current.Physics != null)
                {
                    x += current.Physics.Position.Base.X;
                    y += current.Physics.Position.Base.Y;
                    break;
                }
                x += current.LocalPosition.Current.X;
                y += current.LocalPosition.Current.Y;
                current = current.Parent;
            }
            return new Vector2F(x, y);
        }

        /// <summary>
        /// Snaps an absolute design-space rectangle to device pixels: both edges on each axis
        /// are rounded (so adjacent boxes stay gap-free) under the current
        /// <see cref="GlobalViewport"/> transform.
        /// </summary>
        public static RectangleF SnapRect(float left, float top, float width, float height)
        {
            var offset = GlobalViewport.Offset;
            float l = GlobalViewport.SnapToDevicePixel(left, offset.X);
            float t = GlobalViewport.SnapToDevicePixel(top, offset.Y);
            float r = GlobalViewport.SnapToDevicePixel(left + width, offset.X);
            float b = GlobalViewport.SnapToDevicePixel(top + height, offset.Y);
            return new RectangleF(l, t, System.Math.Max(0f, r - l), System.Math.Max(0f, b - t));
        }

        #endregion
    }
}
