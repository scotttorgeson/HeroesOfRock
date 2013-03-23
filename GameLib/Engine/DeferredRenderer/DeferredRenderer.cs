using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    public class DeferredRenderer : Renderer
    {
        //public static DeferredRenderer Instance;

        //public GraphicsDevice GraphicsDevice;
        //SpriteBatch spriteBatch;
        //public SpriteBatch SpriteBatch { get { return spriteBatch; } }
        Effect Clear;
        Effect GBuffer;
        Effect directionalLight;
        Effect pointLight;
        Effect spotLight;
        Effect compose;
        BlendState LightMapBS;
        RenderTargetBinding[] GBufferTargets;
        Vector2 GBufferTextureSize;
        RenderTarget2D LightMap;
        FullscreenQuad fsq;
        Model pointLightGeometry;
        Model spotLightGeometry;

        //Texture2D defaultNormalMap;
        //Texture2D defaultSpecularMap;

        public RenderTargetBinding[] GetGBuffer() { return GBufferTargets; }
        
        public DeferredRenderer(GraphicsDevice GraphicsDevice, int Width, int Height)
            : base(GraphicsDevice)
        {

            //Create LightMap BlendState
            LightMapBS = new BlendState();
            LightMapBS.ColorSourceBlend = Blend.One;
            LightMapBS.ColorDestinationBlend = Blend.One;
            LightMapBS.ColorBlendFunction = BlendFunction.Add;
            LightMapBS.AlphaSourceBlend = Blend.One;
            LightMapBS.AlphaDestinationBlend = Blend.One;
            LightMapBS.AlphaBlendFunction = BlendFunction.Add;

            //Set GBuffer Texture Size
            GBufferTextureSize = new Vector2(Width, Height);

            //Intialize Each Target of the GBuffer
            GBufferTargets = new RenderTargetBinding[3];
            GBufferTargets[0] = new RenderTargetBinding(new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Rgba64, DepthFormat.Depth24Stencil8));
            GBufferTargets[1] = new RenderTargetBinding(new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Rgba64, DepthFormat.Depth24Stencil8));
            GBufferTargets[2] = new RenderTargetBinding(new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Vector2, DepthFormat.Depth24Stencil8));

            //Initialize LightMap
            LightMap = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            //Create Fullscreen Quad
            fsq = new FullscreenQuad(GraphicsDevice);        
        }
        
        public override void LoadContent()
        {
            base.LoadContent();

            // Load shaders
            Clear = Stage.Content.Load<Effect>("Effects/Clear");
            Clear.CurrentTechnique = Clear.Techniques[0];

            GBuffer = Stage.Content.Load<Effect>("Effects/GBuffer");
            GBuffer.CurrentTechnique = GBuffer.Techniques[0];

            directionalLight = Stage.Content.Load<Effect>("Effects/DirectionalLight");
            directionalLight.CurrentTechnique = directionalLight.Techniques[0];

            pointLight = Stage.Content.Load<Effect>("Effects/PointLight");
            pointLight.CurrentTechnique = pointLight.Techniques[0];

            spotLight = Stage.Content.Load<Effect>("Effects/SpotLight");
            spotLight.CurrentTechnique = spotLight.Techniques[0];

            compose = Stage.Content.Load<Effect>("Effects/Composition");
            compose.CurrentTechnique = compose.Techniques[0];

            //Load Point Light Geometry
            pointLightGeometry = Stage.Content.Load<Model>("Models/PointLightGeometry");

            //Load Spot Light Geometry
            spotLightGeometry = Stage.Content.Load<Model>("Models/SpotLightGeometry");

            //defaultNormalMap = Stage.Content.Load<Texture2D>("DefaultNormalMap");
            //defaultSpecularMap = Stage.Content.Load<Texture2D>("DefaultSpecularMap");
        }

        public override void Draw(float dt)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            LightManager lightManager = Stage.ActiveStage.GetQB<LightManager>();
            lightManager.DrawShadowMaps();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            ClearGBuffer();
            MakeGBuffer();
            MakeLightMap(lightManager);
            MakeFinal(null);
        }

        void MakeGBuffer()
        {
            // Set depth stencil state
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Set global GBuffer parameters
            GBuffer.Parameters["View"].SetValue(CameraQB.ViewMatrix);
            GBuffer.Parameters["Projection"].SetValue(CameraQB.ProjectionMatrix);

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
                            // set the vb and ib from the mesh part
                            GraphicsDevice.SetVertexBuffer(part.VertexBuffer, part.VertexOffset);
                            GraphicsDevice.Indices = part.IndexBuffer;

                            // set the world matrix -- add in its physics object transform
                            GBuffer.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * node.Value.PhysicsObject.TransformMatrix);

                            // world inverse transform -- add in physics object transform
                            GBuffer.Parameters["WorldViewIT"].SetValue(Matrix.Transpose(Matrix.Invert(transforms[mesh.ParentBone.Index] * node.Value.PhysicsObject.TransformMatrix * CameraQB.ViewMatrix)));

                            // set the textures, read from the default effect textures
                            GBuffer.Parameters["Texture"].SetValue(part.Effect.Parameters["Texture"].GetValueTexture2D());
                            GBuffer.Parameters["NormalMap"].SetValue(part.Effect.Parameters["NormalMap"].GetValueTexture2D());
                            GBuffer.Parameters["SpecularMap"].SetValue(part.Effect.Parameters["SpecularMap"].GetValueTexture2D());

                            // apply the effect and draw
                            GBuffer.CurrentTechnique.Passes[0].Apply();
                            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        }
                    }
                }
                node = node.Next;
            } while (node != null);

            // set render targets off
            GraphicsDevice.SetRenderTargets(null);
        }

        void ClearGBuffer()
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.SetRenderTargets(GBufferTargets);
            Clear.CurrentTechnique.Passes[0].Apply();
            fsq.Draw(GraphicsDevice);
        }

        void MakeLightMap(LightManager Lights)
        {
            GraphicsDevice.SetRenderTarget(LightMap);
            GraphicsDevice.Clear(Color.Transparent);
            GraphicsDevice.BlendState = LightMapBS;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            #region Set Global Samplers
            //GBuffer 1 Sampler
            GraphicsDevice.Textures[0] = GBufferTargets[0].RenderTarget;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            //GBuffer 2 Sampler
            GraphicsDevice.Textures[1] = GBufferTargets[1].RenderTarget;
            GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            //GBuffer 3 Sampler
            GraphicsDevice.Textures[2] = GBufferTargets[2].RenderTarget;
            GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

            //SpotLight Cookie Sampler
            GraphicsDevice.SamplerStates[3] = SamplerState.LinearClamp;

            //ShadowMap Sampler
            GraphicsDevice.SamplerStates[4] = SamplerState.PointClamp;
            #endregion

            // inverse view & view-projection
            Matrix inverseView = Matrix.Invert(CameraQB.ViewMatrix);
            Matrix inverseViewProjection = Matrix.Invert(CameraQB.ViewMatrix * CameraQB.ProjectionMatrix);

            // set Directional Lights Globals
            directionalLight.Parameters["InverseViewProjection"].SetValue(inverseViewProjection);
            directionalLight.Parameters["inverseView"].SetValue(inverseView);
            directionalLight.Parameters["CameraPosition"].SetValue(CameraQB.WorldMatrix.Translation);
            directionalLight.Parameters["GBufferTextureSize"].SetValue(GBufferTextureSize);

            fsq.ReadyBuffers(GraphicsDevice);

            // draw directional lights
            foreach (DirectionalLight light in Lights.DirectionalLights)
            {
                directionalLight.Parameters["L"].SetValue(Vector3.Normalize(light.Direction));
                directionalLight.Parameters["LightColor"].SetValue(light.Color);
                directionalLight.Parameters["LightIntensity"].SetValue(light.Intensity);

                // apply technique and draw
                directionalLight.CurrentTechnique.Passes[0].Apply();
                fsq.JustDraw(GraphicsDevice);
            }

            //Set Spot Lights Globals
            spotLight.Parameters["View"].SetValue(CameraQB.ViewMatrix);
            spotLight.Parameters["inverseView"].SetValue(inverseView);
            spotLight.Parameters["Projection"].SetValue(CameraQB.ProjectionMatrix);
            spotLight.Parameters["InverseViewProjection"].SetValue(inverseViewProjection);
            spotLight.Parameters["CameraPosition"].SetValue(CameraQB.WorldMatrix.Translation);
            spotLight.Parameters["GBufferTextureSize"].SetValue(GBufferTextureSize);

            //Set Spot Lights Geometry Buffers
            GraphicsDevice.SetVertexBuffer(spotLightGeometry.Meshes[0].MeshParts[0].VertexBuffer, spotLightGeometry.Meshes[0].MeshParts[0].VertexOffset);
            GraphicsDevice.Indices = spotLightGeometry.Meshes[0].MeshParts[0].IndexBuffer;

            //Draw Spot Lights
            foreach (SpotLight light in Lights.SpotLights)
            {
                //Set Attenuation Cookie Texture and SamplerState
                GraphicsDevice.Textures[3] = light.AttenuationTexture;

                //Set ShadowMap and SamplerState
                GraphicsDevice.Textures[4] = light.ShadowMap;

                //Set Spot Light Parameters
                spotLight.Parameters["World"].SetValue(light.World);
                spotLight.Parameters["LightViewProjection"].SetValue(light.View * light.Projection);
                spotLight.Parameters["LightPosition"].SetValue(light.Position);
                spotLight.Parameters["LightColor"].SetValue(light.Color);
                spotLight.Parameters["LightIntensity"].SetValue(light.Intensity);
                spotLight.Parameters["S"].SetValue(light.Direction);
                spotLight.Parameters["LightAngleCos"].SetValue(light.LightAngleCos());
                spotLight.Parameters["LightHeight"].SetValue(light.FarPlane);
                spotLight.Parameters["Shadows"].SetValue(light.IsWithShadows);
                spotLight.Parameters["shadowMapSize"].SetValue(light.ShadowMapResolution);
                spotLight.Parameters["DepthPrecision"].SetValue(light.FarPlane);
                spotLight.Parameters["DepthBias"].SetValue(light.DepthBias);

                #region Set Cull Mode
                //Calculate L
                Vector3 L = CameraQB.WorldMatrix.Translation - light.Position;

                //Calculate S.L
                float SL = Math.Abs(Vector3.Dot(L, light.Direction));

                //Check if SL is within the LightAngle, if so then draw the BackFaces, if not then draw the FrontFaces
                if (SL < light.LightAngleCos()) GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                else GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                #endregion

                //Apply
                spotLight.CurrentTechnique.Passes[0].Apply();

                //Draw
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                        spotLightGeometry.Meshes[0].MeshParts[0].NumVertices,
                                                        spotLightGeometry.Meshes[0].MeshParts[0].StartIndex,
                                                        spotLightGeometry.Meshes[0].MeshParts[0].PrimitiveCount);
            }

            //Set Point Lights Geometry Buffers
            GraphicsDevice.SetVertexBuffer(pointLightGeometry.Meshes[0].MeshParts[0].VertexBuffer, pointLightGeometry.Meshes[0].MeshParts[0].VertexOffset);
            GraphicsDevice.Indices = pointLightGeometry.Meshes[0].MeshParts[0].IndexBuffer;

            //Set Point Lights Globals
            pointLight.Parameters["inverseView"].SetValue(inverseView);
            pointLight.Parameters["View"].SetValue(CameraQB.ViewMatrix);
            pointLight.Parameters["Projection"].SetValue(CameraQB.ProjectionMatrix);
            pointLight.Parameters["InverseViewProjection"].SetValue(inverseViewProjection);
            pointLight.Parameters["CameraPosition"].SetValue(CameraQB.WorldMatrix.Translation);
            pointLight.Parameters["GBufferTextureSize"].SetValue(GBufferTextureSize);

            //Draw Point Lights without Shadows
            foreach (PointLight light in Lights.PointLights)
            {
                //Set Point Light Sampler
                GraphicsDevice.Textures[4] = light.ShadowMap;
                GraphicsDevice.SamplerStates[4] = SamplerState.PointWrap;

                //Set Point Light Parameters
                pointLight.Parameters["World"].SetValue(light.World);
                pointLight.Parameters["LightPosition"].SetValue(light.Position);
                pointLight.Parameters["LightRadius"].SetValue(light.Radius);
                pointLight.Parameters["LightColor"].SetValue(light.Color);
                pointLight.Parameters["LightIntensity"].SetValue(light.Intensity); ;
                pointLight.Parameters["Shadows"].SetValue(light.IsWithShadows);
                pointLight.Parameters["DepthPrecision"].SetValue(light.Radius);
                pointLight.Parameters["DepthBias"].SetValue(light.DepthBias);
                pointLight.Parameters["shadowMapSize"].SetValue(light.ShadowMapResolution);

                //Set Cull Mode
                Vector3 diff = CameraQB.WorldMatrix.Translation - light.Position;

                float CameraToLight = (float)Math.Sqrt((float)Vector3.Dot(diff, diff)) / 100.0f;

                //If the Camera is in the light, render the backfaces, if it's out of the light, render the frontfaces
                if (CameraToLight <= light.Radius) GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                else if (CameraToLight > light.Radius) GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                //Apply
                pointLight.CurrentTechnique.Passes[0].Apply();

                //Draw
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                        pointLightGeometry.Meshes[0].MeshParts[0].NumVertices,
                                                        pointLightGeometry.Meshes[0].MeshParts[0].StartIndex,
                                                        pointLightGeometry.Meshes[0].MeshParts[0].PrimitiveCount);
            }

            // set States Off
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        void MakeFinal(RenderTarget2D output)
        {
            GraphicsDevice.SetRenderTarget(output);
            GraphicsDevice.Clear(Color.Transparent);

            // set textures
            GraphicsDevice.Textures[0] = GBufferTargets[0].RenderTarget;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            GraphicsDevice.Textures[1] = LightMap;
            GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            // set effect parameters
            compose.Parameters["GBufferTextureSize"].SetValue(GBufferTextureSize);

            // apply and draw
            compose.CurrentTechnique.Passes[0].Apply();
            fsq.Draw(GraphicsDevice);
        }

        public void DebugDraw()
        {
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);

            int width = 128;
            int height = 128;
            Rectangle rect = new Rectangle(0, 0, width, height);

            //Draw GBuffer 0
            SpriteBatch.Draw((Texture2D)GBufferTargets[0].RenderTarget, rect, Color.White);

            //Draw GBuffer 1
            rect.X += width;
            SpriteBatch.Draw((Texture2D)GBufferTargets[1].RenderTarget, rect, Color.White);

            //Draw GBuffer 2
            rect.X += width;
            SpriteBatch.Draw((Texture2D)GBufferTargets[2].RenderTarget, rect, Color.White);

            rect.X += width;
            SpriteBatch.Draw(LightMap, rect, Color.White);

            SpriteBatch.End();
        }
    }
}
