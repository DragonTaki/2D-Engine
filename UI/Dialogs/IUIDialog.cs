/* ----- ----- ----- ----- */
// IUIDialog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Dialogs
{
    /// <summary>
    /// Minimal contract a modal dialog must satisfy to be hosted by the
    /// generic <see cref="Engine.UI.Core.Infrastructure.DialogManager{TDialog}"/>
    /// and <see cref="Engine.UI.Core.Elements.UIOverlayMask"/>. Concrete
    /// dialog content (e.g. a confirm/cancel dialog) is defined by Game,
    /// which implements this interface.
    /// </summary>
    public interface IUIDialog
    {
        bool ShowMaskEffect { get; }
        bool IsVisible { get; set; }
        bool IsEnabled { get; set; }

        /// <summary>Hides the dialog.</summary>
        void Hide();

        /// <summary>
        /// Called when the user dismisses the dialog by clicking the overlay
        /// mask, i.e. outside the dialog itself.
        /// </summary>
        void Cancel();
    }
}
