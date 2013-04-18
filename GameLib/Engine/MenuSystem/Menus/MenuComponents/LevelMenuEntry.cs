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

        private Texture2D blank, lockGraphic, skullGraphic, divider;
        private SpriteFont baseFont;
        private int height, width;
        int[] tscores;
        private float scale;
        private String rating;

        public LevelMenuEntry (string levelName, string text, string graphic, bool useLoadingLevel)
            : base(text) {
                this.LevelName = levelName;
                this.LevelGraphic = Stage.Content.Load<Texture2D>("UI/LevelGraphics/"+graphic);
                this.divider = Stage.Content.Load<Texture2D>("UI/LevelGraphics/divider");
                this.width = LevelGraphic.Width;
                this.height = LevelGraphic.Height;
                this.blank = Stage.Content.Load<Texture2D>("UI/Menu/blank");
                this.lockGraphic = Stage.Content.Load<Texture2D>("UI/LevelGraphics/LevelLock");
                this.baseFont = Stage.Content.Load<SpriteFont>("Arial");
                this.scale =  1f;
                      
            
                //TODO These should be driven from user profile
                PlayerScore();
                IsLocked = !Stage.SaveGame.levelsUnlocked.Contains(levelName);
                StarRating = 0;
                tscores = TopScore();
                UseLoadingLevel = useLoadingLevel;
        }

        private int[] TopScore () {
            
            int[] topScores = Stage.SaveGame.GetHighScores(LevelName).ToArray();
            Array.Sort(topScores, ((a, b) => -1 * a.CompareTo(b)));

            return topScores;
        }

        //TODO PLACE APPROPRIATE VALUES
        private void PlayerScore () {
            float perc = 0;
            skullGraphic = null;
            rating = "";
            perc = Stage.SaveGame.GetPercKillStreak(LevelName);

            if(perc <= 0)
            {
                //do nothing
            }
            else if (perc < .25f) {
                skullGraphic = Stage.Content.Load<Texture2D>("UI/LevelGraphics/skull1");
                rating = "POSER STATUS";
            } else if (perc < .5f) {
                skullGraphic = Stage.Content.Load<Texture2D>("UI/LevelGraphics/skull2");
                rating = "HEAD BANGER STATUS";
            } else if (perc < .75f) {
                skullGraphic = Stage.Content.Load<Texture2D>("UI/LevelGraphics/skull3");
                rating = "METAL HEAD STATUS";
            } else if (perc < 1) {
                skullGraphic = Stage.Content.Load<Texture2D>("UI/LevelGraphics/skull4");
                rating = "HARDCORE STATUS";
            } else {
                skullGraphic = Stage.Content.Load<Texture2D>("UI/LevelGraphics/skull5");
                rating = "ROCK GOD STATUS";
            }
        }

        public override void Draw (GameMenu menu, bool isSelected, float dt) {
            // Modify the alpha to fade text out during transitions.
            Color color = Color.White;
            color *= menu.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            SpriteFont font = menu.MenuSystem.Boycott;

            scale = .5f;

            int x = (int)(Position.X - ((1.6*LevelGraphic.Width) / 2));
            int y = (int)(Position.Y - ((1.3*LevelGraphic.Height) / 2));
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

                Rectangle dividerRec = new Rectangle((int)startPosition.X, (int)startPosition.Y + rec.Height, rec.Width, 1);

                //draw the skull ratings
                if (skullGraphic != null) {
                float skullScale = 0.85f;

                int w = (int)(skullGraphic.Width * skullScale);
                int h = (int)(skullGraphic.Height * skullScale);

                
                    Rectangle skullRec = new Rectangle((int)startPosition.X, (int)startPosition.Y + rec.Height + h / 2, w, h);
                    Stage.renderer.SpriteBatch.Draw(skullGraphic, skullRec, Color.White);


                    //draw the skull rating string
                    Vector2 ratingPos = new Vector2(rec.Right - font.MeasureString(rating).X, rec.Bottom + skullRec.Height/2 + font.MeasureString(rating).Y/2);
                    Stage.renderer.SpriteBatch.DrawString(font, rating, ratingPos, color, 0, origin, 1, SpriteEffects.None, 0);

                    //draw divider
                    int dw = rec.Width;
                    int dh = divider.Height;
                    dividerRec = new Rectangle((int)startPosition.X, (int)ratingPos.Y + (12 * dh), dw, dh);
                    Stage.renderer.SpriteBatch.Draw(divider, dividerRec, Color.White);
                }
                
                //draw Top scores
                if (LevelName != "Tutorial")
                {
                    String title = "TOP SCORES";
                    Vector2 titlePos = new Vector2(dividerRec.Right - font.MeasureString(title).X, dividerRec.Bottom +
                        font.MeasureString(title).Y);
                    Stage.renderer.SpriteBatch.DrawString(font, title, titlePos, color, 0, origin, 1, SpriteEffects.None, 0);

                    for (int i = 0; i < tscores.Length; i++)
                    {
                        String score = (i + 1) + " - " + tscores[i].ToString(System.Globalization.CultureInfo.InvariantCulture);
                        Stage.renderer.SpriteBatch.DrawString(baseFont, score, new Vector2(rec.Right - baseFont.MeasureString(score).X, (int)titlePos.Y
                            + 15 + (int)(1.5 * baseFont.MeasureString(score).Y * (i + 1)) / 2), color, 0, origin, 1, SpriteEffects.None, 0);
                    }
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
