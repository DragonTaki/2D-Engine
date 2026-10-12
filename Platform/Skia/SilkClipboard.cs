/* ----- ----- ----- ----- */
// SilkClipboard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Silk.NET.GLFW;
using Silk.NET.Input;

using Engine.Logging;

namespace Engine.Platform.Skia
{
    /// <summary>
    /// Silk.NET <see cref="IClipboard"/>: the clipboard text of the window's
    /// <see cref="IKeyboard"/> (GLFW's clipboard, on every system GLFW runs on). The keyboard only
    /// exists once the window has loaded, so the launcher registers this in DI at start-up and
    /// attaches the keyboard later (<see cref="Attach"/>); until then it is empty. Call on the
    /// window's (main) thread, where the keyboard events that paste arrive.
    /// </summary>
    public sealed class SilkClipboard : IClipboard
    {
#nullable enable
        private IKeyboard? _keyboard;

        /// <summary>Uses <paramref name="keyboard"/>'s clipboard from now on (null: none, always empty).</summary>
        public void Attach(IKeyboard? keyboard) => _keyboard = keyboard;
#nullable disable

        /// <inheritdoc/>
        public string GetText()
        {
            if (_keyboard == null)
                return string.Empty;

            try
            {
                // GLFW gives null when the clipboard holds no text (e.g. only an image).
                return _keyboard.ClipboardText ?? string.Empty;
            }
            catch (GlfwException ex)
            {
                AppLogger.Log($"[SilkClipboard] Cannot read the clipboard: {ex.Message}", LogLevel.WARN);
                return string.Empty;
            }
        }
    }
}
