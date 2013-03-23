using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.Particles
{
    public class ParticleQB : Quarterback
    {
        public LinkedList<ParticleEmitter> emitters;

        private LinkedList<ParticleEmitter> availableEmitters;

        public LinkedList<FloatingText> floatingTexts;

        public LinkedList<Particle> availableParticles;

        private SpriteFont floatingTextFont;

        public override string Name()
        {
            return "ParticleQB";
        }

        public ParticleQB()
        {
            
        }

        public override void PreLoadInit(ParameterSet Parm)
        {
            emitters = new LinkedList<ParticleEmitter>();
            floatingTexts = new LinkedList<FloatingText>();

            availableParticles = new LinkedList<Particle>();
            for (int i = 0; i < 500; i++)
                availableParticles.AddLast(new Particle());

            availableEmitters = new LinkedList<ParticleEmitter>();
            for (int i = 0; i < 10; i++)
                availableEmitters.AddLast(new ParticleEmitter(this, null, Vector3.Zero, false, 0, 0, 0, 
                    0, 0, Vector2.Zero, Vector2.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, null));
        }

        public override void LoadContent()
        {
            floatingTextFont = Stage.Content.Load<SpriteFont>("DefaultFont");
        }

        public override void Update(float dt)
        {
            if (IsPaused) return;

            LinkedListNode<ParticleEmitter> curr;

            curr = emitters.First;
            while (curr != null)
            {
                if (curr.Value.dead)
                {
                    LinkedListNode<ParticleEmitter> tempNode = curr;
                    curr = curr.Previous;
                    emitters.Remove(tempNode);
                    availableEmitters.AddLast(tempNode);
                }
                else
                    curr.Value.Update(dt);
                if(curr != null)
                    curr = curr.Next;
            }

            LinkedListNode<FloatingText> currText;
            FloatingText floatText;

            currText = floatingTexts.First;

            while(currText != null)
            {
                floatText = currText.Value;
                floatText.Update(dt);
                currText.Value = floatText;

                if (currText.Value.lifeTime <= 0)
                {
                    LinkedListNode<FloatingText> tempNode = currText;
                    currText = currText.Previous;
                    floatingTexts.Remove(tempNode);
                }
                
                if(currText != null)
                    currText = currText.Next;
            }

        }

        public void AddParticleEmitter(ParticleEmitter p)
        {
            emitters.AddLast(p);
        }

        /// <summary>
        /// Add a particle emitter to the game
        /// </summary>
        /// <param name="a">actor to attach to</param>
        /// <param name="pos">if actor is null this is a world pos, else it is an offset from the actor</param>
        /// <param name="isOneShot">whether one seconds worth of particles should be spawned at once and the emitter deleted</param>
        /// <param name="lifeSpan">the life span of the particle emitter, a negative value means it will emit indefinitely</param>
        /// <param name="minRate">the minimum number of particles spawned each second</param>
        /// <param name="maxRate">the maximum number of particles spawned each second</param>
        /// <param name="minLife">the minimum lifespan of each particle</param>
        /// <param name="maxLife">the maximum lifespan of each particle</param>
        /// <param name="minFade">the minimum amount of time used to fade out the particle</param>
        /// <param name="maxFade">the maximum amount of time used to fade out the particle</param>
        /// <param name="minDim">the minimum Width/Height of each particle</param>
        /// <param name="maxDim">the maximum Width/Height of each particle</param>
        /// <param name="spawn">the +/- range range (circle) for the particles to spawn in</param>
        /// <param name="defaultVel">the velocity that all particles will have</param>
        /// <param name="randVel">the +/- random velocity that will be added to the defaultVel to get a particles velocity</param>
        /// <param name="texture">the name of the texture in the ParticleFX folder, if instantiating lots of particles with this
        /// texture it would be better to keep a local copy of the texture and call the version that has a texture2d parameter</param>
        public void AddParticleEmitter(Actor a, Vector3 pos, bool isOneShot, float lifeSpan, int minRate, int maxRate, 
                                        float minLife, float maxLife, Vector2 minDim, Vector2 maxDim, 
                                        Vector3 spawn, Vector3 defaultVel, Vector3 randVel, string texture)
        {
            Texture2D t = Stage.Content.Load<Texture2D>("ParticleFX/"+texture);
            if(availableEmitters.Count > 0)
            {
                ParticleEmitter emitter = availableEmitters.First.Value;
                emitter.ReInit(a, pos, isOneShot, lifeSpan, minRate, maxRate, minLife, maxLife,
                    minDim, maxDim, spawn, defaultVel, randVel, t);
                availableEmitters.RemoveFirst();
                emitters.AddLast(emitter);
            }
            else
                emitters.AddLast(new ParticleEmitter(this, a, pos, isOneShot, lifeSpan, minRate, maxRate, minLife, maxLife, 
                    minDim, maxDim, spawn, defaultVel, randVel, t));
        }

        /// <summary>
        /// Add a particle emitter to the game
        /// </summary>
        /// <param name="a">actor to attach to</param>
        /// <param name="pos">if actor is null this is a world pos, else it is an offset from the actor</param>
        /// <param name="isOneShot">whether one seconds worth of particles should be spawned at once and the emitter deleted</param>
        /// <param name="lifeSpan">the life span of the particle emitter, a negative value means it will emit indefinitely</param>
        /// <param name="minRate">the minimum number of particles spawned each second</param>
        /// <param name="maxRate">the maximum number of particles spawned each second</param>
        /// <param name="minLife">the minimum lifespan of each particle</param>
        /// <param name="maxLife">the maximum lifespan of each particle</param>
        /// <param name="minFade">the minimum amount of time used to fade out the particle</param>
        /// <param name="maxFade">the maximum amount of time used to fade out the particle</param>
        /// <param name="minDim">the minimum Width/Height of each particle</param>
        /// <param name="maxDim">the maximum Width/Height of each particle</param>
        /// <param name="spawn">the +/- range range (circle) for the particles to spawn in</param>
        /// <param name="defaultVel">the velocity that all particles will have</param>
        /// <param name="randVel">the +/- random velocity that will be added to the defaultVel to get a particles velocity</param>
        /// <param name="t">the texture the particles should have</param>
        public void AddParticleEmitter(Actor a, Vector3 pos, bool isOneShot, float lifeSpan, int minRate, int maxRate, 
                                        float minLife, float maxLife, Vector2 minDim, Vector2 maxDim, 
                                        Vector3 spawn, Vector3 defaultVel, Vector3 randVel, Texture2D t)
        {
            if (availableEmitters.Count > 0)
            {
                ParticleEmitter emitter = availableEmitters.First.Value;
                emitter.ReInit(a, pos, isOneShot, lifeSpan, minRate, maxRate, minLife, maxLife, 
                    minDim, maxDim, spawn, defaultVel, randVel, t);
                availableEmitters.RemoveFirst();
                emitters.AddLast(emitter);
            }
            else
                emitters.AddLast(new ParticleEmitter(this, a, pos, isOneShot, lifeSpan, minRate, maxRate, minLife, maxLife,
                    minDim, maxDim, spawn, defaultVel, randVel, t));
        }

        public void AddFloatingText(Vector2 pos, Vector2 vel, float lifeTime, string text)
        {
            floatingTexts.AddLast(new FloatingText(pos, vel, lifeTime, text));
        }

        public override void  DrawUI(float dt)
        {
            foreach (FloatingText f in floatingTexts)
            {
                Stage.renderer.SpriteBatch.DrawString(floatingTextFont, f.text, f.position, Color.White);
            }

 	        base.DrawUI(dt);
        }

        public override void Serialize(ParameterSet parm)
        {

        }

    }

    public struct FloatingText
    {
        public Vector2 position;
        public Vector2 velocity;
        public float lifeTime;
        public string text;

        public FloatingText(Vector2 pos, Vector2 vel, float life, string t)
        {
            position = pos;
            velocity = vel;
            lifeTime = life;
            text = t;
        }

        public void Update(float dt)
        {
            lifeTime -= dt;
            position += velocity * dt;
        }
    }
}
