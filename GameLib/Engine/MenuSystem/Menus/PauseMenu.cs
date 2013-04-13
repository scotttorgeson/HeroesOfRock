using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameLib.Engine.MenuSystem.Menus.MenuComponents;
namespace GameLib.Engine.MenuSystem.Menus {

    public class PauseMenu : GameMenu {
        Texture2D background, select, back;

        public PauseMenu  (string message)
            : this(message, true) { }

        public PauseMenu (string message, bool includeUsageText)
            : base(message) {
            Vector2 position = new Vector2(Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.X, Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.Y);
            
            MenuGraphic continueGame = new MenuGraphic("Pause/continue", "Pause/continue_hover", position, 0.65f);
            MenuGraphic controls = new MenuGraphic("MainMenu/controls", "MainMenu/controls_hover", position, 0.65f);
            MenuGraphic tune = new MenuGraphic("Pause/tuneMyGear", "Pause/tuneMyGear_hover", position, 0.65f);
            MenuGraphic giveUp = new MenuGraphic("Pause/giveUp", "Pause/giveUp_hover", position, 0.65f);

            continueGame.Selected += ResumeGame;
            controls.Selected += ControlsSelected;
            tune.Selected += OptionsSelected;
            giveUp.Selected += QuitGame;

            background = Stage.Content.Load<Texture2D>("UI/Pause/background");
            back = Stage.Content.Load<Texture2D>("UI/Pause/back");
            select = Stage.Content.Load<Texture2D>("UI/Pause/select");

            MenuEntries.Add(continueGame);
            MenuEntries.Add(controls);
            MenuEntries.Add(tune);
            MenuEntries.Add(giveUp);
        }

        void ResumeGame (object sender, EventArgs e) {
            Stage.ActiveStage.ResumeGame();
            ExitScreen();
        }

        void ControlsSelected (object sender, EventArgs e) {
            MenuSystem.AddScreen(new ControlsPopUp(""));
        }

        void OptionsSelected (object sender, EventArgs e) {
            MenuSystem.AddScreen(new OptionPopUp(""));
        }

        void QuitGame (object sender, EventArgs e) {
            MenuSystem.AddScreen(new QuitConfirmPopUp("", false));
        }

        public override void HandleInput (MenuInput input) {
            base.HandleInput(input);
        }

        /// All menu entries are lined up in a vertical list, centered on the menu.
        /// </summary>
        protected override void UpdateMenuEntryLocations () {

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuGraphic MenuGraphic = (MenuGraphic)MenuEntries[i];
                MenuGraphic.CanSelect = true;
                // each entry is to be centered horizontally
                int offSet = 0;
                if (i == 0)
                    offSet = 100;

                position.X = (Renderer.ScreenWidth / 2 - MenuGraphic.Dim.Width / 2) ;

                if (MenuState == MenuState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                MenuGraphic.Dim = new Rectangle((int)position.X, (int)position.Y+ offSet, MenuGraphic.Dim.Width, MenuGraphic.Dim.Height);

                // move down for the next entry the size of this entry
                position.Y += MenuGraphic.Dim.Height / 2 + offSet;
            }
        }

        public override void Draw (float dt) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();
            
            Rectangle fullscreen = Stage.renderer.GraphicsDevice.Viewport.Bounds;

            float scale = 0.7f;
            int w = (int)(background.Width * scale);
            int h = (int)(background.Width * scale);

            Rectangle backgroundRec = new Rectangle(fullscreen.Center.X - w / 2, fullscreen.Center.Y - h / 2, w, h);

            w = (int)(back.Width * scale);
            h = (int)(back.Height * scale);
            Rectangle backRec = new Rectangle(backgroundRec.Right - (int)(2.2* w), backgroundRec.Bottom - (5*h), w, h);

            w = (int)(select.Width * scale);
            h = (int)(select.Height * scale);
            Rectangle selectRec = new Rectangle(backgroundRec.Left + (int)(1.55 * w), backgroundRec.Bottom - (5 *h), w, h);

            Stage.renderer.SpriteBatch.Draw(MenuSystem.BlankTexture, fullscreen, (Color.Black * 0.95f)*TransitionAlpha);
            Stage.renderer.SpriteBatch.Draw(background, backgroundRec, Color.White * TransitionAlpha);
            Stage.renderer.SpriteBatch.Draw(back, backRec, Color.White * TransitionAlpha);
            Stage.renderer.SpriteBatch.Draw(select, selectRec, Color.White * TransitionAlpha);

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuEntry menuEntry = MenuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, dt);
            }
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected override void OnCancel () {
            Stage.ActiveStage.ResumeGame();
            ExitScreen();
        }
    }
}
