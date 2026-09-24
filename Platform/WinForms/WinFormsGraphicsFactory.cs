/* ----- ----- ----- ----- */
// WinFormsGraphicsFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Engine.Platform.WinForms
{
    /// <summary>GDI+-backed <see cref="IGraphicsFactory"/> — the WinForms platform backend.</summary>
    public sealed class WinFormsGraphicsFactory : IGraphicsFactory
    {
        private readonly PrivateFontCollection _loadedFontFiles = new();
        private readonly Dictionary<string, IFontFamily> _loadedFontFamilies = new();

        public IBrush CreateSolidBrush(Color color) => new WinFormsBrush(new SolidBrush(color));

        public IBrush CreateLinearGradientBrush(RectangleF bounds, Color start, Color end, GradientDirection direction)
        {
            var mode = direction == GradientDirection.Horizontal ? LinearGradientMode.Horizontal : LinearGradientMode.Vertical;
            return new WinFormsBrush(new LinearGradientBrush(bounds, start, end, mode));
        }

        public IBrush CreateLinearGradientBrush(RectangleF bounds, GradientDirection direction, (float position, Color color)[] stops)
        {
            var mode = direction == GradientDirection.Horizontal ? LinearGradientMode.Horizontal : LinearGradientMode.Vertical;
            var brush = new LinearGradientBrush(bounds, Color.Empty, Color.Empty, mode);
            brush.InterpolationColors = new ColorBlend
            {
                Positions = System.Array.ConvertAll(stops, s => s.position),
                Colors = System.Array.ConvertAll(stops, s => s.color),
            };
            return new WinFormsBrush(brush);
        }

        public IPen CreatePen(Color color, float width) => new WinFormsPen(new Pen(color, width));

        public void LoadFontFamily(string key, string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException($"Font file not found: {filePath}");

            _loadedFontFiles.AddFontFile(filePath);
            var family = _loadedFontFiles.Families[_loadedFontFiles.Families.Length - 1];
            _loadedFontFamilies[key] = new WinFormsFontFamily(family);
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
                return new WinFormsFontFamily(new FontFamily(name));
            }
            catch
            {
                Console.WriteLine($"System font '{name}' not found, using default system font.");
                return GenericSansSerifFontFamily;
            }
        }

        public IFontFamily GenericSansSerifFontFamily { get; } = new WinFormsFontFamily(FontFamily.GenericSansSerif);

        public IFont CreateFont(IFontFamily fontFamily, float size, FontStyleFlags style = FontStyleFlags.Regular)
        {
            var nativeFamily = ((WinFormsFontFamily)fontFamily).Native;

            var nativeStyle = FontStyle.Regular;
            if ((style & FontStyleFlags.Bold) != 0) nativeStyle |= FontStyle.Bold;
            if ((style & FontStyleFlags.Italic) != 0) nativeStyle |= FontStyle.Italic;

            var nativeFont = new Font(nativeFamily, size, nativeStyle, GraphicsUnit.Pixel);
            return new WinFormsFont(nativeFont, fontFamily);
        }

        public IGraphicsPath CreatePath() => new WinFormsGraphicsPath(new GraphicsPath());
        public IMatrix CreateMatrix() => new WinFormsMatrix(new Matrix());
        public IRegion CreateRegion(IGraphicsPath path) => new WinFormsRegion(new Region(((WinFormsGraphicsPath)path).Native));
        public IStringFormat CreateStringFormat() => new WinFormsStringFormat(new StringFormat());

        public IGraphics CreateMeasurementContext() =>
            new WinFormsGraphics(Graphics.FromHwnd(IntPtr.Zero), ownsNative: true);
    }
}
