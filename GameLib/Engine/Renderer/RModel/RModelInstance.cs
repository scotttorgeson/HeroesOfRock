using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    public class RModelInstance
    {
        public PhysicsObject physicsObject;
        public RModel model;
        public bool Shown { get; set; }
        private readonly bool dontDraw = false;

        public static RModelInstance GetRModelInstance(ParameterSet parm)
        {
            if (parm.HasParm("ModelType"))
                if (parm.GetString("ModelType") == "Skinned")
                    return new SkinnedRModelInstance(parm);
            return new RModelInstance(parm);
        }

        public RModelInstance(ParameterSet parm)
        {
            model = RModel.GetRModel(parm);
            if (parm.HasParm("DontDraw"))
                dontDraw = parm.GetBool("DontDraw");
        }

        public virtual void LoadContent(ContentManager content, ParameterSet parm, Stage stage)
        {
            model.LoadContent(content, parm, stage);
        }

        public virtual void FinishLoad()
        {
            if (!dontDraw)
                model.Instances.Add(this);
        }

        public virtual void Kill()
        {
            if (!dontDraw)
                model.Instances.Remove(this);
        }

        public void SetPhysicsObject(PhysicsObject physicsObject)
        {
            this.physicsObject = physicsObject;
        }

        /// <summary>
        /// For use by the renderer only!!! Does not contain current data!!
        /// </summary>
        public Matrix RenderTransform;
        public BoundingSphere boundingSphere;

        public virtual void SaveRenderData()
        {
            RenderTransform = physicsObject.TransformMatrix;
            model.boundingSphere.Transform(ref RenderTransform, out boundingSphere);
        }
    }
}
