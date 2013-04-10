using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GameLib.Engine.MenuSystem.Menus {
     public class MenuEntry {
        private string text;
        private float selectionFade;
        protected float alpha;
        protected float diff;
        protected Vector2 position;
        private Boolean flashing;
        private Boolean canSelect;
        private Color inActiveColor;
        private Color activeColor;

        public Color ActiveColor {
            get { return activeColor; }
            set { activeColor = value; }
        }
         
        public Color InActiveColor {
            get { return inActiveColor; }
            set { inActiveColor = value; }
        }

        public Boolean CanSelect {
            get { return canSelect; }
            set { canSelect = value; }
        }

        public Boolean Flashing {
            get { return flashing; }
            set { flashing = value; }
        }

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
            this.alpha = 1f;
            this.canSelect = true;
            inActiveColor = Color.SlateGray;
            activeColor = Color.White;
        }

        public virtual void Update (GameMenu screen, bool isSelected, float dt) {
            float fadeSpeed = (float)dt * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }
        public virtual void Draw (GameMenu menu, bool isSelected, float dt) {
          

            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? activeColor : inActiveColor;

            // Modify the alpha to fade text out during transitions.
            color *= menu.TransitionAlpha;

            if (flashing) {
                if (alpha >= 1.0) {
                    diff = -0.7f * dt;
                } else if (alpha <= 0.3) {
                    diff = 0.7f * dt;
                }
                alpha += diff;

                color *= alpha;
            }
            // Draw text, centered on the middle of each line.
            MenuSystem sys = menu.MenuSystem;
            SpriteFont font = sys.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            Stage.renderer.SpriteBatch.DrawString(font, text, position, color, 0,
                                   origin, 1, SpriteEffects.None, 0);
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight (GameMenu screen) {
            return screen.MenuSystem.Font.LineSpacing;
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the menu.
        /// </summary>
        public virtual int GetWidth (GameMenu screen) {
            return (int)screen.MenuSystem.Font.MeasureString(Text).X;
        }
    }
}
