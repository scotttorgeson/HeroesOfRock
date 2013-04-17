using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.MenuSystem.Menus {
    public class CreditsMenu : GameMenu {
        Texture2D credits, teamLogo, hor, back;
        Rectangle rec;
        float alpha;
        float scroll;
        Boolean endCredits, logoShowing;
        bool mainMenuOnExit;

        public CreditsMenu (bool addMainMenuOnExit)
            : base("") {
            alpha = 0f;
            credits = Stage.Content.Load<Texture2D>("UI/Menu/creds");
            teamLogo = Stage.Content.Load<Texture2D>("UI/Menu/11outOf10_Logo_White");
            hor = Stage.Content.Load<Texture2D>("UI/Menu/logo");
            back = Stage.Content.Load<Texture2D>("UI/Menu/back");
            rec = new Rectangle(Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.X - credits.Width*3/4, Stage.renderer.GraphicsDevice.Viewport.Bounds.Bottom - credits.Height/8, credits.Bounds.Width*3/2, credits.Bounds.Height*3/2);
            scroll = rec.Y;
            endCredits = logoShowing = false;
            mainMenuOnExit = addMainMenuOnExit;
            
        }
        

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel () {
            this.MarkedForRemove = true;
            if (mainMenuOnExit)
                MenuSystem.AddScreen(new BackgroundScreen());
            MenuSystem.AddScreen(new MainMenu());
        }

        public override void Update (float dt, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(dt, otherScreenHasFocus, coveredByOtherScreen);

            if (TransitionAlpha >= 1 && alpha <= 1f)
                alpha += dt/2;
            else if (scroll >= (float)(-1 * rec.Height))
                scroll -= (50 * dt);
            else if(!logoShowing){
                endCredits = true;
                alpha = 0;
                logoShowing = true;
            }
               
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw (float dt) {
            Rectangle fullscreen = new Rectangle(0, 0, Renderer.ScreenWidth, Renderer.ScreenHeight);
            int width = (int)(back.Width * 0.8f);
            int height = (int)(back.Height * 0.8f);
            Rectangle backRec = new Rectangle(fullscreen.Right - (2 * width), fullscreen.Bottom - (2 * height), width, height);
            Stage.renderer.SpriteBatch.Draw(MenuSystem.BlankTexture, fullscreen, Color.Black * TransitionAlpha);

            if (!endCredits)
                Stage.renderer.SpriteBatch.Draw(credits, new Rectangle(rec.X, (int)scroll, rec.Width, rec.Height), Color.White * alpha);
            else {
                width = (int)(teamLogo.Width * 0.85);
                height = (int)(teamLogo.Height * 0.85);
                Rectangle trec = new Rectangle(fullscreen.Center.X - width*3/4, fullscreen.Center.Y - height*3/4, width*3/2, height*3/2);
                Stage.renderer.SpriteBatch.Draw(teamLogo, trec, Color.White);
            }
            Stage.renderer.SpriteBatch.Draw(back, backRec, Color.White * TransitionAlpha);
        }

        public override void HandleInput (MenuInput input) {
            if (input.IsMenuCancel()) {
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("Uppercut");
                OnCancel();
            }
        }
    }
}
