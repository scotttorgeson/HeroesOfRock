using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLib
{
    public class PlayerCamera : Camera
    {
        public PhysicsObject ChaseTarget
        {
            get { return chaseTarget; }
            set { if (value != null) chaseTarget = value; }
        }

        PhysicsObject chaseTarget;

        /// <summary>
        /// Desired camera position in the world coordinate system.
        /// </summary>
        public Vector3 DesiredPositionOffset
        {
            get { return desiredPositionOffset; }
            set { desiredPositionOffset = value; }
        }
        private Vector3 desiredPositionOffset = new Vector3(0.0f, 5.0f, 20.0f);

        /// <summary>
        /// Desired camera position in world space.
        /// </summary>
        public Vector3 DesiredPosition
        {
            get
            {
                // Ensure correct value even if update has not been called this frame
                UpdateWorldPositions();

                return desiredPosition;
            }
        }
        private Vector3 desiredPosition;

        /// <summary>
        /// Desired look at point in world space.
        /// </summary>
        public Vector3 DesiredLookAt
        {
            get 
            {
                // Ensure correct value even if update has not been called this frame
                UpdateWorldPositions();

                return desiredLookAt; 
            }
        }
        private Vector3 desiredLookAt;

        /// <summary>
        /// Desited look at point in the chased object's coordinate system.
        /// </summary>
        public Vector3 DesiredLookAtOffset
        {
            get { return desiredLookAtOffset; }
            set { desiredLookAtOffset = value; }
        }
        private Vector3 desiredLookAtOffset = new Vector3(0.0f, 3.0f, 0.0f);

        /// <summary>
        /// Look at point in world space.
        /// </summary>
        public Vector3 LookAt
        {
            get { return lookAt; }
        }
        private Vector3 lookAt;

        /// <summary>
        /// Physics coefficient which controls the influence of the camera's position
        /// over the spring force. The stiffer the spring, the closer it will stay to
        /// the chased object.
        /// </summary>
        public float Stiffness
        {
            get { return stiffness; }
            set { stiffness = value; }
        }
        private float stiffness = 1800.0f;

        /// <summary>
        /// Physics coefficient which approximates internal friction of the spring.
        /// Sufficient damping will prevent the spring from oscillating infinitely.
        /// </summary>
        public float Damping
        {
            get { return damping; }
            set { damping = value; }
        }
        private float damping = 600.0f;

        /// <summary>
        /// Mass of the camera body. Heaver objects require stiffer springs with less
        /// damping to move at the same rate as lighter objects.
        /// </summary>
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        private float mass = 50.0f;

        /// <summary>
        /// Position of camera in world space.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
        }
        private Vector3 position;

        /// <summary>
        /// Velocity of camera.
        /// </summary>
        public Vector3 Velocity
        {
            get { return velocity; }
        }
        private Vector3 velocity;

        /// <summary>
        /// Velocity of look at target.
        /// </summary>
        public Vector3 LookAtVelocity
        {
            get { return lookAtVelocity; }
        }
        private Vector3 lookAtVelocity;

        public PlayerCamera(PhysicsObject target, Matrix projectionMatrix)
        {
            chaseTarget = target;
            ProjectionMatrix = projectionMatrix;
        }

        /// <summary>
        /// Rebuilds object space values in world space. Invoke before publicly
        /// returning or privately accessing world space values.
        /// </summary>
        private void UpdateWorldPositions()
        {
            // Calculate desired camera properties in world space
            desiredPosition = chaseTarget.Position + DesiredPositionOffset;
            desiredLookAt = chaseTarget.Position + DesiredLookAtOffset;
        }

        /// <summary>
        /// Rebuilds camera's view and projection matricies.
        /// </summary>
        private void UpdateMatrices()
        {
            ViewMatrix = Matrix.CreateLookAt(this.Position, this.LookAt, this.chaseTarget.TransformMatrix.Up);
        }

        /// <summary>
        /// Forces camera to be at desired position and to stop moving. The is useful
        /// when the chased object is first created or after it has been teleported.
        /// Failing to call this after a large change to the chased object's position
        /// will result in the camera quickly flying across the world.
        /// </summary>
        public void Reset()
        {
            UpdateWorldPositions();

            // Stop motion
            velocity = Vector3.Zero;
            lookAtVelocity = Vector3.Zero;

            // Force desired position
            position = desiredPosition;
            lookAt = desiredLookAt;

            UpdateMatrices();
        }

        /// <summary>
        /// Animates the camera from its current position towards the desired offset
        /// behind the chased object. The camera's animation is controlled by a simple
        /// physical spring attached to the camera and anchored to the desired position.
        /// </summary>
        public override void Update(float dt)
        {
            UpdateWorldPositions();

            // Calculate spring force
            Vector3 stretch = position - desiredPosition;
            Vector3 force = -stiffness * stretch - damping * velocity;

            // Apply acceleration
            Vector3 acceleration = force / mass;
            velocity += acceleration * dt;

            // Apply velocity
            position += velocity * dt;

            stretch = lookAt - desiredLookAt;
            force = -stiffness * stretch - damping * lookAtVelocity;

            acceleration = force / mass;
            lookAtVelocity += acceleration * dt;

            lookAt += lookAtVelocity * dt;

            UpdateMatrices();
        }
    }
}
