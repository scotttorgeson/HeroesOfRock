using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class SkinnedEffect : BaseEffect
    {
        public const int MaxBones = 75;

        #region Effect Parameters

        EffectParameter bonesParam;

        #endregion

        /// <summary>
        /// Sets an array of skinning bone transform matrices.
        /// </summary>
        public void SetBoneTransforms(Matrix[] boneTransforms)
        {
            if ((boneTransforms == null) || (boneTransforms.Length == 0))
                throw new ArgumentNullException("boneTransforms");

            if (boneTransforms.Length > MaxBones)
                throw new ArgumentException();

            bonesParam.SetValue(boneTransforms);
        }


        /// <summary>
        /// Gets a copy of the current skinning bone transform matrices.
        /// </summary>
        public Matrix[] GetBoneTransforms(int count)
        {
            if (count <= 0 || count > MaxBones)
                throw new ArgumentOutOfRangeException("count");

            Matrix[] bones = bonesParam.GetValueMatrixArray(count);

            // Convert matrices from 43 to 44 format.
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].M44 = 1;
            }

            return bones;
        }

        public SkinnedEffect(Effect effect)
            : base(effect)
        {
            CacheEffectParameters();

            Matrix[] identityBones = new Matrix[MaxBones];

            for (int i = 0; i < MaxBones; i++)
            {
                identityBones[i] = Matrix.Identity;
            }

            SetBoneTransforms(identityBones);
        }

        void CacheEffectParameters()
        {
            bonesParam = Parameters["Bones"];
        }
    }
}
