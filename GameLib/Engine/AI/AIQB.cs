#define SCREWSHIELDEDENEMIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.AI
{
    public class AIQB : Quarterback
    {

        public LinkedList<AI> aliveEnemies { get; private set; }
        private List<SpawnActorTriggerVolume> spawners;

        public List<SpawnActorTriggerVolume> Spawners { get { return spawners; } }

        //pools
        private LinkedList<AI> availableWeakAI;
        private LinkedList<AI> availableRangedAI;
        private LinkedList<AI> availableHeavyAI;

        //the amount of time we need to wait before another enemy can attack (only one can attack at a time)
        private float attackDelayTimer;
        private int enemyAttackIndex;
        public static Random rand;

        public ParameterSet WeakParm;
        public ParameterSet ShieldParm;
        public ParameterSet PogoParm;
        public ParameterSet RangedParm;
        public ParameterSet HeavyParm;
        public ParameterSet AOEParm;
        public ParameterSet ScaredShieldedParm;

        public override string Name()
        {
            return "AIQB";
        }

        public AIQB()
        {
            camLocked = false;
        }

        public override void PreLoadInit(ParameterSet Parm)
        {
            aliveEnemies = new LinkedList<AI>();
            spawners = new List<SpawnActorTriggerVolume>();
            rand = new Random();
        }

        public override void LoadContent()
        {
            MoveDirection = PlayerDirection.Right;
            WeakParm = Stage.Content.Load<ParameterSet>("Actors/EnemyWeak");
            ShieldParm = Stage.Content.Load<ParameterSet>("Actors/EnemyWeakShielded");
            PogoParm = Stage.Content.Load<ParameterSet>("Actors/EnemyRangedPogo");
            RangedParm = Stage.Content.Load<ParameterSet>("Actors/EnemyRanged");
            HeavyParm = Stage.Content.Load<ParameterSet>("Actors/EnemyHeavy");
            AOEParm = Stage.Content.Load<ParameterSet>("Actors/EnemyHeavyAOE");
            ScaredShieldedParm = Stage.Content.Load<ParameterSet>("Actors/EnemyWeakShieldedScared");
        }

        private void InitEnemyPools(ref ParameterSet Parm)
        {
            availableWeakAI = new LinkedList<AI>();
            availableRangedAI = new LinkedList<AI>();
            availableHeavyAI = new LinkedList<AI>();

            //add the number of enemies in the file

            Vector3 zero = Vector3.Zero;

            int num;
            if (Parm.HasParm("EnemyWeakPoolSize"))
                num = Parm.GetInt("EnemyWeakPoolSize");
            else num = 0;

            for(int i = 0; i < num; i++)
                PreLoad("EnemyWeak", ref zero, ref zero);

            if (Parm.HasParm("EnemyRangedPoolSize"))
                num = Parm.GetInt("EnemyRangedPoolSize");
            else num = 0;

            for (int i = 0; i < num; i++)
                PreLoad("EnemyRanged", ref zero, ref zero);

            if (Parm.HasParm("EnemyHeavyPoolSize"))
                num = Parm.GetInt("EnemyHeavyPoolSize");
            else num = 0;

            for(int i = 0; i < num; i++)
                PreLoad("EnemyHeavy", ref zero, ref zero);

            ShutdownAll();
        }

        public override void PostLoadInit(ParameterSet Parm)
        {
            InitEnemyPools(ref Parm);
            base.PostLoadInit(Parm);

            PlayerAgent.Player.RegisterDeathFunction(OnPlayerDeath);
        }

        public void OnPlayerDeath()
        {
            // reset active triggers
            foreach (SpawnActorTriggerVolume spawner in spawners)
            {
                spawner.Reset();
            }

            spawners.Clear();

            ShutdownAll();

            UnboundPlayerOnScreen();
        }

        public override void LevelLoaded()
        {
            //InitEnemyPools();
            //ShutdownAll();
            isBound = false;
            playerBoundingBoxes.Clear();
            base.LevelLoaded();
        }

        public void KillAll()
        {
            foreach (AI ai in aliveEnemies)
            {
                ai.actor.MarkForDeath();
            }
        }

        public void ShutdownAll()
        {
            foreach (AI ai in aliveEnemies)
            {
                ai.actor.ShutDown();
            }
        }

        public override void Update(float dt)
        {
            if (IsPaused) return;

            attackDelayTimer -= dt;

            if (attackDelayTimer <= 0 && enemyAttackIndex < 0)
            {
                //we can attack again, chose a random enemy who is able to attack.
                enemyAttackIndex = rand.Next(aliveEnemies.Count);
            }

            if (aliveEnemies.Count > 0) //if we have enemies that are alive, loop through them
            {
                LinkedListNode<AI> curr;
                curr = aliveEnemies.First;

                Actor a;
                while (curr != null)
                {
                    LinkedListNode<AI> next = curr.Next;
                    a = curr.Value.actor;
                    if (a.IsShutdown || a.MarkedForDeath)
                    {
                        LinkedListNode<AI> temp = curr;

                        aliveEnemies.Remove(temp);
                        if (temp.Value.spawnedFromTrigger)
                        {
                            if (temp.Value.state == AI.AIState.Attacking &&
                                (temp.Value.type != EnemyType.Ranged || temp.Value.type != EnemyType.RangedPogo))
                            {
                                if (attackDelayTimer > .5f) attackDelayTimer = .5f;
                            }
                            switch (curr.Value.type)
                            {
                                case EnemyType.Heavy:
                                    availableHeavyAI.AddLast(temp);
                                    break;
                                case EnemyType.HeavyAOE:
                                    availableHeavyAI.AddLast(temp);
                                    break;
                                case EnemyType.Ranged:
                                    availableRangedAI.AddLast(temp);
                                    break;
                                case EnemyType.RangedPogo:
                                    availableRangedAI.AddLast(temp);
                                    break;
                                case EnemyType.Weak:
                                    availableWeakAI.AddLast(temp);
                                    break;
                                case EnemyType.WeakShielded:
                                    availableWeakAI.AddLast(temp);
                                    break;
                            }
                        }
                        else if(Stage.Editor)
                        {
                            //statically placed enemy, we have to turn shutdown and return to spawn position so that he 
                            //will be saved if we are in the editor
                            temp.Value.actor.PhysicsObject.Position = temp.Value.spawnPos;
                        }

                        if (temp.Value.spawnerIndex >= 0 && temp.Value.spawnerIndex < spawners.Count)
                            spawners[temp.Value.spawnerIndex].aliveEnemyCount--;
                    }
                    else if (curr.Value.state == AI.AIState.WaitingToAttack)
                    {
                        //ranged enemies can attack at will
                        if (curr.Value.type == EnemyType.Ranged || curr.Value.type == EnemyType.RangedPogo)
                        {
                            curr.Value.AttackTarget();
                        }
                        else
                        {
                            if (enemyAttackIndex == 0)
                                attackDelayTimer = curr.Value.AttackTarget();
                            enemyAttackIndex--;
                        }
                    }
                    curr = next;
                }
            }
            //number of enemies alive is zero, spawn the next wave or unlock the camera
            //do note that timed spawners take care of their own spawning locally
            else if (spawners.Count > 0) 
            {
                bool allFinished = true;
                foreach(SpawnActorTriggerVolume s in spawners)
                {
                    if (!s.Finished)
                    {
                        allFinished = false;

                        if (s.WaveSpawn) //else spawn a enemy if we are a wave spawner
                            s.SpawnWave();
                    }
                }
                //only remove if they are all finished, this way if the player dies we can reinit everything
                if (allFinished)
                {
                    foreach (SpawnActorTriggerVolume s in spawners)
                    {
                        s.Kill();
                    }
                    spawners.Clear();
                }
            }
            else if(isBound) //there are no enemies alive, and no spawners active so we should unlock the camera if needed
            {
                UnboundPlayerOnScreen();
            }
        }

        public static bool camLocked = false;

        //this is O(n) time currently, there is probably a better way to do it
        public Actor FindClosestEnemyInDir(GameLib.PlayerDirection dir, ref Vector3 pos, ref DashAttack d, float attackRange)
        {
            Actor toReturn = null;
            float minDist = float.PositiveInfinity;
            float currDist;

            Vector3 enemyPos;

            foreach (AI e in aliveEnemies)
            {

                enemyPos = e.actor.PhysicsObject.Position;
                if (!OnScreen(ref pos, ref enemyPos))
                    continue;
                switch (dir)
                {
                    case PlayerDirection.Right:
                        if (enemyPos.X >= pos.X)
                        {
                            currDist = Math.Abs(enemyPos.X - pos.X);
                            if (currDist < minDist)
                            {
                                minDist = currDist;
                                toReturn = e.actor;
                            }
                        }
                        break;
                    case PlayerDirection.Left:

                        if (enemyPos.X <= pos.X)
                        {
                            currDist = Math.Abs(enemyPos.X - pos.X);
                            if (currDist < minDist)
                            {
                                minDist = currDist;
                                toReturn = e.actor;
                            }
                        }

                        break;
                    case PlayerDirection.Forward:
                        if (enemyPos.Z >= pos.Z)
                        {
                            currDist = Math.Abs(enemyPos.Z - pos.Z);
                            if (currDist < minDist)
                            {
                                minDist = currDist;
                                toReturn = e.actor;
                            }
                        }

                        break;
                    case PlayerDirection.Backward:
                        if (enemyPos.Z <= pos.Z)
                        {
                            currDist = Math.Abs(enemyPos.Z - pos.Z);
                            if (currDist < minDist)
                            {
                                minDist = currDist;
                                toReturn = e.actor;
                            }
                        }

                        break;
                }

            }

            d.Target = toReturn;
            d.Dist = minDist;
            d.AttackRange = attackRange;
            d.Direction = dir;

            return toReturn;
        }

        public void AddLiveEnemy(Actor a)
        {
            AI ai = a.GetAgentByBaseType<AI>();
            if(ai!=null)
                aliveEnemies.AddLast(ai);
        }

        public int AddRefToSpawner(SpawnActorTriggerVolume s)
        {
            spawners.Add(s);
            return spawners.Count - 1;
        }

        public void Spawn(string enemyType, ref Vector3 basePos, ref Vector3 rot, int spawnerIndex)
        {
            Vector3 pos = RandomizeSpawn(ref basePos);

            AI temp = null;
            //we will eventually look in the enemy pool
            switch (enemyType)
            {
                case "EnemyWeak":
                    if (availableWeakAI.Count > 0)
                    {
                        temp = availableWeakAI.First.Value;
                        availableWeakAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
                        temp.ConvertTo(EnemyType.Weak, ref WeakParm);
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
                        Actor a = aQB.CreateActor(enemyType, enemyType, ref pos, ref rot, Stage.ActiveStage);
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case "EnemyWeakShielded":
                    if (availableWeakAI.Count > 0)
                    {
                        temp = availableWeakAI.First.Value;
                        availableWeakAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
#if !SCREWSHIELDEDENEMIES
                        temp.ConvertTo(EnemyType.WeakShielded, ref ShieldParm);
#endif
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
#if SCREWSHIELDEDENEMIES
                        Actor a = aQB.CreateActor("EnemyWeak", "EnemyWeak", ref pos, ref rot, Stage.ActiveStage);
#else
                        Actor a = aQB.CreateActor(enemyType, enemyType, ref pos, ref rot, Stage.ActiveStage);
#endif
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case "EnemyRanged":
                    if (availableRangedAI.Count > 0)
                    {
                        temp = availableRangedAI.First.Value;
                        availableRangedAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
                        temp.ConvertTo(EnemyType.Ranged, ref RangedParm);
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
                        Actor a = aQB.CreateActor(enemyType, enemyType, ref pos, ref rot, Stage.ActiveStage);
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case "EnemyRangedPogo":
                    if (availableRangedAI.Count > 0)
                    {
                        temp = availableRangedAI.First.Value;
                        availableRangedAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
                        temp.ConvertTo(EnemyType.RangedPogo, ref PogoParm);
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
                        Actor a = aQB.CreateActor(enemyType, enemyType, ref basePos, ref rot, Stage.ActiveStage); //no offset if the enemy is on a platform
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case "EnemyHeavy":
                    if (availableHeavyAI.Count > 0)
                    {
                        temp = availableHeavyAI.First.Value;
                        availableHeavyAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
                        temp.ConvertTo(EnemyType.Heavy, ref HeavyParm);
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
                        Actor a = aQB.CreateActor(enemyType, enemyType, ref pos, ref rot, Stage.ActiveStage);
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case "EnemyHeavyAOE":
                    if (availableHeavyAI.Count > 0)
                    {
                        temp = availableHeavyAI.First.Value;
                        availableHeavyAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
                        temp.ConvertTo(EnemyType.HeavyAOE, ref AOEParm);
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
                        Actor a = aQB.CreateActor(enemyType, enemyType, ref pos, ref rot, Stage.ActiveStage);
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case "EnemyDumb": //we do not pool dumb enemies
                    ActorQB actorQB = Stage.ActiveStage.GetQB<ActorQB>();
                    Actor actor = actorQB.CreateActor(enemyType, enemyType, ref basePos, ref rot, Stage.ActiveStage);
                    temp = actor.GetAgentByBaseType<AI>();
                    aliveEnemies.AddLast(temp);
                    break;
            }

            if (temp != null)
            {
                temp.spawnerIndex = spawnerIndex;
                temp.spawnedFromTrigger = true;
            }
        }

        public void Spawn(char enemyType, ref Vector3 basePos, ref Vector3 rot, int spawnerIndex)
        {
            Vector3 pos = RandomizeSpawn(ref basePos);

            AI temp = null;
            //we will eventually look in the enemy pool
            switch (enemyType)
            {
                case 'W':
                    if (availableWeakAI.Count > 0)
                    {
                        temp = availableWeakAI.First.Value;
                        availableWeakAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
                        temp.ConvertTo(EnemyType.Weak, ref WeakParm);
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
                        Actor a = aQB.CreateActor("EnemyWeak", "EnemyWeak", ref pos, ref rot, Stage.ActiveStage);
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case 'S':
                    if (availableWeakAI.Count > 0)
                    {
                        temp = availableWeakAI.First.Value;
                        availableWeakAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
#if !SCREWSHIELDEDENEMIES
                        temp.ConvertTo(EnemyType.WeakShielded, ref ShieldParm);
#endif
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
#if SCREWSHIELDEDENEMIES
                        Actor a = aQB.CreateActor("EnemyWeak", "EnemyWeak", ref pos, ref rot, Stage.ActiveStage);
#else
                        Actor a = aQB.CreateActor("EnemyWeakShielded", "EnemyWeakShielded", ref pos, ref rot, Stage.ActiveStage);
#endif
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case 'R':
                    if (availableRangedAI.Count > 0)
                    {
                        temp = availableRangedAI.First.Value;
                        availableRangedAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
                        temp.ConvertTo(EnemyType.Ranged, ref RangedParm);
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
                        Actor a = aQB.CreateActor("EnemyRanged", "EnemyRanged", ref pos, ref rot, Stage.ActiveStage);
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case 'P':
                    if (availableRangedAI.Count > 0)
                    {
                        temp = availableRangedAI.First.Value;
                        availableRangedAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
                        temp.ConvertTo(EnemyType.RangedPogo, ref PogoParm);
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
                        Actor a = aQB.CreateActor("EnemyRangedPogo", "EnemyRangedPogo", ref basePos, ref rot, Stage.ActiveStage); //no offset if the enemy is on a platform
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case 'H':
                    if (availableHeavyAI.Count > 0)
                    {
                        temp = availableHeavyAI.First.Value;
                        availableHeavyAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
                        temp.ConvertTo(EnemyType.Heavy, ref HeavyParm);
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
                        Actor a = aQB.CreateActor("EnemyHeavy","EnemyHeavy", ref pos, ref rot, Stage.ActiveStage);
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
                case 'A':
                    if (availableHeavyAI.Count > 0)
                    {
                        temp = availableHeavyAI.First.Value;
                        availableHeavyAI.RemoveFirst();
                        Quaternion quat = EulerToQuaternion(rot);
                        temp.ConvertTo(EnemyType.HeavyAOE, ref AOEParm);
                        temp.actor.Revive(ref pos, ref quat);
                        aliveEnemies.AddLast(temp);
                    }
                    else
                    {
                        ActorQB aQB = Stage.ActiveStage.GetQB<ActorQB>();
                        Actor a = aQB.CreateActor("EnemyHeavyAOE", "EnemyHeavyAOE", ref pos, ref rot, Stage.ActiveStage);
                        temp = a.GetAgentByBaseType<AI>();
                        aliveEnemies.AddLast(temp);
                    }
                    break;
            }
            if (temp != null)
            {
                temp.spawnerIndex = spawnerIndex;
                temp.spawnedFromTrigger = true;
            }
        }

        public void PreLoad(string enemyType, ref Vector3 basePos, ref Vector3 rot)
        {
            Vector3 pos = RandomizeSpawn(ref basePos);

            if (Stage.LoadingStage == null) return;

            ActorQB aQB = Stage.LoadingStage.GetQB<ActorQB>();
            Actor a;
            AI ai = null;

            switch (enemyType)
            {
                case "EnemyWeak":
                    a = aQB.CreateActor("EnemyWeak", "EnemyWeak", ref pos, ref rot, Stage.LoadingStage);
                    ai = a.GetAgentByBaseType<AI>();
                    aliveEnemies.AddLast(ai);
                    break;
                case "EnemyRanged":
                    a = aQB.CreateActor(enemyType, enemyType, ref pos, ref rot, Stage.LoadingStage);
                    //Actor a = aQB.CreateActor(enemyType, enemyType, ref basePos, ref rot, Stage.ActiveStage); //no offset if the enemy is on a platform
                    ai = a.GetAgentByBaseType<AI>();
                    aliveEnemies.AddLast(ai);
                    break;
                case "EnemyHeavy":
                    if (MoveDirection == PlayerDirection.Left || MoveDirection == PlayerDirection.Right)
                        pos.X = basePos.X;
                    else
                        pos.Z = basePos.Z;
                    a = aQB.CreateActor(enemyType, enemyType, ref pos, ref rot, Stage.LoadingStage);
                    ai = a.GetAgentByBaseType<AI>();
                    aliveEnemies.AddLast(ai);
                    break;
            }
            if (ai != null)
                ai.spawnedFromTrigger = true;

        }

        public override void DrawUI(float dt)
        {                
            base.DrawUI(dt);
        }

        public override void Serialize(ParameterSet parm)
        {

        }

        public static Quaternion EulerToQuaternion(Vector3 rot)
        {
            Quaternion q = Quaternion.Identity;

            double s1 = Math.Sin(rot.X * .5);
            double s2 = Math.Sin(rot.Y * .5);
            double s3 = Math.Sin(rot.Z * .5);
            double c1 = Math.Cos(rot.X * .5);
            double c2 = Math.Cos(rot.Y * .5);
            double c3 = Math.Cos(rot.Z * .5);

            q.W = (float)(c1 * c2 * c3 + s1 * s2 * s3);
            q.X = (float)(s1 * c2 * c3 - c1 * s2 * s3);
            q.Y = (float)(c1 * s2 * c3 + s1 * c2 * s3);
            q.Z = (float)(c1 * c2 * s3 - s1 * s2 * c3);

            return q;
        }

        public static GameLib.PlayerDirection MoveDirection;

        /// <summary>
        /// Returns the square distance between the two positions, only taking into account 
        /// necessary dimensions based on the current move direction
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float DistanceSquared(Vector3 a, Vector3 b)
        {
            switch (MoveDirection)
            {
                case PlayerDirection.Right:
                    goto case PlayerDirection.Left;
                case PlayerDirection.Left:
                    //X & Y
                    return ((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
                case PlayerDirection.Up:
                    goto case PlayerDirection.Down;
                case PlayerDirection.Down:
                    //X & Z
                    return ((a.X - b.X) * (a.X - b.X) + (a.Z - b.Z) * (a.Z - b.Z));
                case PlayerDirection.Forward:
                    goto case PlayerDirection.Backward;
                case PlayerDirection.Backward:
                    //Z & Y
                    return ((a.Z - b.Z) * (a.Z - b.Z) + (a.Y - b.Y) * (a.Y - b.Y));
                default:
                    return 0;
            }
        }


        private static int spawnLaneIndex = 0;
        private static int numSpawnLanes = 3;
        private static float spawnLaneOffset = 2.0f;

        public static Vector3 RandomizeSpawn(ref Vector3 pos)
        {
            Vector3 returnPos = pos;
            float offset = spawnLaneOffset * spawnLaneIndex++;
            offset -= 1 * spawnLaneOffset; //this will need to be not 2 if we change the numSpawnLanes

            if (spawnLaneIndex >= numSpawnLanes) spawnLaneIndex = 0;

            switch (MoveDirection)
            {
                case PlayerDirection.Left:
                    goto case PlayerDirection.Right;
                case PlayerDirection.Right:
                    returnPos.Z = PlayerAgent.track + offset;
                    returnPos.X += 2*offset; // so they don't all spawn in the same place
                    break;
                case PlayerDirection.Backward:
                    goto case PlayerDirection.Forward;
                case PlayerDirection.Forward:
                    returnPos.X = PlayerAgent.track + offset;
                    returnPos.Z += 2*offset; //so they don't all spawn in the same place
                    break;
            }

            return returnPos;
        }

        private static List<BEPUphysics.Entities.Prefabs.Box> playerBoundingBoxes = new List<BEPUphysics.Entities.Prefabs.Box>(2);
        private static bool isBound = false;
        private const float boundOffset = 15.0f;
        private static float boundBoxThickness = 1.0f;
        public static void BoundPlayerOnScreen(Vector3 center)
        {
            
            if (!isBound)
            {
                CameraQB c = Stage.ActiveStage.GetQB<CameraQB>();
                PlayerCamera p = (c.ActiveCamera as PlayerCamera);

                float halfFov = (float)((Math.PI / 2 - Math.Atan(p.ProjectionMatrix.M22)));
                float aspect = p.ProjectionMatrix.M22 / p.ProjectionMatrix.M11;

                float offset;
                BEPUphysics.Entities.Prefabs.Box box1, box2;
                if (MoveDirection == PlayerDirection.Right || MoveDirection == PlayerDirection.Left)
                {
                    offset = aspect * Math.Abs(PlayerAgent.Player.PhysicsObject.Position.Z - p.DesiredPosition.Z) * (float)Math.Tan(halfFov);
                    Vector3 box1Center = new Vector3(center.X - offset, center.Y, center.Z);
                    Vector3 box2Center = new Vector3(center.X + offset, center.Y, center.Z);
                    box1 = new BEPUphysics.Entities.Prefabs.Box(box1Center, boundBoxThickness, 50.0f, 20.0f);
                    box2 = new BEPUphysics.Entities.Prefabs.Box(box2Center, boundBoxThickness, 50.0f, 20.0f);
                }
                else
                {
                    offset = aspect * Math.Abs(PlayerAgent.Player.PhysicsObject.Position.X - p.DesiredPosition.X) * (float)Math.Tan(halfFov);
                    Vector3 box1Center = new Vector3(center.X, center.Y, center.Z - offset);
                    Vector3 box2Center = new Vector3(center.X, center.Y, center.Z + offset);
                    box1 = new BEPUphysics.Entities.Prefabs.Box(box1Center, 20.0f, 50.0f, boundBoxThickness);
                    box2 = new BEPUphysics.Entities.Prefabs.Box(box2Center, 20.0f, 50.0f, boundBoxThickness);
                }


                box1.CollisionInformation.CollisionRules.Group = PhysicsQB.wallGroup;
                box2.CollisionInformation.CollisionRules.Group = PhysicsQB.wallGroup;

                playerBoundingBoxes.Add(box1);
                playerBoundingBoxes.Add(box2);

                PhysicsQB physicsQB = Stage.ActiveStage.GetQB<PhysicsQB>();

                physicsQB.AddToSpace(box1);
                physicsQB.AddToSpace(box2);

                PlayerAgent.Player.GetAgent<PlayerAgent>().ReachedStopZone();
                PlayerAgent.Player.GetAgent<GameLib.Engine.AttackSystem.RockMeter>().RockLevelLocked = false;
                isBound = true;
            }
        }

        public static void UnboundPlayerOnScreen()
        {
            if (isBound)
            {
                //pop cameras, until we pop the chase camera. (just in case there is a free camera on top of it in the stack)
                CameraQB cameraQB = Stage.ActiveStage.GetQB<CameraQB>();
                //cameraQB.PopCamera(typeof(ChaseCamera), 10.0f);
                (cameraQB.ActiveCamera as PlayerCamera).ChaseTarget = PlayerAgent.Player.PhysicsObject;

                camLocked = false;

                PhysicsQB physicsQB = Stage.ActiveStage.GetQB<PhysicsQB>();
                physicsQB.RemoveFromSpace(playerBoundingBoxes[0]);
                physicsQB.RemoveFromSpace(playerBoundingBoxes[1]);
                playerBoundingBoxes.Clear();

                PlayerAgent.Player.GetAgent<GameLib.Engine.AttackSystem.RockMeter>().RockLevelLocked = true;
                isBound = false;
                PlayerAgent.Player.GetAgent<PlayerAgent>().MoveToNextStopZone();
            }
        }

        public static bool OnScreen(ref Vector3 playerPos, ref Vector3 enemyPos)
        {
            if (isBound)
            {
                if (MoveDirection == PlayerDirection.Right || MoveDirection == PlayerDirection.Left)
                    return (enemyPos.X > playerBoundingBoxes[0].Position.X + boundBoxThickness && 
                            enemyPos.X < playerBoundingBoxes[1].Position.X - boundBoxThickness) ;
                else
                    return (enemyPos.Z > playerBoundingBoxes[0].Position.Z + boundBoxThickness && 
                            enemyPos.Z < playerBoundingBoxes[1].Position.Z - boundBoxThickness);
            }
            else
            {
                if (MoveDirection == PlayerDirection.Left || MoveDirection == PlayerDirection.Right)
                    return (enemyPos.X > playerPos.X - boundOffset && enemyPos.X < playerPos.X + boundOffset);
                else
                    return (enemyPos.Z > playerPos.Z - boundOffset && enemyPos.Z < playerPos.Z + boundOffset);
                
            }
        }

        public static Vector3 RandomPosOnScreen(Vector3 start, float minDist)
        {
            Vector3 newPos = start;
            if (isBound)
            {
                if (MoveDirection == PlayerDirection.Right || MoveDirection == PlayerDirection.Left)
                {
                    newPos.X = (float)rand.NextDouble() * (playerBoundingBoxes[1].Position.X - playerBoundingBoxes[0].Position.X - 
                        boundBoxThickness*4 - minDist*2) + playerBoundingBoxes[0].Position.X + 2*boundBoxThickness + minDist;
                }
                else
                {
                    newPos.Z = (float)rand.NextDouble() * (playerBoundingBoxes[1].Position.Z - playerBoundingBoxes[0].Position.Z -
                        boundBoxThickness*4 - minDist*2) + playerBoundingBoxes[0].Position.Z + 2*boundBoxThickness + minDist;
                }
            }
            return newPos;
        }

        public static Vector3 PosOnSideOfScreen(Vector3 start, int dir, float offset)
        {
            Vector3 newPos = start;
            if (isBound)
            {
                if (MoveDirection == PlayerDirection.Right || MoveDirection == PlayerDirection.Left)
                {
                    if (dir > 0)
                        newPos.X = playerBoundingBoxes[1].Position.X - 2 * boundBoxThickness - offset;
                    else
                        newPos.X = playerBoundingBoxes[0].Position.X + 2 * boundBoxThickness + offset;
                }
                else
                {
                    if (dir > 0)
                        newPos.Z = playerBoundingBoxes[1].Position.Z - 2 * boundBoxThickness - offset;
                    else
                        newPos.Z = playerBoundingBoxes[0].Position.Z + 2 * boundBoxThickness + offset;
                }
            }
            return newPos;
        }

    }
}
