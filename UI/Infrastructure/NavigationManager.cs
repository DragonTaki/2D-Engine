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
    /// NavigationManager manages switching between the game's main UI screens.
    /// It operates on a root UIElement container: it clears the old screen
    /// (non-persistent children) and adds the new one.
    /// </summary>
    public class NavigationManager
    {
        private readonly IUiFactory _factory;
        private UIElement _rootElement;

        // Screens that have been created, keyed by screen type
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
        /// Pre-registers a screen instance (optional); it starts hidden.
        /// </summary>
        public void RegisterScreen<TScreen>(TScreen instance) where TScreen : UIElement
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var type = typeof(TScreen);
            if (_screens.ContainsKey(type))
                throw new InvalidOperationException($"Screen of type {type.Name} already registered.");

            _screens[type] = instance;

            // If a root is set, add the screen to it right away
            if (_rootElement != null && !_rootElement.Children.Contains(instance))
                _rootElement.AddChild(instance);

            instance.IsVisible = false; // Hidden initially
        }

        /// <summary>
        /// Shows the given screen, creating it lazily on first use and optionally rebuilding it (<paramref name="forceReload"/>).
        /// </summary>
        public TScreen Show<TScreen, THandler, TRenderer>(bool forceReload = false)
            where TScreen : UIElement<TScreen, THandler, TRenderer>
            where THandler : UIHandler<TScreen, THandler, TRenderer>
            where TRenderer : UIRenderer<TScreen, THandler, TRenderer>
        {
            if (_rootElement == null)
                throw new InvalidOperationException("NavigationManager not initialized with root element.");

            var screenType = typeof(TScreen);
            _screens.TryGetValue(screenType, out var previousInstance);
            bool alreadyShown = previousInstance != null && _rootElement.Children.Contains(previousInstance) && !forceReload;

            // Screens being replaced get OnExit (IScreen lifecycle, e.g. the main menu closes
            // its open submenu); re-showing the current screen isn't a transition.
            var leaving = _rootElement.Children.Where(c => !c.IsPersistent && (c != previousInstance || forceReload)).ToList();
            ClearNonPersistentChildren(_rootElement);
            foreach (var left in leaving)
                AsScreen(left)?.OnExit();

            if (forceReload)
            {
                // Forced rebuild: unload the old one
                UnloadScreen<TScreen>();
            }

            if (!_screens.TryGetValue(screenType, out UIElementBase screen))
            {
                // Lazy creation: build screen + handler + renderer through the factory
                screen = _factory.CreateDIElement<TScreen, THandler, TRenderer>();
                _screens[screenType] = screen;
            }

            screen.IsVisible = true;

            if (!_rootElement.Children.Contains(screen))
                _rootElement.AddChild(screen);

            if (!alreadyShown)
                AsScreen(screen)?.OnEnter();

            return (TScreen)screen;
        }

        /// <summary>
        /// The screen's <see cref="IScreen"/> implementation: the element itself, or (as with
        /// the main menu) its handler.
        /// </summary>
        private static IScreen AsScreen(UIElementBase element) =>
            element as IScreen ?? element?.HandlerBase as IScreen;

        /// <summary>
        /// Unloads the given screen (removed from the root node and dropped from the cache).
        /// </summary>
        /// <remarks>
        /// Screens are deliberately never disposed: by design each screen is a single
        /// long-lived instance (DI singleton) that is reused every time it's shown, like the
        /// root node. Disposing one here would hand back an already-disposed instance on the
        /// next Show (the container resolves the same singleton), i.e. a blank screen.
        /// Only short-lived elements (e.g. pieces) are disposed, by their owners.
        /// </remarks>
        public void UnloadScreen<TScreen>() where TScreen : UIElementBase
        {
            var screenType = typeof(TScreen);
            if (_screens.TryGetValue(screenType, out var screen))
            {
                _screens.Remove(screenType);
                bool wasShown = _rootElement != null && _rootElement.Children.Contains(screen);
                _rootElement?.RemoveChild(screen);
                if (wasShown)
                    AsScreen(screen)?.OnExit();
            }
        }

        /// <summary>
        /// Hides the given screen (if it is currently visible).
        /// </summary>
        public void Hide<TScreen>() where TScreen : UIElementBase
        {
            var screenType = typeof(TScreen);
            if (_screens.TryGetValue(screenType, out var screen) && screen.IsVisible)
            {
                screen.IsVisible = false;
                AsScreen(screen)?.OnExit();
            }
        }

        /// <summary>
        /// Removes the root's non-persistent child screens.
        /// </summary>
        private static void ClearNonPersistentChildren(UIElement parent)
        {
            var toRemove = parent.Children.Where(c => !c.IsPersistent).ToList();
            foreach (var child in toRemove)
                parent.RemoveChild(child);
        }

        /// <summary>
        /// Returns the already-created screen of the given type, or null if none exists yet.
        /// </summary>
        public TScreen GetScreen<TScreen>() where TScreen : UIElementBase
        {
            _screens.TryGetValue(typeof(TScreen), out var screen);
            return (TScreen)screen;
        }
    }
}
