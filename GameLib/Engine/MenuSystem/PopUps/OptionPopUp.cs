using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace GameLib.Engine.MenuSystem.Menus {

    public class OptionPopUp : PopUpScreen {

        int musicVol;
        int fxVol;
        bool strumMode;
        bool gore;
        bool moreGore;
        int origMusicVol;
        int origFXVol;
        bool origStrumMode;
        bool origGore;
        bool origMoreGore;

        private float[] knobAngles = new float[] { 5.272f, 4.729f, 4.229f, 3.816f, 3.381f, 2.881f, 2.424f, 2.055f, 1.562f, 1.007f, .543f, 0 };

        Texture2D selectBack;
        private Texture2D amp;
        private Texture2D knob;
        private Texture2D switcher;
        private Texture2D fxText;
        private Texture2D goreText;
        private Texture2D moreGoreText;
        private Texture2D musicText;
        private Texture2D strumText;

        private Rectangle ampDims;
        private Rectangle musicDialDims;
        private Rectangle fxDialDims;
        private Rectangle fxDims;
        private Rectangle goreDims;
        private Rectangle moreGoreDims;
        private Rectangle musicDims;
        private Rectangle goreSwitchDims;
        private Rectangle moreGoreSwitchDims;
        private Rectangle strumSwitchDims;
        private Rectangle strumDims;

        int index;

        public delegate void OptionFunc();

        FastList<OptionFunc> options;

        public OptionPopUp(string message)
            : this(message, true)
        {

            //load saved values here
            options = new FastList<OptionFunc>();
            options.Add(ChangeMusicVol);
            options.Add(ChangeFxVol);
            options.Add(ChangeGore);
            options.Add(ChangeMoreGore);
            options.Add(ChangeStrum);

            strumMode = Stage.SaveGame.getStrumMode();
            Stage.SaveGame.getGores(out gore, out moreGore);
            Stage.SaveGame.getVolumes(out musicVol, out fxVol);

            origFXVol = fxVol;
            origGore = gore;
            origMoreGore = moreGore;
            origMusicVol = musicVol;
            origStrumMode = strumMode;
        }

        public OptionPopUp (string message, bool includeUsageText)
            : base(message) {
                
        }

        public override void LoadContent () {
            base.LoadContent();
            knob = Stage.Content.Load<Texture2D>("UI/Options/OptionsKnob");
            amp = Stage.Content.Load<Texture2D>("UI/Options/Options_Amp");
            fxText = Stage.Content.Load<Texture2D>("UI/Options/EffectsVolume");
            goreText = Stage.Content.Load<Texture2D>("UI/Options/Gore");
            moreGoreText = Stage.Content.Load<Texture2D>("UI/Options/MoreGore");
            musicText = Stage.Content.Load<Texture2D>("UI/Options/MusicVolume");
            switcher = Stage.Content.Load<Texture2D>("UI/Options/OptionsSwitch");
            strumText = Stage.Content.Load<Texture2D>("UI/Options/StrumMode");
            selectBack = Stage.Content.Load<Texture2D>("UI/MainMenu/select_back");

            Rectangle screen = Renderer.ScreenRect;
            ampDims = new Rectangle(screen.Width / 8, screen.Height / 8, screen.Width * 3 / 4, screen.Height * 3 / 4);
            int dialDiam = (int)(.13f * ampDims.Width);
            musicDialDims = new Rectangle(ampDims.X + (int)(.19f * ampDims.Width),
                                      ampDims.Y + (int)(.685f * ampDims.Height),
                                      dialDiam,
                                      dialDiam);
            fxDialDims = new Rectangle(ampDims.X + (int)(.34f * ampDims.Width),
                                      ampDims.Y + (int)(.685f * ampDims.Height),
                                      dialDiam,
                                      dialDiam);
            fxDims = new Rectangle(ampDims.X + (int)(.28f * ampDims.Width),
                                      ampDims.Y + (int)(.81f * ampDims.Height),
                                      (int)(.12f * ampDims.Width),
                                      (int)(.03f * ampDims.Height));
            goreDims = new Rectangle(ampDims.X + (int)(.5f * ampDims.Width),
                                      ampDims.Y + (int)(.81f * ampDims.Height),
                                      (int)(.05f * ampDims.Width),
                                      (int)(.03f * ampDims.Height));
            moreGoreDims = new Rectangle(ampDims.X + (int)(.59f * ampDims.Width),
                                      ampDims.Y + (int)(.81f * ampDims.Height),
                                      (int)(.08f * ampDims.Width),
                                      (int)(.03f * ampDims.Height));
            musicDims = new Rectangle(ampDims.X + (int)(.14f * ampDims.Width),
                                      ampDims.Y + (int)(.81f * ampDims.Height),
                                      (int)(.11f * ampDims.Width),
                                      (int)(.03f * ampDims.Height));
            goreSwitchDims = new Rectangle(ampDims.X + (int)(.525f * ampDims.Width),
                                      ampDims.Y + (int)(.69f * ampDims.Height),
                                      (int)(.06f * ampDims.Width),
                                      (int)(.1f * ampDims.Height));
            moreGoreSwitchDims = new Rectangle(ampDims.X + (int)(.63f * ampDims.Width),
                                      ampDims.Y + (int)(.69f * ampDims.Height),
                                      (int)(.06f * ampDims.Width),
                                      (int)(.1f * ampDims.Height));
            strumSwitchDims = new Rectangle(ampDims.X + (int)(.805f * ampDims.Width),
                                      ampDims.Y + (int)(.69f * ampDims.Height),
                                      (int)(.06f * ampDims.Width),
                                      (int)(.1f * ampDims.Height));
            strumDims = new Rectangle(ampDims.X + (int)(.75f * ampDims.Width),
                                      ampDims.Y + (int)(.81f * ampDims.Height),
                                      (int)(.1f * ampDims.Width),
                                      (int)(.03f * ampDims.Height));
        }

        protected override void OnCancel()
        {

            //save stuff only if we need to
            if (strumMode != origStrumMode ||
                musicVol != origMusicVol ||
                fxVol != origFXVol ||
                gore != origGore ||
                moreGore != origGore)
            {
                Stage.SaveGame.StoreOptionData(musicVol, fxVol, gore, moreGore, strumMode);
                Stage.SaveGame.SaveGameData();
                //todo: reload necessary game variables here i.e. sound volumes for audioQB and strum mode for player agent
            }
            ExitScreen();
        }

        public override void Draw(float dt)
        {
            base.Draw(dt);
            Stage.renderer.SpriteBatch.Draw(amp, ampDims, Color.White);

            if (index == 0)
                Stage.renderer.SpriteBatch.Draw(musicText, musicDims, Color.Yellow);
            else
                Stage.renderer.SpriteBatch.Draw(musicText, musicDims, Color.White);

            if(index == 1)
                Stage.renderer.SpriteBatch.Draw(fxText, fxDims, Color.Yellow);
            else
                Stage.renderer.SpriteBatch.Draw(fxText, fxDims, Color.White);

            if(index == 2)
                Stage.renderer.SpriteBatch.Draw(goreText, goreDims, Color.Yellow);
            else
                Stage.renderer.SpriteBatch.Draw(goreText, goreDims, Color.White);

            if(index == 3)
                Stage.renderer.SpriteBatch.Draw(moreGoreText, moreGoreDims, Color.Yellow);
            else
                Stage.renderer.SpriteBatch.Draw(moreGoreText, moreGoreDims, Color.White);

            if(index == 4)
                Stage.renderer.SpriteBatch.Draw(strumText, strumDims, Color.Yellow);
            else
                Stage.renderer.SpriteBatch.Draw(strumText, strumDims, Color.White);

            Vector2 knobMid = new Vector2(knob.Width * .5f, knob.Height * .5f);
            Stage.renderer.SpriteBatch.Draw(knob, musicDialDims, null, Color.White, knobAngles[musicVol], knobMid, SpriteEffects.None, 0);
            Stage.renderer.SpriteBatch.Draw(knob, fxDialDims, null, Color.White, knobAngles[fxVol], knobMid, SpriteEffects.None, 0);

            Vector2 switcherMid = new Vector2(switcher.Width * .5f, switcher.Height * .415f);
            Stage.renderer.SpriteBatch.Draw(switcher, strumSwitchDims, null, Color.White, (!strumMode) ? 0 : (float)Math.PI,
                switcherMid, SpriteEffects.None, 0);
            Stage.renderer.SpriteBatch.Draw(switcher, moreGoreSwitchDims, null, Color.White, (!moreGore) ? 0 : (float)Math.PI,
                switcherMid, SpriteEffects.None, 0);
            Stage.renderer.SpriteBatch.Draw(switcher, goreSwitchDims, null, Color.White, (!gore) ? 0 : (float)Math.PI,
                switcherMid, SpriteEffects.None, 0);

            int width = (int)(Renderer.ScreenWidth * 0.8);
            int height = (int)(Renderer.ScreenHeight * 0.8);
            Rectangle rec = Stage.renderer.GraphicsDevice.Viewport.Bounds;
            Rectangle mainImageRec = new Rectangle((rec.Center.X + 150) - width / 2, rec.Center.Y - height / 2, width, height);

            width = (int)(selectBack.Width * 0.7f);
            height = (int)(selectBack.Height * 0.7f);
            Rectangle selectBackRec = new Rectangle(rec.Right - (int)(1.65 * width), rec.Bottom - (int)(1.65 * height), width, height);

            Stage.renderer.SpriteBatch.Draw(selectBack, selectBackRec, Color.White);
        }

        public override void HandleInput(MenuInput input)
        {
            // Move to the previous menu entry?
            if (input.IsMenuLeft() || input.IsMenuUp())
            {
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("knob-click-1");
                index--;
                if (index < 0)
                    index = options.Count - 1;
            }

            // Move to the next menu entry?
            if (input.IsMenuRight() || input.IsMenuDown())
            {
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("knob-click-1");
                index++;
                if (index >= options.Count)
                    index = 0;
            }

            if (input.IsMenuSelect())
            {
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("MeatSlap2_16");
                options.Data[index]();
            }
            else if (input.IsMenuCancel())
            {
                OnCancel();
            }
        }

        private void ChangeMusicVol()
        {
            musicVol++;
            if (musicVol > 11) musicVol = 0;
        }

        private void ChangeFxVol()
        {
            fxVol++;
            if (fxVol > 11) fxVol = 0;
        }

        private void ChangeGore()
        {
            gore = !gore;
        }

        private void ChangeMoreGore()
        {
            moreGore = !moreGore;
        }

        private void ChangeStrum()
        {
            if(strumMode)
                MenuSystem.AddScreen(new StrumWarningPopUp(""));
            strumMode = !strumMode;
        }
        
    }
}
