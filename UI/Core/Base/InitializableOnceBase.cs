/* ----- ----- ----- ----- */
// InitializableOnceBase.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Interfaces;

namespace Engine.UI.Core.Base
{
    /// <summary>
    /// Abstract base class implementing <see cref="IInitializableOnce{TArg}"/> interface.
    /// Ensures that initialization logic only runs once per instance.
    /// </summary>
    /// <typeparam name="TArg">The type of argument passed to the initialization method.</typeparam>
    public abstract class InitializableOnceBase<TArg> : IInitializableOnce<TArg>
    {
        #region Properties

        /// <summary>
        /// Indicates whether the instance has already been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Performs initialization with the provided argument.
        /// If the instance is already initialized, this method does nothing.
        /// </summary>
        /// <param name="arg">The argument required for initialization.</param>
        public void Init(TArg arg)
        {
            if (IsInitialized) 
                return;  // Skip if already initialized

            IsInitialized = true;

            // Call derived class implementation
            OnInit(arg);
        }

        /// <summary>
        /// Abstract method to be implemented by derived classes to provide actual initialization logic.
        /// This method is guaranteed to be called only once.
        /// </summary>
        /// <param name="arg">The argument required for initialization.</param>
        protected abstract void OnInit(TArg arg);

        #endregion
    }
}
