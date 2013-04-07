using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.MenuSystem.Menus.MenuComponents {
    public class MenuGraphic : MenuEntry{
        private Texture2D graphic;
        private Rectangle dim;

        public MenuGraphic (string textureName, Vector2 position) :
            base(""){
            this.graphic = Stage.Content.Load<Texture2D>("UI/Menu/"+textureName);
            this.position = position;
            this.dim = new Rectangle(Int32.Parse(position.X.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture) - (graphic.Width / 2),
                Int32.Parse(position.Y.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture) - (graphic.Height / 2), graphic.Width, graphic.Height);
            this.CanSelect = false;
        }
        

        public Rectangle Dim {
            get { return dim; }
        }

        public override void Draw (GameMenu menu, bool isSelected, float dt) {
            // Draw text, centered on the middle of each line.
            MenuSystem sys = menu.MenuSystem;
            SpriteFont font = sys.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            Stage.renderer.SpriteBatch.Draw(graphic, dim, Color.White);
        }
    }
}
