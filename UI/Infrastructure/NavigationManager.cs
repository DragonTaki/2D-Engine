/* ----- ----- ----- ----- */
// NavigationManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/18
// Update Date: 2025/10/23
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Engine.UI.Core.Infrastructure
{
    /// <summary>
    /// NavigationManager 負責管理遊戲中各個主要UI畫面的切換。
    /// 它操作一個 Root UIElement 容器，
    /// 可以 Clear 舊畫面、Add 新畫面。
    /// </summary>
    public class NavigationManager
    {
        private readonly IUiFactory _factory;
        private UIElement _rootElement;

        // 儲存已建立的畫面
        private readonly Dictionary<Type, UIElementBase> _screens = new();

        public NavigationManager(IUiFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public void Init(UIElement root)
        {
            _rootElement = root ?? throw new ArgumentNullException(nameof(root));
        }

        /// <summary>
        /// 預先註冊畫面（可選）
        /// </summary>
        public void RegisterScreen<TScreen>(TScreen instance) where TScreen : UIElement
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var type = typeof(TScreen);
            if (_screens.ContainsKey(type))
                throw new InvalidOperationException($"Screen of type {type.Name} already registered.");

            _screens[type] = instance;

            // 如果有 Root，直接加入
            if (_rootElement != null && !_rootElement.Children.Contains(instance))
                _rootElement.AddChild(instance);

            instance.IsVisible = false; // 初始隱藏
        }

        /// <summary>
        /// 顯示指定畫面，支援延遲建立與重建
        /// </summary>
        public TScreen Show<TScreen, THandler, TRenderer>(bool forceReload = false)
            where TScreen : UIElement<TScreen, THandler, TRenderer>
            where THandler : UIHandler<TScreen, THandler, TRenderer>
            where TRenderer : UIRenderer<TScreen, THandler, TRenderer>
        {
            if (_rootElement == null)
                throw new InvalidOperationException("NavigationManager not initialized with root element.");

            ClearNonPersistentChildren(_rootElement);

            var screenType = typeof(TScreen);

            if (forceReload)
            {
                // 強制重建：卸載舊的
                UnloadScreen<TScreen>();
            }

            if (!_screens.TryGetValue(screenType, out UIElementBase screen))
            {
                // 延遲建立：透過工廠建立 screen + handler + renderer
                screen = _factory.CreateDIElement<TScreen, THandler, TRenderer>();
                _screens[screenType] = screen;
            }

            screen.IsVisible = true;

            if (!_rootElement.Children.Contains(screen))
                _rootElement.AddChild(screen);

            return (TScreen)screen;
        }

        /// <summary>
        /// 卸載指定畫面，釋放資源
        /// </summary>
        public void UnloadScreen<TScreen>() where TScreen : UIElementBase
        {
            var screenType = typeof(TScreen);
            if (_screens.TryGetValue(screenType, out var screen))
            {
                _screens.Remove(screenType);
                _rootElement.RemoveChild(screen);

                // 若 UIElement 支援 IDisposable，可在此釋放
                (screen as IDisposable)?.Dispose();
            }
        }

        /// <summary>
        /// 隱藏指定畫面
        /// </summary>
        public void Hide<TScreen>() where TScreen : UIElementBase
        {
            var screenType = typeof(TScreen);
            if (_screens.TryGetValue(screenType, out var screen))
                screen.IsVisible = false;
        }

        /// <summary>
        /// 清除非 Persistent 子畫面
        /// </summary>
        private static void ClearNonPersistentChildren(UIElement parent)
        {
            var toRemove = parent.Children.Where(c => !c.IsPersistent).ToList();
            foreach (var child in toRemove)
                parent.RemoveChild(child);
        }

        /// <summary>
        /// 選擇性取得已建立畫面
        /// </summary>
        public TScreen GetScreen<TScreen>() where TScreen : UIElementBase
        {
            _screens.TryGetValue(typeof(TScreen), out var screen);
            return (TScreen)screen;
        }
    }
}
