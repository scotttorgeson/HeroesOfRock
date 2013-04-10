using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.MenuSystem.Menus.MenuComponents {
    public class MenuGraphic : MenuEntry{
        private Texture2D graphic;
        private Texture2D graphicHover;
        private String textureName;
        private Rectangle dim;
        private float scale;


        public MenuGraphic (string textureName, Vector2 position, float scale) :
            base(""){

            this.textureName = textureName;
            this.graphic = Stage.Content.Load<Texture2D>("UI/"+textureName);
            this.graphicHover = graphic;
            int width = (int)(graphic.Width * scale);
            int height = (int)(graphic.Height * scale);
            this.position = position;
            this.dim = new Rectangle(Int32.Parse(position.X.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture) - (width / 2),
                Int32.Parse(position.Y.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture) - ( height / 2),  width, height);
            CanSelect = false;
        }

        public MenuGraphic (string textureName, string hoverName, Vector2 position, float scale) :
            base("") {

            this.textureName = textureName;
            this.graphic = Stage.Content.Load<Texture2D>("UI/" + textureName);
            this.graphicHover = Stage.Content.Load<Texture2D>("UI/" + hoverName); ;
            int width = (int)(graphic.Width * scale);
            int height = (int)(graphic.Height * scale);
            this.position = position;
            this.dim = new Rectangle(Int32.Parse(position.X.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture) - (width / 2),
                Int32.Parse(position.Y.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture) - (height / 2), width, height);
            CanSelect = false;
        }

        public Rectangle Dim {
            get { return dim; }
            set { dim = value; }
        }

        public String TextureName {
            get { return textureName; }
        }

        public override void Draw (GameMenu menu, bool isSelected, float dt) {
            if (Flashing) {
                if (alpha >= 1.0) {
                    diff = -0.7f * dt;
                } else if (alpha <= 0.0) { //completely fade out
                    diff = 0.7f * dt;
                }
                alpha += diff;
            }

            if (isSelected) {
                Stage.renderer.SpriteBatch.Draw(graphicHover, dim, Color.White * alpha);
            } else {
                Stage.renderer.SpriteBatch.Draw(graphic, dim, Color.White * alpha);
            }
        }
    }
}
