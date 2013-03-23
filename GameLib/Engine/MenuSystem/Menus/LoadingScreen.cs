using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.MenuSystem.Menus {    

    public class LoadingScreen : GameScreen {
        bool loadingIsSlow;
        bool otherScreensAreGone;

        GameScreen[] screensToLoad;

        Texture2D background;

        string levelName;

        /// <summary>
        /// menus
        /// </summary>
        private LoadingScreen (MenuSystem menuSystem, bool loadingIsSlow, string levelName) {
            this.loadingIsSlow = loadingIsSlow;
            this.levelName = levelName;
            this.background = Stage.Content.Load<Texture2D>("Menu/background");
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// menus
        /// </summary>
        private LoadingScreen (MenuSystem menuSystem, bool loadingIsSlow,
                              GameScreen[] screensToLoad) {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Loads Levels
        /// </summary>
        public static void Load (MenuSystem MenuSystem, bool loadingIsSlow, string levelName) {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in MenuSystem.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(MenuSystem,
                                                            loadingIsSlow,
                                                            levelName);
            MenuSystem.AddScreen(loadingScreen);
        }

        /// <summary>
        /// Loads Menus.
        /// </summary>
        public static void Load (MenuSystem MenuSystem, bool loadingIsSlow, params GameScreen[] screensToLoad) {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in MenuSystem.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(MenuSystem,
                                                            loadingIsSlow,
                                                            screensToLoad);

            MenuSystem.AddScreen(loadingScreen);
        }

        public override void Update (GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (otherScreensAreGone) {
                MenuSystem.RemoveScreen(this);

                Stage.LoadStage(levelName, false);
                Stage.GameRunning = true;
                MenuSystem.Game.ResetElapsedTime();
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw (GameTime gameTime) {

            if (((MenuState == MenuState.Active) &&
                (MenuSystem.GetScreens().Length <= 2))) {
                otherScreensAreGone = true;
            }
            if (loadingIsSlow) {
                SpriteBatch spriteBatch = MenuSystem.SpriteBatch;
                SpriteFont font = MenuSystem.Font;

                const string message = "Loading...";

                // Center the text in the viewport.
                Viewport viewport = MenuSystem.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                Color color = Color.White * TransitionAlpha;

                // Draw the text.
                spriteBatch.Begin();
                
                spriteBatch.Draw(background, new Rectangle(0, 0, viewport.Width, viewport.Height), new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
                spriteBatch.DrawString(font, message, textPosition, color);
                
                spriteBatch.End();
            }
        }
    }
}
