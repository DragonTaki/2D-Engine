/* ----- ----- ----- ----- */
// BuiltInStyles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Engine.Platform;
using Engine.UI.Constants.Components;

namespace Engine.Styles.Catalog
{
    internal static class BuiltInStyles
    {
        internal static void Register(StyleCatalog catalog)
        {
            void Add(ComponentKind kind, string name, Func<object> factory, string description = null) => catalog.Register(kind, kind + "/" + name, factory, description);
            Add(ComponentKind.Button, "Default", () => DefaultStyles.DefaultButtonStyle ?? new StatefulButtonStyle());
            Add(ComponentKind.Label, "Default", () => new LabelStyle());
            Add(ComponentKind.ToggleSwitch, "Default", () => ToggleSwitchDefaults.Style);
            Add(ComponentKind.Checkbox, "Default", () => CheckboxDefaults.Style);
            Add(ComponentKind.TextField, "Default", () => TextFieldDefaults.Style);
            Add(ComponentKind.NumberField, "Default", () => TextFieldDefaults.Style);
            Add(ComponentKind.Dropdown, "Default", () => DropdownDefaults.Style);
            Add(ComponentKind.Slider, "Default", () => SliderDefaults.Style);
            Add(ComponentKind.TabBar, "Default", () => TabBarDefaults.Style);

            Add(ComponentKind.Frame, "Default", () => FrameStyles.Default);
            Add(ComponentKind.DialogPanel, "Default", () => FrameStyles.Default);
            foreach (var frame in FrameStyles.All)
            {
                Add(ComponentKind.Frame, frame.Kind.ToString(), () => frame, frame.Name);
                Add(ComponentKind.DialogPanel, frame.Kind.ToString(), () => frame, frame.Name);
            }
            Add(ComponentKind.Frame, "BorderDefault", () => new BorderStyle());
            foreach (var kind in new[] { ComponentKind.Button, ComponentKind.Frame, ComponentKind.DialogPanel })
            {
                Add(kind, "SingleBorderRoundedDefault", () => ExampleStyles.Box(Color.Transparent, Color.Empty, 0f, 0f));
                Add(kind, "DoubleBorderRoundedDefault", () => new DoubleBorderRoundedStyle { OuterBorder = new BorderStyle(), InnerBorder = new BorderStyle(), BackgroundBrushFactory = new SolidBrushFactory(Color.Transparent) });
            }
            Add(ComponentKind.DialogPanel, "InwardCornerDefault", () => new InwardCornerDialogStyle { BorderStyle = new BorderStyle(), BackgroundBrushFactory = new SolidBrushFactory(Color.Transparent) });
            foreach (var kind in new[] { ComponentKind.Button, ComponentKind.Frame, ComponentKind.DialogPanel })
            {
                Add(kind, "SingleBorderRounded", () => ExampleStyles.Box(Color.FromArgb(35, 90, 160, 230), Color.CornflowerBlue, 14f, 2f));
                Add(kind, "DoubleBorderRounded", () => new DoubleBorderRoundedStyle
                {
                    CornerRadius = 16f, Margin = 5f,
                    OuterBorder = new BorderStyle { Color = Color.Gold, Width = 2f },
                    InnerBorder = new BorderStyle { Color = Color.DarkOrange, Width = 1f },
                    BackgroundBrushFactory = new SolidBrushFactory(Color.FromArgb(30, 220, 160, 40))
                });
            }
            Add(ComponentKind.DialogPanel, "InwardCorner", () => new InwardCornerDialogStyle
            {
                CornerRadius = 16f, BorderStyle = new BorderStyle { Color = Color.MediumSeaGreen, Width = 2f },
                BackgroundBrushFactory = new SolidBrushFactory(Color.FromArgb(35, 40, 180, 130))
            });
            Add(ComponentKind.Button, "TextOnly", () => new StatefulButtonStyle { FillColor = Color.Transparent, HoverColor = Color.FromArgb(35, 90, 170, 255), PressedColor = Color.FromArgb(65, 90, 170, 255), TextColor = Color.CornflowerBlue, BorderWidth = 0f });

            foreach (var name in new[] { "FilledPrimary", "Outline", "Ghost", "Danger", "Pill", "FlatTile", "Gradient", "SoftRaised" })
                Add(ComponentKind.Button, name, () => ExampleStyles.Button(name));
            foreach (var name in new[] { "Title", "Heading", "Body", "Caption", "MutedHint", "Error", "Success" })
                Add(ComponentKind.Label, name, () => ExampleStyles.Label(name));
            foreach (var name in new[] { "Ocean", "AmberOutline", "CompactMint", "HighContrast", "SoftViolet" })
            {
                Add(ComponentKind.ToggleSwitch, name, () => ExampleStyles.Toggle(name));
                Add(ComponentKind.Checkbox, name, () => ExampleStyles.Checkbox(name));
            }
            foreach (var name in new[] { "Outline", "SoftFilled", "Pill", "DarkInset", "Error" })
            {
                Add(ComponentKind.TextField, name, () => ExampleStyles.Field(name));
                Add(ComponentKind.NumberField, name, () => ExampleStyles.Field(name));
                Add(ComponentKind.Dropdown, name, () => ExampleStyles.Dropdown(name));
            }
            foreach (var name in new[] { "FineLine", "BoldTrack", "AmberRing", "CompactMint", "SoftViolet" })
                Add(ComponentKind.Slider, name, () => ExampleStyles.Slider(name));
            foreach (var name in new[] { "Underline", "Pills", "Segmented", "Outline", "Tiles" })
                Add(ComponentKind.TabBar, name, () => ExampleStyles.Tabs(name));
        }
    }
}
