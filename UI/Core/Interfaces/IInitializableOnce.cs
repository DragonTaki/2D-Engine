/* ----- ----- ----- ----- */
// IInitializableOnce.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.UI.Core.Interfaces
{
    /// <summary>
    /// Interface for objects that require one-time initialization.
    /// </summary>
    /// <typeparam name="TArg">
    /// The type of argument required for initialization.
    /// </typeparam>
    /// <remarks>
    /// Implementing this interface allows an object to be initialized exactly once.
    /// The <see cref="Init(TArg)"/> method should contain all setup logic.
    /// <see cref="IsInitialized"/> should return true after successful initialization.
    /// </remarks>
    public interface IInitializableOnce<TArg>
    {
        /// <summary>
        /// Indicates whether the object has already been initialized.
        /// </summary>
        /// <returns>
        /// True if the object has been initialized; otherwise, false.
        /// </returns>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes the object with the provided argument.
        /// Should be called only once; subsequent calls may be ignored or throw an exception.
        /// </summary>
        /// <param name="arg">
        /// The argument used to initialize the object. Its type is specified by the generic parameter <typeparamref name="TArg"/>.
        /// </param>
        void Init(TArg arg);
    }
}
