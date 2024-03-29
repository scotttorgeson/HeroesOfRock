﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.Particles
{
    public class Particle
    {
        float lifetime;
        public Quad quad;
        Vector3 velocity;
        float scalar;
        public bool dead { get; private set; }

        public Particle()
        {
            quad = new Quad(Vector3.Zero, Vector3.Backward, Vector3.Up, 1, 1);
        }

        public void Spawn(Vector3 pos, Vector3 vel, float life, float scale)
        {
            dead = false;
            lifetime = life;
            quad.Origin = pos;
            velocity = vel;
            scalar = scale;
            quad.CalcVertices();
        }

        public void Update(float dt, ref Vector3 normal)
        {
            lifetime -= dt;
            quad.Width += scalar * dt;
            quad.Height += scalar * dt;

            if (lifetime <= 0.0f) dead = true;
            quad.Origin += velocity * dt;
            quad.CalcVertices(ref normal);
        }
    }
}
