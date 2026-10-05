/* ----- ----- ----- ----- */
// InputSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

namespace Engine.Configs
{
    /// <summary>The settings area of input (<c>[input]</c>), put into effect on <see cref="InputOptions"/>.</summary>
    public sealed class InputSettings : ISettingsArea
    {
        private static readonly InputSettings Default = new();

        /// <summary>Smallest / largest <see cref="WheelScrollStep"/>.</summary>
        public const float WheelScrollStepMin = 1f, WheelScrollStepMax = 500f;

        /// <summary>Scroll distance per mouse-wheel notch, in UI design units (<see cref="WheelScrollStepMin"/>-<see cref="WheelScrollStepMax"/>). Default: 30</summary>
        public float WheelScrollStep { get; set; } = 30f;

        private IReadOnlyList<SettingsKey> _keys;

        /// <inheritdoc/>
        public IReadOnlyList<SettingsKey> Keys => _keys ??= new[]
        {
            SettingsKey.Float("input", "wheel_scroll_step", () => WheelScrollStep, v => WheelScrollStep = v, Default.WheelScrollStep,
                WheelScrollStepMin, WheelScrollStepMax,
                $"滑鼠滾輪每一格捲動的距離（介面設計單位，{WheelScrollStepMin}～{WheelScrollStepMax}）。"),
        };

        /// <summary>Sets <see cref="InputOptions.WheelScrollStep"/>.</summary>
        public void Apply() => InputOptions.WheelScrollStep = WheelScrollStep;
    }
}
