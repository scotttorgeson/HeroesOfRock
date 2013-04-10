using Microsoft.Xna.Framework;
using System;
using GameLib.Engine.MenuSystem.Menus.MenuComponents;
using Microsoft.Xna.Framework.Graphics;
using GameLib.Engine.MenuSystem.Screens;

namespace GameLib.Engine.MenuSystem.Menus {

    public class MainMenu : GameMenu {
        Texture2D selectBack;

        public MainMenu ()
            : base(" ") {
            //Load Game
            MenuGraphic logo = new MenuGraphic("MainMenu/mainMenuLogo", Vector2.Zero, 0.8f);
            MenuGraphic play = new MenuGraphic("MainMenu/play", "MainMenu/play_hover", Vector2.Zero, 0.8f);
            MenuGraphic options = new MenuGraphic("MainMenu/options", "MainMenu/options_hover", Vector2.Zero, 0.8f);
            //MenuGraphic extras = new MenuGraphic("MainMenu/play"));
            MenuGraphic credits = new MenuGraphic("MainMenu/credits", "MainMenu/credits_hover", Vector2.Zero, 0.8f);
            MenuGraphic exit = new MenuGraphic("MainMenu/exit", "MainMenu/exit_hover", Vector2.Zero, 0.8f);

            // Hook up menu event handlers.
            play.Selected += LevelSelectSelected;
            options.Selected += OptionsSelected;
            //extras.Selected += ExtrasSelected;
            credits.Selected += CredsSelected;
            exit.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(logo);
            MenuEntries.Add(play);
            MenuEntries.Add(options);
            //MenuEntries.Add(extras);
            MenuEntries.Add(credits);
            MenuEntries.Add(exit);

            selectBack = Stage.Content.Load<Texture2D>("UI/MainMenu/select_back");
            selectedEntry = 1;
        }


        void OptionsSelected (object sender, EventArgs e) {
            const string message = "Options!";
            OptionPopUp options = new OptionPopUp(message);
            MenuSystem.AddScreen(options);
        }

        void LevelSelectSelected (object sender, EventArgs e) {
            MenuSystem.AddScreen(new BackgroundScreen());
            MenuSystem.AddScreen(new LevelMenu());
        }

        void ExtrasSelected (object sender, EventArgs e) {
            MenuSystem.AddScreen(new ExtrasPopUp("Rock Goodies"));
        }

        void CredsSelected (object sender, EventArgs e) {
            MenuSystem.AddScreen(new ColoredBackgroundScreen(Color.Black));
            MenuSystem.AddScreen(new CreditsMenu());
        }

        /// All menu entries are lined up in a vertical list, centered on the menu.
        /// </summary>
        protected override void UpdateMenuEntryLocations () {

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuGraphic MenuGraphic = (MenuGraphic)MenuEntries[i];
                
                if(i > 0)
                    MenuGraphic.CanSelect = true;

                // each entry is to be centered horizontally
                position.X = Renderer.ScreenWidth / 6 - MenuGraphic.Dim.Width / 6;

                if (MenuState == MenuState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                MenuGraphic.Dim = new Rectangle((int)position.X, (int)position.Y, MenuGraphic.Dim.Width, MenuGraphic.Dim.Height);

                // move down for the next entry the size of this entry
                if (i == 0)
                    position.Y += MenuGraphic.Dim.Height;
                else
                    position.Y += MenuGraphic.Dim.Height / 2;
            }
        }

        public override void Draw (float dt) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();
            Rectangle rectangle = new Rectangle(0, 0, 0, 0);
            Texture2D mainImage = Stage.Content.Load<Texture2D>("UI/Menu/blank");
            
            //logo
            MenuEntries[0].Draw(this, false, dt);

            // Draw each menu entry in turn.
            for (int i = 1; i < MenuEntries.Count; i++) {
                MenuGraphic menuEntry = (MenuGraphic)MenuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, dt);
                if(isSelected)
                    mainImage = Stage.Content.Load<Texture2D>("UI/"+menuEntry.TextureName +"Main");
               
            }

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            //main menu
            int width = (int)(mainImage.Bounds.Width * 0.8);
            int height = (int)(mainImage.Bounds.Height * 0.8);
            Rectangle rec = Stage.renderer.GraphicsDevice.Viewport.Bounds;
            Rectangle mainImageRec = new Rectangle((rec.Center.X + 150) - width/2, rec.Center.Y - height / 2, width, height);

            width = (int)(selectBack.Width * 0.7f);
            height = (int)(selectBack.Height * 0.7f);
            Rectangle selectBackRec = new Rectangle(rec.Right - (int)(1.65*width), rec.Bottom-(int)(1.65*height), width, height);

            Stage.renderer.SpriteBatch.Draw(mainImage, mainImageRec, Color.White);        
            Stage.renderer.SpriteBatch.Draw(selectBack, selectBackRec, Color.White);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel () {
            //const string message = "Are you sure you want to quit the game? \nGreen = ok" +
            //                         "\nRed = cancel";

            //MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, true);

            //confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            //MenuSystem.AddScreen(confirmExitMessageBox);
            MenuSystem.AddScreen(new QuitConfirmPopUp(""));
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
