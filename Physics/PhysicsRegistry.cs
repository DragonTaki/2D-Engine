/* ----- ----- ----- ----- */
// PhysicsRegistry.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/11
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Physics
{
    /// <summary>
    /// Global registry for managing all Physics2D instances.
    /// Allows centralized updating and cleanup of physics states.
    /// </summary>
    public static class PhysicsRegistry
    {
        private static readonly HashSet<Physics2D> _allPhysics = new();

        /// <summary>
        /// Registers a Physics2D instance into the global registry.
        /// </summary>
        /// <param name="physics">The Physics2D instance to register.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the physics instance is already registered.
        /// </exception>
        public static void Register(Physics2D physics)
        {
            if (!_allPhysics.Add(physics))
                throw new InvalidOperationException("Physics2D instance already registered.");
        }

        /// <summary>
        /// Unregisters a Physics2D instance from the global registry.
        /// </summary>
        /// <param name="physics">The Physics2D instance to remove.</param>
        public static void Unregister(Physics2D physics) => _allPhysics.Remove(physics);


        /// <summary>
        /// Gets an enumerable collection of all registered Physics2D instances.
        /// </summary>
        /// <returns>An IEnumerable of all active Physics2D instances.</returns>
        public static IEnumerable<Physics2D> GetAll() => _allPhysics;


        /// <summary>
        /// Updates all registered Physics2D instances by invoking their SmoothUpdate method.
        /// This method runs on the main thread.
        /// </summary>
        /// <remarks>
        /// Effect cleanup (<see cref="Physics2D.CleanupAllPhysicsEffects"/>) is
        /// not called from here — it needs a caller-supplied set of still-valid
        /// effect IDs, which only the effect owner (e.g. StarAnimation's
        /// <c>StarController.Update</c>) has. Calling it here too used to just
        /// duplicate that same-frame cleanup a second time.
        /// </remarks>
        public static void UpdateAll()
        {
            foreach (var p in _allPhysics)
            {
                p.SmoothUpdate();
            }
        }

        /// <summary>
        /// Updates all registered Physics2D instances in parallel using multiple threads.
        /// Use this method only if thread-safety is guaranteed for all physics operations.
        /// </summary>
        public static void ParallelUpdateAll()
        {
            Parallel.ForEach(_allPhysics, physics => physics.SmoothUpdate());
        }
    }
}
