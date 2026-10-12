/* ----- ----- ----- ----- */
// Program.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Silk.NET.Maths;
using Silk.NET.Windowing;

using Engine.Platform;
using Engine.Platform.Skia;
using Engine.Styles;

namespace Engine.UiGallery.Host
{
    internal static class Program
    {
        private static void Main()
        {
            GraphicsBackend.Factory = new SkiaGraphicsFactory();
            FontManager.LoadFonts();
            var options = WindowOptions.Default with
            {
                Title = "引擎 UI 樣式圖鑑 — Engine UI Style Gallery",
                Size = new Vector2D<int>(1280, 800)
            };
            using var window = Window.Create(options);
            using var app = new GalleryApp(window);
            window.Run();
        }
    }
}
