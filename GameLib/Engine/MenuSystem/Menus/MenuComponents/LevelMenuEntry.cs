using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.MenuSystem.Menus {
    public class LevelMenuEntry : MenuEntry {
        public Texture2D LevelGraphic { get; set; }
        public Boolean IsLocked { get; set; }
        public int StarRating { get; private set; }
        public string LevelName { get; private set; }
        public bool UseLoadingLevel { get; private set; }

        private Texture2D blank;
        private Texture2D lockGraphic;
        private Texture2D skullGraphic;
        private int height;
        private int width;
        private float scale;

        public LevelMenuEntry (string levelName, string text, string graphic, bool useLoadingLevel)
            : base(text) {
                LevelName = levelName;
                LevelGraphic = Stage.Content.Load<Texture2D>("UI/LevelGraphics/"+graphic);
                
                width = LevelGraphic.Width;
                height = LevelGraphic.Height;

                this.blank = Stage.Content.Load<Texture2D>("UI/Menu/blank");
                this.lockGraphic = Stage.Content.Load<Texture2D>("UI/LevelGraphics/LevelLock");
                this.skullGraphic = Stage.Content.Load<Texture2D>("UI/Misc/skull");
                this.scale =  .30f;
            
                //TODO These should be driven from user profile
                IsLocked = true;
                StarRating = 0;

                UseLoadingLevel = useLoadingLevel;
        }

        public override void Draw (GameMenu menu, bool isSelected, float dt) {
            //TODO modify selection shit

            // Modify the alpha to fade text out during transitions.
            Color color = Color.White;
            color *= menu.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            MenuSystem sys = menu.MenuSystem;
            SpriteFont font = sys.Font;

            scale = .25f;

            int x = (int)Position.X - (LevelGraphic.Width / 2);
            int y = (int)Position.Y - (LevelGraphic.Height / 2);
            int scaledWidth = (int)(width * scale);
            int scaledHeight = (int)(height * scale);

            Rectangle rec = new Rectangle(x, y, scaledWidth, scaledHeight);

            //selected highlight 
            if (isSelected) { 
                int border = 2;
                Stage.renderer.SpriteBatch.Draw(blank, new Rectangle(x-border/2, y-border/2, scaledWidth+border, scaledHeight+border), Color.White);
            }

            //draws the level graphic
            Stage.renderer.SpriteBatch.Draw(LevelGraphic, rec, color);

            //draws the locked overlay
            if (IsLocked)
                Stage.renderer.SpriteBatch.Draw(lockGraphic, rec, Color.White);

            //draws the level name below graphic
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            Vector2 startPosition = new Vector2(rec.X + (rec.Width / 2) - font.LineSpacing, rec.Y + rec.Height + font.LineSpacing/2);
            Stage.renderer.SpriteBatch.DrawString(font, Text, startPosition, color, 0,
                                   origin, 1, SpriteEffects.None, 0);

            //draw the skull ratings
            for(int i = 0; i<StarRating;i++){
                float skullScale = 0.01f;
                
                int w = (int)(skullGraphic.Width *skullScale);
                int h = (int)(skullGraphic.Height*skullScale);
                int skullx = (int)startPosition.X + ((w + 10) * i);

                Rectangle skullRec = new Rectangle(skullx, (int)startPosition.Y, w, h);
                Stage.renderer.SpriteBatch.Draw(skullGraphic, skullRec, Color.White);
            }

        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public override int GetHeight (GameMenu screen) {
            return (int)(height * scale);
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the menu.
        /// </summary>
        public override int GetWidth (GameMenu screen) {
            return (int)(width * scale);
        }
    }
}
