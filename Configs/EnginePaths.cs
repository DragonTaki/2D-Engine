/* ----- ----- ----- ----- */
// EnginePaths.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/06
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.IO;

namespace Engine.Configs
{
    /// <summary>
    /// Engine-level system paths (fixed values the player never changes): where the
    /// running app's files are. The single place for these - callers reference them
    /// instead of building the paths from literals. Pure <c>System.IO</c>, no platform
    /// types, so any module (and the Core test harness) can use it.
    /// </summary>
    public static class EnginePaths
    {
        /// <summary>Folder name of the bundled assets, next to the executable.</summary>
        public const string AssetsFolderName = "Assets";

        /// <summary>Subfolder of <see cref="AssetsFolder"/> holding the bundled font files.</summary>
        public const string FontFolderName = "Font";

        /// <summary>Subfolder of <see cref="AssetsFolder"/> holding the language files (<c>Localization.Lang</c>).</summary>
        public const string LangFolderName = "Lang";

        /// <summary>File name of the plain-text log written by <c>LogFileManager</c>.</summary>
        public const string LogFileName = "log.txt";

        /// <summary>The folder the app runs from (where the build copies <c>Assets/</c>).</summary>
        public static string AppBaseDirectory => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary><c>Assets/</c> next to the executable.</summary>
        public static string AssetsFolder => Path.Combine(AppBaseDirectory, AssetsFolderName);

        /// <summary><c>Assets/Font/</c>: the bundled fonts <c>FontManager</c> loads.</summary>
        public static string FontFolder => Path.Combine(AssetsFolder, FontFolderName);

        /// <summary>The bundled language files: <c>Assets/Lang</c> next to the executable.</summary>
        public static string LangFolder => Path.Combine(AssetsFolder, LangFolderName);

        /// <summary>The log file <c>LogFileManager.SaveLog</c> appends to (next to the executable).</summary>
        public static string LogFilePath => Path.Combine(AppBaseDirectory, LogFileName);
    }
}
