using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.MenuSystem {
    public class HUD : GameScreen {
        private Rectangle healthBackground;
        private Rectangle healthBoarder;
        private Rectangle healthBar;

        private int boarderSize;
        private int backgroundSize;

        private int x;
        private int y;
        private int height;
        private double width;

        public HUD (MenuSystem menuSystem) {
            this.MenuSystem = menuSystem;

            //set up constants
                this.boarderSize = 1;
                this.backgroundSize = 2;
                this.x = MenuSystem.Game.Window.ClientBounds.Left+10;
                this.y = MenuSystem.Game.Window.ClientBounds.Top+10;
                this.height = 5;
                this.width = 100;

            //Set up Boarder, Background and Health bar in relation to each other. 
                healthBoarder = new Rectangle(x, y, (int)width + (boarderSize + backgroundSize), height+(boarderSize + backgroundSize));
                healthBackground = new Rectangle(x + boarderSize, y + boarderSize, (int)width + boarderSize, height + boarderSize);
                healthBar = new Rectangle(x + boarderSize, y + boarderSize, (int)width, height);
        }

        public override void Draw (GameTime gameTime) {
            SpriteBatch spriteBatch = MenuSystem.SpriteBatch;
            spriteBatch.Begin();
            healthBar.Width = (int)CharacterControllerInput.Player.GetAgent<HealthAgent>().Health;
            spriteBatch.Draw(MenuSystem.BlankTexture, healthBoarder, Color.Black);
            spriteBatch.Draw(MenuSystem.BlankTexture, healthBackground, Color.Gray);
            spriteBatch.Draw(MenuSystem.BlankTexture, healthBar, Color.Red);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
