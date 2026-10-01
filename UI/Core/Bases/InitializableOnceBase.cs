/* ----- ----- ----- ----- */
// InitializableOnceBase.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Interfaces;

namespace Engine.UI.Core.Bases
{
    /// <summary>
    /// Abstract base class implementing <see cref="IInitializableOnce"/>.
    /// Ensures that initialization logic only runs once per instance.
    /// </summary>
    public abstract class InitializableOnceBase : IInitializableOnce
    {
        #region Properties

        /// <summary>
        /// Indicates whether the instance has already been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Performs initialization. If the instance is already initialized, this method does nothing.
        /// </summary>
        public void Init()
        {
            if (IsInitialized)
                return;  // Skip if already initialized

            // Set before OnInit so a re-entrant Init call is a no-op, but roll back if
            // OnInit throws - otherwise a failed init could never be retried.
            IsInitialized = true;

            try
            {
                // Call derived class implementation
                OnInit();
            }
            catch
            {
                IsInitialized = false;
                throw;
            }
        }

        /// <summary>
        /// Initialization hook for derived classes. Runs once per successful initialization
        /// (again only if a previous run threw).
        /// </summary>
        protected virtual void OnInit() { }

        #endregion
    }

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

            // Set before OnInit so a re-entrant Init call is a no-op, but roll back if
            // OnInit throws - otherwise a failed init could never be retried.
            IsInitialized = true;

            try
            {
                // Call derived class implementation
                OnInit(arg);
            }
            catch
            {
                IsInitialized = false;
                throw;
            }
        }

        /// <summary>
        /// Initialization hook for derived classes. Runs once per successful initialization
        /// (again only if a previous run threw).
        /// </summary>
        /// <param name="arg">The argument required for initialization.</param>
        protected virtual void OnInit(TArg arg) { }

        #endregion
    }
}
