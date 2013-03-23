using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using BEPUphysics;
using GameLib.Engine.AI;

namespace GameLib
{
    public class ActorQB : Quarterback
    {
        LinkedList<Actor> actors; // linked list for fast insertion and removal
        public LinkedList<Actor> Actors { get { return actors; } }

        //Dictionary<string, Spawner> spawns; //dictionary for fast look up
        private ParameterSet Parm;

        public override string Name()
        {
            return "ActorQB";
        }

        public ActorQB()
        {
            actors = new LinkedList<Actor>();
            //spawns = new Dictionary<string, Spawner>();
        }

        public override void PreLoadInit(ParameterSet Parm)
        {
            this.Parm = Parm;
        }

        public override void LoadContent()
        {
            for (int i = 0; Parm.HasParm("Actor" + i); i++)
            {
                string name = Parm.GetString("Actor" + i);
                Vector3 position = Parm.HasParm("Actor" + i + "Position") ? Parm.GetVector3("Actor" + i + "Position") : Vector3.Zero;
                Vector3 rotation = Parm.HasParm("Actor" + i + "Rotation") ? Parm.GetVector3("Actor" + i + "Rotation") : Vector3.Zero;
                rotation = new Vector3(MathHelper.ToRadians(rotation.X), MathHelper.ToRadians(rotation.Y), MathHelper.ToRadians(rotation.Z));
                
                CreateActor(name, name, ref position, ref rotation, Stage.LoadingStage);

                //string[] parm = Parm.GetString("Actor" + i).Split();
                //CreateActor(parm[0], new Vector3(float.Parse(parm[1]),float.Parse(parm[2]),float.Parse(parm[3])));
            }

            for (int i = 0; Parm.HasParm("PreCacheActor" + i); i++)
            {
                string name = Parm.GetString("Actor" + i);
                PreCacheActor(name, Stage.LoadingStage);
            }
        }

        public Actor CreateActor(string parmFile, string instanceName, ref Vector3 position, ref Vector3 rotation, Stage stage)
        {
            Actor newActor = new Actor(parmFile, instanceName, ref position, ref rotation, Stage.Content, stage);
            actors.AddLast(newActor);
            NotifyActorCreatedList(newActor);
            return newActor;
        }

        public Actor CreateActor(ParameterSet parm, string instanceName, ref Vector3 position, ref Vector3 rotation, Stage stage)
        {
            Actor newActor = new Actor(parm, instanceName, ref position, ref rotation, Stage.Content, stage);
            actors.AddLast(newActor);
            NotifyActorCreatedList(newActor);
            return newActor;
        }

        void NotifyActorCreatedList(Actor newActor)
        {
            foreach (ActorCreatedFunc func in actorCreatedFuncList)
            {
                func(newActor);
            }
        }

        public delegate void ActorCreatedFunc(Actor actor);
        List<ActorCreatedFunc> actorCreatedFuncList = new List<ActorCreatedFunc>();
        public void RegisterActorCreatedFunction(ActorCreatedFunc func)
        {
            actorCreatedFuncList.Add(func);
        }

        public void UnRegisterActorCreatedFunction(ActorCreatedFunc func)
        {
            actorCreatedFuncList.Remove(func);
        }

        // precache an actor by loading all its model data, and parm file into the content manager
        // so that its really fast next time
        public static void PreCacheActor(string parmFile, Stage stage)
        {
            ParameterSet parm = Stage.Content.Load<ParameterSet>("Actors/" + parmFile);
            RModel rmodel = new RModel(parm);
            rmodel.LoadContent(Stage.Content, parm, stage);
        }

        public override void Update(float dt)
        {
            if (IsPaused) return;
            LinkedListNode<Actor> node = actors.First;
            if (node == null)
                return;
            do
            {
                if (!node.Value.IsShutdown)
                {
                    node.Value.Update(dt);

                    if (node.Value.MarkedForDeath || node.Value.PhysicsObject.Position.Y < -45.0f) // should be last
                    {
                        LinkedListNode<Actor> tempNode = node;
                        node = node.Previous;
                        actors.Remove(tempNode);

                        tempNode.Value.Kill();
                    }
                }

                if (node != null)
                    node = node.Next;

            } while (node != null);
        }

        public Actor FindActor(string name)
        {
            foreach (Actor actor in actors)
            {
                if (actor.Name == name)
                    return actor;
            }

            return null;
        }

        public LinkedList<Actor> GetActorsByType (GameLib.PhysicsObject.PhysicsType type) {
            LinkedList<Actor> temp = new LinkedList<Actor>();
            foreach (Actor actor in Actors) {
                if (actor.PhysicsObject.physicsType.Equals(type) && !actor.IsShutdown) {
                    temp.AddLast(actor);
                }
            }
            return temp;
        }
        public override void Serialize(ParameterSet parms)
        {
            int index = 0;
            AI ai;
            foreach (Actor actor in Stage.ActiveStage.GetQB<ActorQB>().Actors)
            {
                ai = actor.GetAgentByBaseType<AI>();
                if(ai==null || !ai.spawnedFromTrigger)
                    actor.SerializeStage("Actor" + index++, parms);
            }
        }

        public void EditorKillActor(Actor actor)
        {
            actors.Remove(actor);
            actor.Kill();
        }
    }
}
