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
        private Texture2D continueBack;
        private Texture2D[] skullCounters;
        private Texture2D[] statusSkulls;

        private Rectangle skullRec;
        private Rectangle scoreRec;
        private Rectangle multiplier;
        private Rectangle scoreTitleRec;
        private Rectangle statusTitleRec;
        private Rectangle skullStatusRec;
        private Rectangle window;
        private Rectangle levelCompleteRec;
        private Rectangle bestTitleRec;
        private Rectangle continueBackRec;

        private int baseScore;
        private int score;
        private int bonusPoints;
        private int killStreak;
        private int skullToDraw;
        private int statusSkull;
        private int lerpRate;
        private int sound;

        private float timeOnLevel;
        private float timer;

        private bool doneLerpingScore; //may not need
        private bool doneLerpingKillStreak;
        private bool checkForPerfect;
        private bool drawStatus;
        private bool soundPlaying;

        private RockMeter rm;

        public EndLevelMenu (string title)
            : base(title) {
            
            Stage.ActiveStage.PauseGame();
            GetScoreData();

            timer = 0f;
            
            // save the high score
            Stage.SaveGame.AddHighScore(Stage.ActiveStage.Parm.GetString("AssetName"), rm.GetTotalScore);
            float percEnemiesOnKillStreak = 0;
            if (AI.AIQB.numEnemiesInLevel > 0)
                percEnemiesOnKillStreak = (float)rm.HighestKillStreak / AI.AIQB.numEnemiesInLevel;
            Stage.SaveGame.AddPercKillStreak(Stage.ActiveStage.Parm.GetString("AssetName"), percEnemiesOnKillStreak);
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
            int totScore = rm.GetTotalScore;
            if(totScore > lerpRate * 5)
            {
                lerpRate = totScore / 5;
            }
            skullCounters = new Texture2D[4];
            statusSkulls = new Texture2D[5];
            checkForPerfect = true;
        }

        public override void LoadContent () {
            base.LoadContent();
            int w, h;
            float scale = 0.8f;

            //load textures
            for (int i = 0; i < 4; i++)
                skullCounters[i] = Stage.Content.Load<Texture2D>("UI/HUD/SkullCounter" + (i + 1));
            for (int i = 0; i < 5; i++)
                statusSkulls[i] = Stage.Content.Load<Texture2D>("UI/EndLevel/skull" + (i + 1));

            levelComplete = Stage.Content.Load<Texture2D>("UI/EndLevel/levelComplete");
            scoreTitle = Stage.Content.Load<Texture2D>("UI/EndLevel/scoreTitle");
            bestTitle = Stage.Content.Load<Texture2D>("UI/EndLevel/bestTitle");
            statusTitle = Stage.Content.Load<Texture2D>("UI/EndLevel/statusTitle");
            continueBack = Stage.Content.Load<Texture2D>("UI/EndLevel/continueBack");

            window = Stage.renderer.GraphicsDevice.Viewport.Bounds;

            w = (int)(levelComplete.Width * scale);
            h = (int)(levelComplete.Height * scale);
            levelCompleteRec = new Rectangle(window.Left + w/2, window.Top + h, w, h);

            w = (int)(scoreTitle.Width * scale);
            h = (int)(scoreTitle.Height * scale);
            scoreTitleRec = new Rectangle(levelCompleteRec.X, levelCompleteRec.Y + (2*h), w, h);

            w = (int)(levelCompleteRec.Width);
            h = MenuSystem.Font.LineSpacing / 2;
            scoreRec = new Rectangle(scoreTitleRec.Center.X - scoreTitleRec.Width/2, scoreTitleRec.Bottom + h, w, h);

            multiplier = new Rectangle(scoreRec.Right, scoreTitleRec.Bottom + h, w, h);

            w = (int)(bestTitle.Width * scale);
            h = (int)(bestTitle.Height * scale);
            bestTitleRec = new Rectangle(scoreTitleRec.X, scoreRec.Bottom + (3*h), w, h);

            w = skullCounters[0].Width;
            h = skullCounters[0].Height;
            skullRec = new Rectangle(bestTitleRec.Center.X - w / 2, bestTitleRec.Bottom + h / 4, w, h);

            w = (int)(statusTitle.Width * scale);
            h = (int)(statusTitle.Height * scale);
            statusTitleRec = new Rectangle(levelCompleteRec.Right + h, scoreTitleRec.Y, w, h);

            w = (int)(statusSkulls[0].Width * scale);
            h = (int)(statusSkulls[0].Height * scale);
            skullStatusRec = new Rectangle(statusTitleRec.Center.X, scoreRec.Y, w, h);

            w = (int)(continueBack.Width * scale);
            h = (int)(continueBack.Height * scale);
            continueBackRec = new Rectangle(window.Right - (int)(1.5*w), window.Bottom -  (int)(1.5 *h), w, h);
        }

        void Continue (object sender, EventArgs e) {
            Stage.GameRunning = true;
            Stage.ActiveStage.ResumeGame();
            ExitScreen();
            if (Stage.ActiveStage.Parm.HasParm("NextLevel")) {
                LoadingScreen.Load(MenuSystem, true, Stage.ActiveStage.Parm.GetString("NextLevel"));
            } else {
                LoadingScreen.Load(MenuSystem, true, Stage.ActiveStage.Parm.GetString("MainMenu"));
            }
        }

        void QuitGame (object sender, EventArgs e) {
            Stage.GameRunning = true;
            Stage.ActiveStage.ResumeGame();
            LoadingScreen.Load(MenuSystem, true, Stage.ActiveStage.Parm.GetString("MainMenu"));
        }

        public override void HandleInput (MenuInput input) {
            if (!doneLerpingScore) {
                if (input.IsGreen() || input.IsRed()) {
                    score = baseScore + bonusPoints;
                    doneLerpingScore = true;
                    
                }
            } else if (doneLerpingScore) {
                if (input.IsGreen() || input.IsMenuSelect()) {
                    Continue(this, new EventArgs());
                } else if (input.IsRed() || input.IsMenuCancel()) {
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
                if(!soundPlaying){
                    sound = Stage.ActiveStage.GetQB<AudioQB>().PlaySoundInstance("banknote_counter_COUNTING", true, true);
                    soundPlaying = true;
                }
                    
                int rate = lerpRate;
                if ((baseScore + bonusPoints) > 900000)
                    rate = lerpRate * 100;

                Lerp(baseScore + bonusPoints, ref score, (int)Math.Ceiling(rate * dt));
            }          
            else if (!doneLerpingKillStreak){           
                TallySkulls();
            } else if (doneLerpingScore && doneLerpingKillStreak && checkForPerfect) {
                //Stage.ActiveStage.GetQB<AudioQB>().PlaySound("banknote-counterSTOPPING");
                CalculateStatus();
            }

            if (doneLerpingScore && soundPlaying) {
                Stage.ActiveStage.GetQB<AudioQB>().StopStound(sound);
                soundPlaying = false;
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

            if (drawStatus && doneLerpingScore) {
                spriteBatch.Draw(statusTitle, statusTitleRec, Color.White);
                spriteBatch.Draw(statusSkulls[statusSkull], skullStatusRec, Color.White);
            }

            if (doneLerpingScore && doneLerpingKillStreak && drawStatus) {
                //continueBack
                spriteBatch.Draw(continueBack, continueBackRec, Color.White);
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

        private void TallySkulls()
        {

            killStreak = rm.HighestKillStreak;
            if (killStreak >= 50)
            {
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

            float percEnemiesOnKillStreak = 0;
            
            if(AI.AIQB.numEnemiesInLevel > 0)
                percEnemiesOnKillStreak = (float)killStreak / AI.AIQB.numEnemiesInLevel;

            if (percEnemiesOnKillStreak < .25f)
                statusSkull = 0;
            else if (percEnemiesOnKillStreak < .5f)
                statusSkull = 1;
            else if (percEnemiesOnKillStreak < .75f)
                statusSkull = 2;
            else if (percEnemiesOnKillStreak < 1)
                statusSkull = 3;
            else
                statusSkull = 4;

            doneLerpingKillStreak = true;
            doneLerpingScore = false;
            bonusPoints = rm.KillStreakScore;
        }

        private void CalculateStatus () {
            bonusPoints = rm.RockGodScore;
            if (bonusPoints > 0) {
                drawStatus = true;
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("A-heavy");
            }
            doneLerpingScore = false;
            checkForPerfect = false; ;
        }
    }
}
