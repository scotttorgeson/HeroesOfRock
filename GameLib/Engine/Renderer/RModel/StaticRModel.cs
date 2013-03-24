using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class StaticRModel : RModel
    {
        Matrix[] transforms = null; // for non animated models

        public StaticRModel(ParameterSet parm)
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

            if (!initialized)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        if (IsBumpMapped && IsSpecularMapped)
                        {
                            BumpedSpecularEffect effect = new BumpedSpecularEffect(content.Load<Effect>("Effects/v2/BumpedWithSpecular"));
                            effect.DiffuseTexture = diffuse;
                            effect.NormalMapTexture = bumpMap;
                            effect.SpecularMapTexture = specularMap;
                            effect.Shininess = Shininess;
                            effect.SpecularPower = SpecularPower;
                            part.Effect = effect;
                        }
                        else if (IsBumpMapped && !IsSpecularMapped)
                        {
                            BumpedEffect effect = new BumpedEffect(content.Load<Effect>("Effects/v2/Bumped"));
                            effect.DiffuseTexture = diffuse;
                            effect.NormalMapTexture = bumpMap;
                            effect.Shininess = Shininess;
                            effect.SpecularPower = SpecularPower;
                            part.Effect = effect;
                        }
                        else if (!IsBumpMapped && !IsSpecularMapped)
                        {
                            BaseEffect effect = new BaseEffect(content.Load<Effect>("Effects/v2/Basic"));
                            effect.DiffuseTexture = diffuse;
                            effect.Shininess = Shininess;
                            effect.SpecularPower = SpecularPower;
                            part.Effect = effect;
                        }
                    }
                }
            }

            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            base.LoadContent(content, parm, stage);
        }

        public override void Draw(ref GraphicsDevice graphics, ref Matrix world, Renderer.DrawType technique)
        {
            foreach (ModelMesh modelMesh in model.Meshes)
            {
                if (IsBumpMapped && IsSpecularMapped)
                {
                    // bumped and specular mapped
                    foreach (BumpedSpecularEffect beffect in modelMesh.Effects) // todo: creates garbage because it casts effect up
                    {
                        beffect.CurrentTechnique = beffect.Techniques[(int)technique];
                        Matrix translation;
                        Matrix.Multiply(ref transforms[modelMesh.ParentBone.Index], ref world, out translation);
                        beffect.World = translation;

                        if (technique == Renderer.DrawType.Draw)
                        {
                            beffect.View = Renderer.Instance.view;
                            beffect.Projection = Renderer.Instance.projection;
                            BaseEffect asBase = beffect as BaseEffect;
                            Renderer.Instance.sun.SetLights(ref asBase);
                            beffect.ReceivesShadows = ReceivesShadows;
                        }
                        else
                        {
                            BaseEffect asBase = beffect as BaseEffect;
                            Renderer.Instance.sun.SetLightViewProjection(ref asBase);
                        }
                    }
                }
                else if (IsBumpMapped)
                {
                    // bumped, but not specular mapped
                    foreach (BumpedEffect beffect in modelMesh.Effects) // todo: creates garbage because it casts effect up
                    {
                        beffect.CurrentTechnique = beffect.Techniques[(int)technique];
                        Matrix translation;
                        Matrix.Multiply(ref transforms[modelMesh.ParentBone.Index], ref world, out translation);
                        beffect.World = translation;

                        if (technique == Renderer.DrawType.Draw)
                        {
                            beffect.View = Renderer.Instance.view;
                            beffect.Projection = Renderer.Instance.projection;
                            BaseEffect asBase = beffect as BaseEffect;
                            Renderer.Instance.sun.SetLights(ref asBase);
                            beffect.ReceivesShadows = ReceivesShadows;
                        }
                        else
                        {
                            BaseEffect asBase = beffect as BaseEffect;
                            Renderer.Instance.sun.SetLightViewProjection(ref asBase);
                        }
                    }
                }
                else
                {
                    // not bumped, specular mapped, or animated
                    for (int i = 0; i < modelMesh.Effects.Count; i++)
                    {
                        BaseEffect effect = modelMesh.Effects[i] as BaseEffect;
                        effect.CurrentTechnique = effect.Techniques[(int)technique];
                        Matrix translation;
                        Matrix.Multiply(ref transforms[modelMesh.ParentBone.Index], ref world, out translation);
                        effect.World = translation;

                        // extra work for draw
                        if (technique == Renderer.DrawType.Draw)
                        {
                            effect.View = Renderer.Instance.view;
                            effect.Projection = Renderer.Instance.projection;
                            Renderer.Instance.sun.SetLights(ref effect);
                            effect.ReceivesShadows = ReceivesShadows;
                        }
                        else
                        {
                            Renderer.Instance.sun.SetLightViewProjection(ref effect);
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
