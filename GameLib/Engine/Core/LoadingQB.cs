using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class LoadingQB : Quarterback
    {
        public override string Name()
        {
            return "LoadingQB";
        }

        public static string LoadLevel;

        private bool loaded = false;

        private SpriteFont spriteFont;
        private Texture2D textBackground;
        private InputAction finishLoad;

        public override void LoadContent()
        {
            spriteFont = Stage.Content.Load<Microsoft.Xna.Framework.Graphics.SpriteFont>("DefaultFont");
            textBackground = Stage.Content.Load<Texture2D>("UI/Menu/blank");
            ControlsQB cqb = Stage.ActiveStage.GetQB<ControlsQB>();
            finishLoad = cqb.GetInputAction("FinishLoad");
        }

        public override void PauseQB()
        {
            //unpausable
        }

        public override void Update(float dt)
        {
            if (!loaded)
            {
                Stage.WaitOnLoadFinish = true;
                Stage.LoadStage(LoadLevel, false);
                loaded = true;
            }

            if (Stage.DoneLoading)
            {                
                if (finishLoad.IsNewAction)
                    Stage.WaitOnLoadFinish = false;
            }
        }

        public override void DrawUI(float dt)
        {
            if (Stage.DoneLoading)
            {
                Renderer.Instance.SpriteBatch.Draw(textBackground, new Microsoft.Xna.Framework.Rectangle(20, 50, 550, 80), Microsoft.Xna.Framework.Color.Black);
                Renderer.Instance.SpriteBatch.DrawString(spriteFont, "Loading screen\nPratice your attacks on the enemy\nPress start or enter to continue to the level...", new Microsoft.Xna.Framework.Vector2(20, 50), Microsoft.Xna.Framework.Color.White);
            }
        }
    }
}