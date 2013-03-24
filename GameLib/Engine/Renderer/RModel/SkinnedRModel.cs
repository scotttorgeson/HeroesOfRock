using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class SkinnedRModel : RModel
    {
        /// <summary>
        /// Extra data associated with the XNA model
        /// </summary>
        public ModelExtra modelExtra = null;

        public SkinnedRModel(ParameterSet parm)
            : base(parm)
        {
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, ParameterSet parm, Stage stage)
        {
            if (contentLoaded)
                return;

            Texture2D diffuse = null;
            Texture2D bumpMap = null;
            Texture2D specularMap = null;
            bool initialized = false;

            model = BasicModelLoad(parm, out initialized, out diffuse, out bumpMap, out specularMap);

            this.modelExtra = model.Tag as ModelExtra;
            System.Diagnostics.Debug.Assert(modelExtra != null);

            if (!initialized)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        if (IsBumpMapped && IsSpecularMapped)
                        {
                            SkinnedBumpedSpecularEffect effect = new SkinnedBumpedSpecularEffect(content.Load<Effect>("Effects/v2/SkinnedWithSpecular"));
                            effect.DiffuseTexture = diffuse;
                            effect.NormalMapTexture = bumpMap;
                            effect.SpecularMapTexture = specularMap;
                            effect.Shininess = Shininess;
                            effect.SpecularPower = SpecularPower;
                            part.Effect = effect;
                        }
                        else if (IsBumpMapped && !IsSpecularMapped)
                        {
                            SkinnedBumpedEffect effect = new SkinnedBumpedEffect(content.Load<Effect>("Effects/v2/SkinnedBumped"));
                            effect.DiffuseTexture = diffuse;
                            effect.NormalMapTexture = bumpMap;
                            effect.Shininess = Shininess;
                            effect.SpecularPower = SpecularPower;
                            part.Effect = effect;
                        }
                        else if (!IsBumpMapped && !IsSpecularMapped)
                        {
                            SkinnedEffect effect = new SkinnedEffect(content.Load<Effect>("Effects/v2/Skinned"));
                            effect.DiffuseTexture = diffuse;
                            effect.Shininess = Shininess;
                            effect.SpecularPower = SpecularPower;
                            part.Effect = effect;
                        }
                    }
                }
            }

            base.LoadContent(content, parm, stage);
        }

        private Matrix[] boneTransforms;
        private Matrix[] skeleton;

        public void SetupDraw(ref Matrix[] boneTransforms, ref Matrix[] skeleton)
        {
            this.boneTransforms = boneTransforms;
            this.skeleton = skeleton;
        }

        public override void Draw(ref GraphicsDevice graphics, ref Matrix world, Renderer.DrawType technique)
        {   
            foreach (ModelMesh modelMesh in model.Meshes)
            {
                if (IsBumpMapped && IsSpecularMapped)
                {
                    // bumped and specular mapped
                    foreach (SkinnedBumpedSpecularEffect seffect in modelMesh.Effects) // todo: creates garbage because it casts effect up
                    {
                        seffect.CurrentTechnique = seffect.Techniques[(int)technique];
                        Matrix translation;
                        Matrix.Multiply(ref boneTransforms[modelMesh.ParentBone.Index], ref world, out translation);
                        seffect.World = translation;
                        seffect.SetBoneTransforms(skeleton);

                        if (technique == Renderer.DrawType.Draw)
                        {
                            seffect.View = Renderer.Instance.view;
                            seffect.Projection = Renderer.Instance.projection;
                            BaseEffect asBase = seffect as BaseEffect;
                            Renderer.Instance.sun.SetLights(ref asBase);
                            seffect.ReceivesShadows = ReceivesShadows;
                        }
                        else
                        {
                            BaseEffect asBase = seffect as BaseEffect;
                            Renderer.Instance.sun.SetLightViewProjection(ref asBase);
                        }
                    }
                }
                else if (IsBumpMapped)
                {
                    // bumped, but not specular mapped
                    foreach (SkinnedBumpedEffect seffect in modelMesh.Effects) // todo: creates garbage because it casts effect up
                    {
                        seffect.CurrentTechnique = seffect.Techniques[(int)technique];
                        Matrix translation;
                        Matrix.Multiply(ref boneTransforms[modelMesh.ParentBone.Index], ref world, out translation);
                        seffect.World = translation;
                        seffect.SetBoneTransforms(skeleton);

                        if (technique == Renderer.DrawType.Draw)
                        {
                            seffect.View = Renderer.Instance.view;
                            seffect.Projection = Renderer.Instance.projection;
                            BaseEffect asBase = seffect as BaseEffect;
                            Renderer.Instance.sun.SetLights(ref asBase);
                            seffect.ReceivesShadows = ReceivesShadows;
                        }
                        else
                        {
                            BaseEffect asBase = seffect as BaseEffect;
                            Renderer.Instance.sun.SetLightViewProjection(ref asBase);
                        }
                    }
                }
                else
                {
                    // neither bumped, nor specular mapped
                    foreach (SkinnedEffect seffect in modelMesh.Effects) // todo: creates garbage because it casts effect up
                    {
                        seffect.CurrentTechnique = seffect.Techniques[(int)technique];
                        Matrix translation;
                        Matrix.Multiply(ref boneTransforms[modelMesh.ParentBone.Index], ref world, out translation);
                        seffect.World = translation;
                        seffect.SetBoneTransforms(skeleton);

                        if (technique == Renderer.DrawType.Draw)
                        {
                            seffect.View = Renderer.Instance.view;
                            seffect.Projection = Renderer.Instance.projection;
                            BaseEffect asBase = seffect as BaseEffect;
                            Renderer.Instance.sun.SetLights(ref asBase);
                            seffect.ReceivesShadows = ReceivesShadows;
                        }
                        else
                        {
                            BaseEffect asBase = seffect as BaseEffect;
                            Renderer.Instance.sun.SetLightViewProjection(ref asBase);
                        }
                    }
                }

                modelMesh.Draw();
            }
        }

        public override void DrawInstances(Renderer.DrawType technique)
        {
        }
    }
}