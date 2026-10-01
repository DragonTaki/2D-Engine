/* ----- ----- ----- ----- */
// UIButtonHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using Engine.Platform;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    public class UIButtonHandler : UIHandler<UIButton, UIButtonHandler, UIButtonRenderer>
    {

        // Action invoked when the button is clicked
#nullable enable
        public Action? Action { get; set; }
#nullable disable

        // Highlight state
        public bool IsHighlighted { get; set; } = false;
        public UIButtonHandler() { }

        #region Mouse Handling

        internal override bool HandleMouseClick(IMouseEvent e)
        {
            if (!Element.IsEnabled) return false; // Not triggered while disabled
            Action?.Invoke();
            return true;
        }

        #endregion
    }

    public class UIButtonHandler<TEnum> : UIButtonHandler
        where TEnum : Enum
    {
#nullable enable
        public new Action<TEnum>? Action { get; set; }
#nullable disable

        internal void OnClick(TEnum type)
        {
            Action?.Invoke(type);
        }

        // The typed Action hides the base one, and nothing ever invoked it: an onClick
        // passed to UiFactory.CreateButton<TEnum> was stored here and silently dropped.
        // Run the base (untyped) Action as before, then the typed one with the button's Type.
        internal override bool HandleMouseClick(IMouseEvent e)
        {
            if (!base.HandleMouseClick(e))
                return false;

            if (Element is UIButton<TEnum> typedButton)
                OnClick(typedButton.Type);
            return true;
        }
    }
}
