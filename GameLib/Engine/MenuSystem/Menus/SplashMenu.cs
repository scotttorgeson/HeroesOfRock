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

        public SplashMenu ()
            : base("") {

            SpriteFont font = Stage.Content.Load<SpriteFont>("DefaultFont");
            MenuEntry pressA = new MenuEntry("Press A to Begin");
            MenuGraphic logo = new MenuGraphic("logo", new Vector2(Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.X, Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.Y - 100));
            MenuGraphic bestPlayed = new MenuGraphic("bestPlayed", new Vector2(logo.Dim.Center.X, logo.Dim.Bottom+15));
            pressA.Position = new Vector2(bestPlayed.Dim.Center.X - font.MeasureString(pressA.Text).Length()/2, bestPlayed.Dim.Bottom + font.LineSpacing);
            pressA.Flashing = true;

            // Hook up menu event handlers.
            this.GreenButtonPressed += GoToMainMenu;

            // Add entries to the menu.
            MenuEntries.Add(logo);
            //MenuEntries.Add(bestPlayed);
            MenuEntries.Add(pressA);

            //hack to makes sure press a is selected, fix.
            selectedEntry = 2;

            Stage.SaveGame.LoadGameData();
        }


        void GoToMainMenu (object sender, EventArgs e) {
            this.MarkedForRemove = true;
            MenuSystem.AddScreen(new BackgroundScreen());
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

                bool isSelected = IsActive && (i == selectedEntry);

              //  if(i != 1)
                    menuEntry.Draw(this, isSelected, dt);
            }
        }

        ///EVENTS///
        public event EventHandler<EventArgs> GreenButtonPressed;
        protected override void OnCancel () { }
    }
}
