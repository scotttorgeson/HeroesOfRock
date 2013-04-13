using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameLib.Engine.MenuSystem.Menus {

   public abstract class GameMenu : GameScreen {

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        protected int selectedEntry = 0;
        protected string menuTitle;

        protected IList<MenuEntry> MenuEntries {
            get { return menuEntries; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameMenu (string menuTitle) {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        public override void HandleInput (MenuInput input) {
            if (menuEntries.Count < 1) return; //don't null reference
            // Move to the previous menu entry?
            if (input.IsMenuUp()) {
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("knob-click-1");
                int select = selectedEntry;

                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
                if (!menuEntries[selectedEntry].CanSelect)
                    selectedEntry = select;
            }else if (input.IsMenuDown()) {
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("knob-click-1");
                int select = selectedEntry;

                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
                if (!menuEntries[selectedEntry].CanSelect)
                    selectedEntry = select;
            } else if (input.IsMenuSelect()) {

                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("A");
                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
                else if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                OnSelectEntry(selectedEntry);
            } else if (input.IsMenuCancel()) {
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("Uppercut");
                OnCancel();
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry (int entryIndex) {
            if (menuEntries.Count > 0) {
                menuEntries[entryIndex].OnSelectEntry();
            }
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel () {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel (object sender, EventArgs e) {
            OnCancel();
        }



        /// <summary>
        /// All menu entries are lined up in a vertical list, centered on the menu.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations () {

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++) {
                MenuEntry menuEntry = menuEntries[i];

                // each entry is to be centered horizontally
                position.X = Renderer.ScreenWidth / 2 - menuEntry.GetWidth(this) / 2;

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
        /// Updates the menu.
        /// </summary>
        public override void Update (float dt, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            base.Update(dt, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++) {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, dt);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw (float dt) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = Renderer.Instance.GraphicsDevice;
            SpriteFont font = MenuSystem.Font;
            font.LineSpacing = 50;

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++) {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, dt);
            }

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the menu
            Vector2 titlePosition = new Vector2(Renderer.ScreenWidth / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            Stage.renderer.SpriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
        }
    }
}
