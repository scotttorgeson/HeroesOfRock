using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GameLib.Engine.MenuSystem.Menus {
     public class MenuEntry {
        string text;
        float selectionFade;
        Vector2 position;

        public string Text {
            get { return text; }
            set { text = value; }
        }

        public Vector2 Position {
            get { return position; }
            set { position = value; }
        }


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<EventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry () {
            if (Selected != null)
                Selected(this, new EventArgs());
        }


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry (string text) {
            this.text = text;
        }

        public virtual void Update (MenuScreen screen, bool isSelected, GameTime gameTime) {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }
        public virtual void Draw (MenuScreen menu, bool isSelected, GameTime gameTime) {

            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;

            // Modify the alpha to fade text out during transitions.
            color *= menu.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            MenuSystem sys = menu.MenuSystem;
            SpriteBatch spriteBatch = sys.SpriteBatch;
            SpriteFont font = sys.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, 1, SpriteEffects.None, 0);
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight (MenuScreen screen) {
            return screen.MenuSystem.Font.LineSpacing;
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the menu.
        /// </summary>
        public virtual int GetWidth (MenuScreen screen) {
            return (int)screen.MenuSystem.Font.MeasureString(Text).X;
        }
    }
}
