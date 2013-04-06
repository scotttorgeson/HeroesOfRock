using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.Particles
{
    public class ParticleEmitter
    {

        bool emit; //Should particles be automatically emitted each frame?
        public bool Emit
        {
            set
            {
                emit = value;
                if (emit)
                {
                    numToSpawnThisSecond = rand.Next(minEmission, maxEmission + 1);
                    numSpawnedThisSecond = 0;
                }
            }

            get { return emit; }
        }

        private bool MarkedDead;

        bool oneShot; //if enabled all the particles for one emmission (between minEmmission and maxEmmision) will be spawned at once, and then emit will be turned off
        public bool OneShot //changing one shot will also stop the emitting
        {
            set
            {
                oneShot = value;
                Emit = false;
            }
            get { return oneShot; }
        }

        public bool dead { get; private set; }
        public bool rndAngle; //if enabled, the particles will move in a random angle at the specified velocity
        public Vector2 minSize;	 //The minimum size each particle can be at the time when it is spawned.
        public Vector2 maxSize;	 //The maximum size each particle can be at the time when it is spawned.
        public float minEnergy;	 //The minimum lifetime of each particle, measured in seconds.
        public float maxEnergy;	 //The maximum lifetime of each particle, measured in seconds.
        public int minEmission;	 //The minimum number of particles that will be spawned every second.
        public int maxEmission;	 //The maximum number of particles that will be spawned every second.
        public Vector3 localVelocity;	 //The starting speed of particles along X, Y, and Z, measured in the object's orientation.
        public Vector3 rndVelocity;	 //A random speed along X, Y, and Z that is added to the velocity.
        public bool rndRotation;	 //If enabled, the particles will be spawned with random rotations.
        public Vector3 spawnRange; //the +/- dimensions that define where the particle can spawn
        public float lifeTime; //how long the emiter is alive for, if negative the particle emitter will have to be killed for it to stop
        public float minScaleGrow; //the min rate the particle will grow in scale at
        public float maxScaleGrow; //the max rate the particle will grow in scale at

        private float timer;
        private int numToSpawnThisSecond;
        private int numSpawnedThisSecond;
        private Random rand;

        public Vector3 emitterPos;

        public LinkedList<Particle> liveParticles { get; private set; }
        /*LinkedList<Particle> deadParticles;
        public int DeadParticleCount
        {
            get { return deadParticles.Count; }
            set { }
        }*/

        public Texture2D texture { get; private set; }

        private bool _onActor;
        private Actor actor;
        public Vector3 offsetFromActor; //only used as an offset when attached to an actor

        private Vector3 Normal;

        private ParticleQB pQB;

        public ParticleEmitter(ParticleQB pqb, Actor a, Vector3 pos, bool isOneShot, float life, int minRate, int maxRate, 
            float minLife, float maxLife, float minGrow, float maxGrow, Vector2 minDim, Vector2 maxDim, 
            Vector3 spawn, Vector3 defaultVel, Vector3 randVel, Texture2D t)
        {
            pQB = pqb;
            emit = true;
            MarkedDead = false;
            dead = false;
            lifeTime = life;
            oneShot = isOneShot;
            timer = 0.0f;
            minEmission = minRate;
            maxEmission = maxRate;
            minEnergy = minLife;
            maxEnergy = maxLife;
            minSize = minDim;
            maxSize = maxDim;
            minScaleGrow = minGrow;
            maxScaleGrow = maxGrow;
            spawnRange = spawn;
            localVelocity = defaultVel;
            rndVelocity = randVel;
            rand = new Random();
            texture = t;
            actor = a;
            _onActor = (actor != null);
            emitterPos = (actor==null) ? pos : actor.PhysicsObject.Position + pos;
            offsetFromActor = pos;

            liveParticles = new LinkedList<Particle>();

            numToSpawnThisSecond = rand.Next(minEmission, maxEmission + 1);
            numSpawnedThisSecond = 0;
        }

        public void ReInit(Actor a, Vector3 pos, bool isOneShot, float life, int minRate, int maxRate, 
            float minLife, float maxLife, float minGrow, float maxGrow, Vector2 minDim, Vector2 maxDim, 
            Vector3 spawn, Vector3 defaultVel, Vector3 randVel, Texture2D t)
        {
            emit = true;
            MarkedDead = false;
            dead = false;
            oneShot = isOneShot;
            lifeTime = life;
            timer = 0.0f;
            minEmission = minRate;
            maxEmission = maxRate;
            minEnergy = minLife;
            maxEnergy = maxLife;
            minSize = minDim;
            maxSize = maxDim;
            minScaleGrow = minGrow;
            maxScaleGrow = maxGrow;
            spawnRange = spawn;
            localVelocity = defaultVel;
            rndVelocity = randVel;
            texture = t;
            actor = a;
            _onActor = (actor != null);
            emitterPos = (actor == null) ? pos : actor.PhysicsObject.Position + pos;
            offsetFromActor = pos;

            numToSpawnThisSecond = rand.Next(minEmission, maxEmission + 1);
            numSpawnedThisSecond = 0;
        }

        public void Initialize()
        {

        }

        public void Update(float dt)
        {
            LinkedListNode<Particle> p;

            EmitUpdate(dt);

            if (CameraQB.WorldMatrix != null)
            {
                Normal = CameraQB.WorldMatrix.Translation - emitterPos;
                Normal.Normalize();
            }

            //to-do implement change to up vector (only if we have an overhead view)

            p = liveParticles.First;

            while (p != null && p.Value != null)
            {
                if (p.Value.dead)
                {
                    //store next temporarily so that we can proceed to it after p is removed
                    //(this is an issue because the add changes the p.next value
                    LinkedListNode<Particle> next = p.Next;
                    //remove
                    liveParticles.Remove(p);
                    pQB.availableParticles.AddLast(p);

                    p = next; //go to actual next
                }
                else
                {
                    p.Value.Update(dt, ref Normal);
                    p = p.Next;
                }
            }

            if (liveParticles.Count == 0 && MarkedDead) dead = true;
        }

        private void EmitUpdate(float dt)
        {
            if (emit)
            {
                LinkedListNode<Particle> p;
                if (_onActor)
                {
                    if (actor == null || actor.MarkedForDeath || actor.IsShutdown)
                    {
                        Emit = false;
                        MarkedDead = true;
                        return;
                    }
                    emitterPos = actor.PhysicsObject.Position + offsetFromActor;
                }


                int numToSpawnThisCycle;
                if (oneShot)
                {
                    numToSpawnThisCycle = rand.Next(minEmission, maxEmission + 1);
                    Emit = false;
                    MarkedDead = true;
                }
                else
                {
                    timer += dt;

                    if (lifeTime > 0)
                    {
                        lifeTime -= dt;
                        if (lifeTime <= 0)
                        {
                            Emit = false;
                            MarkedDead = true;
                        }
                    }

                    if (timer >= 1)
                    {
                        numToSpawnThisCycle = numToSpawnThisSecond - numSpawnedThisSecond;
                        timer = 0.0f;

                        numToSpawnThisSecond = rand.Next(minEmission, maxEmission + 1);
                        numSpawnedThisSecond = 0;
                    }
                    else
                    {
                        numToSpawnThisCycle = (int)(numToSpawnThisSecond * timer) - numSpawnedThisSecond;
                        numSpawnedThisSecond += numToSpawnThisCycle;
                    }
                }



                while (pQB.availableParticles.Count > 0 && numToSpawnThisCycle > 0)
                {
                    p = pQB.availableParticles.First;
                    pQB.availableParticles.RemoveFirst();
                    liveParticles.AddLast(p);

                    spawnParticle(p);
                    numToSpawnThisCycle--;
                } //we won't spawn any more particles if we don't have any room
            }
        }

        private void spawnParticle(Particle p)
        {
            //random size
            float randSize = (float)rand.NextDouble() * (maxSize.X - minSize.X) + minSize.X;
            p.quad.Width = randSize;

            randSize = (float)rand.NextDouble() * (maxSize.Y - minSize.Y) + minSize.Y;
            p.quad.Height = randSize;

            //random position
            Vector3 pos = emitterPos;

            pos.X += (spawnRange.X == 0) ? 0 : (float)rand.NextDouble() * (2 * spawnRange.X) - spawnRange.X;
            pos.Y += (spawnRange.Y == 0) ? 0 : (float)rand.NextDouble() * (2 * spawnRange.Y) - spawnRange.Y;
            pos.Z += (spawnRange.Z == 0) ? 0 : (float)rand.NextDouble() * (2 * spawnRange.Z) - spawnRange.Z;

            Vector3 vel = localVelocity;
            vel.X += (rndVelocity.X == 0) ? 0 : (float)rand.NextDouble() * (2 * rndVelocity.X) - rndVelocity.X;
            vel.Y += (rndVelocity.Y == 0) ? 0 : (float)rand.NextDouble() * (2 * rndVelocity.Y) - rndVelocity.Y;
            vel.Z += (rndVelocity.Z == 0) ? 0 : (float)rand.NextDouble() * (2 * rndVelocity.Z) - rndVelocity.Z;

            //spawn
            p.Spawn(pos, vel, (float)rand.NextDouble() * (maxEnergy - minEnergy) + minEnergy, 
                (float)rand.NextDouble() * (maxScaleGrow - minScaleGrow) + minScaleGrow);
        }

        private void spawnParticle(LinkedListNode<Particle> p)
        {
            spawnParticle(p.Value);
        }

        public void Draw()
        {

        }
    }
}