/* ----- ----- ----- ----- */
// ExampleStyles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Platform;

namespace Engine.Styles.Catalog
{
    /// <summary>Backend-neutral examples; no existing defaults are modified.</summary>
    public static class ExampleStyles
    {
        public static StatefulButtonStyle Button(string name)
        {
            var style = new StatefulButtonStyle();
            switch (name)
            {
                case "FilledPrimary": style.FontWeight = FontStyleFlags.Bold; break;
                case "Outline": style.FillColor = Color.Transparent; style.TextColor = Color.CornflowerBlue; style.BorderColor = Color.CornflowerBlue; style.BorderWidth = 2f; style.HoverColor = Color.FromArgb(45, 70, 130, 220); break;
                case "Ghost": style.FillColor = Color.Transparent; style.TextColor = Color.Teal; style.BorderWidth = 0f; style.HoverColor = Color.FromArgb(40, 0, 180, 160); style.PressedColor = Color.FromArgb(80, 0, 180, 160); break;
                case "Danger": style.FillColor = Color.Firebrick; style.HoverColor = Color.IndianRed; style.PressedColor = Color.DarkRed; style.FontWeight = FontStyleFlags.Bold; break;
                case "Pill": style.CornerRadius = 100f; style.FillColor = Color.DarkViolet; style.HoverColor = Color.MediumOrchid; style.PressedColor = Color.Indigo; break;
                case "FlatTile": style.CornerRadius = 0f; style.FillColor = Color.DarkSlateGray; style.HoverColor = Color.Teal; style.PressedColor = Color.Black; style.FontSize = 16f; break;
                case "Gradient": style.FillColor = Color.RoyalBlue; style.GradientEnd = Color.DarkViolet; style.CornerRadius = 14f; break;
                case "SoftRaised": style.FillColor = Color.FromArgb(228, 237, 249); style.TextColor = Color.MidnightBlue; style.HoverColor = Color.White; style.PressedColor = Color.LightSteelBlue; style.Raised = true; style.CornerRadius = 12f; break;
            }
            return style;
        }

        public static LabelStyle Label(string name) => new LabelStyle
        {
            FontSize = name switch { "Title" => 30f, "Heading" => 24f, "Body" => 18f, "Caption" => 13f, _ => 16f },
            FontWeight = name is "Title" or "Heading" ? FontStyleFlags.Bold : FontStyleFlags.Regular,
            TextColor = name switch { "Error" => Color.IndianRed, "Success" => Color.MediumSeaGreen, "MutedHint" => Color.Gray, "Caption" => Color.SlateGray, _ => Color.CornflowerBlue }
        };

        private static Color Accent(string name) => name switch
        {
            "AmberOutline" or "AmberRing" => Color.DarkOrange,
            "CompactMint" => Color.MediumSeaGreen,
            "HighContrast" => Color.Gold,
            "SoftViolet" => Color.MediumPurple,
            "Error" => Color.IndianRed,
            _ => Color.DodgerBlue
        };

        public static SingleBorderRoundedStyle Box(Color fill, Color border, float radius = 8f, float width = 1f) => new SingleBorderRoundedStyle
        {
            CornerRadius = radius,
            BackgroundBrushFactory = new SolidBrushFactory(fill),
            BorderStyle = new BorderStyle { Color = border, Width = width }
        };

        public static ToggleSwitchStyle Toggle(string name) => new ToggleSwitchStyle
        {
            TrackOnColor = Accent(name),
            TrackOffColor = name == "HighContrast" ? Color.Black : name == "SoftViolet" ? Color.Thistle : Color.FromArgb(90, 100, 115),
            KnobColor = name == "AmberOutline" ? Color.DarkOrange : name == "HighContrast" ? Color.Black : Color.White,
            BorderColor = Accent(name),
            BorderWidth = name == "AmberOutline" ? 3f : name == "HighContrast" ? 2f : 0f,
            KnobInset = name == "CompactMint" ? 6f : name == "SoftViolet" ? 2f : 3f,
            DisabledOpacity = 0.4f
        };

        public static CheckboxStyle Checkbox(string name) => new CheckboxStyle
        {
            CheckedColor = Accent(name),
            UncheckedColor = name == "HighContrast" ? Color.Black : name == "SoftViolet" ? Color.Thistle : Color.Transparent,
            BorderColor = Accent(name),
            CheckColor = name == "HighContrast" ? Color.Black : Color.White,
            TextColor = Accent(name),
            FocusColor = Color.Gold,
            BoxSize = name == "CompactMint" ? 18f : name == "HighContrast" ? 30f : 24f,
            BorderWidth = name == "AmberOutline" ? 3f : 1f,
            CheckWidth = name == "HighContrast" ? 4f : 2f,
            LabelGap = name == "CompactMint" ? 6f : 12f
        };

        public static TextFieldStyle Field(string name)
        {
            bool dark = name == "DarkInset";
            var accent = Accent(name);
            float radius = name == "Pill" ? 100f : name == "Outline" ? 0f : 8f;
            var fill = dark ? Color.FromArgb(23, 29, 40) : name == "SoftFilled" ? Color.FromArgb(222, 235, 250) : Color.White;
            return new TextFieldStyle
            {
                Box = Box(fill, dark ? Color.Black : accent, radius, name == "Outline" ? 2f : 1f),
                FocusedBox = Box(fill, accent, radius, 3f),
                TextColor = dark ? Color.WhiteSmoke : Color.FromArgb(30, 45, 65),
                PlaceholderColor = dark ? Color.SlateGray : Color.Gray,
                CaretColor = accent, CaretWidth = 2f,
                TextInset = name == "Pill" ? 20f : 10f,
                DisabledOpacity = 0.4f
            };
        }

        public static DropdownStyle Dropdown(string name)
        {
            var field = Field(name);
            return new DropdownStyle
            {
                Box = field.Box, OpenBox = field.FocusedBox,
                ListBox = Box(name == "DarkInset" ? Color.FromArgb(23, 29, 40) : Color.White, Accent(name)),
                TextColor = field.TextColor, SelectedTextColor = Accent(name), HoverColor = Color.FromArgb(75, 90, 140, 200),
                ArrowColor = Accent(name), ArrowWidth = name == "Pill" ? 16f : 10f,
                ArrowInset = 14f, TextInset = field.TextInset,
                ItemHeight = name == "SoftFilled" ? 44f : 32f, MaxVisibleItems = 4, ListGap = 6f, ListPadding = 6f,
                ScrollBarColor = Accent(name), ScrollBarWidth = 4f, DisabledOpacity = 0.4f
            };
        }

        public static SliderStyle Slider(string name) => new SliderStyle
        {
            TrackColor = name == "SoftViolet" ? Color.Thistle : Color.SlateGray,
            FillColor = Accent(name), KnobColor = name == "AmberRing" ? Color.Transparent : Accent(name),
            KnobBorderColor = name == "SoftViolet" ? Color.White : Accent(name),
            KnobBorderWidth = name == "AmberRing" ? 4f : 2f,
            TrackHeight = name == "FineLine" ? 2f : name == "BoldTrack" ? 12f : 6f,
            KnobDiameter = name == "CompactMint" ? 12f : name == "BoldTrack" ? 28f : 22f,
            LabelColor = Accent(name), LabelWidth = 40f, LabelGap = 8f, DisabledOpacity = 0.4f
        };

        public static TabBarStyle Tabs(string name)
        {
            var normal = Button(name == "Pills" ? "Pill" : name == "Outline" ? "Outline" : name == "Tiles" ? "FlatTile" : "Ghost");
            var selected = Button(name == "Pills" ? "Pill" : "FilledPrimary");
            if (name == "Segmented") { normal.CornerRadius = 0f; selected.CornerRadius = 0f; }
            selected.FillColor = Color.Teal;
            normal.FontSize = selected.FontSize = 14f;
            return new TabBarStyle
            {
                TabStyle = normal, SelectedTabStyle = selected,
                TabGap = name == "Segmented" ? 0f : name == "Tiles" ? 10f : 4f,
                IndicatorColor = Color.DarkOrange, IndicatorHeight = name == "Underline" ? 4f : 0f, IndicatorInset = 10f
            };
        }
    }
}
