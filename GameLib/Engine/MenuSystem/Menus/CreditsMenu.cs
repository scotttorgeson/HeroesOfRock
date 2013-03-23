using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.MenuSystem.Menus {
    public class CreditsMenu : GameMenu {
        Texture2D credits;

        public CreditsMenu ()
            : base("Credits") {
            MenuEntry Back = new MenuEntry("Back");

            Back.Selected += OnCancel;

            MenuEntries.Add(Back);
            credits = Stage.Content.Load<Texture2D>("UI/Menu/creds");
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel () {
            this.MarkedForRemove = true;
            MenuSystem.AddScreen(new MainMenu());
        }

                /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw (float dt) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = Renderer.Instance.GraphicsDevice;
            SpriteFont font = MenuSystem.Font;

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuEntry menuEntry = MenuEntries[i];

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

            Stage.renderer.SpriteBatch.Draw(credits, new Rectangle(Renderer.ScreenRect.Center.X - 275, (int)MenuEntries[0].Position.Y + 50, 550, 430), Color.White);
        }

    }
}
