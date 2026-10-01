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

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// A clickable button. It draws nothing itself: its parent (a menu or dialog renderer)
    /// draws it through <see cref="Style"/>; clicks run the handler's <c>Action</c>.
    /// </summary>
    public class UIButton : UIElement<UIButton, UIButtonHandler, UIButtonRenderer>
    {
#nullable enable
        private Action? _pendingAction;
#nullable disable

        #region Properties

        /// <summary>
        /// Button text (read/write).
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// How the parent draws this button; null means the default button style.
        /// </summary>
        public IButtonDrawStyle Style { get; set; } = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty button (text and action set later).
        /// </summary>
        public UIButton()
            : base(zIndex: 0, isPersistent: false, type: UIElementType.Button)
        {
        }

        /// <summary>
        /// Creates a button with text and an optional click action (bound to the handler on Init).
        /// </summary>
        /// <param name="text">Button text.</param>
        /// <param name="action">Action run when the button is clicked, or null.</param>
#nullable enable
        public UIButton(string text, Action? action = null)
            : this()
#nullable disable
        {
            Text = text;
            _pendingAction = action;
        }

        #endregion

        /// <summary>
        /// Applies the default position and size and binds the constructor's action.
        /// </summary>
        protected override void OnInit()
        {
            LocalPosition = ButtonDefaults.Position;
            Size = ButtonDefaults.Size;
            Handler.Action = _pendingAction;
        }
    }

    /// <summary>
    /// A button identified by an enum value, passed to the typed click action.
    /// </summary>
    /// <typeparam name="TEnum">The enum type identifying the button.</typeparam>
    public class UIButton<TEnum> : UIButton
        where TEnum : Enum
    {
        /// <summary>Value passed to the typed click action (UIButtonHandler&lt;TEnum&gt;.Action).</summary>
        public TEnum Type { get; set; }

        /// <summary>
        /// Creates an empty typed button.
        /// </summary>
        public UIButton() { }
    }
}
