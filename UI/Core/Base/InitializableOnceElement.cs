/* ----- ----- ----- ----- */
// InitializableOnceElement.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

namespace Engine.UI.Core.Base
{
    /// <summary>
    /// Abstract UI element that can be initialized once with an argument.
    /// Extends <see cref="UIElement"/> and implements <see cref="IInitializableOnce{TArg}"/> and <see cref="IDisposable"/>.
    /// Ensures that initialization logic runs only once and provides a standard disposal mechanism.
    /// </summary>
    /// <typeparam name="TArg">The type of argument passed during initialization.</typeparam>
    public abstract class InitializableOnceElement<TArg> : UIElement, IInitializableOnce<TArg>
    {
        #region Fields

        /// <summary>
        /// Tracks whether this element has been initialized.
        /// </summary>
        private bool _isInitialized = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this element has already been initialized.
        /// </summary>
        public bool IsInitialized => _isInitialized;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this element with the given argument.
        /// If already initialized, this method does nothing.
        /// </summary>
        /// <param name="arg">The argument required for initialization.</param>
        public void Init(TArg arg)
        {
            if (_isInitialized) 
                return;  // Skip initialization if already initialized

            _isInitialized = true;

            // Call derived class implementation for actual initialization
            OnInit(arg);
        }

        /// <summary>
        /// Derived classes must implement this method to provide the actual initialization logic.
        /// Guaranteed to be called only once.
        /// </summary>
        /// <param name="arg">The argument required for initialization.</param>
        protected abstract void OnInit(TArg arg);

        #endregion
    }
}
