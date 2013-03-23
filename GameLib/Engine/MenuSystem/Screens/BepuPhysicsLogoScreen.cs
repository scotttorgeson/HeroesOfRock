using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLib.Engine.MenuSystem.Menus;

namespace GameLib.Engine.MenuSystem.Screens {
    class BepuPhysicsLogoScreen : GameMenu {
        ContentManager content;
        Texture2D logoTexture;
        float duration;

        public BepuPhysicsLogoScreen (float seconds)
            : base("") {
            duration = seconds;
        }


        /* OVERRIDES */ 
        public override void LoadContent () {
            if (content == null)
                content = Stage.Content;

            logoTexture = content.Load<Texture2D>("UI/bepu_physics_logo_big");
        }

        public override void Update (float dt, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            duration -= dt;
            if (duration < 0.0f)
                LogoScreenOut();
            base.Update(dt, otherScreenHasFocus, false);
        }

        public override void HandleInput (MenuInput input) {}

        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw (float dt) {
            float scale = 1.0f;
            int w = (int)(logoTexture.Width * scale);
            int h = (int)(logoTexture.Height * scale);

            Rectangle background = new Rectangle(0, 0, Renderer.ScreenWidth, Renderer.ScreenHeight);
            Rectangle logoSize = new Rectangle(background.Center.X-(w/2), 
                background.Center.Y-(h/2), w,h);

            Color color = new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha);

            Stage.renderer.SpriteBatch.Draw(MenuSystem.BlankTexture, background, color);
            Stage.renderer.SpriteBatch.Draw(logoTexture, logoSize, color);
        }
        
        protected override void OnCancel () { }

        /* END OVERRIDES */

        private void LogoScreenOut () {
            this.MarkedForRemove = true;
            MenuSystem.AddScreen(new BackgroundScreen());
            MenuSystem.AddScreen(new SplashMenu());
        }
    }
}
