/* ----- ----- ----- ----- */
// IUiFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/10/23
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Interfaces
{
    /// <summary>
    /// Factory interface for creating UI elements and screens.
    /// Provides methods to create scroll containers, resolve dependencies,
    /// instantiate screens with handlers and renderers, and register custom factories.
    /// </summary>
    public interface IUiFactory
    {
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Resolves a registered service or UI element from the DI container.
        /// </summary>
        /// <typeparam name="T">The type of the service to resolve.</typeparam>
        /// <returns>The resolved service instance of type T.</returns>
        T Resolve<T>();

        /// <summary>
        /// Creates a new UIScrollContainer using the registered scroll input handler.
        /// </summary>
        /// <returns>A UIScrollContainer instance.</returns>
        UIScrollContainer CreateScrollContainer();

#nullable enable
        UIButton CreateButton(Action? onClick = null);
#nullable disable

#nullable enable
        public UIButton<TEnum> CreateButton<TEnum>(Action<TEnum>? onClick = null)
            where TEnum : Enum;
#nullable disable

        TElement CreateElement<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>, new()
            where THandler : UIHandler<TElement, THandler, TRenderer>, new()
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>, new();

        TElement CreateDIElement<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>;

        TElement CreateDI<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>;

        void RegisterFactory<T>(Func<IUiFactory, T> factory)
            where T : UIElementBase;

        void RegisterFactory<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>;

        void ClearCache<T>() where T : UIElementBase;

        void ClearCache<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>;

        /// <summary>
        /// Clears all registered factory caches.
        /// </summary>
        void ClearAllCache();
    }
}
