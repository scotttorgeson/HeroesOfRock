using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class Sun
    {
        public static RenderTarget2D shadowMap = null;

        // light parameters
        private Vector4 lightColor = new Vector4(1, 1, 1, 1);
        private Vector4 ambientLightColor = new Vector4(.5f, .5f, .5f, 1);
        private Vector3 directionToSun = new Vector3(1.0f, 0.0f, 0.0f);

        // const parameters
        private const int SHADOW_MAP_WIDTH_HEIGHT = 1024;
        private const float TexelSize = 1.0f / (float)SHADOW_MAP_WIDTH_HEIGHT;
        private const float ShadowDistance = 1000.0f;
        public const int NUM_CASCADES = 3;
        private static readonly Vector3[] extraBackup = { new Vector3(0.0f, 0.0f, 140.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f) };

        // cascade parameters
        private Matrix[] LightViewProjectionMatrices = new Matrix[NUM_CASCADES];
        private Engine.Utilities.FastFrustum[] LightFrustums = new Engine.Utilities.FastFrustum[NUM_CASCADES];
        private Vector2[] LightClipPlanes = new Vector2[NUM_CASCADES]; // x stores near, y stores far. this is sent to the shader.
        private float[] splitDepthsTmp = new float[NUM_CASCADES + 1];
        private Vector3[] frustumCornersWS = new Vector3[8];
        private Vector3[] frustumCornersVS = new Vector3[8];
        private Vector3[] splitFrustumCornersVS = new Vector3[8];
        private Matrix lightViewProjectionMatrix;

        public Sun(ParameterSet parm)
        {
            if (parm.HasParm("SunColor"))
                lightColor = parm.GetVector4("SunColor");
            if (parm.HasParm("AmbientLightColor"))
                ambientLightColor = parm.GetVector4("AmbientLightColor");
            Vector2 sunAngles = Vector2.Zero;
            if (parm.HasParm("SunAngles"))
                sunAngles = parm.GetVector2("SunAngles");

            if ( shadowMap == null )
                shadowMap = new RenderTarget2D(Renderer.Instance.GraphicsDevice, SHADOW_MAP_WIDTH_HEIGHT * NUM_CASCADES, SHADOW_MAP_WIDTH_HEIGHT, false, SurfaceFormat.Single, DepthFormat.Depth24);
            SetSunDirectionFromSphericalCoords(sunAngles.X, sunAngles.Y);
        }

        public Sun(Vector4 lightColor, Vector4 ambientLightColor, float theta, float phi)
        {
            if (shadowMap == null)
                shadowMap = new RenderTarget2D(Renderer.Instance.GraphicsDevice, SHADOW_MAP_WIDTH_HEIGHT * NUM_CASCADES, SHADOW_MAP_WIDTH_HEIGHT, false, SurfaceFormat.Single, DepthFormat.Depth24);
            SetSunDirectionFromSphericalCoords(theta, phi);
            this.lightColor = lightColor;
            this.ambientLightColor = ambientLightColor;
        }

        public static void UnloadContent()
        {
            if (shadowMap != null)
            {
                shadowMap.Dispose();
                shadowMap = null;
            }
        }

        Matrix CreateLightViewProjectionMatrix(Vector3 lightDir, float farPlane, float minZ, float maxZ, int index)
        {
            for (int i = 0; i < 4; i++)
                splitFrustumCornersVS[i] = frustumCornersVS[i + 4] * (minZ / CameraQB.FarClip);

            for (int i = 4; i < 8; i++)
                splitFrustumCornersVS[i] = frustumCornersVS[i] * (maxZ / CameraQB.FarClip);

            Matrix cameraMat = CameraQB.WorldMatrix;
            Vector3.Transform(splitFrustumCornersVS, ref cameraMat, frustumCornersWS);

            // Matrix with that will rotate in points the direction of the light
            Vector3 cameraUpVector = Vector3.Up;
            if (Math.Abs(Vector3.Dot(cameraUpVector, lightDir)) > 0.9f)
                cameraUpVector = Vector3.Forward;

            Matrix lightRotation = Matrix.CreateLookAt(Vector3.Zero, -lightDir, cameraUpVector);

            // Transform the positions of the corners into the direction of the light
            for (int i = 0; i < frustumCornersWS.Length; i++)
            {
                frustumCornersWS[i] = Vector3.Transform(frustumCornersWS[i], lightRotation);
            }
            
            // Find the smallest box around the points
            Vector3 mins = frustumCornersWS[0], maxes = frustumCornersWS[0];
            for (int i = 1; i < frustumCornersWS.Length; i++)
            {
                Vector3 p = frustumCornersWS[i];
                if (p.X < mins.X) mins.X = p.X;
                if (p.Y < mins.Y) mins.Y = p.Y;
                if (p.Z < mins.Z) mins.Z = p.Z;
                if (p.X > maxes.X) maxes.X = p.X;
                if (p.Y > maxes.Y) maxes.Y = p.Y;
                if (p.Z > maxes.Z) maxes.Z = p.Z;
            }

            BoundingBox _lightBox = new BoundingBox(mins, maxes);
            _lightBox.Min -= extraBackup[index];
            _lightBox.Max += extraBackup[index];

            //bool fixShadowJittering = false;
            //if (fixShadowJittering)
            //{
            //    // I borrowed this code from some forum that I don't remember anymore =/
            //    // We snap the camera to 1 pixel increments so that moving the camera does not cause the shadows to jitter.
            //    // This is a matter of integer dividing by the world space size of a texel
            //    float diagonalLength = (frustumCornersWS[0] - frustumCornersWS[6]).Length();
            //    diagonalLength += 2; //Without this, the shadow map isn't big enough in the world.
            //    float worldsUnitsPerTexel = diagonalLength / (float)SHADOW_MAP_WIDTH_HEIGHT;

            //    Vector3 vBorderOffset = (new Vector3(diagonalLength, diagonalLength, diagonalLength) -
            //                             (_lightBox.Max - _lightBox.Min)) * 0.5f;
            //    _lightBox.Max += vBorderOffset;
            //    _lightBox.Min -= vBorderOffset;

            //    _lightBox.Min /= worldsUnitsPerTexel;
            //    _lightBox.Min.X = (float)Math.Floor(_lightBox.Min.X);
            //    _lightBox.Min.Y = (float)Math.Floor(_lightBox.Min.Y);
            //    _lightBox.Min.Z = (float)Math.Floor(_lightBox.Min.Z);
            //    _lightBox.Min *= worldsUnitsPerTexel;

            //    _lightBox.Max /= worldsUnitsPerTexel;
            //    _lightBox.Max.X = (float)Math.Floor(_lightBox.Max.X);
            //    _lightBox.Max.Y = (float)Math.Floor(_lightBox.Max.Y);
            //    _lightBox.Max.Z = (float)Math.Floor(_lightBox.Max.Z);
            //    _lightBox.Max *= worldsUnitsPerTexel;
            //}

            Vector3 boxSize = _lightBox.Max - _lightBox.Min;
            if (boxSize.X == 0 || boxSize.Y == 0 || boxSize.Z == 0)
                boxSize = Vector3.One;
            Vector3 halfBoxSize = boxSize * 0.5f;

            // The position of the light should be in the center of the back
            // pannel of the box. 
            Vector3 lightPosition = _lightBox.Min + halfBoxSize;
            lightPosition.Z = _lightBox.Min.Z;

            // We need the position back in world coordinates so we transform 
            // the light position by the inverse of the lights rotation
            lightPosition = Vector3.Transform(lightPosition, Matrix.Invert(lightRotation));

            // Create the view matrix for the light
            Matrix lightView = Matrix.CreateLookAt(lightPosition, lightPosition - lightDir, cameraUpVector);

            // Create the projection matrix for the light
            // The projection is orthographic since we are using a directional light
            Matrix lightProjection = Matrix.CreateOrthographic(boxSize.X, boxSize.Y, -boxSize.Z, 0.0f);

            return lightView * lightProjection;
        }

#if TUNE_DEPTH_BIAS
        public static float DepthBias = 0.00262f;
#endif

        public void CreateShadowMap()
        {
#if TUNE_DEPTH_BIAS
            ControlsQB cqb = Stage.ActiveStage.GetQB<ControlsQB>();
            if (cqb.CurrentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.G) && cqb.LastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.G))
                DepthBias += 0.00001f;
            if (cqb.CurrentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.H) && cqb.LastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.H))
                DepthBias -= 0.00001f;
#endif
            GraphicsDevice graphicsDevice = Renderer.Instance.GraphicsDevice;

            //bind the render target
            graphicsDevice.SetRenderTarget(shadowMap);

            //clear it to white, ie, far far away
            graphicsDevice.Clear(Color.White);
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Save current viewport so we can resotre it later
            Viewport restoreViewport = graphicsDevice.Viewport;

            Viewport splitViewport = new Viewport();

            for (int i = 0; i < NUM_CASCADES; i++)
            {
                lightViewProjectionMatrix = LightViewProjectionMatrices[i]; // so we can set the right matrix in the shader later

                // Set the viewport for the current split     
                splitViewport.MinDepth = 0;
                splitViewport.MaxDepth = 1;
                splitViewport.Width = SHADOW_MAP_WIDTH_HEIGHT;
                splitViewport.Height = SHADOW_MAP_WIDTH_HEIGHT;
                splitViewport.X = i * SHADOW_MAP_WIDTH_HEIGHT;
                splitViewport.Y = 0;
                graphicsDevice.Viewport = splitViewport;

                foreach ( RModel rModel in Renderer.Instance.RModels )
                {
                    rModel.DrawInstances(Renderer.DrawType.CreateShadowMap);
                }
                foreach (RModel rModel in Renderer.Instance.AlphaBlendRModels)
                {
                    rModel.DrawInstances(Renderer.DrawType.CreateShadowMap);
                }
            }

            graphicsDevice.Viewport = restoreViewport;
        }

        // run from another thread during update to calc the view projection matrix and do culling
        public void CalcCascades(ref Matrix view, ref Matrix projection, float nearClip, float farClip, float clipRange)
        {
            // Get the corners of the frustum
            BoundingFrustum cameraFrustum = new BoundingFrustum(view * projection);
            cameraFrustum.GetCorners(frustumCornersWS);
            Matrix eyeTransform = view;
            Vector3.Transform(frustumCornersWS, ref eyeTransform, frustumCornersVS);

            // Get camera near and far values
            float near = nearClip;
            float far = MathHelper.Min(farClip, ShadowDistance);

            splitDepthsTmp[0] = near;
            splitDepthsTmp[NUM_CASCADES] = far;

            // Compute cascade split depths
            for (int i = 1; i < splitDepthsTmp.Length - 1; i++)
                splitDepthsTmp[i] = near + (far - near) * (float)Math.Pow((i / (float)NUM_CASCADES), 2);
            //splitDepthsTmp[1] = 35.0f;

            for (int i = 0; i < NUM_CASCADES; i++)
            {
                // Convert the split depths to percent of view range, so we can pass them to shader later
                LightClipPlanes[i].X = (splitDepthsTmp[i] - nearClip) / clipRange;
                LightClipPlanes[i].Y = (splitDepthsTmp[i + 1] - nearClip) / clipRange;

                // Create the view projection matrix for this cascade
                LightViewProjectionMatrices[i] = CreateLightViewProjectionMatrix(directionToSun, far, splitDepthsTmp[i], splitDepthsTmp[i + 1], i);   
                LightFrustums[i] = new Engine.Utilities.FastFrustum(ref LightViewProjectionMatrices[i]);
            }

            Renderer.Instance.DoShadowsCulling(LightFrustums);
        }

        /// <summary>
        /// Set the direction of the sun from spherical coords
        /// </summary>
        /// <param name="theta">theta in degress</param>
        /// <param name="phi">phi in degress</param>
        public void SetSunDirectionFromSphericalCoords(float theta, float phi)
        {
            // spherical coords:
            // r = distance from origin
            // theta = angle from x axis
            // phi = angle from y axis

            // convert to radians
            theta = MathHelper.ToRadians(theta);
            phi = MathHelper.ToRadians(phi);

            // the sun is so far away we just ignore the r...
            directionToSun.X = (float)Math.Sin(theta) * (float)Math.Cos(phi); // x = r sin(theta) cos(phi)
            directionToSun.Y = (float)Math.Cos(theta);                        // y = r cos(theta)
            directionToSun.Z = (float)Math.Sin(theta) * (float)Math.Sin(phi); // z = r sin(theta) sin(phi)            

            // make into unit vector
            directionToSun.Normalize();
        }

        // called when drawing to shadow map
        public void SetLightViewProjection(ref BaseEffect effect)
        {
            effect.LightViewProjection = this.lightViewProjectionMatrix;
        }

        public void SetLightViewProjection(ref SkinnedEffect effect)
        {
            effect.LightViewProjection = this.lightViewProjectionMatrix;
        }

        // called when drawing for real
        public void SetLights(ref BaseEffect effect)
        {
            effect.LightColor = lightColor;
            effect.AmbientLightColor = ambientLightColor;
            effect.LightDirection = directionToSun;
            effect.ShadowMap = shadowMap;
            effect.TexelSize = TexelSize;
            effect.LightViewProjections = this.LightViewProjectionMatrices;
            effect.ClipPlanes = this.LightClipPlanes;
#if TUNE_DEPTH_BIAS
            effect.Parameters["DepthBias"].SetValue(DepthBias);
#endif
        }

        public void SetLights(ref SkinnedEffect effect)
        {
            effect.LightColor = lightColor;
            effect.AmbientLightColor = ambientLightColor;
            effect.LightDirection = directionToSun;
            effect.ShadowMap = shadowMap;
            effect.TexelSize = TexelSize;
            effect.LightViewProjections = this.LightViewProjectionMatrices;
            effect.ClipPlanes = this.LightClipPlanes;
#if TUNE_DEPTH_BIAS
            effect.Parameters["DepthBias"].SetValue(DepthBias);
#endif
        }

        public void SetLights(ref Water water)
        {
            water.AmbientLightColor = ambientLightColor;
            water.LightDirection = directionToSun;
        }

        public void SetLights(ref DecalEffect effect)
        {
            effect.AmbientLightColor = ambientLightColor;
            effect.ShadowMap = shadowMap;
            effect.TexelSize = TexelSize;
            effect.LightViewProjections = this.LightViewProjectionMatrices;
            effect.ClipPlanes = this.LightClipPlanes;
#if TUNE_DEPTH_BIAS
            effect.Parameters["DepthBias"].SetValue(DepthBias);
#endif
        }
    }
}
