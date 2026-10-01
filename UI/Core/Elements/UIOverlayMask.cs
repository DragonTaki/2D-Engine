/* ----- ----- ----- ----- */
// UIOverlayMask.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2026/09/30
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Mathematics;
using Engine.Platform;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;
using Engine.UI.Dialogs;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// A full-screen UI mask that intercepts mouse events and triggers Cancel for the dialog.
    /// </summary>
    public class UIOverlayMask : UIElement
    {
        public readonly IUIDialog Dialog;
        public Color MaskColor { get; set; } = Color.FromArgb(120, 0, 0, 0);

        public UIOverlayMask(IUIDialog dialog)
        {
            Dialog = dialog;
            IsVisible = false;
            IsEnabled = false;

            // The dim renderer and the modal handler below were defined but never attached,
            // so the dialog's ShowMaskEffect dimming was never drawn and, while a dialog was
            // open, hover/wheel/click still reached the screens underneath.
            RendererBase = new UIOverlayMaskRenderer(this);
            HandlerBase = new UIOverlayMaskHandler(this);

            // Covers its parent (the overlay layer, which covers the root, i.e. the whole
            // UI area) on all four edges, so area-tested events (wheel, click) and hit
            // testing see the mask everywhere and it follows window resizes. It used to be
            // sized to GlobalViewport.Size once per Show(), which stopped matching the
            // window as soon as the window was resized while a dialog was open.
            LocalPosition = Vector2F.Zero;
            LayoutRules.PositionMode = PositionMode.Absolute;
            LayoutRules.Left = 0f;
            LayoutRules.Top = 0f;
            LayoutRules.Right = 0f;
            LayoutRules.Bottom = 0f;
        }

        /// <summary>
        /// Show the overlay mask.
        /// </summary>
        public void Show()
        {
            // Sized by the layout (all four edges pinned, see the constructor).
            IsVisible = true;
            IsEnabled = true;
        }

        /// <summary>
        /// Hide the overlay mask.
        /// </summary>
        public void Hide()
        {
            IsVisible = false;
            IsEnabled = false;
        }

        public override bool OnMouseDown(IMouseEvent e)
        {
            // While hidden the mask must not swallow clicks or re-fire the
            // dialog's last result callback.
            if (!IsInteractable)
                return false;

            // Hide everything and trigger cancel
            Hide();
            Dialog.IsVisible = false;
            Dialog.IsEnabled = false;
            Dialog.Cancel();
            return true;
        }
    }

    public class UIOverlayMaskHandler : UIHandler
    {
        private readonly UIOverlayMask _element;

        public UIOverlayMaskHandler(UIOverlayMask element)
        {
            _element = element;
        }

        internal override bool HandleMouseDown(IMouseEvent e) => true;
        internal override bool HandleMouseMove(IMouseEvent e) => true;
        internal override bool HandleMouseUp(IMouseEvent e) => true;
        internal override bool HandleMouseWheel(IMouseEvent e) => true;
        internal override bool HandleMouseClick(IMouseEvent e) => true;
    }

    public class UIOverlayMaskRenderer : UIRenderer<UIOverlayMask>
    {
        private readonly UIOverlayMask _element;

        public UIOverlayMaskRenderer(UIOverlayMask element)
        {
            _element = element;
        }

        protected override void OnRender(IGraphics g, UIOverlayMask element)
        {
            if (_element.Dialog.ShowMaskEffect)
            {
                using var brush = GraphicsBackend.Factory.CreateSolidBrush(_element.MaskColor);
                // The mask's own laid-out bounds, which cover the whole UI area.
                var bounds = element.GetCurrentAbsoluteBounds();
                g.FillRectangle(brush, new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height));
            }
        }
    }
}
