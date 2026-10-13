/* ----- ----- ----- ----- */
// LogLineBackground.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.Logging;

namespace Engine.UI.Utils
{
    /// <summary>Position-based neutral background for all log lines, independent of debug frames.</summary>
    public static class LogLineBackground
    {
        public const float MinimumAlpha = 0.08f;
        public const float MaximumAlpha = 0.55f;

        public static bool IsDebug(LogLevel level) => level == LogLevel.DEBUG;

        /// <summary>Creates a live neutral background for every log record.</summary>
        public static Func<RectangleF, Color?> CreateProvider(Func<RectangleF> viewport)
        {
            return bounds => ColorAt(bounds, viewport());
        }

        /// <summary>Maps a current line center into the current visible area's background opacity.</summary>
        public static float AlphaAt(float labelCenterY, float visibleTop, float visibleHeight)
        {
            if (visibleHeight <= 0f)
                return MinimumAlpha;
            float t = Math.Clamp((labelCenterY - visibleTop) / visibleHeight, 0f, 1f);
            return MinimumAlpha + (MaximumAlpha - MinimumAlpha) * t;
        }

        public static Color ColorAt(RectangleF bounds, RectangleF viewport)
        {
            float alpha = AlphaAt(bounds.Top + bounds.Height / 2f, viewport.Top, viewport.Height);
            return Color.FromArgb((int)Math.Round(alpha * 255f), Color.FromArgb(0x3F, 0x5F, 0x94));
        }
    }
}
