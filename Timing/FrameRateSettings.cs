/* ----- ----- ----- ----- */
// FrameRateSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
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
        public static readonly IReadOnlyList<int> FpsOptions = new[] { 30, 60, 120, 144 };

        /// <summary>Animation frame rate (one of <see cref="FpsOptions"/>). Default: 60</summary>
        public int Fps { get; set; } = 60;

        private IReadOnlyList<SettingsKey> _keys;

        /// <inheritdoc/>
        public IReadOnlyList<SettingsKey> Keys => _keys ??= new[]
        {
            SettingsKey.IntOneOf("display", "fps", () => Fps, v => Fps = v, Default.Fps, FpsOptions,
                $"畫面更新率（每秒幾格動畫）：{string.Join("／", FpsOptions)}。開著垂直同步時最多到螢幕更新率；WinForms 版的計時器實際上大約只到 64。"),
        };

        /// <summary>Sets the engine's frame timer to <see cref="Fps"/>.</summary>
        public void Apply() => TimerSettings.GameAnimationFPS = Fps;
    }
}
