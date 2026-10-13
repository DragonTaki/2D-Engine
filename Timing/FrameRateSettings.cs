/* ----- ----- ----- ----- */
// FrameRateSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/12
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Engine.Configs;

namespace Engine.Timing
{
    /// <summary>
    /// The settings area of the animation frame rate (<c>[display] fps</c>), put into effect on
    /// the engine's frame timer (<see cref="TimerSettings.GameAnimationFPS"/>).
    /// </summary>
    public sealed class FrameRateSettings : ISettingsArea
    {
        private static readonly FrameRateSettings Default = new();

        /// <summary>The frame rates <see cref="Fps"/> can be (the settings screen's FPS choices).</summary>
        public static readonly IReadOnlyList<int> FpsOptions = new[] { 30, 60, 120, 144, 0 };

        /// <summary>Animation frame rate (one of <see cref="FpsOptions"/>). Default: 60</summary>
        public int Fps { get; set; } = 60;

        /// <summary>Temporary safety limit for the unlimited frame-rate choice.</summary>
        public const int UnlimitedFrameRateLimit = 200;

        /// <summary>The runtime frame-rate target; zero is the persisted unlimited choice.</summary>
        public int EffectiveFps => Fps == 0 ? UnlimitedFrameRateLimit : Fps;

        /// <summary>Whether GPU presentation is synchronized with the display. Default: true</summary>
        public bool VSync { get; set; } = true;

        private IReadOnlyList<SettingsKey> _keys;

        /// <inheritdoc/>
        public IReadOnlyList<SettingsKey> Keys => _keys ??= new[]
        {
            SettingsKey.IntOneOf("display", "fps", () => Fps, v => Fps = v, Default.Fps, FpsOptions,
                "畫面更新率（每秒幾格動畫）：30／60／120／144／無限（0）；無限目前使用 200 的保護上限。垂直同步開啟時最多到螢幕更新率；實際更新率仍受平台與繪製負載限制。"),
            SettingsKey.Bool("display", "vsync", () => VSync, v => VSync = v, Default.VSync,
                "垂直同步：跨平台 GPU 版將畫面呈現與螢幕更新同步；WinForms 版不支援此選項。"),
        };

        /// <summary>Sets the engine's frame timer to <see cref="Fps"/>.</summary>
        public void Apply()
        {
            TimerSettings.GameAnimationFPS = EffectiveFps;
            TimerSettings.VSync = VSync;
        }
    }
}
