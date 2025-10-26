/* ----- ----- ----- ----- */
// Physics2D.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/15
// Version: v1.3
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using StarAnimation.Core.Effect;

using Engine.Mathematics;

namespace Engine.Physics
{
    /// <summary>
    /// Handles 2D physics simulation for an object, including position, velocity, acceleration,
    /// and optional boundary and spring/damping effects.
    /// </summary>
    public class Physics2D
    {
        /// <summary>
        /// The base, current and target positions of the object.
        /// </summary>
        public Position Position { get; set; } = new Position();

        /// <summary>
        /// The base, current and target velocities of the object.
        /// </summary>
        public Velocity Velocity { get; set; } = new Velocity();

        /// <summary>
        /// The current and target accelerations of the object.
        /// </summary>
        public Acceleration Acceleration { get; set; } = new Acceleration();

        /// <summary>
        /// A dictionary of externally contributed accelerations indexed by unique effect ID.
        /// </summary>
        public Dictionary<Guid, Vector2F> AccelerationContributions = new();

#nullable enable
        /// <summary>
        /// Optional bounding box within which the object must stay.
        /// </summary>
        public Boundary? Boundary { get; set; } = new Boundary();
#nullable disable

        /// <summary>
        /// Parameters controlling motion behavior such as spring and damping.
        /// </summary>
        public Movement Movement { get; set; } = new Movement();

        #region Settings (Adjustable Parameters)

        private const float AccelerationLerpFactor = 0.05f;
        private const float CalculateThreshold = 0.01f;

        #endregion

        /// <summary>
        /// Initializes and registers a new instance of Physics2D.
        /// </summary>
        public Physics2D()
        {
            PhysicsRegistry.Register(this);
        }

        /// <summary>
        /// Unregisters the Physics2D instance on destruction.
        /// </summary>
        ~Physics2D()
        {
            PhysicsRegistry.Unregister(this);
        }

        /// <summary>
        /// Performs a smooth physics update, modifying position and velocity based on acceleration,
        /// spring, damping, and current motion state.
        /// </summary>
        public void SmoothUpdate()
        {
            // Integrate all sources of acceleration
            Acceleration.Target = Vector2F.Zero;

            AccelerationContributions ??= new Dictionary<Guid, Vector2F>();

            foreach (var accel in AccelerationContributions.Values)
            {
                Acceleration.Target += accel;
            }

            // If position target was set, move towards
            if (Position.HasTarget)
            {

                // Calculate the direction vector from the current position to the target position
                Vector2F delta = Position.Target - Position.Current;

                // Calculate length and normalize direction
                float distance = delta.Length();

                // Spring and Damping logic based on distance to target
                if (distance >= CalculateThreshold)
                {
                    Vector2F direction = delta.Normalize();

                    // Check if spring is enabled
                    if (Movement.CanSpring)
                    {
                        // Apply spring force towards the target position
                        float springForce = distance * Movement.SpringK;
                        Acceleration.Target += direction * springForce;
                    }
                    else
                    {
                        // Without spring, move directly towards the target
                        Acceleration.Target += direction * distance;
                    }

                    // Check if damping is enabled and the object is close to the target
                    if (Movement.CanDamping && distance < CalculateThreshold)
                    {
                        // Apply damping force to slow down as we approach the target
                        float dampingForce = Velocity.Current.Length() * Movement.Damping;
                        Acceleration.Target -= direction * dampingForce;
                    }

                    // Adjust only if the velocity direction deviates from the target direction
                    float angleDifference = Vector2F.AngleBetween(Velocity.Current, direction);

                    if (angleDifference > CalculateThreshold)
                    {
                        // If the velocity direction is not aligned with the target direction, apply corrective acceleration
                        Acceleration.Target = direction * Velocity.Target.Length();
                    }
                    else
                    {
                        // Direction is aligned, stop acceleration
                        Acceleration.Current = Vector2F.Zero;
                    }
                }
                else
                {
                    // When close to the target, stop acceleration to avoid overshooting
                    Acceleration.Target = Vector2F.Zero;

                    // Ensure that the object is precisely at the target position
                    Acceleration.Current = Vector2F.Zero;
                    Velocity.Current = Vector2F.Zero;
                    Position.Current = Position.Target;
                }
            }

            // Interpolate current acceleration towards target acceleration
            Acceleration.Current = Vector2F.Lerp(
                Acceleration.Current,
                Acceleration.Target,
                AccelerationLerpFactor
            );

            // Update velocity
            Velocity.Current += Acceleration.Current;

            // If target-driven movement, check next location before update, prevent "over" the target
            if (Position.HasTarget)
            {
                Vector2F nextPosition = Position.Current + Velocity.Current;
                Vector2F toTargetNow = Position.Target - Position.Current;
                Vector2F toTargetNext = Position.Target - nextPosition;

                // If the direction changes, means already over the target
                if (Vector2F.DotProduct(toTargetNow, toTargetNext) < 0)
                {
                    // Snap to target and stop motion
                    Position.Current = Position.Target;
                    Velocity.Current = Vector2F.Zero;
                    Acceleration.Current = Vector2F.Zero;
                    Acceleration.Target = Vector2F.Zero;
                    return;
                }
            }

            // Update Location
            Position.Current += Velocity.Current;
        }

        /// <summary>
        /// Cleans up invalid physics effects from all registered instances.
        /// </summary>
        public static void CleanupAllPhysicsEffects()
        {
            foreach (var physics in PhysicsRegistry.GetAll())
                physics.CleanupInvalidEffectReferences();
        }

        /// <summary>
        /// Removes acceleration contributions whose effect IDs are no longer active.
        /// </summary>
        private void CleanupInvalidEffectReferences()
        {
            var validIds = EffectInstance.GetAllActiveEffectIds();
            var keysToRemove = AccelerationContributions.Keys
                .Where(id => !validIds.Contains(id))
                .ToList();

            foreach (var id in keysToRemove)
                AccelerationContributions.Remove(id);
        }

        /// <summary>
        /// Constrains the object's current position within the specified boundary if one is set.
        /// </summary>
        public void EnforceBoundaries()
        {
            if (Boundary.Min != null && Boundary.Max != null)
            {
                Position.Current.X = Math.Clamp(Position.Current.X, Boundary.Min.X, Boundary.Max.X);
                Position.Current.Y = Math.Clamp(Position.Current.Y, Boundary.Min.Y, Boundary.Max.Y);
            }
        }
    }

    /// <summary>
    /// Encapsulates the position-related values.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// The base absolute position (X and Y components).
        /// </summary>
        public Vector2F Base { get; set; } = new Vector2F();

        /// <summary>
        /// The current absolute position (X and Y components).
        /// </summary>
        public Vector2F Current { get; set; } = new Vector2F();

        /// <summary>
        /// The target absolute position (X and Y components).
        /// </summary>
        public Vector2F Target { get; set; } = new Vector2F();

        /// <summary>
        /// Determines whether Target-based movement is enabled.
        /// </summary>
        public bool HasTarget { get; set; } = false;

        /// <summary>
        /// Default constructor with all positions set to (0, 0).
        /// </summary>
        public Position() : this(Vector2F.Zero) { }

        /// <summary>
        /// Constructor with a given Vector2F position.
        /// </summary>
        /// <param name="position">The position vector.</param>
        public Position(Vector2F position)
        {
            Base = position;
            Current = position;
            Target = position;
        }

        /// <summary>
        /// Constructor with a given initial position value.
        /// </summary>
        /// <param name="positionX">Initial X position.</param>
        /// <param name="positionY">Initial Y position.</param>
        public Position(float positionX, float positionY)
            : this(new Vector2F(positionX, positionY)) { }

        /// <summary>
        /// Implicitly converts a Vector2F to Position.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        public static implicit operator Position(Vector2F v) =>
            new Position(v);

        /// <summary>
        /// Implicitly converts a PointF to Position.
        /// </summary>
        /// <param name="point">The point to convert.</param>
        public static implicit operator Position(PointF point) =>
            new Position(new Vector2F(point));

        /// <summary>
        /// Implicitly converts a SizeF to Position.
        /// </summary>
        /// <param name="point">The size to convert.</param>
        public static implicit operator Position(SizeF point) =>
            new Position(new Vector2F(point));

        /// <summary>
        /// Reset all positions to (0, 0).
        /// </summary>
        public void Reset()
        {
            Base = Vector2F.Zero;
            Current = Vector2F.Zero;
            Target = Vector2F.Zero;
        }
    }

    /// <summary>
    /// Encapsulates the velocity-related values.
    /// </summary>
    public class Velocity
    {
        /// <summary>
        /// The base velocity (X and Y components).
        /// </summary>
        public Vector2F Base { get; set; } = new Vector2F();

        /// <summary>
        /// The current velocity (X and Y components).
        /// </summary>
        public Vector2F Current { get; set; } = new Vector2F();

        /// <summary>
        /// The target velocity (X and Y components).
        /// </summary>
        public Vector2F Target { get; set; } = new Vector2F();

        /// <summary>
        /// Default constructor with all velocities set to (0, 0).
        /// </summary>
        public Velocity() : this(Vector2F.Zero) { }

        /// <summary>
        /// Constructor with a given Vector2F speed.
        /// </summary>
        /// <param name="speed">The speed vector.</param>
        public Velocity(Vector2F speed)
        {
            Base = speed;
            Current = speed;
            Target = speed;
        }

        /// <summary>
        /// Constructor with a given initial velocity value.
        /// </summary>
        /// <param name="speedX">Initial X speed.</param>
        /// <param name="speedY">Initial Y speed.</param>
        public Velocity(float speedX, float speedY)
            : this(new Vector2F(speedX, speedY)) { }

        /// <summary>
        /// Reset all velocities to (0, 0).
        /// </summary>
        public void Reset()
        {
            Base = Vector2F.Zero;
            Current = Vector2F.Zero;
            Target = Vector2F.Zero;
        }
    }

    /// <summary>
    /// Encapsulates the acceleration-related values.
    /// </summary>
    public class Acceleration
    {
        /// <summary>
        /// The current acceleration (X and Y components).
        /// </summary>
        public Vector2F Current { get; set; } = new Vector2F();

        /// <summary>
        /// The target acceleration (X and Y components).
        /// </summary>
        public Vector2F Target { get; set; } = new Vector2F();

        /// <summary>
        /// Default constructor with all accelerations set to (0, 0).
        /// </summary>
        public Acceleration() : this(Vector2F.Zero) { }

        /// <summary>
        /// Constructor with a given Vector2F acceleration.
        /// </summary>
        /// <param name="accel">The acceleration vector.</param>
        public Acceleration(Vector2F accel)
        {
            Current = accel;
            Target = accel;
        }

        /// <summary>
        /// Constructor with a given initial acceleration value.
        /// </summary>
        /// <param name="speedX">Initial X acceleration.</param>
        /// <param name="speedY">Initial Y acceleration.</param>
        public Acceleration(float accelX, float accelY)
            : this(new Vector2F(accelX, accelY)) { }

        /// <summary>
        /// Reset all accelerations to (0, 0).
        /// </summary>
        public void Reset()
        {
            Current = Vector2F.Zero;
            Target = Vector2F.Zero;
        }
    }

    /// <summary>
    /// Encapsulates the boundary-related values.
    /// </summary>
    public class Boundary
    {
        /// <summary>
        /// The minimum boundary position (X and Y components).
        /// </summary>
        public Vector2F Min { get; set; } = new Vector2F();

        /// <summary>
        /// The maximum boundary position (X and Y components).
        /// </summary>
        public Vector2F Max { get; set; } = new Vector2F();
    }

    /// <summary>
    /// Encapsulates the movement-related values.
    /// </summary>
    public class Movement
    {
        /// <summary>
        /// Spring movement constant.
        /// </summary>
        public float SpringK { get; set; } = 0.9f;

        /// <summary>
        /// Determines when close to target position, whether spring movement is enabled.
        /// </summary>
        public bool CanSpring { get; set; } = false;

        /// <summary>
        /// Damping constant.
        /// </summary>
        public float Damping { get; set; } = 0.2f;

        /// <summary>
        /// Determines when close to target position, whether damping movement is enabled.
        /// </summary>
        public bool CanDamping { get; set; } = false;
    }
}
