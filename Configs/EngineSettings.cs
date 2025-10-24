/* ----- ----- ----- ----- */
// EngineSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Configs
{
    public static class EngineSettings
    {
        // ScrollTextBox 預設字型
        public static Font DefaultScrollTextFont { get; set; } = new Font("Consolas", 12f);

        // ScrollTextBox 預設行高
        public static float DefaultScrollTextLineHeight { get; set; } = 18f;

        // ScrollTextBox 預設背景顏色
        public static Color DefaultScrollTextBackground { get; set; } = Color.Black;

        // ScrollTextBox 預設文字顏色
        public static Color DefaultScrollTextColor { get; set; } = Color.White;
    }
}
