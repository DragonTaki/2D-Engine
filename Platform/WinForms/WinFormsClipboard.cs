/* ----- ----- ----- ----- */
// WinFormsClipboard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Engine.Logging;

namespace Engine.Platform.WinForms
{
    /// <summary>
    /// WinForms <see cref="IClipboard"/>: the Windows clipboard's Unicode text through
    /// <see cref="Clipboard"/>. Must be called on the UI (STA) thread, where the keyboard events
    /// that paste arrive.
    /// </summary>
    public sealed class WinFormsClipboard : IClipboard
    {
        /// <inheritdoc/>
        public string GetText()
        {
            try
            {
                return Clipboard.GetText(TextDataFormat.UnicodeText) ?? string.Empty;
            }
            catch (ExternalException ex)
            {
                // Another program holds the clipboard open.
                AppLogger.Log($"[WinFormsClipboard] Cannot read the clipboard: {ex.Message}", LogLevel.WARN);
                return string.Empty;
            }
            catch (ThreadStateException ex)
            {
                AppLogger.Log($"[WinFormsClipboard] Not on an STA thread: {ex.Message}", LogLevel.WARN);
                return string.Empty;
            }
        }
    }
}
