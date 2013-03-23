using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class BumpedSpecularEffect : BumpedEffect
    {
        #region Effect Parameters

        EffectParameter specularMapTextureParam;

        #endregion

        public Texture2D SpecularMapTexture
        {
            get { return specularMapTextureParam.GetValueTexture2D(); }
            set { specularMapTextureParam.SetValue(value); }
        }

        public BumpedSpecularEffect(Effect effect)
            : base(effect)
        {
            CacheEffectParameters();
        }

        void CacheEffectParameters()
        {
            specularMapTextureParam = Parameters["SpecularMap"];
        }
    }
}
