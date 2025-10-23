/* ----- ----- ----- ----- */
// PositionHelper.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Mathematics
{
    public static class PositionHelper
    {
        public static float Distance(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return (float)System.Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
