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
            this.background = Stage.Content.Load<Texture2D>("UI/Menu/background");
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
            MenuSystem.ExitAll();

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
            MenuSystem.ExitAll();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(MenuSystem,
                                                            loadingIsSlow,
                                                            screensToLoad);

            MenuSystem.AddScreen(loadingScreen);
        }

        public override void Update (float dt, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            base.Update(dt, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (otherScreensAreGone) {
                this.MarkedForRemove = true;

                Stage.LoadStage(levelName, true);
                Stage.ActiveStage.ResumeGame();
                //Stage.Game.ResetElapsedTime();
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw (float dt) {

            if (((MenuState == MenuState.Active) &&
                (MenuSystem.MenuCount <= 2))) {
                otherScreensAreGone = true;
            }
            if (loadingIsSlow) {
                SpriteFont font = MenuSystem.Font;

                const string message = "Loading...";

                // Center the text in the viewport.
                Vector2 viewportSize = new Vector2(Renderer.ScreenWidth, Renderer.ScreenHeight);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                Color color = Color.White * TransitionAlpha;

                // Draw the text.

                Stage.renderer.SpriteBatch.Draw(background, new Rectangle(0, 0, Renderer.ScreenWidth, Renderer.ScreenHeight), new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
                Stage.renderer.SpriteBatch.DrawString(font, message, textPosition, color);
            }
        }
    }
}
