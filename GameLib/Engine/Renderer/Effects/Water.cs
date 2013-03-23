using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class Water : Effect
    {
        #region Effect Parameters

        EffectParameter shininessParam;
        EffectParameter specularPowerParam;
        EffectParameter worldParam;
        EffectParameter viewParam;
        EffectParameter projectionParam;
        EffectParameter worldInverseTransposeParam;
        EffectParameter ambientLightColorParam;
        EffectParameter lightDirectionParam;
        EffectParameter waterBumpMapParam;
        EffectParameter waterColorParam;
        EffectParameter timeParam;
        EffectParameter waterBaseParam;

        #endregion

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.Identity;
        Matrix projection = Matrix.Identity;

        public Matrix World
        {
            get { return world; }

            set
            {
                world = value;
                worldParam.SetValue(value);
            }
        }

        public Matrix View
        {
            get { return view; }

            set
            {
                view = value;
                viewParam.SetValue(value);
            }
        }


        public Matrix Projection
        {
            get { return projection; }

            set
            {
                projection = value;
                projectionParam.SetValue(value);
            }
        }

        public float Shininess
        {
            get { return shininessParam.GetValueSingle(); }
            set { shininessParam.SetValue(value); }
        }


        public float SpecularPower
        {
            get { return specularPowerParam.GetValueSingle(); }
            set { specularPowerParam.SetValue(value); }
        }

        public Vector3 LightDirection
        {
            get { return lightDirectionParam.GetValueVector3(); }
            set { lightDirectionParam.SetValue(value); }
        }

        public Vector4 AmbientLightColor
        {
            get { return ambientLightColorParam.GetValueVector4(); }
            set { ambientLightColorParam.SetValue(value); }
        }

        public Texture2D WaterBumpMap
        {
            get { return waterBumpMapParam.GetValueTexture2D(); }
            set { waterBumpMapParam.SetValue(value); }
        }

        public Vector4 WaterColor
        {
            get { return waterColorParam.GetValueVector4(); }
            set { waterColorParam.SetValue(value); }
        }

        public Vector4 WaterBase
        {
            get { return waterBaseParam.GetValueVector4(); }
            set { waterBaseParam.SetValue(value); }
        }

        public float Time
        {
            get { return timeParam.GetValueSingle(); }
            set { timeParam.SetValue(value); }
        }

        public Water(Effect effect)
            : base(effect)
        {
            CacheEffectParameters();

            // some defaults
            SpecularPower = 32.0f;
            Shininess = 2.0f;
            WaterColor = new Vector4(18.0f / 255.0f, 39.0f / 255.0f, 63.0f / 255.0f, 1.0f); // clean
            //WaterColor = new Vector4(17.0f / 255.0f, 13.0f / 255.0f, 4.0f / 255.0f, 1.0f); // dirty
            //WaterColor = new Vector4(53.0f / 255.0f, 11.0f / 255.0f, 9.0f / 255.0f, 1.0f); // blood

            WaterBase = new Vector4(0.6f, 0.6f, 0.6f, 1.0f);

            //WaterBumpMap = Stage.Content.Load<Texture2D>("water_normals");
            WaterBumpMap = Stage.Content.Load<Texture2D>("waterNormals");
            //WaterBumpMap = Stage.Content.Load<Texture2D>("waterbump");
        }

        void CacheEffectParameters()
        {
            shininessParam = Parameters["Shininess"];
            specularPowerParam = Parameters["SpecularPower"];
            worldParam = Parameters["World"];
            viewParam = Parameters["View"];
            projectionParam = Parameters["Projection"];
            worldInverseTransposeParam = Parameters["WorldInverseTranspose"];
            ambientLightColorParam = Parameters["AmbientLightColor"];
            lightDirectionParam = Parameters["theLightDirection"];
            waterBumpMapParam = Parameters["NormalMap"];
            waterColorParam = Parameters["WaterColor"];
            waterBaseParam = Parameters["WaterBase"];
            timeParam = Parameters["Time"];
        }

        protected override void OnApply()
        {
            worldInverseTransposeParam.SetValue(Matrix.Transpose(Matrix.Invert(world)));
            Time = Stage.ActiveStage.Time;
        }
    }
}
