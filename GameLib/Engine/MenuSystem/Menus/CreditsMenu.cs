using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.MenuSystem.Menus {
    public class CreditsMenu : GameMenu {
        Texture2D credits;
        Rectangle rec;
        float alpha;

        public CreditsMenu ()
            : base("") {
            alpha = 0f;
            credits = Stage.Content.Load<Texture2D>("UI/Menu/creds");
            rec = new Rectangle(Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.X - credits.Width/2, Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.Y-credits.Height, credits.Bounds.Width, credits.Bounds.Height);
        }
        

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel () {
            this.MarkedForRemove = true;
            MenuSystem.AddScreen(new MainMenu());
        }
        public override void Update (float dt, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(dt, otherScreenHasFocus, coveredByOtherScreen);
            alpha += dt;
        }

        public override void HandleInput (MenuInput input) {
            base.HandleInput(input);

           if (input.IsMenuCancel()) {
                OnCancel();
            }
                
        }
                /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw (float dt) {
            Stage.renderer.SpriteBatch.Draw(credits, rec, Color.White * alpha);
        }

    }
}
