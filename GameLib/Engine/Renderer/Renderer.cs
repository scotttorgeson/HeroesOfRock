using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameLib.Engine.Particles;
using System.Threading;
#if DEBUG
using BEPUphysicsDrawer;
#endif

// Forward renderer.
namespace GameLib
{
    public class Renderer
    {
#if DEBUG
        public BEPUphysicsDrawer.Models.InstancedModelDrawer collisionDebugDrawer;
#endif

        protected GraphicsDevice graphicsDevice;
        public GraphicsDevice GraphicsDevice { get { return graphicsDevice; } }

        protected SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch { get { return spriteBatch; } }

        private static Renderer instance;
        public static Renderer Instance { get { return instance; } }

        public static bool DrawTriggers = false;

        public BasicEffect particleEffect;
        public Color skyColor = Color.SkyBlue;

        public const int ScreenWidth = 1280;
        public const int ScreenHeight = 720;
        public const float AspectRatio = (float)ScreenWidth / (float)ScreenHeight;        
        public static readonly Rectangle ScreenRect = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

        private DecalEffect decalEffect;

        public Renderer(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            instance = this;
        }

        public void Initialize()
        {
            rs = new RasterizerState();
        }

        public virtual void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphicsDevice);

            particleEffect = new BasicEffect(graphicsDevice);
            //particleEffect.EnableDefaultLighting();

            particleEffect.TextureEnabled = true;

            decalEffect = new DecalEffect(Stage.Content.Load<Effect>("Effects/v2/Decal"));

            sun = new Sun(Color.White.ToVector4(), new Vector4(.2f, .2f, .2f, 1.0f), 45.0f, 45.0f);

            bloom = new Bloom();
            bloom.LoadContent();
#if DEBUG
            collisionDebugDrawer = new BEPUphysicsDrawer.Models.InstancedModelDrawer(graphicsDevice, Stage.Content.ServiceProvider);
#endif
        }

        public virtual void Draw(float dt)
        {
            sun.CreateShadowMap();

            bloom.BeginDraw();

            // Clear back buffer
            graphicsDevice.Clear(skyColor);
            Renderer.Instance.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = rs;
            
            DrawModels(DrawType.Draw);

            //BLEND STATE NEEDS TO BE ALPHA BLEND AND DEPTHSTENCIL NEEDS TO BE DEPTHREAD FOR TRANSPARANCIES
            // graphicsDevice.BlendState = BlendState.AlphaBlend; // set by DrawModels
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            DrawDecals();
            DrawParticles();

#if DEBUG && SHOW_SHADOW_MAP
            if (sun != null && Sun.shadowMap != null)
            {
                spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
                spriteBatch.Draw(Sun.shadowMap, new Rectangle(ScreenWidth - (256*3), ScreenHeight - 256, 256*3, 256), Color.White);
                spriteBatch.End();
            }
#endif

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = rs;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.Textures[1] = null;

#if DEBUG
            if (collisionDebugDrawer.GetBatchCount() > 0)
            {
                collisionDebugDrawer.Update();
                collisionDebugDrawer.Draw(view, projection);
            }
#endif

            bloom.Draw();

        }

        // called before the stage unloads content
        public virtual void UnloadContent()
        {
            if (bloom != null)
            {
                bloom.UnloadContent();
                bloom = null;
            }
            
            Sun.UnloadContent();
            sun = null;
            
            particleEffect = null;
            decalEffect = null;

            if (spriteBatch != null)
            {
                spriteBatch.Dispose();
                spriteBatch = null;
            }

            RModel.UnloadContent();

            ClearTables();
        }

        public Sun sun;
        public Bloom bloom;

        Dictionary<string, Texture2D> textureTable = new Dictionary<string,Texture2D>();
        Dictionary<string, Model> modelTable = new Dictionary<string,Model>();

        private void ClearTables()
        {
            if ( textureTable != null )
                textureTable.Clear();
            if ( modelTable != null )
                modelTable.Clear();
#if DEBUG
            if ( collisionDebugDrawer != null )
                collisionDebugDrawer.Clear();
#endif
        }

        public Texture2D LookupTexture(string name)
        {
            if (textureTable.ContainsKey(name))
                return textureTable[name];
            else
            {
                textureTable[name] = Stage.Content.Load<Texture2D>(name);
                return textureTable[name];
            }
        }

        public Model LookupModel(string name, out bool initialized)
        {
            if (modelTable.ContainsKey(name))
            {
                initialized = true;
                return modelTable[name];
            }
            else
            {
                initialized = false;
                modelTable[name] = Stage.Content.Load<Model>(name);
                return modelTable[name];
            }
        }

        public static bool ShouldDrawModel(RModel model, ref Matrix transform, ref Engine.Utilities.FastFrustum camFrustum)
        {
            BoundingSphere boundingSphere;
            model.boundingSphere.Transform(ref transform, out boundingSphere);

            return camFrustum.Intersects(ref boundingSphere);
        }

        public enum DrawType
        {
            Draw,
            CreateShadowMap,
        }

        RasterizerState rs;

        private void DrawModels(DrawType technique)
        {
            foreach (RModel rModel in RModels)
            {
                rModel.DrawInstances(technique);
            }

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            foreach (RModel rModel in AlphaBlendRModels)
            {
                rModel.DrawInstances(technique);
            }
            // graphicsDevice.BlendState = BlendState.Opaque; // we want alpha blend for the next thing we draw
        }

        private void DrawDecals()
        {
            if(Stage.ActiveStage.QBTable.ContainsKey(typeof(Engine.Decals.DecalQB)))
            {
                Engine.Decals.DecalQB dqb = Stage.ActiveStage.GetQB<Engine.Decals.DecalQB>();

                if (dqb.decals.Count > 0)
                {
                    decalEffect.View = view;
                    decalEffect.Projection = projection;
                    sun.SetLights(ref decalEffect);

                    foreach (Engine.Decals.Decal decal in dqb.decals)
                    {
                        decalEffect.DecalTexture = decal.Texture;

                        for (int i = 0; i < decal.Actors.Count; i++)
                        {
                            Actor actor = decal.Actors[i];
                            Matrix world = actor.modelInstance.RenderTransform;
                            if (ShouldDrawModel(actor.modelInstance.model, ref world, ref fastCameraBoundingFrustum))
                            {
                                Model model = actor.modelInstance.model.Model;

                                decalEffect.DecalMatrix = decal.ViewProjections[i];

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

                                        Matrix.Multiply(ref world, ref transforms[mesh.ParentBone.Index], out world);
                                        decalEffect.World = world;

                                        // apply the effect and draw
                                        decalEffect.CurrentTechnique.Passes[0].Apply();
                                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawParticles()
        {
            foreach (ParticleEmitter e in Stage.ActiveStage.GetQB<ParticleQB>().emitters)
            {
                particleEffect.Texture = e.texture;
                particleEffect.Parameters["Texture"].SetValue(e.texture);
                particleEffect.World = Matrix.Identity; //need to figure out world matrix
                particleEffect.View = view;
                particleEffect.Projection = projection;
                
                foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    foreach (Particle p in e.liveParticles)
                    {
                        GraphicsDevice.DrawUserIndexedPrimitives
                            <VertexPositionNormalTexture>(
                            PrimitiveType.TriangleList,
                            p.quad.Vertices, 0, 4,
                            p.quad.Indexes, 0, 2);
                    }
                }
            }
        }

        public void DoCulling(ref Engine.Utilities.FastFrustum frustum)
        {
            foreach (RModel rModel in RModels)
            {
                rModel.DrawList.Clear();
                foreach (RModelInstance modelInstance in rModel.Instances)
                {
                    if (modelInstance.Shown && ShouldDrawModel(modelInstance.model, ref modelInstance.RenderTransform, ref frustum))
                    {
                        rModel.DrawList.Add(modelInstance);
                        if (modelInstance is SkinnedRModelInstance)
                        {
                            SkinnedRModelInstance smodelInstance = (SkinnedRModelInstance)modelInstance;
                            smodelInstance.UpdateBones();
                        }
                    }
                }
            }

            foreach (RModel rModel in AlphaBlendRModels)
            {
                rModel.DrawList.Clear();
                foreach (RModelInstance modelInstance in rModel.Instances)
                {
                    if (modelInstance.Shown && ShouldDrawModel(modelInstance.model, ref modelInstance.RenderTransform, ref frustum))
                    {
                        rModel.DrawList.Add(modelInstance);
                        if (modelInstance is SkinnedRModelInstance)
                        {
                            SkinnedRModelInstance smodelInstance = (SkinnedRModelInstance)modelInstance;
                            smodelInstance.UpdateBones();
                        }
                    }
                }
            }
        }

        public void DoShadowsCulling(ref Engine.Utilities.FastFrustum frustum, ref FastList<RModelInstance> drawList)
        {
            drawList.Clear();
            foreach (RModel rModel in RModels)
            {
                foreach (RModelInstance modelInstance in rModel.Instances)
                {
                    if (modelInstance.Shown && modelInstance.model.CastsShadows && ShouldDrawModel(modelInstance.model, ref modelInstance.RenderTransform, ref frustum))
                        drawList.Add(modelInstance);
                }
            }

            foreach (RModel rModel in AlphaBlendRModels)
            {
                foreach (RModelInstance modelInstance in rModel.Instances)
                {
                    if (modelInstance.Shown && modelInstance.model.CastsShadows && ShouldDrawModel(modelInstance.model, ref modelInstance.RenderTransform, ref frustum))
                        drawList.Add(modelInstance);
                }
            }
        }

        public List<RModel> RModels = new List<RModel>();
        public List<RModel> AlphaBlendRModels = new List<RModel>();

        public void AddRModel(RModel model)
        {
            if (model.AlphaBlend)
                AlphaBlendRModels.Add(model);
            else
                RModels.Add(model);
        }

        public void RemoveRModel(RModel model)
        {
            if (model.AlphaBlend)
                AlphaBlendRModels.Remove(model);
            else
                RModels.Remove(model);
        }

        public void ClearRModelInstances()
        {
            foreach (RModel rModel in RModels)
            {
                rModel.Instances.Clear();
                rModel.DrawList.ClearReferences();
            }
            foreach (RModel rModel in AlphaBlendRModels)
            {
                rModel.Instances.Clear();
                rModel.DrawList.ClearReferences();
            }
            if (sun != null)
                sun.ClearRModelInstances();
        }

        public void EditorUpdate()
        {
            foreach (RModel rModel in RModels)
            {
                foreach (RModelInstance modelInstance in rModel.Instances)
                {
                    modelInstance.SaveRenderData();
                }
            }
            foreach (RModel rModel in AlphaBlendRModels)
            {
                foreach (RModelInstance modelInstance in rModel.Instances)
                {
                    modelInstance.SaveRenderData();
                }
            }

            view = CameraQB.ViewMatrix;
            projection = CameraQB.ProjectionMatrix;
            nearClip = CameraQB.NearClip;
            farClip = CameraQB.FarClip;
            clipRange = CameraQB.ClipRange;
            Matrix.Multiply(ref view, ref projection, out viewProj);

            BackgroundThreadTask(null);
        }

        public void BackgroundThreadTask(object temp)
        {
            fastCameraBoundingFrustum = new Engine.Utilities.FastFrustum(ref viewProj);
            DoCulling(ref fastCameraBoundingFrustum);
            sun.CalcCascades(ref view, ref projection, nearClip, farClip, clipRange);
        }

        public Matrix view = new Matrix();
        public Matrix projection = new Matrix();
        public float nearClip = 0.0f;
        public float farClip = 0.0f;
        public float clipRange = 0.0f;
        public Matrix viewProj = new Matrix();

        public void UpdateStart()
        {
            foreach (RModel rModel in RModels)
            {
                foreach (RModelInstance modelInstance in rModel.Instances)
                {
                    modelInstance.SaveRenderData();
                }
            }
            foreach (RModel rModel in AlphaBlendRModels)
            {
                foreach (RModelInstance modelInstance in rModel.Instances)
                {
                    modelInstance.SaveRenderData();
                }
            }

            view = CameraQB.ViewMatrix;
            projection = CameraQB.ProjectionMatrix;
            nearClip = CameraQB.NearClip;
            farClip = CameraQB.FarClip;
            clipRange = CameraQB.ClipRange;
            Matrix.Multiply(ref view, ref projection, out viewProj);

            if (threadManager == null)
            {
                threadManager = new BEPUphysics.Threading.SimpleThreadManager();
#if XBOX
                threadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 5 }); }, null);
#else
                threadManager.AddThread();
#endif
            }

            threadManager.EnqueueTask(BackgroundThreadTask, null);
        }

        public void UpdateEnd()
        {
            threadManager.WaitForTaskCompletion();
        }

        BEPUphysics.Threading.SimpleThreadManager threadManager;

        Engine.Utilities.FastFrustum fastCameraBoundingFrustum;
    }
}
