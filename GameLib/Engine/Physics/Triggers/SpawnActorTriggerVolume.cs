using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameLib.Engine.AI;


namespace GameLib
{
    public class SpawnActorTriggerVolume : TriggerVolume
    {
       
        string ActorType;
        Vector3 position;
        int spawnCount;
        float freq;

        //variables specifying the current progress
        int index;
        bool triggered;
        bool finished;
        public bool Finished
        {
            get { return finished; }
        }

        float delay;
        int numEnemiesSpawned;
        float spawnTimer;
        bool listOfEnemies;
        bool waveSpawn;
        public bool WaveSpawn
        {
            get { return waveSpawn; }
        }
        bool constantSpawn;
        public bool ConstantSpawn
        {
            get { return constantSpawn; }
        }

        public int aliveEnemyCount;

        /// <summary>
        /// Read trigger specific parameters from the world parm and add them to the actor parm
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        new public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            if (worldParm.HasParm("Count"))
                actorParm.AddParm("Count", worldParm.GetInt("Count"));
            if (worldParm.HasParm("Freq"))
                actorParm.AddParm("Freq", worldParm.GetFloat("Freq"));
            if (worldParm.HasParm("SpawnInWaves"))
                actorParm.AddParm("SpawnInWaves", worldParm.GetBool("SpawnInWaves"));
            if (worldParm.HasParm("DelayForFirst"))
                actorParm.AddParm("DelayForFirst", worldParm.GetFloat("DelayForFirst"));
            if (worldParm.HasParm("ConstantSpawns"))
                actorParm.AddParm("ConstantSpawns", worldParm.GetBool("ConstantSpawns"));
            if (worldParm.HasParm("EnemyList"))
                actorParm.AddParm("EnemyList", worldParm.GetBool("EnemyList"));

            System.Diagnostics.Debug.Assert(worldParm.HasParm("ActorType"), "SpawnActorTriggerVolume requires an actor type to spawn!");
            actorParm.AddParm("ActorType", worldParm.GetString("ActorType"));

            System.Diagnostics.Debug.Assert(worldParm.HasParm("SpawnPos"), "SpawnActorTriggerVolume requires a position to spawn at!");
            actorParm.AddParm("SpawnPos", worldParm.GetString("SpawnPos"));
        }

        public SpawnActorTriggerVolume(Actor actor)
            : base(actor)
        {
            Name = "SpawnActorTriggerVolume";
        }

        public override void Initialize(Stage stage)
        {
            spawnCount = 0;
            freq = 0;
            numEnemiesSpawned = 0;
            spawnTimer = 0;
            delay = 0.0f;
            waveSpawn = false;

            ActorType = actor.Parm.GetString("ActorType");
            position = actor.Parm.GetVector3("SpawnPos");
            if (actor.Parm.HasParm("Count"))
                spawnCount = actor.Parm.GetInt("Count");
            if (actor.Parm.HasParm("Freq"))
                freq = actor.Parm.GetFloat("Freq");
            if (actor.Parm.HasParm("SpawnInWaves"))
                waveSpawn = actor.Parm.GetBool("SpawnInWaves");
            if (actor.Parm.HasParm("DelayForFirst"))
                delay = actor.Parm.GetFloat("DelayForFirst");
            if (actor.Parm.HasParm("ConstantSpawns"))
            {
                constantSpawn = actor.Parm.GetBool("ConstantSpawns");
                if (constantSpawn)
                    waveSpawn = false;
            }
            if (actor.Parm.HasParm("EnemyList"))
            {
                listOfEnemies = actor.Parm.GetBool("EnemyList");
                spawnCount = ActorType.Length;
            }


            triggered = false;

            //we only need to update if we have a pause and we spawn multiple people
            //otherwise all of the enemies will be spawned with the first call to Spawn() in OnTriggerEnter
            if (freq > 0.0f && spawnCount > 1 || delay > 0.0f)
                actor.RegisterUpdateFunction(Update);
            
            UsingOnTriggerEnter = true;
            UsingOnTriggerStay = false;
            UsingOnTriggerExit = false;

            base.Initialize(stage);

            DieOnTrigger = false; // camera looks at this trigger, can't kill it.
        }

        public void Reset()
        {
            spawnCount = 0;
            aliveEnemyCount = 0;
            freq = 0;
            numEnemiesSpawned = 0;
            spawnTimer = 0;
            waveSpawn = false;

            ActorType = actor.Parm.GetString("ActorType");
            position = actor.Parm.GetVector3("SpawnPos");
            if (actor.Parm.HasParm("Count"))
                spawnCount = actor.Parm.GetInt("Count");
            if (actor.Parm.HasParm("Freq"))
                freq = actor.Parm.GetFloat("Freq");
            if (actor.Parm.HasParm("SpawnInWaves"))
                waveSpawn = actor.Parm.GetBool("SpawnInWaves");
            if (actor.Parm.HasParm("ConstantSpawns"))
            {
                constantSpawn = actor.Parm.GetBool("ConstantSpawns");
                if (constantSpawn)
                    waveSpawn = false;
            }
            if (actor.Parm.HasParm("EnemyList"))
            {
                listOfEnemies = actor.Parm.GetBool("EnemyList");
                spawnCount = ActorType.Length;
            }

            finished = false;
            triggered = false;
        }
        
        new public void Update(float dt)
        {
            if (triggered && !finished)
            {
                if (!waveSpawn)
                {
                    if (constantSpawn)
                    {
                        if (aliveEnemyCount < freq)
                            Spawn((int)freq - aliveEnemyCount);
                    }
                    else
                    {
                        //if (Stage.ActiveStage.GetQB<TempoQB>().isNewBeat) freq += spawnTimer;
                        spawnTimer += 1;
                        if (spawnTimer >= freq) Spawn(1);
                    }
                }
            }
        }

        /// <summary>
        /// spawn the number of enemies given by count (number of spawns is capped by spawnCount)
        /// </summary>
        /// <param name="count">number of enemies to spawn</param>
        public void Spawn(int count)
        {

            AIQB aiQB = Stage.ActiveStage.GetQB<AIQB>();
            if (aiQB != null)
            {
                Vector3 rot = Vector3.Zero;
                while (count > 0 && numEnemiesSpawned < spawnCount)
                {
                    if (listOfEnemies)
                        aiQB.Spawn(ActorType[numEnemiesSpawned], ref position, ref rot, index);
                    else
                        aiQB.Spawn(ActorType, ref position, ref rot, index);
                    count--;
                    numEnemiesSpawned++;
                    aliveEnemyCount++;
                }
                spawnTimer = 0.0f;

                if (numEnemiesSpawned >= spawnCount)
                {
                    finished = true;
                }
            }
        }

        /// <summary>
        /// call this to spawn a wave (only works for wave spawners)
        /// spawns freq (rounded up to the next integer) number of enemies
        /// </summary>
        public void SpawnWave()
        {
            if (waveSpawn)
                Spawn((int)Math.Ceiling(freq));

        }
        public override void OnTriggerEnter(Actor triggeringActor)
        {
            if (!triggered)
            {
                PlayerAgent.SetRespawnPosition();
                
                triggered = true;
                finished = false;

                AIQB aiQB = Stage.ActiveStage.GetQB<AIQB>();

                
                if (!AIQB.camLocked) // if we aren't already locked....
                {
                    // ...then lock the camera by focusing it on this trigger.
                    AIQB.camLocked = true;
                    CameraQB cameraQB = Stage.ActiveStage.GetQB<CameraQB>();
                    //cameraQB.TransitionToCamera(new ChaseCamera(CameraQB.WorldMatrix.Translation, CameraQB.Speed, cameraQB.ActiveCamera.Pitch, cameraQB.ActiveCamera.Yaw, CameraQB.DefaultProjectionMatrix, actor.PhysicsObject, CameraQB.Offset, false, CameraQB.Distance), 10.0f);
                    (cameraQB.ActiveCamera as PlayerCamera).ChaseTarget = actor.PhysicsObject;

                    AIQB.BoundPlayerOnScreen(actor.PhysicsObject.Position);
                }

                index = aiQB.AddRefToSpawner(this);

                //always spawn the first when we enter the trigger
                if (freq == 0.0 && !waveSpawn)
                    Spawn(spawnCount); //if we do not pause between spawns, spawn them all at the start
                else if (waveSpawn)
                    SpawnWave();
                else if (constantSpawn)
                    Spawn((int)freq);
                else if (delay <= 0.0f)
                    Spawn(1);
  
                spawnTimer = -delay;
            }
        }

        public void Kill()
        {
            actor.MarkForDeath();
        }

        public override void Serialize(ref ParameterSet parm)
        {
            parm.AddParm("ActorType", ActorType);
            parm.AddParm("SpawnPos", position);
            parm.AddParm("Count", spawnCount);
            parm.AddParm("Freq", freq);
            parm.AddParm("SpawnInWaves", waveSpawn);
            parm.AddParm("DelayForFirst", delay);
            parm.AddParm("ConstantSpawns", constantSpawn);
            parm.AddParm("EnemyList", listOfEnemies);

            base.Serialize(ref parm);
        }
    }
}
