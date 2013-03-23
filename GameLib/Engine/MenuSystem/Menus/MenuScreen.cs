using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameLib.Engine.MenuSystem.Menus {

   public abstract class MenuScreen : GameScreen {

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        protected int selectedEntry = 0;
        protected string menuTitle;

        protected IList<MenuEntry> MenuEntries {
            get { return menuEntries; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen (string menuTitle) {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        public override void HandleInput (MenuInput input) {
            // Move to the previous menu entry?
            if (input.IsMenuUp()) {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.IsMenuDown()) {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            if (input.IsMenuSelect()) {
                OnSelectEntry(selectedEntry);
            } else if (input.IsMenuCancel()) {
                OnCancel();
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry (int entryIndex) {
            menuEntries[entryIndex].OnSelectEntry();
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
                position.X = MenuSystem.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

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
        public override void Update (GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++) {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw (GameTime gameTime) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = MenuSystem.GraphicsDevice;
            SpriteBatch spriteBatch = MenuSystem.SpriteBatch;
            SpriteFont font = MenuSystem.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++) {
                MenuEntry menuEntry = menuEntries[i];

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
