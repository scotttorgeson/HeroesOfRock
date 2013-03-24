using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameLib
{
    public class RModelInstance : IComparable<RModelInstance>
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

        public virtual void Draw(ref GraphicsDevice graphics, Renderer.DrawType technique)
        {
            model.Draw(ref graphics, ref RenderTransform, technique);
        }

        public int CompareTo(RModelInstance other)
        {
            // < 0 means we are less than other
            // 0 means we are equal to other
            // 1 means we are greater than other
            if (other.model.AlphaBlend != model.AlphaBlend)
            {
                if (model.AlphaBlend)
                    return 1;
                else
                    return -1;
            }

            // same alpha blend
            if (model.AlphaBlend)
            {
                // both alpha blend
                // todo: sort by depth
                return 0;
            }
            else
            {
                // both not alpha blend
                return 0;
            }
        }

        /// <summary>
        /// For use by the renderer only!!! Does not contain current data!!
        /// </summary>
        public Matrix RenderTransform;

        public virtual void SaveRenderData()
        {
            RenderTransform = physicsObject.TransformMatrix;
        }
    }
}
