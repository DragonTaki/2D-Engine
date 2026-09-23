/* ----- ----- ----- ----- */
// WinFormsStringFormat.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Platform.WinForms
{
    /// <summary>GDI+-backed <see cref="IStringFormat"/>.</summary>
    internal sealed class WinFormsStringFormat : IStringFormat
    {
        public StringFormat Native { get; }

        public WinFormsStringFormat(StringFormat native)
        {
            Native = native;
        }

        private static StringAlignment ToNative(TextAlign align) => align switch
        {
            TextAlign.Near => StringAlignment.Near,
            TextAlign.Far => StringAlignment.Far,
            _ => StringAlignment.Center,
        };

        private static TextAlign FromNative(StringAlignment align) => align switch
        {
            StringAlignment.Near => TextAlign.Near,
            StringAlignment.Far => TextAlign.Far,
            _ => TextAlign.Center,
        };

        public TextAlign Alignment
        {
            get => FromNative(Native.Alignment);
            set => Native.Alignment = ToNative(value);
        }

        public TextAlign LineAlignment
        {
            get => FromNative(Native.LineAlignment);
            set => Native.LineAlignment = ToNative(value);
        }

        public bool WordWrap
        {
            get => (Native.FormatFlags & StringFormatFlags.NoWrap) == 0;
            set => Native.FormatFlags = value ? Native.FormatFlags & ~StringFormatFlags.NoWrap : Native.FormatFlags | StringFormatFlags.NoWrap;
        }

        public bool EllipsisTrimming
        {
            get => Native.Trimming == StringTrimming.EllipsisCharacter;
            set => Native.Trimming = value ? StringTrimming.EllipsisCharacter : StringTrimming.None;
        }

        public void Dispose() => Native.Dispose();
    }
}
