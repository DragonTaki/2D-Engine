/* ----- ----- ----- ----- */
// TextFieldStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Styles
{
    /// <summary>
    /// Looks of a one-line text field (<c>UITextField</c>): its box (another one while it has
    /// the keyboard focus), the text, placeholder and caret colours, and the text's inset.
    /// </summary>
    public class TextFieldStyle
    {
        /// <summary>The box while the field is not focused; null = no box.</summary>
        public IBoxDrawStyle Box { get; set; }

        /// <summary>The box while the field has the keyboard focus; null = <see cref="Box"/>.</summary>
        public IBoxDrawStyle FocusedBox { get; set; }

        /// <summary>Colour of the text.</summary>
        public Color TextColor { get; set; }

        /// <summary>Colour of the placeholder (shown while the field is empty and not focused).</summary>
        public Color PlaceholderColor { get; set; }

        /// <summary>Colour of the caret.</summary>
        public Color CaretColor { get; set; }

        /// <summary>Width of the caret line (design units).</summary>
        public float CaretWidth { get; set; }

        /// <summary>Horizontal inset of the text from the box's left and right edges (design units).</summary>
        public float TextInset { get; set; }

        /// <summary>Opacity (0 to 1) the text colours are multiplied by while the field is disabled.</summary>
        public float DisabledOpacity { get; set; }
    }
}
