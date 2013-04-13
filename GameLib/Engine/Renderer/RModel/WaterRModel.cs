using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class WaterRModel : RModel
    {
        Matrix[] transforms = null; // for non animated models

        public WaterRModel(ParameterSet parm)
            : base(parm)
        {
            parm.AddParm("ApplyDecals", "false");
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, ParameterSet parm, Stage stage)
        {
            if (contentLoaded)
                return;

            bool initialized = false;

            model = BasicModelLoad(parm, out initialized);

            if (!initialized)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        Water effect = new Water(content.Load<Effect>("Effects/v2/Water"));
                        if (stage.Parm.HasParm("WaterSpecularPower"))
                            effect.SpecularPower = stage.Parm.GetFloat("WaterSpecularPower");
                        if (stage.Parm.HasParm("WaterShininess"))
                            effect.Shininess = stage.Parm.GetFloat("WaterShininess");
                        if (stage.Parm.HasParm("WaterColor"))
                            effect.WaterColor = stage.Parm.GetVector4("WaterColor");
                        if (stage.Parm.HasParm("WaterBase"))
                            effect.WaterBase = stage.Parm.GetVector4("WaterBase");
                        part.Effect = effect;
                    }
                }
            }

            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            base.LoadContent(content, parm, stage);
        }

        public override void DrawInstances(Renderer.DrawType technique)
        {
            if (technique == Renderer.DrawType.Draw)
            {
                GraphicsDevice GraphicsDevice = Renderer.Instance.GraphicsDevice;

                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
                        GraphicsDevice.Indices = meshPart.IndexBuffer;

                        // Set up the rendering effect.
                        Water effect = (Water)meshPart.Effect;
                        effect.CurrentTechnique = effect.Techniques[(int)technique];

                        effect.View = Renderer.Instance.view;
                        effect.Projection = Renderer.Instance.projection;
                        Renderer.Instance.sun.SetLights(ref effect);

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
}
