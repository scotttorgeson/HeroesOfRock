using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    public class CameraQB : Quarterback
    {
        public const float Speed = 5.0f;
        public const float NearClip = 0.1f;
        public const float FarClip = 1000.0f;
        public const float ClipRange = FarClip - NearClip;

        InputAction toggleCamera;

        public static Matrix DefaultProjectionMatrix;
        public static Matrix ViewMatrix;
        public static Matrix ProjectionMatrix;
        public static Matrix WorldMatrix;

        public Camera ActiveCamera { get { return cameras[freeCamEnabled ? 1 : 0]; } }

        public override string Name()
        {
            return "CameraQB";
        }

        FreeCamera freeCamera;

        public CameraQB()
        {
            DefaultProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Renderer.AspectRatio, NearClip, FarClip);                

            cameras.Add(new Camera());
        }

        private List<Camera> cameras = new List<Camera>(4);
        private float transitionTotalTime = 0.0f;
        private float transitionCurrentTime = 0.0f;

        public void JumpToCamera(Camera camera)
        {
            cameras.Insert(freeCamEnabled ? 1 : 0, camera);
            ViewMatrix = camera.ViewMatrix;
            ProjectionMatrix = camera.ProjectionMatrix;
            transitionTotalTime = 0.0f;
            transitionCurrentTime = 1.0f;
        }

        public void TransitionToCamera(Camera camera, float time)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(time > 0.0f, "CameraQB::TransitionToCamera time must be > 0");
#endif
            cameras.Insert(freeCamEnabled ? 1 : 0, camera);
            transitionTotalTime = time;
            transitionCurrentTime = 0.0f;
        }

        public void PopCamera()
        {
            if (freeCamEnabled)
                cameras.RemoveAt(1); // don't remove the free cam
            else
                cameras.RemoveAt(0);
        }

        public void PopCamera(float time)
        {
            if (freeCamEnabled)
                cameras.RemoveAt(1); // don't remove the free cam
            else
                cameras.RemoveAt(0);
            transitionTotalTime = time;
            transitionCurrentTime = 0.0f;
        }

        public void PopCamera(Type cameraType)
        {
            for (int i = freeCamEnabled ? 1 : 0; i < cameras.Count; i++) // skip the free cam
            {
                if ( cameras[i].GetType().Equals( cameraType ) )
                {
                    cameras.RemoveAt(i);
                    return;
                }
            }
        }

        public void PopCamera(Type cameraType, float time)
        {
            for (int i = freeCamEnabled ? 1 : 0; i < cameras.Count; i++) // skip the free cam
            {
                if (cameras[i].GetType().Equals(cameraType))
                {
                    cameras.RemoveAt(i);
                    transitionTotalTime = time;
                    transitionCurrentTime = 0.0f;
                    return;
                }
            }
        }

        public override void PostLoadInit(ParameterSet Parm)
        {
            toggleCamera = Stage.LoadingStage.GetQB<ControlsQB>().GetInputAction("ToggleCamera");
            freeCamera = new FreeCamera(Vector3.Zero, Speed, -0.1f, 0.0f, DefaultProjectionMatrix, Stage.LoadingStage);
        }

        bool freeCamEnabled = false;
        public bool FreeCamEnabled { get { return freeCamEnabled; } }

        public void StartFreeCam()
        {
#if DEBUG || WINDOWS
            if (!freeCamEnabled)
            {
                // jump to free camera
                freeCamera.Yaw = (float)Math.Acos(Matrix.Invert(cameras[0].ViewMatrix).M33);
                freeCamera.Position = Matrix.Invert(cameras[0].ViewMatrix).Translation;
                JumpToCamera(freeCamera);
                freeCamEnabled = true;
            }
#endif
        }

        public void EndFreeCam()
        {
#if DEBUG || WINDOWS
            if (freeCamEnabled)
            {
                // remove the free camera
                cameras.RemoveAt(0);
                freeCamEnabled = false;
            }
#endif
        }

        public FreeCamera EditorCamera
        {
            get { return freeCamera; }
        }

        public override void Update(float dt)
        {
            if (IsPaused) return;
            if (toggleCamera.IsNewAction)
            {
                if (freeCamEnabled)
                    EndFreeCam();
                else
                    StartFreeCam();
            }

            cameras[0].Update(dt);
            if (freeCamEnabled) // if the free cam is enabled we also update the next camera
                cameras[1].Update(dt);

            if (transitionCurrentTime < transitionTotalTime && !freeCamEnabled)
            {
                transitionCurrentTime += dt;
                transitionCurrentTime = Math.Min(transitionCurrentTime, transitionTotalTime);
                float lerp = transitionCurrentTime / transitionTotalTime;

                Matrix ourProjection = ProjectionMatrix;
                Matrix theirProjection = cameras[0].ProjectionMatrix;
                Matrix.Lerp(ref ourProjection, ref theirProjection, lerp, out ourProjection);
                ProjectionMatrix = ourProjection;

                Matrix ourView = ViewMatrix;
                Matrix theirView = cameras[0].ViewMatrix;
                Matrix.Lerp(ref ourView, ref theirView, lerp, out ourView);
                ViewMatrix = ourView;

                Matrix ourWorld = WorldMatrix;
                Matrix theirWorld = Matrix.Invert(cameras[0].ViewMatrix);
                Matrix.Lerp(ref ourWorld, ref theirWorld, lerp, out ourWorld);
                WorldMatrix = ourWorld;
            }
            else
            {
                ProjectionMatrix = cameras[0].ProjectionMatrix;
                ViewMatrix = cameras[0].ViewMatrix;
                WorldMatrix = Matrix.Invert(cameras[0].ViewMatrix);
            }
        }

        public override void Serialize(ParameterSet parm)
        {
            //TODO::
            //figure out what needs to be updated here

        }

        public override void PauseQB()
        {
            
        }
    }
}
