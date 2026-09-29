/* ----- ----- ----- ----- */
// UIOverlayMask.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Globals;
using Engine.Mathematics;
using Engine.Platform;
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
        public readonly IUIDialog _dialog;
        public Color MaskColor { get; set; } = Color.FromArgb(120, 0, 0, 0);

        public UIOverlayMask(IUIDialog dialog)
        {
            _dialog = dialog;
            IsVisible = false;
            IsEnabled = false;

            // The dim renderer and the modal handler below were defined but never attached,
            // so the dialog's ShowMaskEffect dimming was never drawn and, while a dialog was
            // open, hover/wheel/click still reached the screens underneath.
            RendererBase = new UIOverlayMaskRenderer(this);
            HandlerBase = new UIOverlayMaskHandler(this);

            LocalPosition = Vector2F.Zero;
        }

        /// <summary>
        /// Show the overlay mask.
        /// </summary>
        public void Show()
        {
            // Cover the whole UI design space, so area-tested events (wheel, click) and hit
            // testing see the mask too - with a zero size only move/up were intercepted.
            // Sized here, not in the constructor, which can run before the launcher sets
            // GlobalViewport.DesignSize.
            Size = new Vector2F(GlobalViewport.Size.X, GlobalViewport.Size.Y);
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
            _dialog.IsVisible = false;
            _dialog.IsEnabled = false;
            _dialog.Cancel();
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
            if (_element._dialog.ShowMaskEffect)
            {
                using var brush = GraphicsBackend.Factory.CreateSolidBrush(_element.MaskColor);
                var bounds = new RectangleF(0, 0, GlobalViewport.Size.X, GlobalViewport.Size.Y);
                g.FillRectangle(brush, bounds);
            }
        }
    }
}
