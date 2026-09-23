/* ----- ----- ----- ----- */
// IGraphicsFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Engine.Platform
{
    /// <summary>
    /// Direction a linear gradient brush blends along, mirroring the subset
    /// of GDI+'s <c>LinearGradientMode</c> actually used here.
    /// </summary>
    public enum GradientDirection
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Creates every backend-specific drawing object (<see cref="IBrush"/>,
    /// <see cref="IPen"/>, <see cref="IFont"/>, <see cref="IGraphicsPath"/>,
    /// <see cref="IMatrix"/>, <see cref="IRegion"/>, <see cref="IStringFormat"/>)
    /// and a throwaway <see cref="IGraphics"/> context for text measurement
    /// outside of a paint pass. One instance is registered for the app's
    /// lifetime (see <c>Launcher/Program.cs</c>); nothing above this layer
    /// constructs a backend type directly.
    /// </summary>
    public interface IGraphicsFactory
    {
        IBrush CreateSolidBrush(Color color);
        IBrush CreateLinearGradientBrush(RectangleF bounds, Color start, Color end, GradientDirection direction);

        IPen CreatePen(Color color, float width);

        /// <summary>Loads a font family from a font file (e.g. a bundled .ttf/.otf), keyed for later lookup by <paramref name="key"/>.</summary>
        void LoadFontFamily(string key, string filePath);

        /// <summary>Gets a previously-loaded font family by the key passed to <see cref="LoadFontFamily"/>.</summary>
        IFontFamily GetLoadedFontFamily(string key);

        /// <summary>Looks up an installed system font family by name, falling back to a generic sans-serif family if not found.</summary>
        IFontFamily GetSystemFontFamily(string name);

        IFontFamily GenericSansSerifFontFamily { get; }

        IFont CreateFont(IFontFamily fontFamily, float size, FontStyleFlags style = FontStyleFlags.Regular);

        IGraphicsPath CreatePath();
        IMatrix CreateMatrix();
        IRegion CreateRegion(IGraphicsPath path);
        IStringFormat CreateStringFormat();

        /// <summary>
        /// An <see cref="IGraphics"/> context usable for text measurement
        /// (e.g. <see cref="IGraphics.MeasureString(string, IFont)"/>) before
        /// any window/paint surface exists. Not for drawing.
        /// </summary>
        IGraphics CreateMeasurementContext();
    }
}
