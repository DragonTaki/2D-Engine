/* ----- ----- ----- ----- */
// DialogManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.UI.Dialogs;

using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Dialogs;
using Engine.UI.Utils;

namespace Engine.UI.Core.Infrastructure
{
    public class DialogManager : InitializableOnceBase<UIRootNode>
    {
        private static UIConfirmDialog _confirmDialog;
        private static UIOverlayMask _overlayMask;
        private static UIOverlayNode _overlayNode;

        public DialogManager() { }

        protected override void OnInit(UIRootNode root)
        {
            _overlayNode = UIElementUtils.GetOrCreateOverlay(root);

            if (_confirmDialog == null)
            {
                _confirmDialog = new UIConfirmDialog(new UIConfirmDialogRenderer())
                {
                    ZIndex = int.MaxValue
                };
                _overlayMask = new UIOverlayMask(_confirmDialog)
                {
                    ZIndex = int.MaxValue - 1
                };

                _overlayNode.AddChild(_overlayMask);
                _overlayNode.AddChild(_confirmDialog);
            }
        }

        public static void ShowConfirm(
            string message,
            ConfirmDialogType type,
            Action<ConfirmDialogResult> callback)
        {
            _overlayMask.Show();
            _confirmDialog.Show(message, type, callback);
        }
        public static void HideConfirm()
        {
            _overlayMask?.Hide();
            _confirmDialog?.Hide();
        }
    }
}
