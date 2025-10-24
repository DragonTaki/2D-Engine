/* ----- ----- ----- ----- */
// PaddingF.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Constants.Core
{
    /// <summary>Floating-point padding struct.</summary>
    public struct PaddingF
    {
        public float Left, Top, Right, Bottom;
        public static readonly PaddingF Zero = new(0, 0, 0, 0);

        public PaddingF(float all) => (Left, Top, Right, Bottom) = (all, all, all, all);
        public PaddingF(float left, float top, float right, float bottom)
        {
            Left = left; Top = top; Right = right; Bottom = bottom;
        }
    }
}
