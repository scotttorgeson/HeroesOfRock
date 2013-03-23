using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameLib
{
    public class FreeCamera : Camera
    {   
        private InputAction moveForward;
        private InputAction moveRight;
        private InputAction moveUp;
        private InputAction rotateUp;
        private InputAction rotateRight;
        private InputAction speedMultiplier;

        private float yaw;
        private float goalYaw;
        private float pitch;
        private float rotateScalar;
        private float rotateTime;

        /// <summary>
        /// Gets or sets the yaw rotation of the camera.
        /// </summary>
        public float Yaw
        {
            get { return yaw; }
            set { yaw = MathHelper.WrapAngle(value); }
        }

        /// <summary>
        /// Gets or sets he goal yaw rotation of the camera (value to rotate to).
        /// </summary>
        public float GoalYaw
        {
            get { return goalYaw; }
            set { goalYaw = MathHelper.WrapAngle(value); }
        }

        /// <summary>
        /// Gets or sets the pitch rotation of the camera.
        /// </summary>
        public float Pitch
        {
            get { return pitch; }
            set
            {
                pitch = value;
                if (pitch > MathHelper.PiOver2 * .99f)
                    pitch = MathHelper.PiOver2 * .99f;
                else if (pitch < -MathHelper.PiOver2 * .99f)
                    pitch = -MathHelper.PiOver2 * .99f;
            }
        }

        public float RotateScalar
        {
            get { return rotateScalar; }
            set { rotateScalar = value; }
        }

        public float RotateTime
        {
            get { return rotateTime; }
            set { rotateTime = value; }
        }

        /// <summary>
        /// Gets or sets the speed at which the camera moves.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets the world transformation of the camera.
        /// </summary>
        public Matrix WorldMatrix { get; protected set; }

        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Moves the camera forward.
        /// </summary>
        /// <param name="distance">Distance to move.</param>
        public void MoveForward(float distance)
        {
            Position += WorldMatrix.Forward * distance;
        }

        /// <summary>
        /// Moves the camera to the right.
        /// </summary>
        /// <param name="distance">Distance to move.</param>
        public void MoveRight(float distance)
        {
            Position += WorldMatrix.Right * distance;
        }

        /// <summary>
        /// Moves the camera up.
        /// </summary>
        /// <param name="distance">Distanec to move.</param>
        public void MoveUp(float distance)
        {
            Position += new Vector3(0, distance, 0);
        }

        /// <summary>
        /// Creates a camera.
        /// </summary>
        /// <param name="position">Initial position of the camera.</param>
        /// <param name="speed">Speed of the camera per second.</param>
        /// <param name="pitch">Initial pitch angle of the camera.</param>
        /// <param name="yaw">Initial yaw value of the camera.</param>
        /// <param name="projectionMatrix">Projection matrix used.</param>
        public FreeCamera(Vector3 position, float speed, float pitch, float yaw, Matrix projectionMatrix, Stage stage)
        {
            Position = position;
            Speed = speed;
            Yaw = yaw;
            Pitch = pitch;
            ProjectionMatrix = projectionMatrix;

            ControlsQB controlsQB = stage.GetQB<ControlsQB>();
            moveForward = controlsQB.GetInputAction("MoveForward");
            moveRight = controlsQB.GetInputAction("MoveRight");
            moveUp = controlsQB.GetInputAction("FreeCamMoveUp");
            rotateUp = controlsQB.GetInputAction("FreeCamRotateUp");
            rotateRight = controlsQB.GetInputAction("FreeCamRotateRight");
            speedMultiplier = controlsQB.GetInputAction("FreeCamSpeed");
        }

        /// <summary>
        /// Updates the state of the camera.
        /// </summary>
        /// <param name="dt">Time since the last frame in seconds.</param>
        public override void Update(float dt)
        {
            Yaw += rotateRight.value * -0.75f * dt;
            Pitch += rotateUp.value * 0.75f * dt;

            WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Right, Pitch) * Matrix.CreateFromAxisAngle(Vector3.Up, Yaw);

            float distance = (speedMultiplier.value + 1.0f) * 3.0f * Speed * dt;
            

            MoveForward(moveForward.value * distance);
            MoveRight(moveRight.value * distance);
            MoveUp(moveUp.value * distance);

            WorldMatrix = WorldMatrix * Matrix.CreateTranslation(Position);

            ViewMatrix = Matrix.Invert(WorldMatrix);
        }

        public override string ToString()
        {
            return "FreeCamera";
        }
    }
}
