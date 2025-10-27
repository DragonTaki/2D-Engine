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

using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
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
        public IServiceProvider ServiceProvider => _sp;

        // Dictionary storing registered factories for type T without context
        private readonly Dictionary<Type, Func<IUiFactory, UIElementBase>> _factories = new();

        // Dictionary storing registered factories for type T with context
        private readonly Dictionary<Type, Func<IUiFactoryContext, UIElementBase>> _factoriesWithContext = new();

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

            var handler = new UIScrollContainerHandler();
            var renderer = new UIScrollContainerRenderer();
            var element = new UIScrollContainer(scroll);

            element.Init(this, handler, renderer);

            return element;
        }

#nullable enable
        public UIButton CreateButton(Action? onClick = null)
#nullable disable
        {
            // 建立 Handler / Renderer
            var handler = new UIButtonHandler();
            var renderer = new UIButtonRenderer();

            // 建立 UIButton
            var button = new UIButton();

            // 初始化綁定
            button.Init(this, handler, renderer);

            // 綁定點擊事件
            button.Handler.Action = onClick;

            return button;
        }

#nullable enable
        public UIButton<TEnum> CreateButton<TEnum>(Action<TEnum>? onClick = null)
            where TEnum : Enum
#nullable disable
        {
            var button = new UIButton<TEnum>();
            var handler = new UIButtonHandler<TEnum>();
            var renderer = new UIButtonRenderer<TEnum>();

            handler.Init(this, button);
            renderer.Init(button);
            button.Init(this, handler, renderer);

            handler.Action = onClick; // 只綁定 Action，其他屬性由外部設定
            return button;
        }

        public TElement CreateElement<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>, new()
            where THandler : UIHandler<TElement, THandler, TRenderer>, new()
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>, new()
        {
            var element = new TElement();
            Console.WriteLine($"Factory not DI: {element.GetType().FullName}");
            var handler = new THandler();
            var renderer = new TRenderer();

            handler.Init(this, element);
            renderer.Init(element);
            element.Init(this, handler, renderer);

            return element;
        }

        public TElement CreateDIElement<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>
        {
            var type = typeof(TElement);

            // 如果尚未註冊 factory，自動註冊
            if (!_factoriesWithContext.ContainsKey(type))
            {
                RegisterFactory<TElement, THandler, TRenderer>();
            }

            // 用統一的三參數 Create
            return CreateDI<TElement, THandler, TRenderer>();
        }

        public TElement CreateDI<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>
        {
            var element = _sp.GetRequiredService<TElement>();
            var handler = _sp.GetRequiredService<THandler>();
            var renderer = _sp.GetRequiredService<TRenderer>();

            // 直接初始化，不用 interface
            handler.Init(this, element);
            renderer.Init(element);
            element.Init(this, handler, renderer);

            return element;
        }

        /// <summary>
        /// Registers a factory method for creating a UIElement of type T using this factory.
        /// </summary>
        /// <typeparam name="T">The UIElement type to register.</typeparam>
        /// <param name="factory">The factory method accepting an IUiFactory and returning an instance of T.</param>
        public void RegisterFactory<T>(Func<IUiFactory, T> factory)
            where T : UIElementBase
        {
            _factories[typeof(T)] = factory;
        }

        /// <summary>
        /// Registers a default factory for a element, handler, and renderer combination.
        /// </summary>
        public void RegisterFactory<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>
        {
            _factoriesWithContext[typeof(TElement)] = ctx =>
            {
                // 從 DI 容器取得實例
                var element = ctx.ServiceProvider.GetRequiredService<TElement>();
                var handler = ctx.ServiceProvider.GetRequiredService<THandler>();
                var renderer = ctx.ServiceProvider.GetRequiredService<TRenderer>();

                // 初始化 Handler
                handler.Init(ctx.UiFactory, element);

                // 初始化 Renderer
                renderer.Init(element);

                // 初始化 Element，綁定 Handler + Renderer
                element.Init(ctx.UiFactory, handler, renderer);

                return element;
            };
        }

        /// <summary>
        /// Removes a previously registered factory for the specified UIElement type T.
        /// </summary>
        /// <typeparam name="T">The UIElement type to clear.</typeparam>
        public void ClearCache<T>() where T : UIElementBase
        {
            _factories.Remove(typeof(T));
        }

        /// <summary>
        /// Removes a registered factory for a fully-typed UIElement.
        /// </summary>
        public void ClearCache<TElement, THandler, TRenderer>()
            where TElement : UIElement<TElement, THandler, TRenderer>
            where THandler : UIHandler<TElement, THandler, TRenderer>
            where TRenderer : UIRenderer<TElement, THandler, TRenderer>
        {
            _factoriesWithContext.Remove(typeof(TElement));
        }

        /// <summary>
        /// Clears all registered factories.
        /// </summary>
        public void ClearAllCache()
        {
            _factories.Clear();
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
