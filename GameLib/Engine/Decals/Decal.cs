using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.Decals
{
    // draws from the top of the list down, so those at the bottom get drawn last which means they'll be on top when draw in game
    public enum DecalLayers
    {
        CracksLayer,
        BloodLayer,
    }

    // todo: support actor death or removal from world
    // actually draw the decal
    public class Decal : IComparable<Decal>
    {
        public Vector3 Position;
        public Texture2D Texture;
        public float Lifetime; // updated by qb
        public float Size;
        public List<Actor> Actors = new List<Actor>();
        public List<Matrix> ViewProjections = new List<Matrix>();
        public DecalLayers Layer;
        private static Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
        private static int MAX_ACTORS = 24;

        public Decal(ref Vector3 position, Texture2D texture, float lifetime, float size, DecalLayers layer)
        {
            size *= 2.0f;
            Position = position;
            Size = size;
            Texture = texture;
            Lifetime = lifetime;
            Layer = layer;
        }

        public bool ApplyToActor(Actor actor, Vector3 hitLocation, Vector3 up)
        {
            if (Actors.Count >= MAX_ACTORS)
                return false;

            Actors.Add(actor);

            // compute view-projection for that actor, add to view-projections list
            Matrix view;
            Matrix projection;
            Matrix.CreateLookAt(ref Position, ref hitLocation, ref up, out view);
            Matrix.CreateOrthographic(Size, Size, 0.01f, 1000.0f, out projection);
            Matrix viewProj;
            Matrix.Multiply(ref view, ref projection, out viewProj);
            ViewProjections.Add(viewProj);
            return true;
        }

        public void RemoveFromActor(int index) // not sure why we would do this
        {
            Actors.RemoveAt(index);
            ViewProjections.RemoveAt(index);
        }

        public int CompareTo(Decal other)
        {
            if (other == null) return 1;
            return Layer.CompareTo(other.Layer);
        }
    }
}

