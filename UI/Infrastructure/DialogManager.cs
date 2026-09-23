/* ----- ----- ----- ----- */
// DialogManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2026/09/23
// Version: v2.0
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Dialogs;
using Engine.UI.Utils;

namespace Engine.UI.Core.Infrastructure
{
    /// <summary>
    /// Generic single-instance modal dialog manager: hosts exactly one
    /// <typeparamref name="TDialog"/> plus its overlay mask on the UI root's
    /// overlay layer. Engine only knows about <see cref="IUIDialog"/>; the
    /// concrete dialog type and its content are supplied by Game.
    /// </summary>
    /// <typeparam name="TDialog">
    /// The concrete dialog element type this manager hosts.
    /// </typeparam>
    public class DialogManager<TDialog> : InitializableOnceBase<UIRootNode>
        where TDialog : UIElement, IUIDialog
    {
        private static TDialog _dialog;
        private static UIOverlayMask _overlayMask;
        private static UIOverlayNode _overlayNode;

        private readonly Func<TDialog> _dialogFactory;

        public DialogManager(Func<TDialog> dialogFactory)
        {
            _dialogFactory = dialogFactory;
        }

        protected override void OnInit(UIRootNode root)
        {
            _overlayNode = UIElementUtils.GetOrCreateOverlay(root);

            if (_dialog == null)
            {
                _dialog = _dialogFactory();
                _dialog.ZIndex = int.MaxValue;

                _overlayMask = new UIOverlayMask(_dialog)
                {
                    ZIndex = int.MaxValue - 1
                };

                _overlayNode.AddChild(_overlayMask);
                _overlayNode.AddChild(_dialog);
            }
        }

        /// <summary>The single hosted dialog instance.</summary>
        public static TDialog Dialog => _dialog;

        /// <summary>Shows the overlay mask behind the dialog.</summary>
        public static void ShowMask() => _overlayMask.Show();

        /// <summary>Hides both the overlay mask and the dialog.</summary>
        public static void HideAll()
        {
            _overlayMask?.Hide();
            _dialog?.Hide();
        }
    }
}
