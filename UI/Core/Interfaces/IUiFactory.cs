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
        /// <summary>
        /// The dependency injection container the factory resolves from.
        /// </summary>
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
        /// <summary>
        /// Creates and initializes a plain button.
        /// </summary>
        /// <param name="onClick">Action run when the button is clicked, or null.</param>
        /// <returns>The initialized button.</returns>
        UIButton CreateButton(Action? onClick = null);
#nullable disable

#nullable enable
        /// <summary>
        /// Creates and initializes a button whose click action receives the button's
        /// <see cref="UIButton{TEnum}.Type"/> (set it on the returned button).
        /// </summary>
        /// <typeparam name="TEnum">The enum type identifying the button.</typeparam>
        /// <param name="onClick">Typed action run when the button is clicked, or null.</param>
        /// <returns>The initialized button.</returns>
        UIButton<TEnum> CreateButton<TEnum>(Action<TEnum>? onClick = null)
            where TEnum : Enum;
#nullable disable

        /// <summary>
        /// Creates an element with <c>new()</c> (no DI), binds a new handler and renderer to it and initializes it.
        /// </summary>
        /// <returns>The initialized element.</returns>
        TElement CreateElement<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>, new()
            where THandler : UIHandler<TElement, THandler, TRenderer>, new()
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>, new();

        /// <summary>
        /// Creates an element through a registered factory: a custom one from
        /// <see cref="RegisterFactory{T}(Func{IUiFactory, T})"/> if present, otherwise the
        /// default DI factory (registered on first use).
        /// </summary>
        /// <returns>The initialized element.</returns>
        TElement CreateDIElement<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>;

        /// <summary>
        /// Resolves the element, handler and renderer from the DI container and initializes them, bypassing registered factories.
        /// </summary>
        /// <returns>The initialized element.</returns>
        TElement CreateDI<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>;

        /// <summary>
        /// Registers a custom creation function for element type <typeparamref name="T"/>,
        /// used by <c>CreateDIElement</c> in preference to the default DI factory.
        /// </summary>
        /// <param name="factory">Creates an initialized element using this factory.</param>
        void RegisterFactory<T>(Func<IUiFactory, T> factory)
            where T : UIElementBase;

        /// <summary>
        /// Registers the default DI factory for an element/handler/renderer combination.
        /// </summary>
        void RegisterFactory<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>;

        /// <summary>
        /// Removes the custom factory registered for <typeparamref name="T"/>.
        /// </summary>
        void ClearCache<T>() where T : UIElementBase;

        /// <summary>
        /// Removes the default DI factory registered for <typeparamref name="TElement"/>.
        /// </summary>
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
