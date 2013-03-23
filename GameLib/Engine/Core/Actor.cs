using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics;

namespace GameLib
{
    public class Actor
    {
        public RModelInstance modelInstance;
        public PhysicsObject PhysicsObject { get; set; }
        public bool MarkedForDeath { get; private set; }
        public bool Shown { get { return modelInstance.Shown; } set { modelInstance.Shown = value; } }
        private bool isShutdown;
        public bool IsShutdown { get { return isShutdown; } private set { isShutdown = value; if (isShutdown) Shown = false; } }
        public string Name { get; private set; }
        private ParameterSet parm;
        public List<Agent> agents;     

        public ParameterSet Parm {
            get { return parm; }
        }
        

        public Actor(string dnafile, string name, ref Vector3 position, ref Vector3 rotation, ContentManager content, Stage stage)
            : this(content.Load<ParameterSet>("Actors/" + dnafile), name, ref position, ref rotation, content, stage)
        {
        }

        public Actor(ParameterSet Parm, string name, ref Vector3 position, ref Vector3 rotation, ContentManager content, Stage stage)
        {
            this.parm = Parm;
            Name = name;

            modelInstance = RModelInstance.GetRModelInstance(Parm);
            modelInstance.LoadContent(content, Parm, stage);
            PhysicsObject = new PhysicsObject(this, Parm, modelInstance.model.Model, position, rotation, stage);
            modelInstance.SetPhysicsObject(PhysicsObject);
            modelInstance.FinishLoad();
            
            Shown = true;
            if (Parm.HasParm("Hidden") && Parm.GetBool("Hidden"))
                Shown = false;
            IsShutdown = false;
            MarkedForDeath = false;

            agents = new List<Agent>();
            if (Parm.HasParm("Agents"))
            {
                //read in agents
                string[] agentNames = Parm.GetString("Agents").Split(',');
                foreach (string agentName in agentNames)
                {
                    agents.Add(Agent.CreateAgent(agentName, this));
                }
            }
            foreach (Agent agent in agents)
            {
                if(agent != null)
                    agent.Initialize(stage);
            }
        }
       
        public void AddAgent(Agent agent, Stage stage)
        {
            agents.Add(agent);
            agent.Initialize(stage);
        }

        public T GetAgent<T>() where T : Agent
        {
            foreach (Agent a in agents)
            {
                if (a.GetType() == typeof(T))
                {
                    return a as T;
                }
            }
            return null;
        }

        public T GetAgentByBaseType<T>() where T : Agent
        {
            foreach (Agent a in agents)
            {
                if (a.GetType().BaseType == typeof(T))
                {
                    return a as T;
                }
            }
            return null;
        }

        public void MarkForDeath()
        {
            MarkedForDeath = true;
        }
        
        public void Update(float dt)
        {
            foreach (UpdateFunc update in updateList)
            {
                update(dt);
            }
        }

        public void NotifyDeathList()
        {
            foreach (DeathFunc deathFunc in deathList)
            {
                deathFunc();
            }
        }

        public void NotifyDisarmList()
        {
            foreach (DisarmFunc disarmFunc in disarmList)
            {
                disarmFunc();
            }
        }

        public void Kill()
        {
            NotifyDeathList();
            PhysicsObject.Kill();
            modelInstance.Kill();
        }

        public void KillShutDown()
        {
            NotifyDeathList();
            ShutDown();
        }

        public void ShutDown()
        {
            this.IsShutdown = true;
            if(PhysicsObject.physicsType == GameLib.PhysicsObject.PhysicsType.CylinderCharacter)
                this.PhysicsObject.CylinderCharController.Deactivate();
        }

        public void Revive(ref Vector3 pos, ref Quaternion quat)
        {
            this.MarkedForDeath = false;
            this.PhysicsObject.Position = pos;
            this.PhysicsObject.Orientation = quat;
            foreach (ReviveFunc r in reviveList)
            {
                r.Invoke();
            }

            WakeUp();
        }

        public void WakeUp()
        {
            if (PhysicsObject.physicsType == GameLib.PhysicsObject.PhysicsType.CylinderCharacter)
                this.PhysicsObject.CylinderCharController.Activate();
            this.IsShutdown = false;
            this.Shown = true;
        }
        
        /// <summary>
        /// Converts this actor into a string that can be saved into the stage's parm file.
        /// </summary>
        /// <returns></returns>
        public void SerializeStage(string key, ParameterSet parms)
        {
            //name
            parms.AddParm(key, Name);
            //position
            parms.AddParm(key + "Position", PhysicsObject.Position);
            //rotation
            Vector3 r;
            Quaternion q = PhysicsObject.Orientation;
            PhysicsHelpers.QuaternionToEuler(ref q, out r);
            parms.AddParm(key + "Rotation", r);
        }

        public delegate void UpdateFunc(float dt);
        List<UpdateFunc> updateList = new List<UpdateFunc>();
        public void RegisterUpdateFunction(UpdateFunc update)
        {
            updateList.Add(update);
        }

        public delegate void DeathFunc();
        List<DeathFunc> deathList = new List<DeathFunc>();
        public void RegisterDeathFunction(DeathFunc deathFunc)
        {
            deathList.Add(deathFunc);
        }

        public delegate void ReviveFunc();
        List<ReviveFunc> reviveList = new List<ReviveFunc>();
        public void RegisterReviveFunction(ReviveFunc reviveFunc)
        {
            reviveList.Add(reviveFunc);
        }

        public delegate void DisarmFunc();
        List<DisarmFunc> disarmList = new List<DisarmFunc>();
        public void RegisterDisarmFunction(DisarmFunc disarmFunc)
        {
            disarmList.Add(disarmFunc);
        }

        public delegate void CollideFunc(Actor collidingActor);
        List<CollideFunc> beginCollideList = new List<CollideFunc>();
        List<CollideFunc> endCollideList = new List<CollideFunc>();
        public void RegisterBeginCollideFunction(CollideFunc collide)
        {
            beginCollideList.Add(collide);
        }

        public void RegisterEndCollideFunction(CollideFunc collide)
        {
            endCollideList.Add(collide);
        }

        public void BeginCollision(Actor collidingActor)
        {
            if (collidingActor == null)
                return;
            foreach(CollideFunc func in beginCollideList)
            {
                func(collidingActor);
            }
        }

        public void EndCollision(Actor collidingActor)
        {
            if (collidingActor == null)
                return;
            foreach (CollideFunc func in endCollideList)
            {
                func(collidingActor);
            }
        }
    }
}
