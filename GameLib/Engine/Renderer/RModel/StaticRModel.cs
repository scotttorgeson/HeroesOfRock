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
                    BaseEffect effect = (BaseEffect)meshPart.Effect;
                    effect.CurrentTechnique = effect.Techniques[(int)technique];

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

                    for (int i = 0; i < DrawList.Count; i++)
                    {
                        // get the transform for the current instance
                        Matrix translation;
                        Matrix.Multiply(ref transforms[mesh.ParentBone.Index], ref DrawList.Data[i].RenderTransform, out translation);
                        effect.World = translation;

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
