using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Gameplay
{
    public class BloodSplatterQB : Quarterback
    {
        const int MAX_BLOODSPLATTERS = 3;
        const int NUM_TEXTURES = 2;
        BloodSplatter[] bloodSplatter = new BloodSplatter[MAX_BLOODSPLATTERS];
        Texture2D[] splatterTextures = new Texture2D[NUM_TEXTURES];

        const int min_size = (int)(0.55f * (float)Renderer.ScreenHeight);
        const int max_size = (int)(0.75f * (float)Renderer.ScreenHeight);
        const float min_distance = (float)Renderer.ScreenHeight * 0.3f;

        const float lifetime_min = 2.0f;
        const float lifetime_max = 3.5f;

        const float splatter_chance = 0.1f;

        Random random = new Random();

        public override void LoadContent()
        {
            splatterTextures[0] = Renderer.Instance.LookupTexture("UI/BloodSplatter/BloodSplatter01");
            splatterTextures[1] = Renderer.Instance.LookupTexture("UI/BloodSplatter/BloodSplatter02");
            for (int i = 0; i < bloodSplatter.Length; i++)
            {
                bloodSplatter[i] = new BloodSplatter();
            }
        }

        public override string Name()
        {
            return "BloodSplatterQB";
        }

        public override void Update(float dt)
        {
            for (int i = 0; i < bloodSplatter.Length; i++)
                bloodSplatter[i].Update(dt);
        }

        public override void DrawUI(float dt)
        {
            for (int i = 0; i < bloodSplatter.Length; i++)
                bloodSplatter[i].Draw();
        }

        /// <summary>
        /// Tries to splat blood on screen.
        /// </summary>
        /// <returns>True if successful, false if not. (Too much blood onscreen already, or random chance failed)</returns>
        public bool SplatBlood()
        {
            if (random.NextDouble() > splatter_chance)
                return false;

            for (int i = 0; i < bloodSplatter.Length; i++)
            {
                if (bloodSplatter[i].lifetime < 0.0f)
                {
                    bloodSplatter[i].lifetime = lifetime_min + ((float)random.NextDouble() * (lifetime_max - lifetime_min));
                    bloodSplatter[i].splatterTexture = splatterTextures[random.Next(NUM_TEXTURES)];

                    do
                    {
                        int size = random.Next(min_size, max_size);
                        int x = random.Next(0, Renderer.ScreenWidth - size);
                        int y = random.Next(0, Renderer.ScreenHeight - size);
                        bloodSplatter[i].rect = new Rectangle(x, y, size, size);
                    } while (CheckOverlaps());
                    
                    return true;
                }
            }

            return false;
        }

        // true means there is overlap, false means there isn't
        private bool CheckOverlaps()
        {
            for (int i = 0; i < bloodSplatter.Length; i++)
            {
                for (int j = 0; j < bloodSplatter.Length; j++)
                {
                    if (i != j && bloodSplatter[i].lifetime > 0.0f && bloodSplatter[j].lifetime > 0.0f)
                    {
                        Vector2 left = new Vector2(bloodSplatter[i].rect.X, bloodSplatter[i].rect.Y);
                        Vector2 right = new Vector2(bloodSplatter[j].rect.X, bloodSplatter[j].rect.Y);

                        if (Vector2.Distance(left, right) < min_distance)
                            return true;
                    }
                }
            }
                    
            return false;
        }
    }
}
