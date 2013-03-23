using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameLib;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Entities;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.MathExtensions;

// todo: fix selection outline
// implement rotate
// move based on camera

namespace Editor
{
    public class Manipulator
    {
        public Actor selectedActor;

        protected float distFromActor;
        protected Vector3 ColliderScaling;
        public const float rotateModelScalingConst = .1f;
        public const float moveModelScalingConst = .046f;
        public bool enableRayCast;

        RasterizerState normalState = new RasterizerState();
        RasterizerState wireframeState;
        Viewport viewport;

        Effect editorEffect;

        public virtual void Initialize(ContentManager content, GraphicsDevice device)
        {
            wireframeState = new RasterizerState();
            wireframeState.FillMode = FillMode.WireFrame;
            //editorEffect = new BasicEffect(device);
            editorEffect = content.Load<Effect>("Effects/EditorEffect");
            ColliderScaling = Vector3.One;
            enableRayCast = true;
        }

        public virtual void Draw(float dt, GraphicsDevice device)
        {
            viewport = device.Viewport;
            if (Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
                && Stage.ActiveStage.GetQB<ControlsQB>().LastMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released
                && viewport.Bounds.Contains(new Point(Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.X, Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.Y))
                && CanSelect())
                Pick();

            if (selectedActor != null)
            {
                device.RasterizerState = wireframeState;

                RModel model = selectedActor.modelInstance.model;
                Matrix[] transforms = new Matrix[model.Model.Bones.Count];
                model.Model.CopyAbsoluteBoneTransformsTo(transforms);


                distFromActor = Vector3.Distance(CameraQB.WorldMatrix.Translation, selectedActor.PhysicsObject.Position);

                foreach (ModelMesh mesh in model.Model.Meshes)
                {
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        editorEffect.CurrentTechnique = editorEffect.Techniques["Technique1"];
                        editorEffect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * Matrix.CreateScale(1.0f) * selectedActor.PhysicsObject.TransformMatrix);
                        editorEffect.Parameters["View"].SetValue(CameraQB.ViewMatrix);
                        editorEffect.Parameters["Projection"].SetValue(CameraQB.ProjectionMatrix);
                        editorEffect.CurrentTechnique.Passes[0].Apply();
                        device.SetVertexBuffer(meshPart.VertexBuffer);
                        device.Indices = meshPart.IndexBuffer;
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.VertexOffset, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                    }
                }

                device.RasterizerState = normalState;
            }
        }

        private void Pick()
        {
            Ray mouseRay = PhysicsHelpers.MouseClickToRay(new Vector2(Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.X, Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.Y), CameraQB.ProjectionMatrix, CameraQB.ViewMatrix, viewport);

            RayCastResult result;
            
            if (enableRayCast)
            {
                if (Stage.ActiveStage.GetQB<PhysicsQB>().Space.RayCast(mouseRay, 1000.0f, out result))
                {
                    selectedActor = result.HitObject.OwningActor as Actor;
                }
                else
                {
                    selectedActor = null;
                }
            }
        }

        protected Actor Pick(List<Actor> actors)
        {
            Ray mouseRay = PhysicsHelpers.MouseClickToRay(new Vector2(Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.X, Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.Y), CameraQB.ProjectionMatrix, CameraQB.ViewMatrix, viewport);

            if (selectedActor != null)
            {
                Vector3 old = mouseRay.Position;
                mouseRay.Position -= selectedActor.PhysicsObject.Position;

                mouseRay.Position.Normalize();
                mouseRay.Position *= 10.0f;
                mouseRay.Position += selectedActor.PhysicsObject.Position;
            }

            foreach (Actor actor in actors)
            {
                RayHit rayHit;
                bool hit = false;
                if (actor.PhysicsObject.SpaceObject is BEPUphysics.BroadPhaseEntries.BroadPhaseEntry)
                {
                    BEPUphysics.BroadPhaseEntries.BroadPhaseEntry entry = actor.PhysicsObject.SpaceObject as BEPUphysics.BroadPhaseEntries.BroadPhaseEntry;
                    hit = entry.RayCast(mouseRay, 1000.0f, out rayHit);

                }
                else if (actor.PhysicsObject.SpaceObject is BEPUphysics.BroadPhaseSystems.IBroadPhaseEntryOwner)
                {
                    BEPUphysics.BroadPhaseSystems.IBroadPhaseEntryOwner entryOwner = actor.PhysicsObject.SpaceObject as BEPUphysics.BroadPhaseSystems.IBroadPhaseEntryOwner;
                    hit = entryOwner.Entry.RayCast(mouseRay, 1000.0f, out rayHit);
                }

                if (hit)
                {
                    return actor;
                }
            }

            return null;
        }

        protected virtual bool CanSelect()
        {
            return true;
        }

        protected string getParmKey()
        {
            //fix that doesn't make you save to file
      
            int index = 0;
            string prefix = "";

            if (selectedActor.Name.Contains("Trigger"))
            {
                foreach (Actor a in Stage.ActiveStage.GetQB<TriggerQB>().triggers)
                {
                    if (a.PhysicsObject == selectedActor.PhysicsObject)
                    {
                        prefix = "TriggerVolume";
                        break;
                    }
                    else
                        index++;
                }
            }
            else
            {
                foreach (Actor a in Stage.ActiveStage.GetQB<ActorQB>().Actors)
                {
                    if (a.PhysicsObject == selectedActor.PhysicsObject)
                    {
                        prefix = "Actor";
                        break;
                    }
                    else
                        index++;
                }
            }
  
            //change the Stage.parm file
            return prefix + index.ToString();
        }
    }

    public class RotateManipulator : Manipulator
    {
        RasterizerState rs = new RasterizerState();
        bool rotating;
        public BEPUphysicsDrawer.Models.InstancedModelDrawer modelDrawer;
        Actor selectedCircle = null;
        List<Actor> circles = new List<Actor>();

        public override void Initialize(ContentManager content, GraphicsDevice device)
        {
            base.Initialize(content, device);

            circles.Add(MakeCircle("RotatePitch", content));
            circles.Add(MakeCircle("RotateYaw", content));
            circles.Add(MakeCircle("RotateRoll", content));

            foreach (Actor circle in circles)
            {
                modelDrawer.Add(circle.PhysicsObject.SpaceObject);
            }
        }

        public override void Draw(float dt, GraphicsDevice device)
        {
            base.Draw(dt, device);

            if (selectedActor != null)
            {

                foreach (Actor circle in circles)
                {
                    circle.PhysicsObject.Position = selectedActor.PhysicsObject.Position;
                }

                //Console.WriteLine(distFromActor * modelScalingConst);
                Rotate();


                device.RasterizerState = rs;
                device.DepthStencilState = DepthStencilState.Default;
                device.Clear(ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                if (selectedCircle != null)
                {
                    DrawCircle(selectedCircle);
                }
                else
                {
                    foreach (Actor circle in circles)
                    {
                        DrawCircle(circle);
                    }
                }
            }

            modelDrawer.Update();
        }

        protected override bool CanSelect()
        {
            // called when the user clicked in the world and the manipulator wants to respond
            // return whether or not the manipulator should try to select an object
            // in this case if we clicked on an arrow, start moving an return false
            // not on an arrow or not moving, then true
            if (rotating == false)
            {
                if (Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    selectedCircle = Pick(circles);

                    if (selectedCircle != null)
                        rotating = true;
                }
            }

            return rotating == false;
        }

        public Actor MakeCircle(string circleName, ContentManager content)
        {
            ParameterSet circleParm = new ParameterSet();
            circleParm.AddParm("AssetName", circleName);
            circleParm.AddParm("ModelName", circleName);
            circleParm.AddParm("Mass", "-1.0");
            circleParm.AddParm("PhysicsType", "Box");
            circleParm.AddParm("ModelType", "EditorManipulator");
            Vector3 zero = Vector3.Zero;
            return new Actor(circleParm, circleName, ref zero, ref zero, content, Stage.ActiveStage);
        }

        public void DrawCircle(Actor circle)
        {
            foreach (ModelMesh mesh in circle.modelInstance.model.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateScale(distFromActor * rotateModelScalingConst) * circle.PhysicsObject.TransformMatrix;
                    effect.View = CameraQB.ViewMatrix;
                    effect.Projection = CameraQB.ProjectionMatrix;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        public void Rotate()
        {
            // check if the user is grabbing and arrow and moving it, if so then move that actor!

            if (rotating)
            {
                if (Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.LeftButton == ButtonState.Released)
                {
                    selectedCircle = null;
                    rotating = false;
                    return;
                }

                Vector2 lastMouse = new Vector2(Stage.ActiveStage.GetQB<ControlsQB>().LastMouseState.X, Stage.ActiveStage.GetQB<ControlsQB>().LastMouseState.Y);
                Vector2 currentMouse = new Vector2(Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.X, Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.Y);
                if (lastMouse != currentMouse)
                {
                    Quaternion RotChange = Quaternion.Identity;
                    Vector2 change = currentMouse - lastMouse;
                    change.Y = -change.Y;

                    CameraQB cameraQB = Stage.ActiveStage.GetQB<CameraQB>();
                    switch (selectedCircle.Name)
                    {
                        case "RotatePitch":
                            RotChange = Quaternion.CreateFromAxisAngle(Vector3.Forward, .1f * (-change.Y * (float)Math.Cos(cameraQB.EditorCamera.Yaw) + -change.X * (float)Math.Sin(cameraQB.EditorCamera.Yaw))); ;
                            break;
                        case "RotateYaw":
                            RotChange = Quaternion.CreateFromAxisAngle(Vector3.Up, (.1f * change.X));
                            break;
                        case "RotateRoll":
                            RotChange = Quaternion.CreateFromAxisAngle(Vector3.Right, .1f * (change.Y * (float)Math.Sin(cameraQB.EditorCamera.Yaw) + -change.X * (float)Math.Cos(cameraQB.EditorCamera.Yaw))); ;
                            break;
                    }

                    string key = getParmKey() + "Rotation";

                    selectedActor.PhysicsObject.Orientation *= RotChange;
                    Vector3 r;
                    Quaternion q = selectedActor.PhysicsObject.Orientation;
                    PhysicsHelpers.QuaternionToEuler(ref q, out r);
                    Stage.ActiveStage.Parm.SetParm(key, r);

                }
            }
        }
    }

    public class MoveManipulator : Manipulator
    {
        RasterizerState rs = new RasterizerState();
        List<Actor> arrows = new List<Actor>();
        Actor selectedArrow = null;
        public BEPUphysicsDrawer.Models.InstancedModelDrawer modelDrawer;
        bool moving = false;

        public override void Initialize(ContentManager content, GraphicsDevice device)
        {
            base.Initialize(content, device);

            arrows.Add(MakeArrow("ForwardArrow", content));
            arrows.Add(MakeArrow("RightArrow", content));
            arrows.Add(MakeArrow("UpArrow", content));

            foreach (Actor arrow in arrows)
            {
                modelDrawer.Add(arrow.PhysicsObject.SpaceObject);
            }
        }

        public override void Draw(float dt, GraphicsDevice device)
        {
            base.Draw(dt, device);

            if (selectedActor != null)
            {
                foreach (Actor arrow in arrows)
                {
                    Vector3 offset = Vector3.Zero;
                    switch (arrow.Name)
                    {
                        case "ForwardArrow":
                            offset = Vector3.Forward;
                            break;
                        case "RightArrow":
                            offset = Vector3.Right;
                            break;
                        case "UpArrow":
                            offset = Vector3.Up;
                            break;
                    }

                    arrow.PhysicsObject.Position = selectedActor.PhysicsObject.Position + offset;
                }

                Move();

                device.RasterizerState = rs;
                device.DepthStencilState = DepthStencilState.Default;
                device.Clear(ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                if (selectedArrow != null)
                {
                    DrawArrow(selectedArrow);
                }
                else
                {
                    foreach (Actor arrow in arrows)
                    {
                        DrawArrow(arrow);
                    }
                }
            }

            modelDrawer.Update();
        }

        protected override bool CanSelect()
        {
            // called when the user clicked in the world and the manipulator wants to respond
            // return whether or not the manipulator should try to select an object
            // in this case if we clicked on an arrow, start moving an return false
            // not on an arrow or not moving, then true
            if (moving == false)
            {
                if (Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    selectedArrow = Pick(arrows);

                    if (selectedArrow != null)
                        moving = true;
                }
            }

            return moving == false;
        }

        public Actor MakeArrow(string arrowName, ContentManager content)
        {
            ParameterSet arrowParm = new ParameterSet();
            arrowParm.AddParm("AssetName", arrowName);
            arrowParm.AddParm("ModelName", arrowName);
            arrowParm.AddParm("Mass", "-1.0");
            arrowParm.AddParm("PhysicsType", "Box");
            arrowParm.AddParm("ModelType", "EditorManipulator");
            Vector3 zero = Vector3.Zero;
            
            return new Actor(arrowParm, arrowName, ref zero, ref zero, content, Stage.ActiveStage);
        }

        public void DrawArrow(Actor arrow)
        {
            Vector3 drawOffset = Vector3.Zero;

            switch (arrow.Name)
            {
                case "ForwardArrow":
                    drawOffset = Vector3.Forward * (distFromActor * moveModelScalingConst - 1);
                    break;
                case "RightArrow":
                    drawOffset = Vector3.Right * (distFromActor * moveModelScalingConst - 1);
                    break;
                case "UpArrow":
                    drawOffset = Vector3.Up * (distFromActor * moveModelScalingConst - 1);
                    break;
            }


            foreach (ModelMesh mesh in arrow.modelInstance.model.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateScale(distFromActor * moveModelScalingConst) * Matrix.CreateTranslation(drawOffset) * arrow.PhysicsObject.TransformMatrix;
                    effect.View = CameraQB.ViewMatrix;
                    effect.Projection = CameraQB.ProjectionMatrix;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        public void Move()
        {
            // check if the user is grabbing and arrow and moving it, if so then move that actor!

            if (moving)
            {
                if (Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.LeftButton == ButtonState.Released)
                {
                    selectedArrow = null;
                    moving = false;
                    return;
                }

                Vector2 lastMouse = new Vector2(Stage.ActiveStage.GetQB<ControlsQB>().LastMouseState.X, Stage.ActiveStage.GetQB<ControlsQB>().LastMouseState.Y);
                Vector2 currentMouse = new Vector2(Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.X, Stage.ActiveStage.GetQB<ControlsQB>().CurrentMouseState.Y);
                if (lastMouse != currentMouse)
                {
                    Vector3 positionChange = Vector3.Zero;
                    Vector2 change = currentMouse - lastMouse;
                    change.Y = -change.Y;

                    // direction we move should be based on the camera
                    // not hard coded as below

                    CameraQB cameraQB = Stage.ActiveStage.GetQB<CameraQB>();

                    switch (selectedArrow.Name)
                    {
                        case "ForwardArrow":
                            positionChange += (change.Y * (float)Math.Cos(cameraQB.EditorCamera.Yaw) + change.X * (float)Math.Sin(cameraQB.EditorCamera.Yaw)) * Vector3.Forward;
                            break;
                        case "RightArrow":
                            positionChange += (-change.Y * (float)Math.Sin(cameraQB.EditorCamera.Yaw) + change.X * (float)Math.Cos(cameraQB.EditorCamera.Yaw)) * Vector3.Right;
                            break;
                        case "UpArrow":
                            positionChange += change.Y * Vector3.Up;
                            break;
                    }
                    string key = getParmKey() + "Position";

                    positionChange *= 0.1f;
                    selectedActor.PhysicsObject.Position += positionChange;
                    Stage.ActiveStage.Parm.SetParm(key, selectedActor.PhysicsObject.Position);

                }
            }
        }
    }
}
