using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    public class DirectionalLight
    {
        Vector3 direction;
        Vector4 color;
        float intensity;

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

        public DirectionalLight(Vector3 direction, Vector4 color, float intensity)
        {
            Direction = direction;
            Color = color;
            Intensity = intensity;
        }

        public DirectionalLight(Vector3 direction, Color color, float intensity)
        {
            Direction = direction;
            Color = color.ToVector4();
            Intensity = intensity;
        }
    }
}
