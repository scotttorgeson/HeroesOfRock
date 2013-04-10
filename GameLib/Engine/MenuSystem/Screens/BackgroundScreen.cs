using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace GameLib.Engine.MenuSystem.Menus {
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// </summary>
    public class BackgroundScreen : GameScreen {

        private ContentManager content;
        private Texture2D backgroundTexture;
        private String backgroundPath;
        private float scale;

        public BackgroundScreen () {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            backgroundPath = "MainMenu/background";
            scale = 0.8f;
        }

        public BackgroundScreen (String backgroundPath, float scale) {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.backgroundPath = backgroundPath;
            this.scale = scale;
        }

        public override void LoadContent () {
            if (content == null)
                content = Stage.Content;
            Stage.renderer.GraphicsDevice.Clear(Color.Black);
            backgroundTexture = content.Load<Texture2D>("UI/"+backgroundPath);
        }

        public override void Update (float dt, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            base.Update(dt, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw (float dt) {
            scale = 0.68f;
            int width = (int)(backgroundTexture.Width * scale);
            int height = (int)(backgroundTexture.Height * scale);
            int centerX = (int)(Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.X - width / 2);
            int centerY = (int)(Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.Y- height/ 2);

            Rectangle fullscreen = new Rectangle(centerX, centerY, width, height);
            Stage.renderer.GraphicsDevice.Clear(new Color(3,3,3));
            Stage.renderer.SpriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
        }
    }
}
