/* ----- ----- ----- ----- */
// LayoutSize.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/29
// Update Date: 2026/09/29
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Globalization;

using Engine.UI.Constants.Core;

namespace Engine.UI.Models
{
    /// <summary>
    /// One axis of a layout-managed element's size: a <see cref="SizeMode"/> plus its value.
    /// <para>
    /// Sizes are border-box: they include the element's own padding, never its margin.
    /// </para>
    /// <para>
    /// <c>default(LayoutSize)</c> (= <see cref="Declared"/>) is <see cref="SizeMode.Fixed"/>
    /// with no value, meaning "the element's declared size" - whatever code last assigned
    /// to <c>Size</c> on that axis - so switching an existing element to a managed
    /// <see cref="PositionMode"/> keeps the size it was authored with.
    /// </para>
    /// </summary>
    public readonly struct LayoutSize : IEquatable<LayoutSize>
    {
        #region Properties

        /// <summary>How this axis is resolved.</summary>
        public SizeMode Mode { get; }

        /// <summary>
        /// <see cref="SizeMode.Fixed"/>: length in design units (null = declared size).
        /// <see cref="SizeMode.Percent"/>: fraction of the parent's content size (0.5 = 50%).
        /// Unused by the other modes.
        /// </summary>
        public float? Value { get; }

        /// <summary>True for the default "use the element's declared Size" value.</summary>
        public bool IsDeclared => Mode == SizeMode.Fixed && Value == null;

        #endregion

        #region Constructors / Factories

        private LayoutSize(SizeMode mode, float? value)
        {
            Mode = mode;
            Value = value;
        }

        /// <summary>The element's declared <c>Size</c> on this axis (the default).</summary>
        public static LayoutSize Declared => default;

        /// <summary>A fixed length in design units.</summary>
        public static LayoutSize Fixed(float length) => new(SizeMode.Fixed, length);

        /// <summary>A fraction of the parent's content size (0.5 = 50%).</summary>
        public static LayoutSize Percent(float fraction) => new(SizeMode.Percent, fraction);

        /// <summary>Fill the available space minus margins.</summary>
        public static LayoutSize Stretch => new(SizeMode.Stretch, null);

        /// <summary>Intrinsic (content) size.</summary>
        public static LayoutSize Auto => new(SizeMode.Auto, null);

        #endregion

        #region Equality

        public bool Equals(LayoutSize other) => Mode == other.Mode && Nullable.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is LayoutSize other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Mode, Value);
        public static bool operator ==(LayoutSize a, LayoutSize b) => a.Equals(b);
        public static bool operator !=(LayoutSize a, LayoutSize b) => !a.Equals(b);

        public override string ToString() => Mode switch
        {
            SizeMode.Fixed => Value.HasValue ? Value.Value.ToString(CultureInfo.InvariantCulture) : "Declared",
            SizeMode.Percent => (Value.GetValueOrDefault() * 100f).ToString(CultureInfo.InvariantCulture) + "%",
            _ => Mode.ToString()
        };

        #endregion
    }
}
