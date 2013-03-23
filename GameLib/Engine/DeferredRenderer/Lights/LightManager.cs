using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    public class LightManager : Quarterback
    {
        public override string Name()
        {
            return "LightManager";
        }

        List<DirectionalLight> directionalLights;

        public List<DirectionalLight> DirectionalLights
        {
            get { return directionalLights; }
        }

        List<SpotLight> spotLights;

        public List<SpotLight> SpotLights
        {
            get { return spotLights; }
        }

        List<PointLight> pointLights;

        public List<PointLight> PointLights
        {
            get { return pointLights; }
        }

        Effect depthWriter;
        Texture2D spotCookie;
        Texture2D squareCookie;

        public LightManager()
        {
            directionalLights = new List<DirectionalLight>();
            spotLights = new List<SpotLight>();
            pointLights = new List<PointLight>();
        }

        public override void PreLoadInit(ParameterSet Parm)
        {
            
        }

        public override void LoadContent()
        {
            depthWriter = Stage.Content.Load<Effect>("Effects/DepthWriter");
            depthWriter.CurrentTechnique = depthWriter.Techniques[0];
            spotCookie = Stage.Content.Load<Texture2D>("SpotCookie");
            squareCookie = Stage.Content.Load<Texture2D>("SquareCookie");
        }

        public override void PostLoadInit(ParameterSet Parm)
        {
            int index = 0;

            while (Parm.HasParm("Light" + index))
            {
                string keyString = "Light" + index;
                string lightType = Parm.GetString(keyString);

                switch (lightType)
                {
                    case "Directional":
                        {
                            // read directional light parameters and create light
                            Vector3 direction = Parm.GetVector3(keyString + "Direction");
                            Vector4 color = Parm.GetVector4(keyString + "Color");
                            float intensity = Parm.GetFloat(keyString + "Intensity");
                            directionalLights.Add(new DirectionalLight(direction, color, intensity));
                        }
                        break;
                    case "Spot":
                        {
                            Vector3 position = Parm.GetVector3(keyString + "Position");
                            Vector3 direction = Parm.GetVector3(keyString + "Direction");
                            Vector4 color = Parm.GetVector4(keyString + "Color");
                            float intensity = Parm.GetFloat(keyString + "Intensity");
                            spotLights.Add(new SpotLight(DeferredRenderer.Instance.GraphicsDevice, position, direction, color, intensity, true, 1024, spotCookie));
                        }
                        break;
                    case "Point":
                        {
                            Vector3 position = Parm.GetVector3(keyString + "Position");
                            float radius = Parm.GetFloat(keyString + "Radius");
                            Vector4 color = Parm.GetVector4(keyString + "Color");
                            float intensity = Parm.GetFloat(keyString + "Intensity");
                            pointLights.Add(new PointLight(DeferredRenderer.Instance.GraphicsDevice, position, radius, color, intensity, true, 256));
                        }
                        break;
                }
                index++;
            }
        }

        public void DrawShadowMaps()
        {
            DeferredRenderer.Instance.GraphicsDevice.BlendState = BlendState.Opaque;
            DeferredRenderer.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            DeferredRenderer.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            // for each light with shadows
            foreach (SpotLight spotLight in spotLights)
            {
                spotLight.Update();
                if (spotLight.IsWithShadows)
                    DrawShadowMap(spotLight);
            }

            foreach (PointLight pointLight in pointLights)
            {
                if (pointLight.IsWithShadows)
                    DrawShadowMap(pointLight);
            }
        }

        public void DrawShadowMap(SpotLight spotLight)
        {
            DeferredRenderer.Instance.GraphicsDevice.SetRenderTarget(spotLight.ShadowMap);
            DeferredRenderer.Instance.GraphicsDevice.Clear(Color.Transparent);
            depthWriter.Parameters["View"].SetValue(spotLight.View);
            depthWriter.Parameters["Projection"].SetValue(spotLight.Projection);
            depthWriter.Parameters["LightPosition"].SetValue(spotLight.Position);
            depthWriter.Parameters["DepthPrecision"].SetValue(spotLight.FarPlane);

            DrawModels();
        }

        public void DrawShadowMap(PointLight pointLight)
        {
            Matrix[] views = new Matrix[6];
            views[0] = Matrix.CreateLookAt(pointLight.Position, pointLight.Position + Vector3.Forward, Vector3.Up);
            views[1] = Matrix.CreateLookAt(pointLight.Position, pointLight.Position + Vector3.Backward, Vector3.Up);
            views[2] = Matrix.CreateLookAt(pointLight.Position, pointLight.Position + Vector3.Left, Vector3.Up);
            views[3] = Matrix.CreateLookAt(pointLight.Position, pointLight.Position + Vector3.Right, Vector3.Up);
            views[4] = Matrix.CreateLookAt(pointLight.Position, pointLight.Position + Vector3.Down, Vector3.Forward);
            views[5] = Matrix.CreateLookAt(pointLight.Position, pointLight.Position + Vector3.Up, Vector3.Backward);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), 1.0f, 1.0f, pointLight.Radius);

            depthWriter.Parameters["Projection"].SetValue(projection);
            depthWriter.Parameters["LightPosition"].SetValue(pointLight.Position);
            depthWriter.Parameters["DepthPrecision"].SetValue(pointLight.Radius);

            #region Forward
            DeferredRenderer.Instance.GraphicsDevice.SetRenderTarget(pointLight.ShadowMap, CubeMapFace.PositiveZ);

            //Clear Target
            DeferredRenderer.Instance.GraphicsDevice.Clear(Color.Transparent);

            //Set global Effect parameters
            depthWriter.Parameters["View"].SetValue(views[0]);

            //Draw Models
            DrawModels();
            #endregion

            #region Backward
            DeferredRenderer.Instance.GraphicsDevice.SetRenderTarget(pointLight.ShadowMap, CubeMapFace.NegativeZ);

            //Clear Target
            DeferredRenderer.Instance.GraphicsDevice.Clear(Color.Transparent);

            //Set global Effect parameters
            depthWriter.Parameters["View"].SetValue(views[1]);

            //Draw Models
            DrawModels();
            #endregion

            #region Left
            DeferredRenderer.Instance.GraphicsDevice.SetRenderTarget(pointLight.ShadowMap, CubeMapFace.NegativeX);

            //Clear Target
            DeferredRenderer.Instance.GraphicsDevice.Clear(Color.Transparent);

            //Set global Effect parameters
            depthWriter.Parameters["View"].SetValue(views[2]);

            //Draw Models
            DrawModels();
            #endregion

            #region Right
            DeferredRenderer.Instance.GraphicsDevice.SetRenderTarget(pointLight.ShadowMap, CubeMapFace.PositiveX);

            //Clear Target
            DeferredRenderer.Instance.GraphicsDevice.Clear(Color.Transparent);

            //Set global Effect parameters
            depthWriter.Parameters["View"].SetValue(views[3]);

            //Draw Models
            DrawModels();
            #endregion

            #region Down
            DeferredRenderer.Instance.GraphicsDevice.SetRenderTarget(pointLight.ShadowMap, CubeMapFace.NegativeY);

            //Clear Target
            DeferredRenderer.Instance.GraphicsDevice.Clear(Color.Transparent);

            //Set global Effect parameters
            depthWriter.Parameters["View"].SetValue(views[4]);

            //Draw Models
            DrawModels();
            #endregion

            #region Up
            DeferredRenderer.Instance.GraphicsDevice.SetRenderTarget(pointLight.ShadowMap, CubeMapFace.PositiveY);

            //Clear Target
            DeferredRenderer.Instance.GraphicsDevice.Clear(Color.Transparent);

            //Set global Effect parameters
            depthWriter.Parameters["View"].SetValue(views[5]);

            //Draw Models
            DrawModels();
            #endregion
        }

        public void DrawModels()
        {
            LinkedListNode<Actor> node = Stage.ActiveStage.GetQB<ActorQB>().Actors.First;
            if (node == null)
                return;
            do
            {
                if (node.Value.model.Shown)
                {
                    Model model = node.Value.model.Model;

                    Matrix[] transforms = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            DeferredRenderer.Instance.GraphicsDevice.SetVertexBuffer(part.VertexBuffer, part.VertexOffset);
                            DeferredRenderer.Instance.GraphicsDevice.Indices = part.IndexBuffer;
                            depthWriter.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * node.Value.PhysicsObject.TransformMatrix); // add in actor position
                            depthWriter.CurrentTechnique.Passes[0].Apply();
                            DeferredRenderer.Instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        }
                    }
                }
                node = node.Next;
            } while (node != null);
        }

        public void AddLight(DirectionalLight light)
        {
            directionalLights.Add(light);
        }

        public void RemoveLight(DirectionalLight light)
        {
            directionalLights.Remove(light);
        }

        public void AddLight(SpotLight light)
        {
            spotLights.Add(light);
        }

        public void RemoveLight(SpotLight light)
        {
            spotLights.Remove(light);
        }

        public void AddLight(PointLight light)
        {
            pointLights.Add(light);
        }

        public void RemoveLight(PointLight light)
        {
            pointLights.Remove(light);
        }
    }
}
