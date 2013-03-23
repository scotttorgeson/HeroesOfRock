using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    public class PointLight
    {
        Vector3 position;
        float radius;
        Vector4 color;
        float intensity;
        RenderTargetCube shadowMap;
        bool isWithShadows;
        int shadowMapResolution;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
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
            get { return (1.0f / (20 * radius)); }
        }

        public RenderTargetCube ShadowMap
        {
            get { return shadowMap; }
        }

        public Matrix World
        {
            get
            {
                Matrix scale = Matrix.CreateScale(radius / 100.0f);
                Matrix translation = Matrix.CreateTranslation(position);
                return scale * translation;
            }
        }

        public PointLight(GraphicsDevice GraphicsDevice, Vector3 position, float radius, Vector4 color, float intensity, bool isWithShadows, int shadowMapResolution)
        {
            Position = position;
            Radius = radius;
            Color = color;
            Intensity = intensity;
            IsWithShadows = isWithShadows;
            this.shadowMapResolution = shadowMapResolution;
            shadowMap = new RenderTargetCube(GraphicsDevice, shadowMapResolution, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
        }
    }
}
