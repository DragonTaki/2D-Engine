/* ----- ----- ----- ----- */
// UIStyledButton.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.Styles;

namespace Engine.UI.Core.Elements
{
    /// <summary>A self-drawing button for standalone layouts; existing parent-drawn buttons are unchanged.</summary>
    public class UIStyledButton : UIButton
    {
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }

        public override void Draw(IGraphics g)
        {
            UpdateLayout();
            if (DisableRender || IsDisposed)
                return;
            var style = Style ?? DefaultStyles.DefaultButtonStyle;
            if (style is IStatefulButtonDrawStyle stateful)
                stateful.Draw(g, Text, GetCurrentAbsoluteBounds(), new ButtonVisualState(IsEnabled, IsHovered, IsPressed));
            else
                style?.Draw(g, Text, GetCurrentAbsoluteBounds());
            base.Draw(g);
        }
    }
}
