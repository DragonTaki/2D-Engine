/* ----- ----- ----- ----- */
// UIOverlayMask.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Game.UI.Dialogs;

using Engine.Globals;
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
        public readonly UIConfirmDialog _dialog;
        public Color MaskColor { get; set; } = Color.FromArgb(120, 0, 0, 0);

        public UIOverlayMask(UIConfirmDialog dialog)
        {
            _dialog = dialog;
            IsVisible = false;
            IsEnabled = false;
        }

        /// <summary>
        /// Show the overlay mask.
        /// </summary>
        public void Show()
        {
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

        public override bool OnMouseDown(MouseEventArgs e)
        {
            // Hide everything and trigger cancel
            Hide();
            _dialog.IsVisible = false;
            _dialog.IsEnabled = false;
            _dialog._onResult?.Invoke(ConfirmDialogResult.Cancel);
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

        internal override bool HandleMouseDown(MouseEventArgs e) => true;
        internal override bool HandleMouseMove(MouseEventArgs e) => true;
        internal override bool HandleMouseUp(MouseEventArgs e) => true;
        internal override bool HandleMouseWheel(MouseEventArgs e) => true;
        internal override bool HandleMouseClick(MouseEventArgs e) => true;
    }

    public class UIOverlayMaskRenderer : UIRenderer<UIOverlayMask>
    {
        private readonly UIOverlayMask _element;

        public UIOverlayMaskRenderer(UIOverlayMask element)
        {
            _element = element;
        }

        protected override void OnRender(Graphics g, UIOverlayMask element)
        {
            if (_element._dialog.ShowMaskEffect)
            {
                using var brush = new SolidBrush(_element.MaskColor);
                var bounds = new RectangleF(0, 0, GlobalWindow.Width, GlobalWindow.Height);
                g.FillRectangle(brush, bounds);
            }
        }
    }
}
