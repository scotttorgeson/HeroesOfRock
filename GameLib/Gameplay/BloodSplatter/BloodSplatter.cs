using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Gameplay
{
    class BloodSplatter
    {
        public Texture2D splatterTexture;
        public float lifetime = -1.0f;
        public Rectangle rect;
        private float alpha = 1.0f;

        public void Update(float dt)
        {
            lifetime -= dt;
            alpha = Math.Min(lifetime+0.5f, 1.0f);
        }

        public void Draw()
        {
            if (lifetime < 0.0f)
                return;

            Color color = new Color(1.0f, 1.0f, 1.0f, alpha);
            Renderer.Instance.SpriteBatch.Draw(splatterTexture, rect, color);
        }
    }
}
