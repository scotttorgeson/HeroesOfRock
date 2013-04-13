using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameLib.Engine.MenuSystem.Menus.MenuComponents;
namespace GameLib.Engine.MenuSystem.Menus {

    public class QuitConfirmPopUp : PopUpScreen {
        public QuitConfirmPopUp (string message)
            : this(message, true) { }

        public QuitConfirmPopUp (string message, bool includeUsageText)
            : base(message) {
            Vector2 position = new Vector2(Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.X, Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.Y);
            MenuGraphic confirm = new MenuGraphic("Menu/quitConfirm", position, 0.5f);

            MenuEntries.Add(confirm);
        }

        public override void HandleInput (MenuInput input) {
            if (input.IsMenuCancel())
                ExitScreen();
            if (input.IsMenuSelect())
                Stage.QuitGame = true;
        }

        public override void Draw (float dt) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();
            
            Rectangle fullscreen = Stage.renderer.GraphicsDevice.Viewport.Bounds;
            Stage.renderer.SpriteBatch.Draw(MenuSystem.BlankTexture, fullscreen, Color.Black * 0.5f);

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuEntry menuEntry = MenuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, dt);
            }
        }
    }
}
