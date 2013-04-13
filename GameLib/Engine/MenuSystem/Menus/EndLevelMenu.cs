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
        private Texture2D levelComplete;
        private Texture2D scoreTitle;
        private Texture2D bestTitle;
        private Texture2D statusTitle;
        private Texture2D skullStatus;
        private Texture2D selectBack;
        private Texture2D[] skullCounters;

        private Rectangle skullRec;
        private Rectangle scoreRec;
        private Rectangle scoreTitleRec;
        private Rectangle statusTitleRec;
        private Rectangle skullStatusRec;
        private Rectangle window;
        private Rectangle levelCompleteRec;
        private Rectangle bestTitleRec;
        private Rectangle selectBackRec;

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
        private bool drawStatus;

        private RockMeter rm;

        public EndLevelMenu (string title)
            : base(title) {
            
            Stage.ActiveStage.PauseGame();
            GetScoreData();

            timer = 0f;
            
            // save the high score
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
            lerpRate = 10000;
            skullCounters = new Texture2D[4];
            checkForPerfect = true;
        }

        public override void LoadContent () {
            base.LoadContent();
            int w, h;
            float scale = 0.8f;

            //load textures
            for (int i = 0; i < 4; i++)
                skullCounters[i] = Stage.Content.Load<Texture2D>("UI/HUD/SkullCounter" + (i + 1));

            levelComplete = Stage.Content.Load<Texture2D>("UI/EndLevel/levelComplete");
            scoreTitle = Stage.Content.Load<Texture2D>("UI/EndLevel/scoreTitle");
            bestTitle = Stage.Content.Load<Texture2D>("UI/EndLevel/bestTitle");
            statusTitle = Stage.Content.Load<Texture2D>("UI/EndLevel/statusTitle");
            skullStatus = Stage.Content.Load<Texture2D>("UI/EndLevel/skull1");
            selectBack = Stage.Content.Load<Texture2D>("UI/MainMenu/select_back");

            window = Stage.renderer.GraphicsDevice.Viewport.Bounds;

            w = (int)(levelComplete.Width * scale);
            h = (int)(levelComplete.Height * scale);
            levelCompleteRec = new Rectangle(window.Left + w/2, window.Top + h, w, h);

            w = (int)(scoreTitle.Width * scale);
            h = (int)(scoreTitle.Height * scale);
            scoreTitleRec = new Rectangle(levelCompleteRec.X, levelCompleteRec.Y + (2*h), w, h);

            w = (int)(levelCompleteRec.Width);
            h = MenuSystem.Font.LineSpacing / 2;
            scoreRec = new Rectangle(scoreTitleRec.Center.X, scoreTitleRec.Bottom + h, w, h);

            w = (int)(bestTitle.Width * scale);
            h = (int)(bestTitle.Height * scale);
            bestTitleRec = new Rectangle(scoreTitleRec.X, scoreRec.Bottom + h, w, h);

            w = skullCounters[0].Width;
            h = skullCounters[0].Height;
            skullRec = new Rectangle(bestTitleRec.Center.X - w / 2, bestTitleRec.Bottom + h / 4, w, h);

            w = (int)(statusTitle.Width * scale);
            h = (int)(statusTitle.Height * scale);
            statusTitleRec = new Rectangle(levelCompleteRec.Right + h, scoreTitleRec.Y, w, h);

            w = (int)(skullStatus.Width * scale);
            h = (int)(skullStatus.Height * scale);
            skullStatusRec = new Rectangle(statusTitleRec.Center.X, statusTitleRec.Bottom + h / 4, w, h);

            w = (int)(selectBack.Width * scale);
            h = (int)(selectBack.Height * scale);
            selectBackRec = new Rectangle(window.Right - w, window.Bottom -  h, w, h);
        }

        void Continue (object sender, EventArgs e) {
            Stage.GameRunning = true;
            Stage.ActiveStage.ResumeGame();
            ExitScreen();
            LoadingScreen.Load(MenuSystem, true, Stage.ActiveStage.Parm.GetString("NextLevel"));
        }

        void QuitGame (object sender, EventArgs e) {
            Stage.GameRunning = true;
            Stage.ActiveStage.ResumeGame();
            Stage.LoadStage("MainMenu", true);
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
                    rate = lerpRate * 10;

                Lerp(baseScore + bonusPoints, ref score, (int)Math.Ceiling(rate * dt));
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("banknote_counter_COUNTING"); 
            }          
            else if (!doneLerpingKillStreak){
           
                TallySkulls();
            } else if (doneLerpingScore && doneLerpingKillStreak && checkForPerfect) {
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("banknote-counterSTOPPING");
                CalculateStatus();
            }

#if DEBUG && WINDOWS
            ControlsQB c = Stage.ActiveStage.GetQB<ControlsQB>();
            if (c.CurrentKeyboardState.IsKeyDown(Keys.End) && c.LastKeyboardState.IsKeyUp(Keys.End)) {
                drawStatus = true;
            }

#endif
            base.Update(dt, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw (float dt) {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            SpriteBatch spriteBatch = Stage.renderer.SpriteBatch;
            SpriteFont font = MenuSystem.Font;

            spriteBatch.Draw(MenuSystem.BlankTexture, window, (Color.Black*0.5f)*TransitionAlpha);

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuEntry menuEntry = MenuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, dt);
            }

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            spriteBatch.Draw(levelComplete, levelCompleteRec, Color.White);
                        
            spriteBatch.Draw(scoreTitle, scoreTitleRec, Color.White);
            spriteBatch.DrawString(font, score.ToString(System.Globalization.CultureInfo.InvariantCulture), new Vector2(scoreRec.Center.X, scoreRec.Center.Y + (2 * font.LineSpacing)), Color.Yellow, 0,
                       font.MeasureString(score.ToString(System.Globalization.CultureInfo.InvariantCulture)) / 2, 2, SpriteEffects.None, 0);

            if (doneLerpingKillStreak)
            {
                spriteBatch.Draw(bestTitle, bestTitleRec, Color.White);

                spriteBatch.Draw(skullCounters[skullToDraw], skullRec, Color.White);

                Vector2 killStreakTextSize = font.MeasureString("x" + killStreak);
                spriteBatch.DrawString(font, "x" + killStreak, new Vector2(skullRec.Center.X + 30, skullRec.Center.Y), 
                    Color.White, 0, new Vector2(0, killStreakTextSize.Y/2), 2, SpriteEffects.None, 0);
            }

            if (drawStatus) {
                spriteBatch.Draw(statusTitle, statusTitleRec, Color.White);
                spriteBatch.Draw(skullStatus, skullStatusRec, Color.White);
            }

            if (doneLerpingScore && doneLerpingKillStreak && drawStatus) {
                //selectBack
                spriteBatch.Draw(selectBack, selectBackRec, Color.White);
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
                if (killStreak >= 50) {
                    skullToDraw = 3;
                    Stage.ActiveStage.GetQB<AudioQB>().PlaySound("Arail-attack2");
                }
                else if (killStreak >= 20)
                {
                    skullToDraw = 2;
                    Stage.ActiveStage.GetQB<AudioQB>().PlaySound("A-heavy");
                }
                else if (killStreak >= 5)
                {
                    skullToDraw = 1;
                    Stage.ActiveStage.GetQB<AudioQB>().PlaySound("A-heavy");
                }

                doneLerpingKillStreak = true;
                doneLerpingScore = false;
                bonusPoints = rm.KillStreakScore;
                //Stage.ActiveStage.GetQB<ParticleQB>().AddFloatingText(new Vector2(skullRec.Center.X, skullRec.Top), new Vector2(0, 10), 3f, "+" + bonusPoints);
        }

        private void CalculateStatus () {
            bonusPoints = rm.RockGodScore;
            if (bonusPoints > 0) {
                drawStatus = true;
                Stage.ActiveStage.GetQB<ParticleQB>().AddFloatingText(Vector2.Zero, new Vector2(0, 10), 3f, "+" + bonusPoints);
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("A-heavy");
            }
            doneLerpingScore = false;
            checkForPerfect = false; ;
        }
    }
}
