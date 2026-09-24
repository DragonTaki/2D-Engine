/* ----- ----- ----- ----- */
// IGraphics.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace Engine.Platform
{
    /// <summary>
    /// An active drawing surface for one frame/paint pass. Every renderer in
    /// this codebase draws through this interface instead of a concrete
    /// backend's native graphics type, so the backend (currently GDI+ via
    /// WinForms — see <c>Engine/Platform/WinForms/</c>) can be swapped
    /// without touching any renderer.
    /// </summary>
    public interface IGraphics : IDisposable
    {
        void FillRectangle(IBrush brush, float x, float y, float width, float height);
        void FillRectangle(IBrush brush, RectangleF bounds);

        void DrawRectangle(IPen pen, float x, float y, float width, float height);
        void DrawRectangle(IPen pen, RectangleF bounds);

        void FillEllipse(IBrush brush, float x, float y, float width, float height);
        void DrawEllipse(IPen pen, float x, float y, float width, float height);

        void DrawLine(IPen pen, float x1, float y1, float x2, float y2);

        void FillPath(IBrush brush, IGraphicsPath path);
        void DrawPath(IPen pen, IGraphicsPath path);

        void FillRegion(IBrush brush, IRegion region);

        void DrawString(string text, IFont font, IBrush brush, float x, float y);
        void DrawString(string text, IFont font, IBrush brush, RectangleF bounds, IStringFormat format);

        /// <summary>Measures the rendered size of <paramref name="text"/> in pixels.</summary>
        SizeF MeasureString(string text, IFont font);

        /// <summary>Measures the rendered size of <paramref name="text"/> wrapped to <paramref name="maxWidth"/> pixels.</summary>
        SizeF MeasureString(string text, IFont font, int maxWidth);

        void SetClip(RectangleF bounds);
        void ResetClip();

        /// <summary>
        /// Pushes a combined translate + uniform-scale transform onto the
        /// drawing state (saving whatever state — including clip — was
        /// active before), so everything drawn afterward is offset and
        /// scaled accordingly. Used once per frame to map the fixed-aspect
        /// content coordinate space onto the actual window size — see
        /// <c>Engine.Globals.GlobalViewport</c>. Must be paired with exactly
        /// one matching <see cref="PopTransform"/>.
        /// </summary>
        void PushTransform(float scale, float offsetX, float offsetY);

        /// <summary>Restores the drawing state saved by the matching <see cref="PushTransform"/>.</summary>
        void PopTransform();

        /// <summary>Enables anti-aliasing and high-quality text/image rendering.</summary>
        void ApplyHighQualitySettings();
    }
}
