/* ----- ----- ----- ----- */
// ISettingsArea.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

namespace Engine.Configs
{
    /// <summary>
    /// One area of the player's settings (e.g. the debug switches, the frame rate, a game's
    /// rules): it holds its values (starting at its defaults), lists its keys in the settings
    /// file, and puts its values into effect. Each area lives in the layer it belongs to; the
    /// settings file (<see cref="SettingsFile"/>, the single entry that reads and writes the file)
    /// hands each registered area its values.
    /// </summary>
    public interface ISettingsArea
    {
        /// <summary>The area's keys, in file order, bound to its values.</summary>
        IReadOnlyList<SettingsKey> Keys { get; }

        /// <summary>Puts the current values into effect (called after the file is read and after every change).</summary>
        void Apply();
    }
}
