/* ----- ----- ----- ----- */
// AppControl.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Platform
{
    /// <summary>
    /// Application lifecycle hook, pushed in once by the composition root
    /// (e.g. <c>Launcher/Program.cs</c>) — the WinForms backend sets this to
    /// <c>Application.Exit</c>; a windowing backend would set it to close its
    /// own window/message loop.
    /// </summary>
    public static class AppControl
    {
        public static Action ExitCallback { get; set; }
    }
}
