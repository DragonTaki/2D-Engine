/* ----- ----- ----- ----- */
// DebugOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

namespace Engine.Diagnostics
{
    /// <summary>
    /// The switchboard of every debug feature (除錯功能): one plain switch per feature, read
    /// right where the feature runs, so a switch that is off means the feature is not executed
    /// or drawn at all. Engine owns it because most debug output lives in Engine (renderers,
    /// logger, element init traces) and Engine must not read Game's settings; the app's
    /// composition root (the launchers) and the settings screen push the player's
    /// <c>[debug]</c> settings in. A new debug feature gets its own switch here (and a row on
    /// the settings screen's DEBUG tab) instead of running unconditionally.
    /// <para>
    /// The initial values are what runs before any settings are pushed (nothing extra):
    /// every switch off. Genuine error output (missing font, network or logger failures) is
    /// not a debug feature and is never gated by these switches.
    /// </para>
    /// </summary>
    public static class DebugOptions
    {
        #region 紀錄 (logging)

        /// <summary>Whether DEBUG-level log lines are written (<c>AppLogger</c>; the log's verbosity). The game's log box also draws its lines' red background with it.</summary>
        public static bool VerboseLog { get; set; } = false;

        /// <summary>
        /// Whether developer trace lines are printed to the console: UI element init / type
        /// traces, the menus' "selected" lines, the placeholder menu buttons' click lines and
        /// the network layer's status lines (not its errors).
        /// </summary>
        public static bool ConsoleTrace { get; set; } = false;

        #endregion

        #region 視覺除錯 (visual debugging)

        /// <summary>Whether a label's text gets a semi-transparent red background (shows the label's bounds); a label with its own <c>DebugBackgroundSwitch</c> (the log box's lines) follows that instead.</summary>
        public static bool LabelBackgrounds { get; set; } = false;

        /// <summary>Whether text boxes get a grey solid outline (shows their layout bounds; a menu's dashed outline is design and always drawn).</summary>
        public static bool LayoutOutlines { get; set; } = false;

        /// <summary>Whether the star background outlines the area of each effect it starts (in the effect's debug colour).</summary>
        public static bool StarEffectFrames { get; set; } = false;

        #endregion

        #region 效能與連線 (performance and network)

        /// <summary>Whether the measured frame rate is drawn in the window's top-left corner.</summary>
        public static bool ShowFps { get; set; } = false;

        /// <summary>
        /// Whether the network latency is drawn under the frame rate. Shows a dash until the
        /// network layer reports a latency (there is no live connection yet).
        /// </summary>
        public static bool ShowNetworkLatency { get; set; } = false;

        #endregion
    }
}
