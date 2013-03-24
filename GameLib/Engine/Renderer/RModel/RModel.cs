using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

// Every actor has an RModel which is used by the renderer to draw the actor.
// Allows us to change the implementation of the renderer/model without changing anything in Actor.cs.
// Just an XNA model for now...

namespace GameLib
{
    public class RModel
    {
        protected Model model;
        public Model Model { get { return model; } }
        public string Name { get; private set; }

        public bool IsBumpMapped = false;
        public bool IsSpecularMapped = false;
        public bool CastsShadows = true;
        public bool ReceivesShadows = true;
        public bool AlphaBlend = false;
        public float Shininess = 0.3f;
        public float SpecularPower = 4.0f;

        public BoundingSphere boundingSphere;

        protected bool contentLoaded = false;

        public List<RModelInstance> Instances = new List<RModelInstance>();
        public FastList<RModelInstance> DrawList = new FastList<RModelInstance>();  

        private static Dictionary<string, RModel> modelDictionary = new Dictionary<string, RModel>();
        public static void UnloadContent()
        {
            modelDictionary.Clear();
        }

        public static RModel GetRModel(ParameterSet parm)
        {
            string modelName = parm.GetString("ModelName");
            if (modelDictionary.ContainsKey(modelName))
                return modelDictionary[modelName];
            else
                return CreateRModel(parm);
        }

        private static RModel CreateRModel(ParameterSet parm)
        {
            // if we have a model type, read it and create the right rmodel type
            if (parm.HasParm("ModelType"))
            {
                switch (parm.GetString("ModelType"))
                {
                    case "Skinned":
                        return new SkinnedRModel(parm);
                    case "Static":
                        return new StaticRModel(parm);
                    case "Water":
                        return new WaterRModel(parm);
                    case "EditorManipulator":
                        return new EditorManipulatorRModel(parm);
                    default:
                        System.Diagnostics.Debug.Assert(false, "Invalid model type: " + parm.GetString("ModelType"));
                        break;
                }
            }

            // default to static
            return new StaticRModel(parm);
        }

        public RModel(ParameterSet parm)
        {
            string modelName = parm.GetString("ModelName");
            Name = modelName;

            if (parm.HasParm("BumpMap"))
                IsBumpMapped = true;
            if (parm.HasParm("Shininess"))
                Shininess = parm.GetFloat("Shininess");
            if (parm.HasParm("SpecularPower"))
                SpecularPower = parm.GetFloat("SpecularPower");
            if (parm.HasParm("SpecularMap"))
                IsSpecularMapped = true;
            if (parm.HasParm("CastsShadows"))
                CastsShadows = parm.GetBool("CastsShadows");
            if (parm.HasParm("ReceivesShadows"))
                ReceivesShadows = parm.GetBool("ReceivesShadows");
            if (parm.HasParm("AlphaBlend"))
                AlphaBlend = parm.GetBool("AlphaBlend");

            modelDictionary.Add(Name, this);
            Renderer.Instance.AddRModel(this);
        }

        // used by skinnedrmodel, staticrmodel, waterrmodel to load basic parameters
        protected Model BasicModelLoad(ParameterSet parm, out bool initialized, out Texture2D diffuse, out Texture2D bumpMap, out Texture2D specularMap)
        {
            Microsoft.Xna.Framework.Graphics.Model model = Renderer.Instance.LookupModel("Models/" + Name, out initialized);

            diffuse = null;
            bumpMap = null;
            specularMap = null;

            if (parm.HasParm("Texture"))
                diffuse = Renderer.Instance.LookupTexture("Models/" + parm.GetString("Texture"));
            else
                diffuse = Renderer.Instance.LookupTexture("DefaultDiffuse");

            if (IsBumpMapped)
                bumpMap = Renderer.Instance.LookupTexture("Models/" + parm.GetString("BumpMap"));

            if (IsSpecularMapped)
                specularMap = Renderer.Instance.LookupTexture("Models/" + parm.GetString("SpecularMap"));

            return model;
        }

        public virtual void LoadContent(ContentManager content, ParameterSet parm, Stage stage)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            boundingSphere = new BoundingSphere();
            foreach (ModelMesh mesh in Model.Meshes)
            {
                BoundingSphere meshBoundingSphere;
                mesh.BoundingSphere.Transform(ref transforms[mesh.ParentBone.Index], out meshBoundingSphere);
                boundingSphere = BoundingSphere.CreateMerged(boundingSphere, meshBoundingSphere);
            }

            contentLoaded = true;
        }

        public virtual void Draw(ref GraphicsDevice graphics, ref Matrix world, Renderer.DrawType technique)
        {
        }

        public virtual void DrawInstances(Renderer.DrawType technique)
        {
        }
    }
}
