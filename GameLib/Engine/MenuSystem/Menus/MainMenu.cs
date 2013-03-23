using Microsoft.Xna.Framework;
using System;

namespace GameLib.Engine.MenuSystem.Menus {

    public class MainMenu : GameMenu {

        public MainMenu ()
            : base(" ") {
            //Load Game
            


            //if there was a previously saved game, go to the level they were on
            MenuEntry continueMenuEntry = new MenuEntry("Continue");
            MenuEntry levelSelect = new MenuEntry("Level Select");
            MenuEntry options = new MenuEntry("Options");
            MenuEntry extras = new MenuEntry("Extras");
            MenuEntry creds = new MenuEntry("Credits");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            //continueMenuEntry.Selected += ContinueSelected;
            levelSelect.Selected += LevelSelectSelected;
            options.Selected += OptionsSelected;
            extras.Selected += ExtrasSelected;
            creds.Selected += CredsSelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            //MenuEntries.Add(continueMenuEntry);
            MenuEntries.Add(levelSelect);
           // MenuEntries.Add(options);
           // MenuEntries.Add(extras);
            MenuEntries.Add(creds);
            MenuEntries.Add(exitMenuEntry);
        }


        void OptionsSelected (object sender, EventArgs e) {
            const string message = "Options!";
            OptionPopUp options = new OptionPopUp(message);
            MenuSystem.AddScreen(options);
        }

        void ContinueSelected (object sender, EventArgs e) {
            //Stage.LoadStage(USER NEXT STAGE)
        }

        void LevelSelectSelected (object sender, EventArgs e) {
            MenuSystem.AddScreen(new BackgroundScreen());
            MenuSystem.AddScreen(new LevelMenu());
        }

        void ExtrasSelected (object sender, EventArgs e) {
            MenuSystem.AddScreen(new ExtrasPopUp("Rock Goodies"));
        }

        void CredsSelected (object sender, EventArgs e) {
            MenuSystem.AddScreen(new BackgroundScreen());
            MenuSystem.AddScreen(new CreditsMenu());
        }

        /// All menu entries are lined up in a vertical list, centered on the menu.
        /// </summary>
        protected override void UpdateMenuEntryLocations () {

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuEntry menuEntry = MenuEntries[i];

                // each entry is to be centered horizontally
                position.X = Renderer.ScreenWidth / 6 - menuEntry.GetWidth(this) / 6;

                if (MenuState == MenuState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += menuEntry.GetHeight(this);
            }
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel ()
        {
            const string message = "Are you sure you want to quit the game? \nGreen = ok" +
                                     "\nRed = cancel";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, true);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            MenuSystem.AddScreen(confirmExitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted (object sender, EventArgs e) {
            Stage.QuitGame = true;
        }
    }
}
