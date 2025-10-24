/* ----- ----- ----- ----- */
// IUiFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/10/23
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Base;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Infrastructure;

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
        /// Creates a new UIScrollContainer using the registered scroll input handler.
        /// </summary>
        /// <returns>A UIScrollContainer instance.</returns>
        UIScrollContainer CreateScrollContainer();

        /// <summary>
        /// Resolves a registered service or UI element from the DI container.
        /// </summary>
        /// <typeparam name="T">The type of the service to resolve.</typeparam>
        /// <returns>The resolved service instance of type T.</returns>
        T Resolve<T>();

        /// <summary>
        /// Creates an InitializableOnceElement screen with its handler and renderer.
        /// Registers a default factory if none exists for this screen type.
        /// </summary>
        /// <typeparam name="TScreen">The screen type to create.</typeparam>
        /// <typeparam name="THandler">The handler type associated with the screen.</typeparam>
        /// <typeparam name="TRenderer">The renderer type associated with the screen.</typeparam>
        /// <returns>The instantiated and initialized screen of type TScreen.</returns>
        TScreen CreateScreen<TScreen, THandler, TRenderer>()  //OLD
            where TScreen : UIElement, IInitializableOnce<(IUiFactory, THandler, TRenderer)>
            where THandler : class
            where TRenderer : class;
        TContainer CreateScreen<TContainer, THandler>()
            where TContainer : UIContainer<THandler>
            where THandler : UIContainerHandler<THandler>;

        /// <summary>
        /// Creates an InitializableOnceElement screen with its handler and renderer.
        /// Does not auto-register a factory; uses the registered or DI-provided instances.
        /// </summary>
        /// <typeparam name="TScreen">The screen type to create.</typeparam>
        /// <typeparam name="THandler">The handler type associated with the screen.</typeparam>
        /// <typeparam name="TRenderer">The renderer type associated with the screen.</typeparam>
        /// <returns>The instantiated and initialized screen of type TScreen.</returns>
        TContainer Create<TContainer, THandler>()
            where TContainer : UIContainer<THandler>
            where THandler : UIContainerHandler<THandler>;
        TScreen CreateOLD<TScreen, THandler, TRenderer>()  //OLD
            where TScreen : UIElement, IInitializableOnce<(IUiFactory, THandler, TRenderer)>
            where THandler : class
            where TRenderer : class;

        /// <summary>
        /// Registers a custom factory function for a specific UIElement type.
        /// The factory can take a UI factory context as input and return a UIElement instance.
        /// </summary>
        /// <typeparam name="T">The type of UIElement to register the factory for.</typeparam>
        /// <param name="factory">The factory function to create the UIElement.</param>
        void RegisterFactory<T>(Func<IUiFactoryContext, T> factory) where T : UIElement;  //OLD

        /// <summary>
        /// Clears the cached factory for the specified UIElement type.
        /// </summary>
        /// <typeparam name="T">The type of UIElement to clear from cache.</typeparam>
        void ClearCache<T>() where T : UIElement;

        /// <summary>
        /// Clears all registered factory caches.
        /// </summary>
        void ClearAllCache();
    }
}
