/* ----- ----- ----- ----- */
// UIButton.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/10/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System;

using Engine.Styles;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;
using Engine.UI.Widgets;

namespace Engine.UI.Core.Elements
{
    public class UIButton : UIElement<UIButton, UIButtonHandler, UIButtonRenderer>
    {
#nullable enable
        private Action? _pendingAction;
#nullable disable

        #region Properties

        // Button text (read/write)
        public string Text { get; set; }

        public IButtonDrawStyle Style { get; set; } = null;

        #endregion

        #region Constructors

        public UIButton()
            : base(zIndex: 0, isPersistent: false, type: UIElementType.Button)
        {
        }

#nullable enable
        public UIButton(string text, Action? action = null)
            : this()
#nullable disable
        {
            Text = text;
            _pendingAction = action;
        }

        #endregion

        protected override void OnInit()
        {
            LocalPosition = ButtonDefaults.Position;
            Size = ButtonDefaults.Size;
            Handler.Action = _pendingAction;
        }

    }

    public class UIButton<TEnum> : UIButton
        where TEnum : Enum
    {
        /// <summary>Value passed to the typed click action (UIButtonHandler&lt;TEnum&gt;.Action).</summary>
        public TEnum Type { get; set; }

        public UIButton() { }

    }
}