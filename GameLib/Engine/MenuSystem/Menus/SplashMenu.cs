using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLib.Engine.MenuSystem.Menus.MenuComponents;
using Microsoft.Xna.Framework.Input;

namespace GameLib.Engine.MenuSystem.Menus {
    class SplashMenu : GameMenu {
        ControlsQB controlsQB = Stage.ActiveStage.GetQB<ControlsQB>();

        public SplashMenu (bool playThemeSong)
            : base("") {

            SpriteFont font = Stage.Content.Load<SpriteFont>("DefaultFont");
            MenuGraphic logo = new MenuGraphic("Menu/logo", new Vector2(Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.X, Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.Y), .75f);
            MenuGraphic pressA = new MenuGraphic("Menu/pressA", new Vector2(logo.Dim.Center.X, logo.Dim.Bottom - logo.Dim.Height / 2 + Stage.Content.Load<Texture2D>("UI/Menu/pressA").Height), .75f);
            MenuGraphic bestPlayed = new MenuGraphic("Menu/bestPlayed", new Vector2(pressA.Dim.Center.X, Stage.renderer.GraphicsDevice.Viewport.Bounds.Bottom - Stage.Content.Load<Texture2D>("UI/Menu/bestPlayed").Height), .6f);
            pressA.Flashing = true;

            // Hook up menu event handlers.
            this.GreenButtonPressed += GoToMainMenu;

            // Add entries to the menu.
            MenuEntries.Add(logo);            
            MenuEntries.Add(pressA);
            MenuEntries.Add(bestPlayed);

            //hack to makes sure press a is selected, fix.
            selectedEntry = 1;

            Stage.SaveGame.LoadGameData();

            if ( playThemeSong )
                Stage.ActiveStage.GetQB<AudioQB>().ChangeTheme("HORcityday");
        }


        void GoToMainMenu (object sender, EventArgs e) {
            Stage.ActiveStage.GetQB<AudioQB>().PlaySound("Arail-attack2");
            this.MarkedForRemove = true;
            //MenuSystem.AddScreen(new BackgroundScreen("MainMenu/background", 0.8f));
            MenuSystem.AddScreen(new MainMenu());
        }

        private void FindActivePlayer () {
#if WINDOWS

            if (Keyboard.GetState().IsKeyDown(Keys.Enter)) {
                SetPlayerIndexAndControllerType(PlayerIndex.One);
                GreenButtonPressed(this, new EventArgs());
            }
#endif
            for (PlayerIndex i = PlayerIndex.One; i <= PlayerIndex.Four; i++) {
                GamePadState state = GamePad.GetState(i);
                if (state.IsConnected && state.IsButtonDown(Buttons.A)) {
                    SetPlayerIndexAndControllerType(i);
                    GreenButtonPressed(this, new EventArgs());
                }
            }
        }

        private void SetPlayerIndexAndControllerType (PlayerIndex i) {
            Stage.PlayerIndex = i;
            /*Stage.GamePadType = GamePad.GetCapabilities(i).GamePadType;
            switch (Stage.GamePadType) {
                case GamePadType.AlternateGuitar:
                    Stage.ControlScheme = GameLib.ControlsQB.controlSchemEnum.Guitar_buttonStrumAll;
                    break;
                case GamePadType.Guitar:
                    Stage.ControlScheme = GameLib.ControlsQB.controlSchemEnum.Guitar_buttonStrumAll;
                    break;
                case GamePadType.GamePad:
                    Stage.ControlScheme = GameLib.ControlsQB.controlSchemEnum.GamePad;
                    break;
                default:
                    //error?
                    break;
            }*/
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update (float dt, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            base.Update(dt, otherScreenHasFocus, coveredByOtherScreen);
            FindActivePlayer();
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw (float dt) {
            // make sure our entries are in the right place before we draw them
            GraphicsDevice graphics = Renderer.Instance.GraphicsDevice;
            SpriteFont font = MenuSystem.Font;

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuEntry menuEntry = MenuEntries[i];
                if (i == 1)
                    menuEntry.Flashing = true;
                bool isSelected = IsActive && (i == selectedEntry);

              //  if(i != 1)
                    menuEntry.Draw(this, isSelected, dt);
            }
        }

        public override void HandleInput (MenuInput input) {
            if (input.IsMenuSelect()) {
                //Stage.ActiveStage.GetQB<AudioQB>().PlaySound("Arail-attack2");
            } 
        }

        ///EVENTS///
        public event EventHandler<EventArgs> GreenButtonPressed;
        protected override void OnCancel () { }
    }
}
