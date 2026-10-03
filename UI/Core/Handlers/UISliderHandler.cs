/* ----- ----- ----- ----- */
// UISliderHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.Mathematics;
using Engine.Platform;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;
using Engine.UI.Input;

namespace Engine.UI.Core.Handlers
{
    /// <summary>
    /// Handler for <see cref="UISlider"/>: a press jumps the knob to the pressed point and starts
    /// a drag, moves drag it, the release ends it; every change of the (snapped) value is
    /// reported through <see cref="ValueChanged"/>, and the end of a press that changed it through <see cref="ValueCommitted"/>. It captures the pointer
    /// (<see cref="IPointerCaptureTarget"/>) while enabled, so the drag gets every move and the
    /// page underneath never drag-scrolls. A disabled slider takes no input.
    /// </summary>
    public class UISliderHandler : UIHandler<UISlider, UISliderHandler, UISliderRenderer>, IPointerCaptureTarget
    {
#nullable enable
        /// <summary>Invoked with the new value whenever the mouse (or <see cref="SetValue"/>) changed it, also during a drag.</summary>
        public Action<float>? ValueChanged { get; set; }

        /// <summary>Invoked once with the final value when a press-drag-release ends and the value changed during it (not on every drag step).</summary>
        public Action<float>? ValueCommitted { get; set; }
#nullable disable

        /// <summary>Whether the value changed since the current press began (reported by <see cref="ValueCommitted"/> when it ends).</summary>
        private bool _changedSincePress;

        public UISliderHandler() { }

        /// <summary>
        /// Sets the value (snapped) and invokes <see cref="ValueChanged"/> when it changed; does
        /// nothing while the slider is disabled.
        /// </summary>
        /// <returns>Whether the value changed.</returns>
        public bool SetValue(float value)
        {
            if (!Element.IsEnabled)
                return false;

            float old = Element.Value;
            Element.Value = value;
            if (Element.Value == old)
                return false;

            _changedSincePress = true;
            ValueChanged?.Invoke(Element.Value);
            return true;
        }

        #region IPointerCaptureTarget

        /// <summary>An enabled slider takes the whole press-drag-release.</summary>
        public bool CapturesPress(Vector2F location) => Element.IsEnabled;

        /// <summary>The capture ended (release delivered, or input cancelled): the drag is over.</summary>
        public void OnCaptureLost()
        {
            Element.IsDragging = false;
            Commit();
        }

        #endregion

        #region Mouse Handling

        /// <summary>Jumps the knob to the pressed point and starts dragging.</summary>
        internal override bool HandleMouseDown(IMouseEvent e)
        {
            if (!Element.IsEnabled)
                return false;

            Element.IsDragging = true;
            _changedSincePress = false;
            SetValue(Element.ValueAt(e.X));
            return true;
        }

        /// <summary>While dragging, moves the knob to the mouse's x (past the ends: min / max).</summary>
        internal override bool HandleMouseMove(IMouseEvent e)
        {
            if (!Element.IsDragging)
                return false;

            SetValue(Element.ValueAt(e.X));
            return true;
        }

        /// <summary>Ends the drag.</summary>
        internal override bool HandleMouseUp(IMouseEvent e)
        {
            if (!Element.IsDragging)
                return false;

            Element.IsDragging = false;
            Commit();
            return true;
        }

        /// <summary>Reports the end of a press (once) through <see cref="ValueCommitted"/> when the value changed during it.</summary>
        private void Commit()
        {
            if (!_changedSincePress)
                return;

            _changedSincePress = false;
            ValueCommitted?.Invoke(Element.Value);
        }

        /// <summary>The press already moved the knob; the click is only taken so it does not fall through.</summary>
        internal override bool HandleMouseClick(IMouseEvent e) => Element.IsEnabled;

        #endregion
    }
}
