/* ----- ----- ----- ----- */
// UIElementType.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Constants.Components
{
    /// <summary>
    /// Category tag of a UI element (<c>UIElementBase.ElementType</c>), used to filter
    /// children by kind (e.g. <c>UIElement.RemoveAllChild</c>'s only/exclude type lists).
    /// </summary>
    public enum UIElementType
    {
        Generic,
        Root,
        Overlay,
        Button,
        Label,
        ScrollContainer,
        Dialog,
        Mask,
        HUD,
        Popup,
        Debug,
        Piece,
    }
}
