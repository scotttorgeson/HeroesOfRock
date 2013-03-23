using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class DecalEffect : Effect
    {
        #region Effect Parameters

        EffectParameter decalTextureParam;
        EffectParameter worldParam;
        EffectParameter viewParam;
        EffectParameter projectionParam;
        EffectParameter decalMatrixParam;

        EffectParameter ambientLightColorParam;
        EffectParameter lightViewProjsParam;
        EffectParameter shadowMapParam;
        EffectParameter clipPlanesParam;
        EffectParameter worldInverseTransposeParam;
        EffectParameter texelSizeParam;

        #endregion

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.Identity;
        Matrix projection = Matrix.Identity;
        Matrix decalMatrix = Matrix.Identity;

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

        public Matrix DecalMatrix
        {
            get { return projection; }

            set
            {
                decalMatrix = value;
                decalMatrixParam.SetValue(value);
            }
        }

        public Texture2D DecalTexture
        {
            set { decalTextureParam.SetValue(value); }
        }

        public Vector4 AmbientLightColor
        {
            get { return ambientLightColorParam.GetValueVector4(); }
            set { ambientLightColorParam.SetValue(value); }
        }

        public Texture2D ShadowMap
        {
            get { return shadowMapParam.GetValueTexture2D(); }
            set { shadowMapParam.SetValue(value); }
        }

        public float TexelSize
        {
            get { return texelSizeParam.GetValueSingle(); }
            set { texelSizeParam.SetValue(value); }
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

        public DecalEffect(Effect effect)
            : base(effect)
        {
            CacheEffectParameters();
        }

        void CacheEffectParameters()
        {
            worldParam = Parameters["World"];
            viewParam = Parameters["View"];
            projectionParam = Parameters["Projection"];
            decalMatrixParam = Parameters["DecalMatrix"];
            decalTextureParam = Parameters["DecalTexture"];

            ambientLightColorParam = Parameters["AmbientLightColor"];
            worldInverseTransposeParam = Parameters["WorldInverseTranspose"];
            lightViewProjsParam = Parameters["LightViewProjs"];
            shadowMapParam = Parameters["ShadowMap"];
            clipPlanesParam = Parameters["ClipPlanes"];
            texelSizeParam = Parameters["texelSize"];
        }

        protected override void OnApply()
        {
            worldInverseTransposeParam.SetValue(Matrix.Transpose(Matrix.Invert(world)));
        }
    }
}
