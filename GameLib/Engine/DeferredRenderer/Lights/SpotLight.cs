using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    public class SpotLight
    {
        Vector3 position;
        Vector3 direction;
        Vector4 color;
        float intensity;
        float nearPlane;
        float farPlane;
        float FOV;
        bool isWithShadows;
        int shadowMapResolution;
        float depthBias;
        Matrix world;
        Matrix view;
        Matrix projection;
        RenderTarget2D shadowMap;
        Texture2D attenuationTexture;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; direction.Normalize(); }
        }

        public Vector4 Color
        {
            get { return color; }
            set { color = value; }
        }

        public float Intensity
        {
            get { return intensity; }
            set { intensity = value; }
        }

        public float NearPlane
        {
            get { return nearPlane; }
        }

        public float FarPlane
        {
            get { return farPlane; }
        }

        public float FieldOfView
        {
            get { return FOV; }
        }

        public bool IsWithShadows
        {
            get { return isWithShadows; }
            set { isWithShadows = value; }
        }

        public int ShadowMapResolution
        {
            get
            {
                if (shadowMapResolution < 2048)
                    return shadowMapResolution;
                else
                    return 2048;
            }
        }

        public float DepthBias
        {
            get { return depthBias; }
            set { depthBias = value; }
        }

        public Matrix World
        {
            get { return world; }
        }

        public Matrix View
        {
            get { return view; }
        }

        public Matrix Projection
        {
            get { return projection; }
        }

        public RenderTarget2D ShadowMap
        {
            get { return shadowMap; }
        }

        public Texture2D AttenuationTexture
        {
            get { return attenuationTexture; }
            set { attenuationTexture = value; }
        }

        public SpotLight(GraphicsDevice GraphicsDevice, Vector3 position, Vector3 direction, Vector4 color, float intensity, bool isWithShadows, int shadowMapResolution, Texture2D attenuationTexture)
        {
            Position = position;
            Direction = direction;
            Color = color;
            Intensity = intensity;
            nearPlane = 1.0f;
            farPlane = 100.0f;
            FOV = MathHelper.PiOver2;
            IsWithShadows = isWithShadows;
            this.shadowMapResolution = shadowMapResolution;
            depthBias = 1.0f / 2000.0f;
            projection = Matrix.CreatePerspectiveFieldOfView(FOV, 1.0f, nearPlane, farPlane);
            shadowMap = new RenderTarget2D(GraphicsDevice, shadowMapResolution, shadowMapResolution, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
            this.attenuationTexture = attenuationTexture;
            Update();
        }

        /// <summary>
        /// Calculate the Cosine of the light angle
        /// </summary>
        /// <returns></returns>
        public float LightAngleCos()
        {
            // float coneAngle = 2 * atanf(Radius / Height);
            float coneAngle = FOV;
            return (float)Math.Cos((double)coneAngle);
        }

        public void Update()
        {
            Vector3 target = position + direction;

            if (target == Vector3.Zero) target = -Vector3.Up; // zero is no good

            Vector3 up = Vector3.Cross(direction, Vector3.Up);
            if (up == Vector3.Zero)
                up = Vector3.Right;
            else
                up = Vector3.Up;

            view = Matrix.CreateLookAt(position, target, up);

            // scaling factor
            float radial = (float)Math.Tan((double)FOV / 2.0) * 2 * farPlane;
            Matrix scaling = Matrix.CreateScale(radial, radial, farPlane);
            Matrix translation = Matrix.CreateTranslation(position);
            Matrix inverseView = Matrix.Invert(view);
            Matrix semiProduct = scaling * inverseView;

            // decompose semi-product
            Vector3 S;
            Vector3 P;
            Quaternion Q;
            semiProduct.Decompose(out S, out Q, out P);

            Matrix rotation = Matrix.CreateFromQuaternion(Q);
            world = scaling * rotation * translation;
        }
    }
}
