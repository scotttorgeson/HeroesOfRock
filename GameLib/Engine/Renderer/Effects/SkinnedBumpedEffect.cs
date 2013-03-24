using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class SkinnedBumpedEffect : SkinnedEffect
    {
        #region Effect Parameters

        EffectParameter normalMapTextureParam;

        #endregion

        public Texture2D NormalMapTexture
        {
            get { return normalMapTextureParam.GetValueTexture2D(); }
            set { normalMapTextureParam.SetValue(value); }
        }

        public SkinnedBumpedEffect(Effect effect)
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
