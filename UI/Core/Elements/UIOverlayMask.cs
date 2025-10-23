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
using Engine.UI.Dialogs;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// A full-screen UI mask that intercepts mouse events and triggers Cancel for the dialog.
    /// </summary>
    public class UIOverlayMask : UIElement
    {
        private readonly ConfirmDialog _dialog;
        public Color MaskColor { get; set; } = Color.FromArgb(120, 0, 0, 0);

        public UIOverlayMask(ConfirmDialog dialog)
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

        protected override void OnDraw(Graphics g)
        {
            if (_dialog.ShowMaskEffect)
            {
                using var brush = new SolidBrush(MaskColor);
                var bounds = new RectangleF(0, 0, GlobalWindow.Width, GlobalWindow.Height);
                g.FillRectangle(brush, bounds);
            }
        }

        protected override bool HandleMouseDown(MouseEventArgs e) => true;
        protected override bool HandleMouseMove(MouseEventArgs e) => true;
        protected override bool HandleMouseUp(MouseEventArgs e) => true;
        protected override bool HandleMouseWheel(MouseEventArgs e) => true;
        public override bool HandleMouseClick(MouseEventArgs e) => true;
    }
}
