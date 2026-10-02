/* ----- ----- ----- ----- */
// UISlider.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Globalization;

using Engine.Platform;
using Engine.Styles;
using Engine.UI.Constants.Components;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;
using Engine.UI.Utils;

namespace Engine.UI.Core.Elements
{
    /// <summary>
    /// A slider: a track with its part left of the knob filled, a knob at the value, and the
    /// value's text (<see cref="ValueText"/>, formatted by the caller) at the right. Pressing on
    /// the track jumps the knob there and dragging moves it; the value always snaps to
    /// <see cref="Step"/> within <see cref="Minimum"/>..<see cref="Maximum"/>
    /// (<see cref="SliderMath"/>), and the handler reports each change while dragging
    /// (<see cref="UISliderHandler.ValueChanged"/>). The handler captures the pointer
    /// (<see cref="Input.IPointerCaptureTarget"/>), so a drag never scrolls the page the slider
    /// is on. Drawn by <see cref="UISliderRenderer"/> with <see cref="Style"/>.
    /// <para>
    /// Disabled (<see cref="Bases.UIElementBase.IsEnabled"/> false): still drawn, faded by
    /// <see cref="SliderStyle.DisabledOpacity"/>, and ignores the mouse (it is not hit-tested,
    /// so a press falls through to its parent).
    /// </para>
    /// </summary>
    public class UISlider : UIElement<UISlider, UISliderHandler, UISliderRenderer>
    {
        #region Fields

        private float _minimum = SliderDefaults.Minimum;
        private float _maximum = SliderDefaults.Maximum;
        private float _step = SliderDefaults.Step;
        private float _value = SliderDefaults.Minimum;

        #endregion

        #region Properties

        /// <summary>Smallest value (set with <see cref="SetRange"/>).</summary>
        public float Minimum => _minimum;

        /// <summary>Largest value (set with <see cref="SetRange"/>).</summary>
        public float Maximum => _maximum;

        /// <summary>Distance between two offered values (set with <see cref="SetRange"/>).</summary>
        public float Step => _step;

        /// <summary>
        /// The value, always snapped (<see cref="SliderMath.Snap"/>). Setting it only changes what
        /// is shown (no <see cref="UISliderHandler.ValueChanged"/>); the mouse goes through
        /// <see cref="UISliderHandler.SetValue"/>, which does notify.
        /// </summary>
        public float Value
        {
            get => _value;
            set => _value = SliderMath.Snap(value, _minimum, _maximum, _step);
        }

        /// <summary>Formats the value for its text (e.g. <c>v => $"{v} 分"</c>); null = the number itself.</summary>
#nullable enable
        public Func<float, string>? ValueFormatter { get; set; }
#nullable disable

        /// <summary>The value's text at the right of the track.</summary>
        public string ValueText =>
            ValueFormatter?.Invoke(_value) ?? _value.ToString("0.###", CultureInfo.InvariantCulture);

        /// <summary>Font of the value's text (owned by whoever assigns it; never disposed here). No text is drawn without one.</summary>
        public IFont Font { get; set; }

        /// <summary>How the slider is drawn; null = <see cref="SliderDefaults.Style"/>.</summary>
        public SliderStyle Style { get; set; }

        /// <summary>Whether the knob is being dragged (set by the handler).</summary>
        public bool IsDragging { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>Creates a slider over the default range, at its minimum.</summary>
        public UISlider()
            : base(zIndex: 0, isPersistent: false, type: UIElementType.Generic)
        {
        }

        #endregion

        /// <summary>Applies the default size (a layout style replaces it).</summary>
        protected override void OnInit()
        {
            Size = SliderDefaults.Size;
        }

        #region Methods

        /// <summary>Sets the range and step; the value is snapped into the new range.</summary>
        /// <exception cref="ArgumentException"><paramref name="minimum"/> is above <paramref name="maximum"/> (or either is not a finite number).</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="step"/> is not positive.</exception>
        public void SetRange(float minimum, float maximum, float step)
        {
            if (!float.IsFinite(minimum) || !float.IsFinite(maximum) || minimum > maximum)
                throw new ArgumentException($"Invalid range {minimum}..{maximum}.", nameof(minimum));
            if (!(step > 0f))
                throw new ArgumentOutOfRangeException(nameof(step), step, "The step must be positive.");

            _minimum = minimum;
            _maximum = maximum;
            _step = step;
            Value = _value;
        }

        /// <summary>
        /// Where the track lies now (absolute): its left end, width and vertical center. The
        /// track is the slider's width minus the value column and its gap, inset by half the
        /// knob on both ends so the knob stays inside the slider.
        /// </summary>
        public (float Left, float Width, float CenterY) GetTrack()
        {
            var style = Style ?? SliderDefaults.Style;
            var bounds = GetCurrentAbsoluteBounds();
            float radius = Math.Max(0f, style.KnobDiameter) / 2f;
            float label = style.LabelWidth > 0f ? style.LabelWidth + Math.Max(0f, style.LabelGap) : 0f;
            float left = bounds.X + radius;
            float width = Math.Max(0f, bounds.Width - label - radius * 2f);
            return (left, width, bounds.Y + bounds.Height / 2f);
        }

        /// <summary>The offered value at absolute x coordinate <paramref name="x"/> (where a press there puts the knob).</summary>
        public float ValueAt(float x)
        {
            var (left, width, _) = GetTrack();
            return SliderMath.ValueAt(x, left, width, _minimum, _maximum, _step);
        }

        #endregion
    }
}
