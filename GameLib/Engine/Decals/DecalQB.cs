using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.Decals
{
    // manages all the decals in the game
    // todo:
    // support removing an actor from decal world
    // filter out things that fully contain decal area (backdrop) or don't add them to decalqb?
    // we have to find the closest spot on the mesh and splat to that position, not to the actor's position
    // this might involve doing raycasts in the cardinal directions, finding the closest one that hits
    public class DecalQB : Quarterback
    {
        BEPUphysics.Space decalsWorld;
        public List<Decal> decals = new List<Decal>();
        private List<Ray> cardinalRays = new List<Ray>();
        private List<Vector3> ups = new List<Vector3>();
        private static int MAX_DECALS = 8;

        public override string Name()
        {
            return "DecalQB";
        }

        public DecalQB()
        {
            cardinalRays.Add(new Ray(Vector3.Zero, Vector3.Up));
            cardinalRays.Add(new Ray(Vector3.Zero, Vector3.Down));
            cardinalRays.Add(new Ray(Vector3.Zero, Vector3.Left));
            cardinalRays.Add(new Ray(Vector3.Zero, Vector3.Right));
            cardinalRays.Add(new Ray(Vector3.Zero, Vector3.Forward));
            cardinalRays.Add(new Ray(Vector3.Zero, Vector3.Backward));

            ups.Add(Vector3.Backward);
            ups.Add(Vector3.Forward);
            ups.Add(Vector3.Up);
            ups.Add(Vector3.Up);
            ups.Add(Vector3.Up);
            ups.Add(Vector3.Up);
        }

        public override void PreLoadInit(ParameterSet Parm)
        {
            decalsWorld = new BEPUphysics.Space();

            ActorQB actorQB = Stage.LoadingStage.GetQB<ActorQB>();
            actorQB.RegisterActorCreatedFunction(AddToDecalWorld);
        }

        public override void KillInstance()
        {
            if (decals != null)
            {
                decals.Clear();
                decals = null;
            }

            if (decalsWorld != null)
            {
                decalsWorld.Dispose();
                decalsWorld = null;
            }
        }

        public void AddToDecalWorld(Actor actor)
        {
            // generate a StaticMesh for the actor
            // then add that to the world
            // todo: copy stuff from the actors physics object if we can? (triangle mesh)
            if (actor.PhysicsObject.IsStatic)
            {
                BEPUphysics.Collidables.StaticMesh staticMesh = PhysicsHelpers.ModelToStaticMesh(actor.modelInstance.model.Model, new BEPUphysics.MathExtensions.AffineTransform(actor.PhysicsObject.Orientation, actor.PhysicsObject.Position));
                staticMesh.OwningActor = actor;
                decalsWorld.Add(staticMesh);
            }
        }

        public void RemoveFromDecalWorld(Actor actor)
        {
            // remove actor from physics world
            throw new NotImplementedException();
        }

        public void CreateDecal(Vector3 decalPosition, BoundingBox area, string textureName, float lifetime, float size, DecalLayers layer)
        {
            Texture2D texture = Renderer.Instance.LookupTexture(textureName);
            Decal decal = new Decal(ref decalPosition, texture, lifetime, size, layer);

            Vector3 difference = area.Max - area.Min;
            float length = difference.Z;
            float height = difference.Y;
            float width = difference.X;

            BEPUphysics.CollisionShapes.ConvexShapes.SphereShape sphereShape = new BEPUphysics.CollisionShapes.ConvexShapes.SphereShape(width);
            //BEPUphysics.CollisionShapes.ConvexShapes.BoxShape boxShape = new BEPUphysics.CollisionShapes.ConvexShapes.BoxShape(width, height, length);
            BEPUphysics.MathExtensions.RigidTransform startingTransform = new BEPUphysics.MathExtensions.RigidTransform(decalPosition);
            Vector3 sweep = Vector3.Zero;

            // area is is local space, transform to world space
            area.Min += decalPosition;
            area.Max += decalPosition;

            // use the broad phase's acceleration structure to find all the objects which are in the decal area
            var candidates = BEPUphysics.ResourceManagement.Resources.GetBroadPhaseEntryList();
            decalsWorld.BroadPhase.QueryAccelerator.GetEntries(area, candidates);

            foreach (BEPUphysics.BroadPhaseEntries.BroadPhaseEntry candidate in candidates)
            {
                if (candidate.OwningActor != null)
                {
                    BEPUphysics.RayHit rayHit;
                    bool gotHit = false;
                    Vector3 location = Vector3.Zero;
                    Vector3 up = Vector3.Up;

                    for(int i = 0; i < cardinalRays.Count; i++)
                    {
                        Ray ray = cardinalRays[i];
                        ray.Position = decalPosition;
                        if (candidate.RayCast(ray, width, out rayHit))
                        {
                            gotHit = true;
                            location = rayHit.Location;
                            up = ups[i];
                            break;
                        }
                    }

                    if (!gotHit)
                    {
                        gotHit = true;
                        Actor actor = (Actor)candidate.OwningActor;
                        location = actor.PhysicsObject.Position;
                        //if (candidate.ConvexCast(sphereShape, ref startingTransform, ref sweep, out rayHit))
                        //{
                        //    gotHit = true;
                        //    location = rayHit.Location;
                        //}
                    }

                    if (gotHit)
                    {
                        // apply the decal
                        Actor actor = (Actor)candidate.OwningActor;
                        if (!decal.ApplyToActor(actor, location, up))
                            break;
                    }
                }
            }

            AddDecal(decal);   
        }

        private void AddDecal(Decal decal)
        {
            if (decal.Actors.Count > 0)
            {
                decals.Add(decal);
                decals.Sort();

                if (decals.Count > MAX_DECALS)
                {
                    int index = 0;
                    float life = float.MaxValue;
                    for (int i = 0; i < decals.Count; i++)
                    {
                        if (decals[i].Lifetime < life)
                        {
                            index = i;
                            life = decals[i].Lifetime;
                        }
                    }
                    decals.RemoveAt(index);
                }
            }
        }

        public void CreateDecal(Ray ray, float length, string textureName, float lifetime, float size, DecalLayers layer)
        {
            Texture2D texture = Renderer.Instance.LookupTexture(textureName);
            Decal decal = new Decal(ref ray.Position, texture, lifetime, size, layer);        

            // collide the ray with the world, apply decal to everything it hits
            IList<BEPUphysics.RayCastResult> results = new List<BEPUphysics.RayCastResult>();
            decalsWorld.RayCast(ray, length, results);

            foreach (BEPUphysics.RayCastResult result in results)
            {
                if (result.HitObject.OwningActor != null)
                {
                    // apply the decal
                    if (!decal.ApplyToActor((Actor)result.HitObject.OwningActor, result.HitData.Location, Vector3.Backward))
                        break;
                }
            }

            AddDecal(decal);
        }

        public override void Update(float dt)
        {
            //if (Stage.ActiveStage.GetQB<ControlsQB>().CurrentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V) && Stage.ActiveStage.GetQB<ControlsQB>().LastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.V))
            //{
            //    Vector3 position = new Vector3(153.3582f, 15.80108f, -74.2481f);
            //    Stage.ActiveStage.GetQB<Decals.DecalQB>().CreateDecal(position, new BoundingBox(new Vector3(-20.0f, -20.0f, -20.0f), new Vector3(20.0f, 20.0f, 20.0f)), "Decals/blood", 10.0f, 5.0f, Decals.DecalLayers.BloodLayer);
            //}

            for (int i = decals.Count - 1; i >= 0; i--)
            {
                decals[i].Lifetime -= dt;
                if (decals[i].Lifetime <= 0.0f)
                {
                    decals[i].Actors.Clear(); // just in case
                    decals.RemoveAt(i);
                }
            }
        }
    }
}
