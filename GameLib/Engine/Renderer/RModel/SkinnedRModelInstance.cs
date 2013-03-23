using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    /// <summary>
    /// An encloser for an XNA model that we will use that includes support for
    /// bones, animation, and some manipulations.
    /// </summary>
    public class SkinnedRModelInstance : RModelInstance
    {
        /// <summary>
        /// The model bones
        /// </summary>
        private Bone[] bones;

        /// <summary>
        /// An associated animation clip player
        /// </summary>
        private AnimationPlayer player = null;

        /// <summary>
        /// The underlying bones for the model
        /// </summary>
        public Bone[] Bones { get { return bones; } }

        /// <summary>
        /// The model animation clips
        /// </summary>
        public List<AnimationClip> Clips { get { return ((SkinnedRModel)model).modelExtra.Clips; } }

        /// <summary>
        /// Get the bones from the model and create a bone class object for
        /// each bone. We use our bone class to do the real animated bone work.
        /// </summary>
        private void ObtainBones()
        {
            ModelBoneCollection modelBones = model.Model.Bones;
            bones = new Bone[modelBones.Count];
            for (int i = 0; i < modelBones.Count; i++)
                bones[i] = new Bone(modelBones[i].Name, modelBones[i].Transform, modelBones[i].Parent != null ? bones[modelBones[i].Parent.Index] : null);

            boneTransforms = new Matrix[Bones.Length];
            skeleton = new Matrix[((SkinnedRModel)model).modelExtra.Skeleton.Count];
            bonesNeedUpdating = true;
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            foreach (Bone bone in Bones)
            {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name">name of bone to find</param>
        /// <param name="substring">true if bone name is a substring of full bone name</param>
        /// <returns></returns>
        public Bone FindBone(string name, bool substring)
        {
            foreach (Bone bone in Bones)
            {
                if (substring)
                {
                    if (bone.Name.Contains(name))
                        return bone;
                }
                else
                {
                    if (bone.Name == name)
                        return bone;
                }
            }

            return null;
        }

        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip)
        {
            // Create a clip player and assign it to this model
            player = new AnimationPlayer(clip, this);
            return player;
        }

        /// <summary>
        /// Sets the animation player
        /// </summary>
        /// <param name="player"></param>
        public void SetAnimationPlayer(AnimationPlayer player)
        {
            this.player = player;
        }

        /// <summary>
        /// Update animation for the model.
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateAnimations(float dt)
        {
            if (player != null)
            {
                player.Update(dt);
            }
        }

        public Matrix[] boneTransforms;
        public Matrix[] skeleton;
        private bool bonesNeedUpdating;

        public void UpdateBones()
        {
            if (bonesNeedUpdating)
            {
                if ( player != null )
                    player.SetBonePositions();

                for (int i = 0; i < Bones.Length; i++)
                {
                    Bone bone = Bones[i]; // does this copy the bone?
                    bone.ComputeAbsoluteTransform();

                    boneTransforms[i] = bone.AbsoluteTransform;  // does this copy the transform?
                }

                ModelExtra modelExtra = ((SkinnedRModel)model).modelExtra;
                for (int s = 0; s < modelExtra.Skeleton.Count; s++)
                {
                    Bone bone = Bones[modelExtra.Skeleton[s]];

                    Matrix.Multiply(ref bone.SkinTransform, ref bone.AbsoluteTransform, out skeleton[s]);
                }

                bonesNeedUpdating = false;
            }
        }        

        public SkinnedRModelInstance(ParameterSet parm)
            : base(parm)
        {
        }

        public override void LoadContent(ContentManager content, ParameterSet parm, Stage stage)
        {
            base.LoadContent(content, parm, stage);
            ObtainBones();
            UpdateBones();
        }

        public override void Draw(ref GraphicsDevice graphics, Renderer.DrawType technique)
        {
            ((SkinnedRModel)model).SetupDraw(ref boneTransforms, ref skeleton);
            base.Draw(ref graphics, technique);
        }

        private float RenderPosition = 0.0f;
        public override void SaveRenderData()
        {
            base.SaveRenderData();

            if (player != null)
            {
                float position = player.Position;
                if (position != RenderPosition)
                {
                    RenderPosition = position;
                    bonesNeedUpdating = true;
                }
            }
        }
    }
}
