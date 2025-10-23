/* ----- ----- ----- ----- */
// UiFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/10/23
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Engine.UI.Core.Base;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;
using Engine.UI.Input;

using Microsoft.Extensions.DependencyInjection;

namespace Engine.UI.Core.Infrastructure
{
    /// <summary>
    /// Central factory responsible for creating UI elements, screens, and scroll containers.
    /// <para>
    /// Supports both context-based creation (with IUiFactoryContext) and simple DI-based creation.
    /// Can register custom factories for any UIElement-derived type.
    /// </para>
    /// </summary>
    public class UiFactory : IUiFactory
    {
        private readonly IServiceProvider _sp;

        // Dictionary storing registered factories for type T without context
        private readonly Dictionary<Type, Func<IUiFactory, UIElement>> _factories = new();

        // Dictionary storing registered factories for type T with context
        private readonly Dictionary<Type, Func<IUiFactoryContext, UIElement>> _factoriesWithContext = new();

        /// <summary>
        /// Initializes a new instance of <see cref="UiFactory"/> using the specified service provider.
        /// </summary>
        /// <param name="sp">The dependency injection service provider.</param>
        public UiFactory(IServiceProvider sp)
        {
            _sp = sp;
        }

        /// <summary>
        /// Resolves an instance of the requested type T from the service provider.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>An instance of type T.</returns>
        public T Resolve<T>() => _sp.GetRequiredService<T>();

        /// <summary>
        /// Creates a scrollable container using the registered <see cref="IScrollInputHandler"/>.
        /// </summary>
        /// <returns>A new <see cref="UIScrollContainer"/> instance.</returns>
        public UIScrollContainer CreateScrollContainer()
        {
            var scroll = _sp.GetRequiredService<IScrollInputHandler>();
            return new UIScrollContainer(scroll);
        }

        /// <summary>
        /// Creates a UI screen, automatically registering a generic factory if necessary.
        /// </summary>
        /// <typeparam name="TScreen">The screen type to create.</typeparam>
        /// <typeparam name="THandler">The handler type associated with the screen.</typeparam>
        /// <typeparam name="TRenderer">The renderer type associated with the screen.</typeparam>
        /// <returns>An initialized screen of type TScreen.</returns>
        public TScreen CreateScreen<TScreen, THandler, TRenderer>()
            where TScreen : InitializableOnceElement<(IUiFactory, THandler, TRenderer)>
            where THandler : class
            where TRenderer : class
        {
            var type = typeof(TScreen);

            // If factory is not registered, auto-register generic factory
            if (!_factoriesWithContext.ContainsKey(type))
            {
                RegisterFactory<TScreen, THandler, TRenderer>();
            }

            // Use generic factory to create
            return Create<TScreen, THandler, TRenderer>();
        }

        /// <summary>
        /// Creates and initializes a screen along with its handler and renderer.
        /// </summary>
        /// <typeparam name="TScreen">The screen type.</typeparam>
        /// <typeparam name="THandler">The handler type.</typeparam>
        /// <typeparam name="TRenderer">The renderer type.</typeparam>
        /// <returns>An initialized screen instance.</returns>
        public TScreen Create<TScreen, THandler, TRenderer>()
            where TScreen : InitializableOnceElement<(IUiFactory, THandler, TRenderer)>
            where THandler : class
            where TRenderer : class
        {
            var screen = _sp.GetRequiredService<TScreen>();
            var handler = _sp.GetRequiredService<THandler>();
            var renderer = _sp.GetRequiredService<TRenderer>();

            // Initialize handler if it implements IInitializableOnce
            if (handler is IInitializableOnce<(IUiFactory, TScreen)> hInit)
                hInit.Init((this, screen));

            // Initialize screen with factory, handler, and renderer
            screen.Init((this, handler, renderer));

            return screen;
        }
        
        /// <summary>
        /// Registers a factory method for creating a UIElement of type T using this factory.
        /// </summary>
        /// <typeparam name="T">The UIElement type to register.</typeparam>
        /// <param name="factory">The factory method accepting an IUiFactory and returning an instance of T.</param>
        public void RegisterFactory<T>(Func<IUiFactory, T> factory) where T : UIElement
        {
            _factories[typeof(T)] = factory;
        }

        /// <summary>
        /// Registers a factory method for creating a UIElement of type T using a context.
        /// </summary>
        /// <typeparam name="T">The UIElement type to register.</typeparam>
        /// <param name="factory">The factory method accepting an IUiFactoryContext and returning an instance of T.</param>
        public void RegisterFactory<T>(Func<IUiFactoryContext, T> factory) where T : UIElement
        {
            _factoriesWithContext[typeof(T)] = ctx => factory(ctx);
        }

        /// <summary>
        /// Removes a previously registered factory for the specified UIElement type T.
        /// </summary>
        /// <typeparam name="T">The UIElement type to clear.</typeparam>
        public void ClearCache<T>() where T : UIElement
        {
            _factories.Remove(typeof(T));
        }

        /// <summary>
        /// Clears all registered factories.
        /// </summary>
        public void ClearAllCache()
        {
            _factories.Clear();
        }

        /// <summary>
        /// Registers a default factory for a screen, handler, and renderer combination.
        /// </summary>
        private void RegisterFactory<TScreen, THandler, TRenderer>()
            where TScreen : InitializableOnceElement<(IUiFactory, THandler, TRenderer)>
            where THandler : class
            where TRenderer : class
        {
            _factoriesWithContext[typeof(TScreen)] = ctx =>
            {
                var screen = ctx.ServiceProvider.GetRequiredService<TScreen>();
                var handler = ctx.ServiceProvider.GetRequiredService<THandler>();
                var renderer = ctx.ServiceProvider.GetRequiredService<TRenderer>();

                // Initialize handler if it implements IInitializableOnce
                if (handler is IInitializableOnce<(IUiFactory, TScreen)> hInit)
                    hInit.Init((ctx.UiFactory, screen));

                // Initialize screen with factory, handler, and renderer
                screen.Init((ctx.UiFactory, handler, renderer));
                return screen;
            };
        }

        /// <summary>
        /// Internal implementation of <see cref="IUiFactoryContext"/> providing service provider and factory.
        /// </summary>
        private class UiFactoryContext : IUiFactoryContext
        {
            /// <summary>
            /// Gets the service provider.
            /// </summary>
            public IServiceProvider ServiceProvider { get; }

            /// <summary>
            /// Gets the UI factory.
            /// </summary>
            public IUiFactory UiFactory { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="UiFactoryContext"/> class.
            /// </summary>
            /// <param name="sp">The service provider.</param>
            /// <param name="factory">The UI factory.</param>
            public UiFactoryContext(IServiceProvider sp, IUiFactory factory)
            {
                ServiceProvider = sp;
                UiFactory = factory;
            }
        }
    }
}
