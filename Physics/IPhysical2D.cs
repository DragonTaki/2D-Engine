/* ----- ----- ----- ----- */
// IPhysical2D.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/11
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Engine.Physics
{
    /// <summary>
    /// Represents an object that participates in 2D physics simulation.
    /// Any class that implements this interface must provide a Physics2D instance.
    /// </summary>
    public interface IPhysical2D
    {
        /// <summary>
        /// Gets the Physics2D instance associated with this object.
        /// This instance manages the object's position, velocity, and acceleration.
        /// </summary>
        /// <value>
        /// A <see cref="Physics2D"/> object used for 2D physics calculation.
        /// </value>
        Physics2D Physics { get; }
    }
}
