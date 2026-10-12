/* ----- ----- ----- ----- */
// StyleCatalog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Styles.Catalog
{
    public enum ComponentKind
    {
        Button, Label, ToggleSwitch, Checkbox, TextField, NumberField,
        Dropdown, Slider, TabBar, Frame, DialogPanel
    }

    /// <summary>A stable identity and a lazy style factory; construction needs no graphics backend.</summary>
    public sealed class StyleEntry
    {
        internal StyleEntry(ComponentKind kind, string name, Func<object> factory, string description)
        {
            Kind = kind;
            Name = name;
            Factory = factory;
            Description = description;
        }

        public ComponentKind Kind { get; }
        public string Name { get; }
        public string Description { get; }
        public Func<object> Factory { get; }
    }

    /// <summary>An extensible catalog. Factories run only when a caller requests a style.</summary>
    public sealed class StyleCatalog
    {
        private readonly List<StyleEntry> _entries = new();
        private readonly Dictionary<ComponentKind, List<StyleEntry>> _byKind =
            Enum.GetValues<ComponentKind>().ToDictionary(k => k, _ => new List<StyleEntry>());
        private readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyList<StyleEntry> All => _entries.AsReadOnly();

        public IReadOnlyList<StyleEntry> ForKind(ComponentKind kind)
        {
            if (!_byKind.TryGetValue(kind, out var entries))
                throw new ArgumentOutOfRangeException(nameof(kind));
            return entries.AsReadOnly();
        }

        /// <summary>Registers a globally unique name. Factories return a style suitable for the kind.</summary>
        public StyleEntry Register(ComponentKind kind, string name, Func<object> factory, string description = null)
        {
            if (!_byKind.ContainsKey(kind))
                throw new ArgumentOutOfRangeException(nameof(kind));
            if (string.IsNullOrWhiteSpace(name) || name != name.Trim())
                throw new ArgumentException("A style name must be non-empty and have no outer whitespace.", nameof(name));
            ArgumentNullException.ThrowIfNull(factory);
            if (!_names.Add(name))
                throw new ArgumentException("The style name is already registered.", nameof(name));
            var entry = new StyleEntry(kind, name, factory, description);
            _entries.Add(entry);
            _byKind[kind].Add(entry);
            return entry;
        }

        /// <summary>Creates an independent catalog containing all engine defaults and examples.</summary>
        public static StyleCatalog CreateDefault()
        {
            var catalog = new StyleCatalog();
            BuiltInStyles.Register(catalog);
            return catalog;
        }
    }
}
