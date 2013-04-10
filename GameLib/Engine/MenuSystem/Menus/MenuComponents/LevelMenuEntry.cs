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
            // Modify the alpha to fade text out during transitions.
            Color color = Color.White;
            color *= menu.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            MenuSystem sys = menu.MenuSystem;
            SpriteFont baseFont = sys.Font;
            SpriteFont font = sys.Boycott;//TODO ADD THE NEW FONT

            scale = .25f;

            int x = (int)Position.X - (LevelGraphic.Width / 2);
            int y = (int)Position.Y - (LevelGraphic.Height / 2);
            int scaledWidth = (int)(width * scale);
            int scaledHeight = (int)(height * scale);

            Rectangle rec = new Rectangle(x, y, scaledWidth, scaledHeight);

            //draws the level graphic
            Stage.renderer.SpriteBatch.Draw(LevelGraphic, rec, color);

            //draw level name above graphic
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            Vector2 startPosition = new Vector2(rec.X, rec.Y - font.LineSpacing / 2);
            Stage.renderer.SpriteBatch.DrawString(font, Text.ToUpper(), startPosition, color, 0,
                                   origin, 1, SpriteEffects.None, 0);

            //draws the locked overlay
            if (IsLocked)
                Stage.renderer.SpriteBatch.Draw(lockGraphic, new Rectangle(rec.X-1, rec.Y-1, rec.Width + 2, rec.Height + 2), Color.White);

            //grey out if not selected
            if (!isSelected) {
                Stage.renderer.SpriteBatch.Draw(blank, rec, Color.Black * 0.8f);
            } else {

                //draw the skull ratings
                float skullScale = 0.1f;

                int w = (int)(skullGraphic.Width * skullScale);
                int h = (int)(skullGraphic.Height * skullScale);

                Rectangle skullRec = new Rectangle((int)startPosition.X, (int)startPosition.Y + rec.Height + h / 2, w, h);
                Stage.renderer.SpriteBatch.Draw(skullGraphic, skullRec, Color.White);

                //draw the skull rating string TODO get correct skull
                String rating = "HARDCORE STATUS";
                Stage.renderer.SpriteBatch.DrawString(font, rating, new Vector2(rec.Right - font.MeasureString(rating).X, rec.Bottom +
                    font.MeasureString(rating).Y), color, 0, origin, 1, SpriteEffects.None, 0);

                //draw divider TODO

                //draw Top scores TODO set below divider
                String title = "TOP SCORES";
                Stage.renderer.SpriteBatch.DrawString(font, title, new Vector2(rec.Right - font.MeasureString(title).X, rec.Bottom +
                    +font.MeasureString(rating).Y + font.MeasureString(title).Y), color, 0, origin, 1, SpriteEffects.None, 0);

                int[] tscores = { 3333, 2222, 1111 };//TODO get this from the level
                for (int i = 0; i < 3; i++) {
                    String score = (i + 1) + " - " + tscores[i]; //TODO use shit parsing
                    Stage.renderer.SpriteBatch.DrawString(baseFont, score, new Vector2(rec.Right - baseFont.MeasureString(score).X, rec.Bottom +
                     +baseFont.MeasureString(rating).Y + baseFont.MeasureString(title).Y + (baseFont.MeasureString(score).Y * (i + 1)) / 2), color, 0, origin, 1, SpriteEffects.None, 0);
                }
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
