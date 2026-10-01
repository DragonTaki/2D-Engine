/* ----- ----- ----- ----- */
// EngineSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Platform;

namespace Engine.Configs
{
    public static class EngineSettings
    {
        // Default ScrollTextBox font.
        // Created on first use rather than in the static initializer: touching any member of
        // this class before a launcher set GraphicsBackend.Factory would otherwise throw
        // TypeInitializationException (and make the whole class unusable afterwards).
        public static IFont DefaultScrollTextFont
        {
            get => _defaultScrollTextFont ??=
                GraphicsBackend.Factory.CreateFont(GraphicsBackend.Factory.GetSystemFontFamily("Consolas"), 12f);
            set => _defaultScrollTextFont = value;
        }
        private static IFont _defaultScrollTextFont;

        // Default ScrollTextBox line height.
        public static float DefaultScrollTextLineHeight { get; set; } = 18f;

        // Default ScrollTextBox background color.
        public static Color DefaultScrollTextBackground { get; set; } = Color.Black;

        // Default ScrollTextBox text color.
        public static Color DefaultScrollTextColor { get; set; } = Color.White;
    }
}
