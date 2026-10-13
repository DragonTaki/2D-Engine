/* ----- ----- ----- ----- */
// GalleryApp.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Diagnostics;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using SkiaSharp;

using Engine.Globals;
using Engine.Mathematics;
using Engine.Physics;
using Engine.Platform;
using Engine.Platform.Skia;
using Engine.Styles;
using Engine.Styles.Catalog;
using Engine.Timing;
using Engine.UI.Core.Interfaces;
using Engine.UI.Infrastructure;
using Engine.UI.Input;
using Engine.UiGallery.UI;

using SilkWindow = Silk.NET.Windowing.IWindow;

namespace Engine.UiGallery.Host
{
    internal sealed class GalleryApp : IDisposable
    {
        private readonly SilkWindow _window;
        private readonly ManualTimerProvider _timer = new();
        private readonly FramePacer _renderPacer = new(TimerSettings.GameAnimationFPS, Stopwatch.Frequency);
        private readonly FramePacer _updatePacer = new(TimerSettings.GameAnimationFPS, Stopwatch.Frequency);
        private long _lastUpdateTimestamp;
        private ServiceProvider _services;
        private GalleryRoot _root;
        private UIInputManager _input;
        private IInputContext _inputContext;
        private SilkInputAdapter _inputAdapter;
        private GL _gl;
        private GRGlInterface _glInterface;
        private GRContext _context;
        private bool _disposed;

        public GalleryApp(SilkWindow window)
        {
            _window = window;
            window.Load += Load;
            window.Update += Update;
            window.Render += Render;
            window.FramebufferResize += Resize;
            window.FocusChanged += FocusChanged;
            window.ShouldSwapAutomatically = false;
            ApplyFrameRate(TimerSettings.GameAnimationFPS);
            ApplyVSync(TimerSettings.VSync);
            TimerSettings.GameAnimationFpsChanged += ApplyFrameRate;
            TimerSettings.VSyncChanged += ApplyVSync;
        }

        private void ApplyFrameRate(int fps)
        {
            _window.FramesPerSecond = fps;
            _window.UpdatesPerSecond = fps;
            long now = Stopwatch.GetTimestamp();
            _renderPacer.SetFrameRate(fps, now);
            _updatePacer.SetFrameRate(fps, now);
        }

        private void ApplyVSync(bool enabled) => _window.VSync = enabled;

        private void Load()
        {
            GlobalViewport.DesignSize = new Vector2F(1280f, 800f);
            Resize(_window.FramebufferSize);
            _gl = GL.GetApi(_window);
            _glInterface = GRGlInterface.Create();
            _context = GRContext.CreateGl(_glInterface);
            var scroll = new ScrollInputHandler();
            _services = new ServiceCollection()
                .AddSingleton<IUiFactory, UiFactory>()
                .AddSingleton<NavigationManager>()
                .BuildServiceProvider();
            var factory = (UiFactory)_services.GetRequiredService<IUiFactory>();
            DefaultStyles.DefaultButtonStyle = ExampleStyles.Button("FilledPrimary");
            _root = new GalleryRoot(factory, scroll, StyleCatalog.CreateDefault());
            _root.MainWindow = new Engine.Platform.Skia.SilkWindow();
            _services.GetRequiredService<NavigationManager>().Init(_root);
            _inputContext = _window.CreateInput();
            var keyboard = _inputContext.Keyboards.Count > 0 ? _inputContext.Keyboards[0] : null;
            var clipboard = new SilkClipboard();
            clipboard.Attach(keyboard);
            _input = new UIInputManager(_root, scroll, clipboard);
            _input.RegisterHandler(_root);
            if (_inputContext.Mice.Count > 0)
                _inputAdapter = new SilkInputAdapter(_input, _inputContext.Mice[0], _window, keyboard);
            GlobalTime.Timer = _timer;
            _timer.OnAnimationFrame += Tick;
            _lastUpdateTimestamp = Stopwatch.GetTimestamp();
            _renderPacer.Reset(_lastUpdateTimestamp);
            _updatePacer.Reset(_lastUpdateTimestamp);
            _timer.Start();
        }

        private void Tick()
        {
            _root?.Update();
            PhysicsRegistry.UpdateAll();
            _input?.EndFrame();
        }

        private void Update(double seconds)
        {
            long now = Stopwatch.GetTimestamp();
            if (!_updatePacer.TryAdvance(now))
                return;
            float elapsed = (float)((now - _lastUpdateTimestamp) / (double)Stopwatch.Frequency);
            _lastUpdateTimestamp = now;
            _timer.Tick(elapsed);
        }

        private void Resize(Silk.NET.Maths.Vector2D<int> size)
        {
            GlobalWindow.UpdateSize(size.X, size.Y);
            if (_window.Size.X > 0)
                GlobalWindow.UpdatePixelScale(size.X / (float)_window.Size.X);
            GlobalViewport.Recalculate(size.X, size.Y);
        }

        private void FocusChanged(bool focused)
        {
            if (focused) return;
            _input?.CancelInput();
            _input?.Keyboard.ClearFocus();
            _root?.CancelPointer();
        }

        private void Render(double seconds)
        {
            var size = _window.FramebufferSize;
            if (size.X <= 0 || size.Y <= 0 || _context == null)
                return;
            if (!_renderPacer.TryAdvance(Stopwatch.GetTimestamp()))
            {
                // Yield briefly while keeping window events responsive between frame deadlines.
                Thread.Sleep(1);
                return;
            }
            var info = new GRGlFramebufferInfo((uint)_gl.GetInteger(GLEnum.FramebufferBinding), (uint)GLEnum.Rgba8);
            using var target = new GRBackendRenderTarget(size.X, size.Y, 0, 8, info);
            using var surface = SKSurface.Create(_context, target, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
            surface.Canvas.Clear(new SKColor(20, 24, 34));
            using IGraphics graphics = new SkiaGraphics(surface.Canvas);
            graphics.PushTransform(GlobalViewport.Scale, GlobalViewport.Offset.X, GlobalViewport.Offset.Y);
            try { _root?.Draw(graphics); }
            finally { graphics.PopTransform(); }
            surface.Canvas.Flush();
            _context.Flush();
            _window.GLContext.SwapBuffers();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _window.Load -= Load;
            _window.Update -= Update;
            _window.Render -= Render;
            _window.FramebufferResize -= Resize;
            _window.FocusChanged -= FocusChanged;
            TimerSettings.GameAnimationFpsChanged -= ApplyFrameRate;
            TimerSettings.VSyncChanged -= ApplyVSync;
            _timer.Stop();
            _timer.OnAnimationFrame -= Tick;
            _input?.CancelInput();
            _input?.Keyboard.ClearFocus();
            _root?.Dispose();
            _inputContext?.Dispose();
            _services?.Dispose();
            _context?.Dispose();
            _glInterface?.Dispose();
            _gl?.Dispose();
        }
    }
}
