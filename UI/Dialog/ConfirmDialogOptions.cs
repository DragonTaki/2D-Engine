/* ----- ----- ----- ----- */
// ConfirmDialogOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Engine.UI.Widgets;

namespace Engine.UI.Dialogs
{
    public static class ConfirmDialogOptions
    {
        private static readonly Dictionary<ConfirmDialogType, ConfirmDialogResult[]> DialogMappings =
            new()
            {
                { ConfirmDialogType.Default,       new[] { ConfirmDialogResult.None } },
                { ConfirmDialogType.Ok,            new[] { ConfirmDialogResult.Ok } },
                { ConfirmDialogType.OkCancel,      new[] { ConfirmDialogResult.Ok, ConfirmDialogResult.Cancel } },
                { ConfirmDialogType.YesNo,         new[] { ConfirmDialogResult.Yes, ConfirmDialogResult.No } },
                { ConfirmDialogType.YesNoCancel,   new[] { ConfirmDialogResult.Yes, ConfirmDialogResult.No, ConfirmDialogResult.Cancel } },
            };

        private static readonly Dictionary<ConfirmDialogResult, string> LabelMap =
            new()
            {
                { ConfirmDialogResult.None,   "<　>" },
                { ConfirmDialogResult.Ok,     "確認" },
                { ConfirmDialogResult.Cancel, "取消" },
                { ConfirmDialogResult.Yes,    "是" },
                { ConfirmDialogResult.No,     "否" }
            };

        /// <summary>
        /// Generate corresponding button items according to ConfirmDialogType
        /// </summary>
        /// <param name="type">Dialog type</param>
        /// <param name="onSelect">Call when user clicks</param>
        public static List<ButtonEntry<ConfirmDialogResult>> Create(ConfirmDialogType type, Action<ConfirmDialogResult> onSelect)
        {
            var results = DialogMappings.TryGetValue(type, out var values)
                ? values
                : new[] { ConfirmDialogResult.Ok };

            return results.Select(result => new ButtonEntry<ConfirmDialogResult>(
                LabelMap[result],
                result,
                () => onSelect(result)
            )).ToList();
        }
    }
}
