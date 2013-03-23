using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.MenuSystem.Menus {
    public class PauseScreen : MenuScreen {
        Texture2D smoked;
        Rectangle window;

        public PauseScreen (string title)
            : base(title) {
            //Stop Game
            Stage.GameRunning = false;

            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");

            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += ResumeGame;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }


        public override void LoadContent () {
            smoked = MenuSystem.Game.Content.Load<Texture2D>("Menu/smoked");

            window = MenuSystem.Game.Window.ClientBounds;
            window.X = 0;
            window.Y = 0;

            base.LoadContent();
        }

        void ResumeGame (object sender, EventArgs e) {
            Stage.GameRunning = true;
            ExitScreen();
        }

        void QuitGameMenuEntrySelected (object sender, EventArgs e) {
            const string message = "Are you sure you want to quit this game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            MenuSystem.AddScreen(confirmQuitMessageBox);
        }

        void ConfirmQuitMessageBoxAccepted (object sender, EventArgs e) {
            Stage.LoadStage("MainMenu", true);
            MenuSystem.AddScreen(new BackgroundScreen());
            MenuSystem.AddScreen(new MainMenu());
        }

        public override void Draw (GameTime gameTime) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = MenuSystem.GraphicsDevice;
            SpriteBatch spriteBatch = MenuSystem.SpriteBatch;
            SpriteFont font = MenuSystem.Font;

            spriteBatch.Begin();

            spriteBatch.Draw(smoked, window, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuEntry menuEntry = MenuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the menu
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.End();
            
        }

    }
}
