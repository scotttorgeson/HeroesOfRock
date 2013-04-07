using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLib.Engine.MenuSystem.Menus.MenuComponents;
using GameLib.Engine.Particles;
using Microsoft.Xna.Framework.Input;
using GameLib.Engine.AttackSystem;

namespace GameLib.Engine.MenuSystem.Menus {
    public class EndLevelMenu : GameMenu {
        private Texture2D smoked;
        private Texture2D perfection;
        private Texture2D[] skullCounters;

        private Rectangle skullRec;
        private Rectangle scoreRec;
        private Rectangle bonusRec;
        private Rectangle optionsRec;
        private Rectangle window;

        private int baseScore;
        private int score;
        private int bonusPoints;
        private int killStreak;
        private int skullToDraw;
        private int lerpRate;

        private float timeOnLevel;
        private float timer;

        private bool doneLerpingScore; //may not need
        private bool doneLerpingKillStreak;
        private bool checkForPerfect;
        private bool drawPerfect;

        private RockMeter rm;

        public EndLevelMenu (string title)
            : base(title) {
            skullToDraw = 0;
            GetScoreData();
            timer = 0f;
            checkForPerfect = true;
            Stage.ActiveStage.PauseGame();

            // todo: update to save the real high score
            Stage.SaveGame.AddHighScore(Stage.ActiveStage.Parm.GetString("AssetName"), rm.GetTotalScore);
            if (Stage.ActiveStage.Parm.HasParm("NextLevel"))
                Stage.SaveGame.UnlockLevel(Stage.ActiveStage.Parm.GetString("NextLevel"));
            Stage.SaveGame.SaveGameData();
        }

        private void GetScoreData () {
            //get our data
            rm = PlayerAgent.Player.GetAgent<RockMeter>();

            baseScore = rm.Score;
            timeOnLevel = Stage.ActiveStage.Time;
            killStreak = 0;
            skullToDraw = 0;
            bonusPoints = 0;
            lerpRate = 50000;
            skullCounters = new Texture2D[4];
        }

        public override void LoadContent () {
            smoked = Stage.Content.Load<Texture2D>("UI/Menu/smoked");
            perfection = Stage.Content.Load<Texture2D>("UI/Menu/perfect");

            for (int i = 0; i < 4; i++)
                skullCounters[i] = Stage.Content.Load<Texture2D>("UI/HUD/SkullCounter" + (i + 1));

            window = Stage.renderer.GraphicsDevice.Viewport.Bounds;
            skullRec = new Rectangle(window.Right - (int)(skullCounters[0].Width * 0.5), window.Bottom - (int)(skullCounters[0].Height * 0.5),
                (int)(skullCounters[0].Width * 0.5), (int)(skullCounters[0].Height * 0.5));
            scoreRec = new Rectangle(skullRec.X, skullRec.Y + skullRec.Height, skullRec.Width, skullRec.Height);
            bonusRec = new Rectangle(scoreRec.X, scoreRec.Y + scoreRec.Height, scoreRec.Width, scoreRec.Height * 3);
            optionsRec = new Rectangle(bonusRec.X, bonusRec.Y + bonusRec.Height, bonusRec.Width, bonusRec.Height / 3);

            base.LoadContent();
        }

        void Continue (object sender, EventArgs e) {
            Stage.GameRunning = true;
            Stage.ActiveStage.ResumeGame();
            ExitScreen();
            Stage.LoadStage(Stage.ActiveStage.Parm.GetString("NextLevel"), true);
        }

        void QuitGame (object sender, EventArgs e) {
            Stage.GameRunning = true;
            Stage.ActiveStage.ResumeGame();
            Stage.LoadStage("MainMenu", true);
            MenuSystem.AddScreen(new BackgroundScreen());
            MenuSystem.AddScreen(new MainMenu());
        }

        public override void HandleInput (MenuInput input) {
            if (!doneLerpingScore) {
                if (input.IsGreen() || input.IsRed()) {
                    score = baseScore + bonusPoints;
                    doneLerpingScore = true;
                }
            } else if (doneLerpingScore) {
                if (input.IsGreen()) {
                    if (Stage.ActiveStage.Parm.HasParm("NextLevel")) {
                        Stage.LoadStage(Stage.ActiveStage.Parm.GetString("NextLevel"), true);
                    } else {
                        Stage.LoadStage("MainMenu", true);
                    }

                } else if (input.IsRed()) {
                    QuitGame(this, new EventArgs());
                }
            }

            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Home)) {
                ExitScreen();
            }

            base.HandleInput(input);
        }

        public override void Update (float dt, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            timer += dt;

            if (!doneLerpingScore) {
                int rate = lerpRate;
                if ((baseScore + bonusPoints) > 900000)
                    rate = 100000;

                Lerp(baseScore + bonusPoints, ref score, (int)Math.Ceiling(rate * dt));
            }
               
            else if (!doneLerpingKillStreak)
                TallySkulls();
            else if (doneLerpingScore && doneLerpingKillStreak && checkForPerfect) {
                CheckForPerfect();
            }

#if DEBUG && WINDOWS
            ControlsQB c = Stage.ActiveStage.GetQB<ControlsQB>();
            if (c.CurrentKeyboardState.IsKeyDown(Keys.End) && c.LastKeyboardState.IsKeyUp(Keys.End)) {
                drawPerfect = true;
            }

#endif
            base.Update(dt, otherScreenHasFocus, coveredByOtherScreen);

                /*if (alpha >= 1.0) {
                    diff = -0.7f * dt;
                } else if (alpha <= 0.3) {
                    diff = 0.7f * dt;
                }
                alpha += diff;

                color *= alpha;
                 */
        }

        public override void Draw (float dt) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = Renderer.Instance.GraphicsDevice;
            SpriteBatch spriteBatch = Stage.renderer.SpriteBatch;
            SpriteFont font = MenuSystem.Font;
            SpriteFont font2 = Stage.Content.Load<SpriteFont>("belligerent");

            spriteBatch.Draw(smoked, window, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            scoreRec = new Rectangle(window.Center.X, window.Center.Y - window.Height / 2, window.Width / 2, window.Height / 2);
            skullRec = new Rectangle(scoreRec.Center.X - (int)(skullCounters[0].Width * 0.5) / 2, window.Bottom - skullCounters[0].Height - 35,
            (int)(skullCounters[0].Width * 0.5), (int)(skullCounters[0].Height * 0.5));
            
            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuEntry menuEntry = MenuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, dt);
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

            if (doneLerpingKillStreak)
            {
                spriteBatch.Draw(skullCounters[skullToDraw], skullRec, Color.White);


                spriteBatch.DrawString(font2, "Kill Streak", new Vector2(skullRec.Center.X, skullRec.Top + 5), Color.White, 0,
                          font.MeasureString("Kill Streak") / 2, 2, SpriteEffects.None, 0);

                spriteBatch.DrawString(font2, "x" + killStreak, new Vector2(skullRec.Center.X + 50, skullRec.Center.Y), Color.White, 0,
                          font.MeasureString("x" + killStreak) / 2, 2, SpriteEffects.None, 0);
            }

            spriteBatch.DrawString(font, "Final Score ", new Vector2(scoreRec.Center.X, scoreRec.Center.Y), Color.White, 0,
                      font.MeasureString("Final Score ") / 2, 2, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, score.ToString(System.Globalization.CultureInfo.InvariantCulture), new Vector2(scoreRec.Center.X, scoreRec.Center.Y + (2 * font.LineSpacing)), Color.White, 0,
                       font.MeasureString(score.ToString(System.Globalization.CultureInfo.InvariantCulture)) / 2, 2, SpriteEffects.None, 0);

            if (drawPerfect)
                spriteBatch.Draw(perfection, new Rectangle(scoreRec.Center.X - (int)(perfection.Width * 0.5) / 2, scoreRec.Bottom - 100, (int)(perfection.Width * 0.5),
                            (int)(perfection.Height * 0.5)), Color.White);

            if (doneLerpingScore) {
                spriteBatch.DrawString(font, "Press Green to Continue", new Vector2(skullRec.Center.X, skullRec.Bottom + 30), Color.White, 0,
                    font.MeasureString("Press Green to Continue") / 2, 1, SpriteEffects.None, 0);

                spriteBatch.DrawString(font, "Press Red to Return to Main Menu", new Vector2(skullRec.Center.X, skullRec.Bottom + 10 + skullRec.Height / 2), Color.White, 0,
                    font.MeasureString("Press Red to Return to Main Menu") / 2, 1, SpriteEffects.None, 0);
            }
        }

        public void Lerp (int goalVal, ref int currVal, int lerpAmount) {
            if (goalVal < currVal) {
                currVal -= lerpAmount;
                if (currVal <= goalVal) currVal = goalVal;
            } else if (goalVal == currVal) {
                doneLerpingScore = true;
            } else {
                currVal += lerpAmount;
                if (currVal >= goalVal) currVal = goalVal;
            }
        }

        private void TallySkulls () {

            killStreak = rm.HighestKillStreak;
                if (killStreak >= 5) {
                    skullToDraw = 1;
                    Stage.ActiveStage.GetQB<AudioQB>().PlaySound("A-heavy");
                } else if (killStreak >= 20) {
                    skullToDraw = 2;
                    Stage.ActiveStage.GetQB<AudioQB>().PlaySound("A-heavy");
                } else if (killStreak >= 50) {
                    skullToDraw = 3;
                    Stage.ActiveStage.GetQB<AudioQB>().PlaySound("Arail-attack2");
                }
                doneLerpingKillStreak = true;
                doneLerpingScore = false;
                bonusPoints = rm.KillStreakScore;
                Stage.ActiveStage.GetQB<ParticleQB>().AddFloatingText(new Vector2(skullRec.Center.X, skullRec.Top), new Vector2(0, 10), 3f, "+" + bonusPoints);
        }

        private void CheckForPerfect () {
            bonusPoints = rm.RockGodScore;
            if (bonusPoints > 0) {
                drawPerfect = true;
                Stage.ActiveStage.GetQB<ParticleQB>().AddFloatingText(Vector2.Zero, new Vector2(0, 10), 3f, "+" + bonusPoints);
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("A-heavy");
            }
            doneLerpingScore = false;
            checkForPerfect = false; ;
        }
    }
}
