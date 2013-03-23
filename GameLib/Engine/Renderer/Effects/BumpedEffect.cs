using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class BumpedEffect : BaseEffect
    {
        #region Effect Parameters

        EffectParameter normalMapTextureParam;

        #endregion

        public Texture2D NormalMapTexture
        {
            get { return normalMapTextureParam.GetValueTexture2D(); }
            set { normalMapTextureParam.SetValue(value); }
        }

        public BumpedEffect(Effect effect)
            : base(effect)
        {
            CacheEffectParameters();
        }

        void CacheEffectParameters()
        {
            normalMapTextureParam = Parameters["NormalMap"];
        }
    }
}
