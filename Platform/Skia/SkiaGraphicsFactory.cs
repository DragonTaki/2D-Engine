/* ----- ----- ----- ----- */
// SkiaGraphicsFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using SkiaSharp;

namespace Engine.Platform.Skia
{
    /// <summary>SkiaSharp-backed <see cref="IGraphicsFactory"/> — the cross-platform backend.</summary>
    public sealed class SkiaGraphicsFactory : IGraphicsFactory
    {
        private readonly Dictionary<string, IFontFamily> _loadedFontFamilies = new();

        public IBrush CreateSolidBrush(Color color) =>
            new SkiaBrush(new SKPaint { Color = ToSKColor(color), Style = SKPaintStyle.Fill, IsAntialias = true });

        public IBrush CreateLinearGradientBrush(RectangleF bounds, Color start, Color end, GradientDirection direction)
        {
            var (p0, p1) = GradientEndpoints(bounds, direction);
            var shader = SKShader.CreateLinearGradient(p0, p1, new[] { ToSKColor(start), ToSKColor(end) }, null, SKShaderTileMode.Clamp);
            return new SkiaBrush(new SKPaint { Shader = shader, Style = SKPaintStyle.Fill, IsAntialias = true });
        }

        public IBrush CreateLinearGradientBrush(RectangleF bounds, GradientDirection direction, (float position, Color color)[] stops)
        {
            var (p0, p1) = GradientEndpoints(bounds, direction);
            var colors = new SKColor[stops.Length];
            var positions = new float[stops.Length];
            for (int i = 0; i < stops.Length; i++)
            {
                positions[i] = stops[i].position;
                colors[i] = ToSKColor(stops[i].color);
            }
            var shader = SKShader.CreateLinearGradient(p0, p1, colors, positions, SKShaderTileMode.Clamp);
            return new SkiaBrush(new SKPaint { Shader = shader, Style = SKPaintStyle.Fill, IsAntialias = true });
        }

        private static (SKPoint, SKPoint) GradientEndpoints(RectangleF bounds, GradientDirection direction) =>
            direction == GradientDirection.Horizontal
                ? (new SKPoint(bounds.Left, bounds.Top), new SKPoint(bounds.Right, bounds.Top))
                : (new SKPoint(bounds.Left, bounds.Top), new SKPoint(bounds.Left, bounds.Bottom));

        public IPen CreatePen(Color color, float width) =>
            new SkiaPen(new SKPaint { Color = ToSKColor(color), Style = SKPaintStyle.Stroke, StrokeWidth = width, IsAntialias = true });

        public void LoadFontFamily(string key, string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException($"Font file not found: {filePath}");

            _loadedFontFamilies[key] = new SkiaFontFamily(SKTypeface.FromFile(filePath));
        }

        public IFontFamily GetLoadedFontFamily(string key)
        {
            if (!_loadedFontFamilies.TryGetValue(key, out var family))
                throw new ArgumentException($"Font key '{key}' not found. Did you call LoadFontFamily()?");

            return family;
        }

        public IFontFamily GetSystemFontFamily(string name)
        {
            try
            {
                var typeface = SKTypeface.FromFamilyName(name);
                if (typeface == null)
                {
                    Console.WriteLine($"System font '{name}' not found, using default system font.");
                    return GenericSansSerifFontFamily;
                }
                return new SkiaFontFamily(typeface);
            }
            catch
            {
                Console.WriteLine($"System font '{name}' not found, using default system font.");
                return GenericSansSerifFontFamily;
            }
        }

        public IFontFamily GenericSansSerifFontFamily { get; } = new SkiaFontFamily(SKTypeface.Default);

        public IFont CreateFont(IFontFamily fontFamily, float size, FontStyleFlags style = FontStyleFlags.Regular)
        {
            var typeface = ((SkiaFontFamily)fontFamily).Native;
            var font = new SKFont(typeface, size);

            // The custom-loaded font files here are single-weight, so bold/
            // italic are synthesized rather than switched to a real bold/
            // italic typeface variant.
            if ((style & FontStyleFlags.Bold) != 0) font.Embolden = true;
            if ((style & FontStyleFlags.Italic) != 0) font.SkewX = 0.25f;

            return new SkiaFont(font, fontFamily, style);
        }

        public IGraphicsPath CreatePath() => new SkiaGraphicsPath(new SKPath());
        public IMatrix CreateMatrix() => new SkiaMatrix();
        public IRegion CreateRegion(IGraphicsPath path) => new SkiaRegion(new SKRegion(((SkiaGraphicsPath)path).Native));
        public IStringFormat CreateStringFormat() => new SkiaStringFormat();

        public IGraphics CreateMeasurementContext() => SkiaGraphics.CreateMeasurementContext();

        private static SKColor ToSKColor(Color c) => new SKColor(c.R, c.G, c.B, c.A);
    }
}
