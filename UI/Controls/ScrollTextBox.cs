/* ----- ----- ----- ----- */
// ScrollTextBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/24
// Update Date: 2025/10/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using Engine.Configs;
using Engine.Geometry;
using Engine.UI.Core.Elements;

public class ScrollTextBox : UIElement
{
    private float _scrollOffsetY = 0f;
    private float _contentHeight = 0f;

    protected List<TextFragment> _fragments = new();

    private LayoutF _bounds;
    private float _viewHeight;

    // 可動態設定
    public Font Font { get; set; } = SystemFonts.DefaultFont;
    public float LineHeight { get; set; } = 18f;
    public Color BackgroundColor { get; set; } = Color.Black;
    public Color TextColor { get; set; } = Color.White;
    
    public ScrollTextBox(LayoutF bounds)
    {
        Layout = bounds;
    }

    public void SetFragments(List<TextFragment> fragments)
    {
        _fragments = fragments;
        RecalculateContentHeight();
    }

    private void RecalculateContentHeight()
    {
        int lines = 0;
        foreach (var frag in _fragments)
            lines += frag.Text.Split('\n').Length;

        _contentHeight = lines * LineHeight;
    }

    public void Scroll(float delta)
    {
        _scrollOffsetY -= delta;
        _scrollOffsetY = Math.Clamp(_scrollOffsetY, 0, Math.Max(0, _contentHeight - Layout.Size.Y));
    }

    protected override void OnDraw(Graphics g)
    {
        var bounds = GetCurrentAbsoluteBounds();

        // 背景
        using var bgBrush = new SolidBrush(BackgroundColor);
        g.FillRectangle(bgBrush, bounds);

        // 文字
        g.SetClip(bounds.ToRectangleF());
        float y = bounds.Y - _scrollOffsetY;

        foreach (var frag in _fragments)
        {
            using var brush = new SolidBrush(frag.Color);
            g.DrawString(frag.Text, Font, brush, bounds.X, y);

            y += LineHeight * frag.Text.Split('\n').Length;
        }

        g.ResetClip();
    }
}

public struct TextFragment
{
    public string Text;
    public Color Color;
    public bool Bold;
    public bool Italic;
}
