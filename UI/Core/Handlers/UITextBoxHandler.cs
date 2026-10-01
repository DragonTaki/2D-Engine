/* ----- ----- ----- ----- */
// UITextBoxHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/25
// Update Date: 2025/10/25
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Handlers
{
    public abstract class UITextBoxHandler<TElement, THandler, TRenderer> : UIContainerHandler<TElement, THandler, TRenderer>
        where TElement : UITextBox<TElement, THandler, TRenderer>
        where THandler : UITextBoxHandler<TElement, THandler, TRenderer>
        where TRenderer : UITextBoxRenderer<TElement, THandler, TRenderer>
    {
        private UITextBox<TElement, THandler, TRenderer> TextBox => (UITextBox<TElement, THandler, TRenderer>)Element;
        public event Action<string> OnMessageAdded;

        /// <summary>
        /// Adds a message using the text box's default text color and plain style.
        /// </summary>
        public void AddMessage(string msg)
        {
            AddMessage(msg, TextBox.TextColor, bold: false, italic: false);
        }

        /// <summary>
        /// Adds a message with a custom text color and style.
        /// </summary>
        public void AddMessage(string msg, Color color, bool bold = false, bool italic = false)
        {
            // Append the message as a line of text to the UITextBox (not wrapped in a TextFragment)
            TextBox.AppendLine(msg, color, bold, italic);

            // Keep the original event notification
            OnMessageAdded?.Invoke(msg);
        }

        /// <summary>
        /// Adds a multi-colored message (a sequence of text fragments shown as one line).
        /// </summary>
        public void AddMessage(IEnumerable<TextFragment> fragments)
        {
            // Materialize once: the sequence used to be enumerated twice (a one-shot
            // iterator would log an empty message on the second pass).
            var list = fragments?.ToList() ?? new List<TextFragment>();

            // One message = one line of inline runs (it used to become one line per fragment).
            TextBox.AppendFragmentLine(list);

            // TODO: The event currently carries the plain concatenated text; it could pass the fragments instead.
            OnMessageAdded?.Invoke(string.Concat(list.Select(f => f.Text)));
        }
    }
}
