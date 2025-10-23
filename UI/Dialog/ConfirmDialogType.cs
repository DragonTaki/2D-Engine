/* ----- ----- ----- ----- */
// ConfirmDialogType.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Dialogs
{
    public enum ConfirmDialogType
    {
        Default,
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel
    }

    public enum ConfirmDialogResult
    {
        None,
        Ok,
        Cancel,
        Yes,
        No
    }
}
