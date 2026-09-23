/* ----- ----- ----- ----- */
// PhysicsRegistry.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2026/09/23
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Engine.Physics
{
    /// <summary>
    /// Global registry for managing all Physics2D instances.
    /// Allows centralized updating and cleanup of physics states.
    /// </summary>
    /// <remarks>
    /// Holds only weak references. A strong reference here would keep every
    /// Physics2D instance permanently reachable from this static field, which
    /// prevents the GC from ever collecting it — and its finalizer, the only
    /// place that used to call <see cref="Unregister"/>, would then never run.
    /// That was an unbounded memory leak: every Physics2D ever created stayed
    /// registered for the lifetime of the process. Entries whose target has
    /// already been collected are swept out lazily as the registry is used.
    /// </remarks>
    public static class PhysicsRegistry
    {
        private static readonly List<WeakReference<Physics2D>> _allPhysics = new();

        /// <summary>
        /// Registers a Physics2D instance into the global registry.
        /// </summary>
        /// <param name="physics">The Physics2D instance to register.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the physics instance is already registered.
        /// </exception>
        public static void Register(Physics2D physics)
        {
            foreach (var weakRef in _allPhysics)
            {
                if (weakRef.TryGetTarget(out var existing) && existing == physics)
                    throw new InvalidOperationException("Physics2D instance already registered.");
            }

            _allPhysics.Add(new WeakReference<Physics2D>(physics));
        }

        /// <summary>
        /// Unregisters a Physics2D instance from the global registry, and
        /// opportunistically sweeps out any other entries whose target has
        /// already been garbage-collected.
        /// </summary>
        /// <param name="physics">The Physics2D instance to remove.</param>
        public static void Unregister(Physics2D physics) =>
            _allPhysics.RemoveAll(weakRef => !weakRef.TryGetTarget(out var target) || target == physics);

        /// <summary>
        /// Gets an enumerable collection of all registered Physics2D instances
        /// that are still alive. Entries whose target has been garbage-collected
        /// are skipped.
        /// </summary>
        /// <returns>An IEnumerable of all active Physics2D instances.</returns>
        public static IEnumerable<Physics2D> GetAll() =>
            _allPhysics
                .Select(weakRef => weakRef.TryGetTarget(out var physics) ? physics : null)
                .Where(physics => physics != null);

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
            foreach (var p in GetAll())
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
            Parallel.ForEach(GetAll(), physics => physics.SmoothUpdate());
        }
    }
}
