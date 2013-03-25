using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLib.Engine.MenuSystem.Menus;
using GameLib.Engine.MenuSystem.HUD.HudComponents;
using GameLib.Engine.AttackSystem;
using Microsoft.Xna.Framework.Input;

namespace GameLib.Engine.MenuSystem {
    public class HudScreen : GameScreen {
        private Texture2D timeBG;
        private Texture2D scoreBG;
        private Texture2D skull1;
        private Texture2D skull2;
        private Texture2D skull3;
        private Texture2D skull4;

        private Rectangle healthRec;
        private Rectangle timeRec;
        private Rectangle scoreRec;
        private Rectangle fullScreenRec;
        private Rectangle crowdRec;
        private Rectangle skullRec;
        //private Rectangle skullRec2;
        //private Rectangle skullRec3;
        //private Rectangle skullRec4;
        private HealthKnob health;

        private Texture2D crowd;
        private SpriteFont font;
        private float time;
        private float crowdOffset;
        private bool crowdOn;

        public HudScreen () {
            fullScreenRec = Renderer.ScreenRect;

            this.time = 0f;
            this.crowdOffset = fullScreenRec.Bottom;
            this.crowdOn = false;
        }

        public override void LoadContent () {
            font = Stage.Content.Load<SpriteFont>("belligerent");
            crowd = Stage.Content.Load<Texture2D>("UI/HUD/crowd");
            timeBG = Stage.Content.Load<Texture2D>("UI/HUD/time");
            scoreBG = Stage.Content.Load<Texture2D>("UI/HUD/score"); 
            skull1 = Stage.Content.Load<Texture2D>("UI/HUD/SkullCounter1");
            skull2 = Stage.Content.Load<Texture2D>("UI/HUD/SkullCounter2");
            skull3 = Stage.Content.Load<Texture2D>("UI/HUD/SkullCounter3");
            skull4 = Stage.Content.Load<Texture2D>("UI/HUD/SkullCounter4");

            int w = fullScreenRec.Width;
            int h = fullScreenRec.Height;
            float crowdScale = (float)fullScreenRec.Width / crowd.Width;

            int height = h / 5;
            healthRec = new Rectangle(fullScreenRec.Left, fullScreenRec.Top, w / 6, h / 3);
            scoreRec = new Rectangle(fullScreenRec.Right - (int)(1.1f * height), fullScreenRec.Top, (int)(1.1f * height), height);
            skullRec = new Rectangle(fullScreenRec.Center.X - (int)(.5f * 1.32f * height), fullScreenRec.Top, (int)(1.32f * height), height);
            timeRec = scoreRec;
            timeRec.X -= (int)(1.32f * height);
            timeRec.Width = (int)(1.32f * height);
            crowdRec = new Rectangle(fullScreenRec.X, fullScreenRec.Bottom, (int)(crowd.Width * crowdScale), (int)(crowd.Height * crowdScale));                              
              
            health = new HealthKnob(healthRec);
            health.LoadContent();
        }

        public override void Draw (float dt) {
            if (PlayerAgent.Player != null) {
                base.Draw(dt);
                //draw time TODO FIX TO REFLECT ACTUAL TIME; 
                String timeString = FormatTime(time+=dt);
                Vector2 timePos = new Vector2(timeRec.Center.X , timeRec.Center.Y);
                Stage.renderer.SpriteBatch.Draw(timeBG, timeRec, Color.White);
                Stage.renderer.SpriteBatch.DrawString(font, timeString, timePos, Color.White);

                //draw health
                health.Draw();

                RockMeter rockMeter = PlayerAgent.Player.GetAgent<RockMeter>();
                //draw score
                String score = rockMeter.Score.ToString();
                Vector2 scorePos = new Vector2(scoreRec.Center.X - (font.MeasureString(score).Length() / 2), scoreRec.Center.Y);
                Stage.renderer.SpriteBatch.Draw(scoreBG, scoreRec, Color.White);               
                Stage.renderer.SpriteBatch.DrawString(font, score, scorePos, Color.White);

                //draw skulls
                String kills = String.Format("x{0}", rockMeter.KillStreak);
                Vector2 size = font.MeasureString(kills);
                if (rockMeter.KillStreak >= 50)
                {
                    Stage.renderer.SpriteBatch.Draw(skull4, skullRec, Color.White);
                }
                else if (rockMeter.KillStreak >= 20)
                {
                    Stage.renderer.SpriteBatch.Draw(skull3, skullRec, Color.White);
                }
                else if (rockMeter.KillStreak >= 5)
                {   
                    Stage.renderer.SpriteBatch.Draw(skull2, skullRec, Color.White);
                }
                else
                {
                    Stage.renderer.SpriteBatch.Draw(skull1, skullRec, Color.White);
                }

                Stage.renderer.SpriteBatch.DrawString(font, kills, new Vector2(skullRec.Left + skullRec.Width * .6f, skullRec.Center.Y - size.Y * .45f), Color.White);

                //draw crowd
                //Stage.renderer.SpriteBatch.Draw(crowd, new Rectangle(fullScreenRec.Center.X, (int)crowdOffset + new Random().Next(-2,2), crowd.Width / 2, crowd.Height / 2), Color.White * 0.75f);
                Stage.renderer.SpriteBatch.Draw(crowd, new Rectangle(crowdRec.X, (int)crowdOffset + new Random().Next(-2, 2), crowdRec.Width, crowdRec.Height), Color.White * 0.75f);              
            }
        }

        private string FormatTime (float p) {
            int minutes = (int)p/60;
            int seconds = (int)p%60;

            return string.Format("{0}:{1}", minutes.ToString("D2"), seconds.ToString("D2"));
        }

        public override void HandleInput (MenuInput input) {
            base.HandleInput(input);
            if (input.IsPauseGame()) {
                MenuSystem.AddScreen(new PauseMenu("Pause Menu"));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Home)) {
                MenuSystem.AddScreen(new EndLevelMenu("Level Complete"));
            }
        }

        public override void Update (float dt, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(dt, otherScreenHasFocus, coveredByOtherScreen);
            health.Update(dt); 

            //Crowd Transitions
            if (PlayerAgent.Player.GetAgent<RockMeter>().RockLevel > 10)
                crowdOn = true;
            else
                crowdOn = false;
            if (crowdOn && crowdOffset > fullScreenRec.Bottom-(int)(2 * crowdRec.Height) / 5)
                crowdOffset -= 15f;
            else if (!crowdOn && crowdOffset < fullScreenRec.Bottom)
                crowdOffset += 15f;            
        }

    }
}

