/* ----- ----- ----- ----- */
// DebugSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Engine.Configs;

namespace Engine.Diagnostics
{
    /// <summary>
    /// The settings area of the debug features (<c>[debug]</c>; the settings screen's
    /// 開發人員選項 tab): the player's switches, put into effect on the engine's switchboard
    /// (<see cref="DebugOptions"/>), where each debug feature reads its own. The property
    /// initializers are the defaults.
    /// </summary>
    public sealed class DebugSettings : ISettingsArea
    {
        private static readonly DebugSettings Default = new();

        /// <summary>Whether DEBUG-level log lines are written (the log's verbosity); the game's log box draws its lines' red background with it. Default: true</summary>
        public bool VerboseLog { get; set; } = true;

        /// <summary>Whether developer trace lines (UI init / menu selections / network status) are printed to the console. Default: true</summary>
        public bool ConsoleTrace { get; set; } = true;

        /// <summary>Whether labels get a semi-transparent red background (visual debugging; always drawn before the switch existed). Default: true</summary>
        public bool LabelBackgrounds { get; set; } = true;

        /// <summary>Whether text boxes get a grey layout outline (visual debugging; always drawn before the switch existed). Default: true</summary>
        public bool LayoutOutlines { get; set; } = true;

        /// <summary>Whether the star background outlines each effect's area (visual debugging). Default: false</summary>
        public bool StarEffectFrames { get; set; } = false;

        /// <summary>Whether the measured frame rate is shown in the window's top-left corner. Default: false</summary>
        public bool ShowFps { get; set; } = false;

        /// <summary>Whether the network latency is shown under the frame rate (a dash until there is a connection). Default: false</summary>
        public bool ShowNetworkLatency { get; set; } = false;

        private IReadOnlyList<SettingsKey> _keys;

        /// <inheritdoc/>
        public IReadOnlyList<SettingsKey> Keys => _keys ??= new[]
        {
            SettingsKey.Bool("debug", "verbose_log", () => VerboseLog, v => VerboseLog = v, Default.VerboseLog,
                "── 除錯功能（設定畫面的 DEBUG 分頁；改了立即生效）──",
                "紀錄：是否寫出 DEBUG 等級的紀錄（較詳細）；紀錄框的訊息同時畫紅色背景。"),
            SettingsKey.Bool("debug", "console_trace", () => ConsoleTrace, v => ConsoleTrace = v, Default.ConsoleTrace,
                "紀錄：是否在主控台印出開發用的追蹤訊息（介面元件初始化、選單選擇、連線狀態；錯誤訊息不受影響，一律會印）。"),
            SettingsKey.Bool("debug", "label_backgrounds", () => LabelBackgrounds, v => LabelBackgrounds = v, Default.LabelBackgrounds,
                "視覺除錯：文字標籤後面畫半透明紅色背景（看出標籤的範圍；紀錄框的訊息跟著「除錯訊息」）。"),
            SettingsKey.Bool("debug", "layout_outlines", () => LayoutOutlines, v => LayoutOutlines = v, Default.LayoutOutlines,
                "視覺除錯：文字框畫灰色實線外框（看出排版範圍；選單的虛線外框是設計，一律畫）。"),
            SettingsKey.Bool("debug", "star_effect_frames", () => StarEffectFrames, v => StarEffectFrames = v, Default.StarEffectFrames,
                "視覺除錯：星空背景每個特效開始時，用特效的除錯顏色框出它的範圍。"),
            SettingsKey.Bool("debug", "show_fps", () => ShowFps, v => ShowFps = v, Default.ShowFps,
                "效能與連線：在視窗左上角顯示實際量到的畫面更新率（FPS）。"),
            SettingsKey.Bool("debug", "show_network_latency", () => ShowNetworkLatency, v => ShowNetworkLatency = v, Default.ShowNetworkLatency,
                "效能與連線：在 FPS 下面顯示網路延遲（目前還沒有連線功能，只會顯示「—」）。"),
        };

        /// <summary>Pushes the switches into the engine (<see cref="DebugOptions"/>), where the debug features read them.</summary>
        public void Apply()
        {
            DebugOptions.VerboseLog = VerboseLog;
            DebugOptions.ConsoleTrace = ConsoleTrace;
            DebugOptions.LabelBackgrounds = LabelBackgrounds;
            DebugOptions.LayoutOutlines = LayoutOutlines;
            DebugOptions.StarEffectFrames = StarEffectFrames;
            DebugOptions.ShowFps = ShowFps;
            DebugOptions.ShowNetworkLatency = ShowNetworkLatency;
        }
    }
}
