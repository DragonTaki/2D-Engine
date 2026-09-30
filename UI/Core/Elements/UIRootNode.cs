/* ----- ----- ----- ----- */
// UIRootNode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2026/09/30
// Version: v1.2
/* ----- ----- ----- ----- */

using Engine.Globals;
using Engine.Mathematics;
using Engine.Platform;
using Engine.UI.Constants.Components;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// Represents the root container of the UI hierarchy.
    /// This is typically the topmost element that holds all other UI elements,
    /// such as screens, panels, dialogs, or overlays.
    /// </summary>
    public class UIRootNode : UINode
    {

#nullable enable
        public IWindow? MainWindow { get; set; }
#nullable disable

        /// <summary>
        /// Initializes a new instance of the <see cref="UIRootNode"/> class.
        /// </summary>
        /// <param name="zIndex">
        /// The Z-order index of the root node.
        /// Elements with higher <paramref name="zIndex"/> are rendered on top.
        /// Default is 0.
        /// </param>
        /// <param name="isPersistent">
        /// Indicates whether this root node should persist across different UI screens.
        /// Default is true.
        /// </param>
        /// <param name="type">
        /// The type of this UI element.
        /// Should be <see cref="UIElementType.Root"/> for root nodes.
        /// </param>
        public UIRootNode(int zIndex = 0, bool isPersistent = true, UIElementType type = UIElementType.Root)
            : base(zIndex, isPersistent, type)
        {
            GlobalViewport.Changed += OnViewportChanged;
        }

        public override void RequestRedraw()
        {
            MainWindow?.Invalidate();
        }

        /// <summary>
        /// Keeps the root's size equal to the viewport's UI area (<see cref="GlobalViewport.Size"/>,
        /// the whole window in design units - the box its layout-managed children are
        /// arranged in), then draws the tree - which runs any pending layout pass first.
        /// </summary>
        public override void Draw(IGraphics g)
        {
            SyncToViewport();
            base.Draw(g);
        }

        /// <summary>
        /// Window resize (<see cref="GlobalViewport.Recalculate"/>): the UI area's size and/or
        /// the scale that pixel snapping rounds to changed, so the root is resized and the
        /// whole tree's layout is invalidated. Legacy elements keep their positions (their
        /// rules are one-shot).
        /// </summary>
        private void OnViewportChanged()
        {
            SyncToViewport();
            InvalidateLayoutRecursive();
        }

        /// <summary>
        /// The root is exactly <see cref="GlobalViewport.Size"/>: the whole window in design
        /// units (no letterbox), so full-window layouts stretch with the window.
        /// </summary>
        private void SyncToViewport()
        {
            var viewport = GlobalViewport.Size;
            if (Size.X != viewport.X || Size.Y != viewport.Y)
                Size = new Vector2F(viewport.X, viewport.Y);
        }

        protected override void DisposeUI()
        {
            base.DisposeUI();
            GlobalViewport.Changed -= OnViewportChanged;
        }
    }
}
