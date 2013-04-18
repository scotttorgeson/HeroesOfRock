using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics;
using System.Threading;
using BEPUphysics.CollisionRuleManagement;

namespace GameLib
{
    public class PhysicsQB : Quarterback
    {
        Space space;
        public Space Space { get { return space; } }

        public static CollisionGroup playerGroup { get; private set; }
        public static CollisionGroup wallGroup { get; private set; }
        public static CollisionGroup normalAIGroup { get; private set; }
        public static CollisionGroup heavyAIGroup { get; private set; }
        public static CollisionGroup missileGroup { get; private set; }
        public static CollisionGroup platformGroup { get; private set; }

        //NOTE: if the player doesn't ever collide with an enemy then we don't need the CollisionGroupPair
        private CollisionGroupPair playerNormalAIPair;
        private CollisionGroupPair playerHeavyAIPair;
        private CollisionGroupPair playerMissilePair;

#if DEBUG
        private InputAction toggleDebugDraw;
#endif

        public override string Name()
        {
 	         return "PhysicsQB";
        }

        static BEPUphysics.Threading.SpecializedThreadManager threadManager;

        public PhysicsQB()
        {
            if (threadManager == null)
            {
                threadManager = new BEPUphysics.Threading.SpecializedThreadManager();
                //This section lets the engine know that it can make use of multithreaded systems
                //by adding threads to its thread pool.
#if XBOX360
            threadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 1 }); }, null);
            threadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 3 }); }, null);
            threadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 4 }); }, null);
            //threadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 5 }); }, null);

#else
                if (Environment.ProcessorCount > 1)
                {
                    for (int i = 0; i < Environment.ProcessorCount; i++)
                    {
                        threadManager.AddThread();
                    }
                }
#endif
            }

            space = new Space(threadManager);
        }

        public override void KillInstance()
        {
            space.Dispose();
        }

        public override void PostLoadInit(ParameterSet Parm)
        {
#if DEBUG
            toggleDebugDraw = Stage.LoadingStage.GetQB<ControlsQB>().GetInputAction("ToggleCollisionDebugDraw");
#endif
        }

#if DEBUG
        private bool debugDrawEnabled = false;
        public void EnableDebugDraw(bool enable)
        {
            if (enable != debugDrawEnabled)
            {
                debugDrawEnabled = enable;

                if (debugDrawEnabled)
                {
                    foreach ( var entity in space.Entities )
                        Renderer.Instance.collisionDebugDrawer.Add(entity);
                }
                else
                {
                    foreach ( var entity in space.Entities )
                        Renderer.Instance.collisionDebugDrawer.Remove(entity);
                }
            }
        }
#endif

        public void AddToSpace(ISpaceObject spaceObject)
        {
            space.Add(spaceObject);

#if DEBUG
            if (debugDrawEnabled)
            {
                Renderer.Instance.collisionDebugDrawer.Add(spaceObject);
            }
#endif
        }

        public void RemoveFromSpace(ISpaceObject spaceObject)
        {
            System.Diagnostics.Debug.Assert(spaceObject.Space == space, "SpaceObject does not belong to this space. Tell Scott what you did to make this happen.");

            if (spaceObject.Space == space)
            {
                space.Remove(spaceObject);

#if DEBUG
                if (debugDrawEnabled)
                {
                    Renderer.Instance.collisionDebugDrawer.Remove(spaceObject);
                }
#endif
            }
        }

        public override void PreLoadInit(ParameterSet Parm)
        {
            setupCollisionGroups();   
            if (Parm.HasParm("Gravity"))
            {
                string[] gravity = Parm.GetString("Gravity").Split();
                float x = float.Parse(gravity[0], System.Globalization.CultureInfo.InvariantCulture);
                float y = float.Parse(gravity[1], System.Globalization.CultureInfo.InvariantCulture);
                float z = float.Parse(gravity[2], System.Globalization.CultureInfo.InvariantCulture);
                Space.ForceUpdater.Gravity = new Vector3(x, y, z);
            }
        }

        private void setupCollisionGroups()
        {

            playerGroup = new CollisionGroup();
            wallGroup = new CollisionGroup();
            missileGroup = new CollisionGroup();
            heavyAIGroup = new CollisionGroup();
            normalAIGroup = new CollisionGroup();
            platformGroup = new CollisionGroup();

            //for player
            playerNormalAIPair = new CollisionGroupPair(playerGroup, normalAIGroup);
            playerHeavyAIPair = new CollisionGroupPair(playerGroup, heavyAIGroup);
            playerMissilePair = new CollisionGroupPair(playerGroup, missileGroup);

            //NOTE: comment this line out if the player should collide with that enemy type
            CollisionRules.CollisionGroupRules.Add(playerNormalAIPair, CollisionRule.NoBroadPhase);
            CollisionRules.CollisionGroupRules.Add(playerHeavyAIPair, CollisionRule.NoBroadPhase);
            CollisionRules.CollisionGroupRules.Add(playerMissilePair, CollisionRule.Normal);
            
            //for missile
            CollisionRules.CollisionGroupRules.Add(new CollisionGroupPair(missileGroup, normalAIGroup), CollisionRule.NoBroadPhase);
            CollisionRules.CollisionGroupRules.Add(new CollisionGroupPair(missileGroup, heavyAIGroup), CollisionRule.NoBroadPhase);
            CollisionRules.CollisionGroupRules.Add(new CollisionGroupPair(missileGroup, platformGroup), CollisionRule.NoBroadPhase);

            //for enemies
            CollisionRules.CollisionGroupRules.Add(new CollisionGroupPair(normalAIGroup, normalAIGroup), CollisionRule.NoBroadPhase);
            CollisionRules.CollisionGroupRules.Add(new CollisionGroupPair(normalAIGroup, heavyAIGroup), CollisionRule.NoBroadPhase);
            CollisionRules.CollisionGroupRules.Add(new CollisionGroupPair(heavyAIGroup, heavyAIGroup), CollisionRule.NoBroadPhase);
            CollisionRules.CollisionGroupRules.Add(new CollisionGroupPair(platformGroup, heavyAIGroup), CollisionRule.NoBroadPhase);

            //for wall
            CollisionRules.CollisionGroupRules.Add(new CollisionGroupPair(wallGroup, normalAIGroup), CollisionRule.NoBroadPhase);
            CollisionRules.CollisionGroupRules.Add(new CollisionGroupPair(wallGroup, heavyAIGroup), CollisionRule.NoBroadPhase);
            CollisionRules.CollisionGroupRules.Add(new CollisionGroupPair(wallGroup, missileGroup), CollisionRule.NoBroadPhase);
        }

        public void startDash()
        {
            CollisionRules.CollisionGroupRules[playerNormalAIPair] = CollisionRule.NoBroadPhase;
            CollisionRules.CollisionGroupRules[playerHeavyAIPair] = CollisionRule.NoBroadPhase;
            CollisionRules.CollisionGroupRules[playerMissilePair] = CollisionRule.NoBroadPhase;
        }

        public void endDash()
        {
            //NOTE: uncomment this line if the player should collide with that enemy type
            //CollisionRules.CollisionGroupRules[playerWeakPair] = CollisionRule.Normal;
            //CollisionRules.CollisionGroupRules[playerRangedPair] = CollisionRule.Normal;
            //CollisionRules.CollisionGroupRules[playerHeavyPair] = CollisionRule.Normal;
            CollisionRules.CollisionGroupRules[playerMissilePair] = CollisionRule.Normal;
        }

        public override void Update(float dt)
        {
            if (IsPaused) return;

            space.Update(dt);

#if DEBUG
            if (toggleDebugDraw.IsNewAction)
            {
                EnableDebugDraw(!debugDrawEnabled);
            }
#endif
        }

        public override void Serialize(ParameterSet parm)
        {
            //TODO::
            //figure out what needs to be updated here
            parm.AddParm("Gravity", space.ForceUpdater.Gravity.X + " " + space.ForceUpdater.Gravity.Y + " " + space.ForceUpdater.Gravity.Z);
        }

        public static bool EnemyRayFilter(CollisionGroup g)
        {
            if (g == missileGroup ||
                g == heavyAIGroup ||
                g == normalAIGroup ||
                g == wallGroup ||
                g == playerGroup ||
                g == platformGroup)
                return false;
            return true;
        }
    }
}
