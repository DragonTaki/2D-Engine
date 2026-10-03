/* ----- ----- ----- ----- */
// DebugOverlay.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using Engine.Globals;
using Engine.Platform;

namespace Engine.Diagnostics
{
    /// <summary>
    /// The on-screen debug readouts (效能與連線): the measured frame rate
    /// (<see cref="DebugOptions.ShowFps"/>) and the network latency
    /// (<see cref="DebugOptions.ShowNetworkLatency"/>), as small text in the window's
    /// top-left corner over everything else. Both launchers call <see cref="Draw"/> once per
    /// drawn frame, after the UI, so the frame rate is measured from the frames actually
    /// drawn on either backend. With both switches off nothing is measured or drawn.
    /// </summary>
    public static class DebugOverlay
    {
        /// <summary>Distance of the text from the window's top-left corner, and the padding around it (logical window units).</summary>
        private const float Margin = 8f, Padding = 4f;

        /// <summary>Text size of the readouts.</summary>
        private const float FontSize = 12f;

        private static readonly FrameRateCounter FrameRate = new();

        // -1: no latency reported. An int so the network thread can set it atomically.
        private static int _networkLatencyMs = -1;

        private static IFont _font;
        private static IBrush _textBrush;
        private static IBrush _backgroundBrush;

        /// <summary>
        /// The network round-trip latency in milliseconds, set by the network layer; null while
        /// unknown (there is no live connection yet, so nothing sets it - the readout shows a
        /// dash). Safe to set from any thread.
        /// </summary>
        public static int? NetworkLatencyMs
        {
            get
            {
                int value = Volatile.Read(ref _networkLatencyMs);
                return value < 0 ? null : value;
            }
            set => Volatile.Write(ref _networkLatencyMs, value is >= 0 ? value.Value : -1);
        }

        /// <summary>
        /// Records this frame for the frame rate and draws the readouts that are switched on.
        /// Call once per drawn frame with no transform pushed (window pixels); the text is
        /// drawn in logical window units (scaled by <see cref="GlobalWindow.PixelScale"/>, so
        /// it is the same size on HiDPI displays).
        /// </summary>
        /// <param name="g">The frame's graphics surface.</param>
        public static void Draw(IGraphics g)
        {
            if (g == null)
                return;

            bool showFps = DebugOptions.ShowFps;
            bool showLatency = DebugOptions.ShowNetworkLatency;

            if (showFps)
                FrameRate.AddFrame();
            else
                FrameRate.Reset();  // measure afresh when it is turned on again

            if (!showFps && !showLatency)
                return;

            var lines = new List<string>(2);
            if (showFps)
                lines.Add(FpsText(FrameRate.FramesPerSecond));
            if (showLatency)
                lines.Add(LatencyText(NetworkLatencyMs));

            EnsureResources();

            g.PushTransform(GlobalWindow.PixelScale, 0f, 0f);
            try
            {
                float y = Margin;
                foreach (string line in lines)
                {
                    SizeF size = g.MeasureString(line, _font);
                    g.FillRectangle(_backgroundBrush, Margin, y, size.Width + Padding * 2, size.Height + Padding);
                    g.DrawString(line, _font, _textBrush, Margin + Padding, y + Padding / 2);
                    y += size.Height + Padding;
                }
            }
            finally
            {
                g.PopTransform();
            }
        }

        /// <summary>The frame rate readout: "更新率 60", or "更新率 —" before the first measurement.</summary>
        public static string FpsText(double? framesPerSecond) =>
            framesPerSecond.HasValue ? $"更新率 {framesPerSecond.Value:0}" : "更新率 —";

        /// <summary>The latency readout: "延遲 42 毫秒", or "延遲 —" while no latency is known.</summary>
        public static string LatencyText(int? latencyMs) =>
            latencyMs.HasValue ? $"延遲 {latencyMs.Value} 毫秒" : "延遲 —";

        /// <summary>Creates the font and brushes on first use (the graphics backend is chosen at startup).</summary>
        private static void EnsureResources()
        {
            _font ??= GraphicsBackend.Factory.CreateFont(GraphicsBackend.Factory.GenericSansSerifFontFamily, FontSize);
            _textBrush ??= GraphicsBackend.Factory.CreateSolidBrush(Color.White);
            _backgroundBrush ??= GraphicsBackend.Factory.CreateSolidBrush(Color.FromArgb(160, 0, 0, 0));
        }
    }
}
