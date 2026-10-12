/* ----- ----- ----- ----- */
// GalleryRoot.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Engine.Mathematics;
using Engine.Platform;
using Engine.Styles;
using Engine.Styles.Catalog;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;
using Engine.UI.Infrastructure;
using Engine.UI.Input;

namespace Engine.UiGallery.UI
{
    internal sealed class GalleryRoot : UIRootNode, IInputHandler
    {
        private readonly StyleCatalog _catalog;
        private readonly GalleryScrollContainer _scroll;
        private readonly List<IDisposable> _resources = new();
        private readonly List<IDisposable> _sampleResources = new();
        private readonly List<UIElementBase> _samples = new();
        private readonly List<GalleryCard> _cards = new();
        private readonly List<UILabel> _names = new();
        private readonly List<(UILabel Label, Color Color)> _sampleLabels = new();
        private readonly List<UIStyledButton> _buttons = new();
        private readonly List<(ComponentKind Kind, UIStyledButton Button)> _tabs = new();
        private readonly IFont _font;
        private readonly IFont _nameFont;
        private bool _enabled = true;
        private bool _light;
        private UIStyledButton _pressed;
        private IMouseEvent _pointer;

        public GalleryRoot(UiFactory factory, IScrollInputHandler scroll, StyleCatalog catalog)
        {
            _factory = factory;
            _catalog = catalog;
            Init();
            _font = Own(StyleHelper.GetFont("NotoSerif", 17f));
            _nameFont = Own(GraphicsBackend.Factory.CreateFont(GraphicsBackend.Factory.GenericSansSerifFontFamily, 15f));
            Label(this, "引擎 UI 樣式圖鑑", 20f, 14f, 240f, 40f);
            int index = 0;
            foreach (var kind in Enum.GetValues<ComponentKind>())
            {
                var tab = Button(this, KindLabel(kind), () => Select(kind), 16f, 72f + index++ * 54f, 255f, 46f);
                _tabs.Add((kind, tab));
            }
            Label(this, "啟用範例", 300f, 16f, 100f, 40f);
            var enabled = _factory.CreateElement<UIToggleSwitch, UIToggleSwitchHandler, UIToggleSwitchRenderer>();
            enabled.IsOn = true;
            enabled.Style = ExampleStyles.Toggle("Ocean");
            Place(enabled, 412f, 21f, 64f, 32f);
            enabled.Handler.ValueChanged = value => SetEnabled(value);
            AddChild(enabled);
            Label(this, "背景", 500f, 16f, 80f, 40f);
            var background = _factory.CreateElement<UIDropdown, UIDropdownHandler, UIDropdownRenderer>();
            background.Font = _font;
            background.Style = ExampleStyles.Dropdown("DarkInset");
            background.SetOptions(new[] { "遊戲深色背景", "淺色背景" });
            background.Handler.SelectionChanged = i => { _light = i == 1; UpdateNames(); };
            Place(background, 588f, 16f, 230f, 40f);
            AddChild(background);
            Label(this, "點擊、切換或輸入以檢視狀態", 835f, 16f, 415f, 40f);
            _scroll = new GalleryScrollContainer(scroll);
            var handler = new UIScrollContainerHandler();
            var renderer = new UIScrollContainerRenderer();
            handler.Init(factory, _scroll);
            renderer.Init(_scroll);
            _scroll.Init(factory, handler, renderer);
            Place(_scroll, 290f, 76f, 970f, 700f);
            AddChild(_scroll);
            Select(ComponentKind.Button);
        }

        private static string KindLabel(ComponentKind kind) => kind switch
        {
            ComponentKind.Button => "按鈕 Buttons",
            ComponentKind.Label => "標籤 Labels",
            ComponentKind.ToggleSwitch => "開關 Toggle switches",
            ComponentKind.Checkbox => "勾選框 Checkboxes",
            ComponentKind.TextField => "輸入欄 Text fields",
            ComponentKind.NumberField => "數字欄 Number fields",
            ComponentKind.Dropdown => "下拉選單 Dropdowns",
            ComponentKind.Slider => "滑桿 Sliders",
            ComponentKind.TabBar => "分頁列 Tab bars",
            ComponentKind.Frame => "外框 Frames",
            ComponentKind.DialogPanel => "對話方塊外框 Dialog panels",
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        private T Own<T>(T resource) where T : IDisposable { _resources.Add(resource); return resource; }
        private T OwnSample<T>(T resource) where T : IDisposable { _sampleResources.Add(resource); return resource; }

        private static void Place(UIElementBase element, float x, float y, float width, float height)
        {
            element.LocalPosition = new Vector2F(x, y);
            element.Size = new Vector2F(width, height);
        }

        private UILabel Label(UIElementBase parent, string text, float x, float y, float width, float height)
        {
            var label = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();
            label.Text = text;
            label.Font = _font;
            label.ForeColor = Color.WhiteSmoke;
            Place(label, x, y, width, height);
            parent.AddChild(label);
            return label;
        }

        private UIStyledButton Button(UIElementBase parent, string text, Action action, float x, float y, float width, float height)
        {
            var button = new UIStyledButton();
            var handler = new UIButtonHandler();
            var renderer = new UIButtonRenderer();
            handler.Init(_factory, button);
            renderer.Init(button);
            button.Init(_factory, handler, renderer);
            button.Text = text;
            button.Style = ExampleStyles.Button("FlatTile");
            handler.Action = action;
            Place(button, x, y, width, height);
            parent.AddChild(button);
            _buttons.Add(button);
            return button;
        }

        private void Select(ComponentKind kind)
        {
            foreach (var child in _scroll.Children.ToArray()) child.Dispose();
            foreach (var resource in _sampleResources) resource.Dispose();
            _sampleResources.Clear();
            _samples.Clear();
            _cards.Clear();
            _names.Clear();
            _sampleLabels.Clear();
            _buttons.RemoveAll(b => b.IsDisposed);
            _pressed = null;
            foreach (var tab in _tabs)
                tab.Button.Style = ExampleStyles.Button(tab.Kind == kind ? "FilledPrimary" : "FlatTile");
            int index = 0;
            foreach (var entry in _catalog.ForKind(kind))
            {
                bool frame = kind is ComponentKind.Frame or ComponentKind.DialogPanel;
                float height = frame ? 255f : 195f;
                var card = new GalleryCard();
                card.Init();
                Place(card, index % 3 * 320f, index / 3 * (height + 16f), 306f, height);
                _scroll.AddChild(card);
                _cards.Add(card);
                object style = entry.Factory();
                if (style is IDisposable owned) OwnSample(owned);
                if (frame)
                {
                    PrepareBox(style);
                    card.SampleStyle = style;
                }
                else
                {
                    var sample = CreateSample(card, kind, style);
                    _samples.Add(sample);
                }
                var name = Label(card, entry.Name, 8f, height - 50f, 290f, 42f);
                name.Font = _nameFont;
                _names.Add(name);
                index++;
            }
            _scroll.ContentHeight = (float)Math.Ceiling(index / 3f) * ((kind is ComponentKind.Frame or ComponentKind.DialogPanel ? 255f : 195f) + 16f);
            _scroll.ScrollVelocity = 0f;
            _scroll.ScrollY = 0f;
            SetEnabled(_enabled);
            UpdateNames();
            PerformLayout();
        }

        private void PrepareBox(object style)
        {
            if (style is SingleBorderRoundedStyle single)
            {
                single.BorderStyle ??= new BorderStyle();
                single.BackgroundBrushFactory ??= new SolidBrushFactory(Color.Transparent);
                single.Font ??= _font;
                single.TextBrush ??= OwnSample(GraphicsBackend.Factory.CreateSolidBrush(Color.CornflowerBlue));
            }
            else if (style is DoubleBorderRoundedStyle doubleBorder)
            {
                doubleBorder.OuterBorder ??= new BorderStyle();
                doubleBorder.InnerBorder ??= new BorderStyle();
                doubleBorder.BackgroundBrushFactory ??= new SolidBrushFactory(Color.Transparent);
                doubleBorder.Font ??= _font;
                doubleBorder.TextBrush ??= OwnSample(GraphicsBackend.Factory.CreateSolidBrush(Color.CornflowerBlue));
            }
            else if (style is InwardCornerDialogStyle inward)
            {
                inward.BorderStyle ??= new BorderStyle();
                inward.BackgroundBrushFactory ??= new SolidBrushFactory(Color.Transparent);
            }
        }

        private UIElementBase CreateSample(GalleryCard card, ComponentKind kind, object style)
        {
            UIElementBase element;
            switch (kind)
            {
                case ComponentKind.Button:
                    var button = Button(card, "按一下", null, 22f, 45f, 262f, 58f);
                    PrepareBox(style);
                    button.Style = (IButtonDrawStyle)style;
                    button.Handler.Action = () => button.Text = button.Text == "按一下" ? "已點擊" : "按一下";
                    return button;
                case ComponentKind.Label:
                    var label = Label(card, "文字樣式範例", 12f, 36f, 282f, 72f);
                    var typography = (LabelStyle)style;
                    label.Font = OwnSample(typography.CreateFont());
                    label.ForeColor = typography.TextColor;
                    label.TextAlign = typography.Alignment;
                    _sampleLabels.Add((label, typography.TextColor));
                    return label;
                case ComponentKind.ToggleSwitch:
                    var toggle = _factory.CreateElement<UIToggleSwitch, UIToggleSwitchHandler, UIToggleSwitchRenderer>();
                    toggle.Style = (ToggleSwitchStyle)style;
                    toggle.IsOn = true;
                    element = toggle;
                    break;
                case ComponentKind.Checkbox:
                    var checkbox = _factory.CreateElement<UICheckbox, UICheckboxHandler, UICheckboxRenderer>();
                    checkbox.Style = (CheckboxStyle)style;
                    checkbox.Font = _font;
                    checkbox.Text = "記住選擇";
                    checkbox.IsChecked = true;
                    element = checkbox;
                    break;
                case ComponentKind.TextField:
                    var field = _factory.CreateElement<UITextField, UITextFieldHandler, UITextFieldRenderer>();
                    field.Style = (TextFieldStyle)style;
                    field.Font = _font;
                    field.Placeholder = "輸入文字";
                    element = field;
                    break;
                case ComponentKind.NumberField:
                    var number = _factory.CreateElement<UINumberField, UINumberFieldHandler, UINumberFieldRenderer>();
                    number.Style = (TextFieldStyle)style;
                    number.Font = _font;
                    number.Min = 0; number.Max = 999; number.Value = 42;
                    element = number;
                    break;
                case ComponentKind.Dropdown:
                    var dropdown = _factory.CreateElement<UIDropdown, UIDropdownHandler, UIDropdownRenderer>();
                    dropdown.Style = (DropdownStyle)style;
                    dropdown.Font = _font;
                    dropdown.SetOptions(new[] { "第一個選項", "第二個選項", "第三個選項", "第四個選項", "第五個選項" });
                    element = dropdown;
                    break;
                case ComponentKind.Slider:
                    var slider = _factory.CreateElement<UISlider, UISliderHandler, UISliderRenderer>();
                    slider.Style = (SliderStyle)style;
                    slider.Font = _font;
                    slider.Value = 45f;
                    element = slider;
                    break;
                case ComponentKind.TabBar:
                    var tabs = _factory.CreateElement<UITabBar, UITabBarHandler, UITabBarRenderer>();
                    tabs.Style = (TabBarStyle)style;
                    tabs.SetTabs(new[] { "第一頁", "第二頁", "第三頁" });
                    element = tabs;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(kind));
            }
            Place(element, 22f, 48f, kind == ComponentKind.ToggleSwitch ? 76f : 262f, 44f);
            card.AddChild(element);
            return element;
        }

        private void SetEnabled(bool enabled)
        {
            _enabled = enabled;
            foreach (var sample in _samples)
            {
                if (!enabled && sample is UIDropdown dropdown) dropdown.Handler.Close();
                sample.IsEnabled = enabled;
            }
            foreach (var card in _cards) card.SamplesEnabled = enabled;
            foreach (var sample in _sampleLabels)
                sample.Label.ForeColor = enabled ? sample.Color : Color.FromArgb((int)(sample.Color.A * 0.4f), sample.Color);
            CancelPointer();
        }

        private void UpdateNames()
        {
            foreach (var name in _names) name.ForeColor = _light ? Color.FromArgb(35, 45, 60) : Color.WhiteSmoke;
        }

        public override void Draw(IGraphics g)
        {
            using var background = GraphicsBackend.Factory.CreateSolidBrush(Color.FromArgb(20, 24, 34));
            g.FillRectangle(background, 0f, 0f, 1280f, 800f);
            using var sampleBackground = GraphicsBackend.Factory.CreateSolidBrush(_light ? Color.FromArgb(247, 248, 252) : Color.Black);
            g.FillRectangle(sampleBackground, 280f, 68f, 1000f, 732f);
            base.Draw(g);
        }

        public void CancelPointer()
        {
            _pressed = null;
            _pointer = null;
            foreach (var button in _buttons) { button.IsHovered = false; button.IsPressed = false; }
        }

        private void UpdatePointer()
        {
            var hovered = _pointer == null ? null : HitTestDeep(_pointer.Location) as UIStyledButton;
            foreach (var button in _buttons)
            {
                button.IsHovered = ReferenceEquals(button, hovered);
                button.IsPressed = button.IsEnabled && ReferenceEquals(button, _pressed) && button.IsHovered;
            }
        }

        bool IInputHandler.OnMouseDown(IMouseEvent e) { _pointer = e; _pressed = HitTestDeep(e.Location) as UIStyledButton; UpdatePointer(); return false; }
        bool IInputHandler.OnMouseMove(IMouseEvent e) { _pointer = e; UpdatePointer(); return false; }
        bool IInputHandler.OnMouseUp(IMouseEvent e) { _pointer = e; _pressed = null; UpdatePointer(); return false; }
        bool IInputHandler.OnMouseWheel(IMouseEvent e) { _pointer = e; UpdatePointer(); return false; }
        bool IInputHandler.OnMouseClick(IMouseEvent e) => false;
        void IInputHandler.EndFrame() => UpdatePointer();

        protected override void DisposeUI()
        {
            base.DisposeUI();
            foreach (var resource in _sampleResources) resource.Dispose();
            foreach (var resource in _resources) resource.Dispose();
        }
    }
}
