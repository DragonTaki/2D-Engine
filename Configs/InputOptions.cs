/* ----- ----- ----- ----- */
// InputOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Configs
{
    /// <summary>
    /// The engine's input values in effect, read where input is handled (the scroll handler);
    /// set by <see cref="InputSettings.Apply"/> from the player's settings.
    /// </summary>
    public static class InputOptions
    {
        /// <summary>Scroll distance per mouse-wheel notch, in UI design units (30 until settings are applied).</summary>
        public static float WheelScrollStep { get; set; } = 30f;
    }
}
