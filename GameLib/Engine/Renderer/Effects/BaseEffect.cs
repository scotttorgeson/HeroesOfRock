using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class BaseEffect : Effect
    {
        #region Effect Parameters

        EffectParameter diffuseTextureParam;
        EffectParameter shininessParam;
        EffectParameter specularPowerParam;
        EffectParameter worldParam;
        EffectParameter viewParam;
        EffectParameter projectionParam;
        EffectParameter worldInverseTransposeParam;
        EffectParameter lightColorParam;
        EffectParameter ambientLightColorParam;
        EffectParameter lightDirectionParam;
        EffectParameter texelSizeParam;
        EffectParameter lightViewProjParam;
        EffectParameter lightViewProjsParam;
        EffectParameter shadowMapParam;
        EffectParameter clipPlanesParam;
        EffectParameter receivesShadowsParam;

        #endregion

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.Identity;
        Matrix projection = Matrix.Identity;
        Matrix lightViewProj = Matrix.Identity;

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

        public Texture2D DiffuseTexture
        {
            get { return diffuseTextureParam.GetValueTexture2D(); }
            set { diffuseTextureParam.SetValue(value); }
        }

        public Texture2D ShadowMap
        {
            get { return shadowMapParam.GetValueTexture2D(); }
            set { shadowMapParam.SetValue(value); }
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

        public Vector4 LightColor
        {
            get { return lightColorParam.GetValueVector4(); }
            set { lightColorParam.SetValue(value); }
        }

        public float TexelSize
        {
            get { return texelSizeParam.GetValueSingle(); }
            set { texelSizeParam.SetValue(value); }
        }

        public Matrix LightViewProjection
        {
            get { return lightViewProj; }

            set
            {
                lightViewProj = value;
                lightViewProjParam.SetValue(value);
            }
        }

        public Matrix[] LightViewProjections
        {
            set
            {
                lightViewProjsParam.SetValue(value);
            }
        }

        public Vector2[] ClipPlanes
        {
            set
            {
                clipPlanesParam.SetValue(value);
            }
        }

        public bool ReceivesShadows
        {
            set { receivesShadowsParam.SetValue(value); }
            get { return receivesShadowsParam.GetValueBoolean(); }
        }

        public BaseEffect(Effect effect)
            : base(effect)
        {
            CacheEffectParameters();

            SpecularPower = 4.0f;
            Shininess = 0.1f;
        }

        void CacheEffectParameters()
        {
            diffuseTextureParam = Parameters["Diffuse"];
            shininessParam = Parameters["Shininess"];
            specularPowerParam = Parameters["SpecularPower"];
            worldParam = Parameters["World"];
            viewParam = Parameters["View"];
            projectionParam = Parameters["Projection"];
            worldInverseTransposeParam = Parameters["WorldInverseTranspose"];
            lightColorParam = Parameters["LightColor"];
            ambientLightColorParam = Parameters["AmbientLightColor"];
            lightDirectionParam = Parameters["theLightDirection"];
            texelSizeParam = Parameters["texelSize"];
            lightViewProjParam = Parameters["LightViewProj"];
            lightViewProjsParam = Parameters["LightViewProjs"];
            shadowMapParam = Parameters["ShadowMap"];
            clipPlanesParam = Parameters["ClipPlanes"];
            receivesShadowsParam = Parameters["receivesShadows"];
        }

        protected override void OnApply()
        {
            worldInverseTransposeParam.SetValue(Matrix.Transpose(Matrix.Invert(world)));
        }
    }
}
