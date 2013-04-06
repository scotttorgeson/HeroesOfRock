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

            bool initialized = false;

            model = BasicModelLoad(parm, out initialized);

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
                            effect.Shininess = Shininess;
                            effect.SpecularPower = SpecularPower;
                            part.Effect = effect;
                        }
                        else if (IsBumpMapped && !IsSpecularMapped)
                        {
                            SkinnedBumpedEffect effect = new SkinnedBumpedEffect(content.Load<Effect>("Effects/v2/SkinnedBumped"));
                            effect.Shininess = Shininess;
                            effect.SpecularPower = SpecularPower;
                            part.Effect = effect;
                        }
                        else if (!IsBumpMapped && !IsSpecularMapped)
                        {
                            SkinnedEffect effect = new SkinnedEffect(content.Load<Effect>("Effects/v2/Skinned"));
                            effect.Shininess = Shininess;
                            effect.SpecularPower = SpecularPower;
                            part.Effect = effect;
                        }
                    }
                }
            }

            base.LoadContent(content, parm, stage);
        }

        public override void DrawInstances(Renderer.DrawType technique)
        {
            GraphicsDevice GraphicsDevice = Renderer.Instance.GraphicsDevice;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
                    GraphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the rendering effect.
                    SkinnedEffect effect = (SkinnedEffect)meshPart.Effect;
                    effect.CurrentTechnique = effect.Techniques[(int)technique];

                    if (technique == Renderer.DrawType.Draw)
                    {
                        effect.View = Renderer.Instance.view;
                        effect.Projection = Renderer.Instance.projection;
                        effect.DiffuseTexture = diffuse;
                        Renderer.Instance.sun.SetLights(ref effect);
                        effect.ReceivesShadows = ReceivesShadows;

                        if (effect is SkinnedBumpedEffect)
                        {
                            ((SkinnedBumpedEffect)effect).NormalMapTexture = bumpMap;
                        }

                        if (effect is SkinnedBumpedSpecularEffect)
                        {
                            ((SkinnedBumpedSpecularEffect)effect).SpecularMapTexture = specularMap;
                        }
                    }
                    else
                    {
                        Renderer.Instance.sun.SetLightViewProjection(ref effect);
                    }

                    for (int i = 0; i < DrawList.Count; i++)
                    {
                        SkinnedRModelInstance instance = (SkinnedRModelInstance)DrawList.Data[i];
                        Matrix translation;
                        Matrix.Multiply(ref instance.boneTransforms[mesh.ParentBone.Index], ref instance.RenderTransform, out translation);
                        effect.World = translation;
                        effect.SetBoneTransforms(instance.skeleton);

                        // draw the mesh
                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();

                            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                                    meshPart.NumVertices, meshPart.StartIndex,
                                                                    meshPart.PrimitiveCount);
                        }
                    }
                }
            }
        }
    }
}