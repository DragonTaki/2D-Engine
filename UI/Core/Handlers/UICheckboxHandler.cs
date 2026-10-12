/* ----- ----- ----- ----- */
// UICheckboxHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;
using Engine.UI.Input;

namespace Engine.UI.Core.Handlers
{
    /// <summary>Checkbox mouse and keyboard input using the shared keyboard focus mechanism.</summary>
    public class UICheckboxHandler : UIHandler<UICheckbox, UICheckboxHandler, UICheckboxRenderer>, IKeyboardInputTarget
    {
        UIElementBase IKeyboardInputTarget.TargetElement => Element;
        IKeyboardInputTarget IKeyboardInputTarget.NextTarget => Element.NextFocus?.HandlerBase as IKeyboardInputTarget;

        /// <summary>Flips the checked state and raises Changed only while enabled.</summary>
        /// <returns>Whether input changed the state.</returns>
        public bool Toggle()
        {
            if (!Element.IsEnabled || Element.IsDisposed)
                return false;
            Element.IsChecked = !Element.IsChecked;
            Element.NotifyChanged();
            return true;
        }

        public bool HandleTextInput(char c) => Element.IsFocused && c == ' ' && Toggle();

        public bool HandleKey(UIKey key)
        {
            if (!Element.IsEnabled || Element.IsDisposed || !Element.IsFocused)
                return false;
            return key switch
            {
                UIKey.Enter => Toggle(),
                UIKey.Tab or UIKey.Escape => true,
                _ => false,
            };
        }

        public void OnFocusChanged(bool focused) => Element.IsFocused = focused;

        protected internal override bool HandleMouseClick(IMouseEvent e) => Toggle();
    }
}
