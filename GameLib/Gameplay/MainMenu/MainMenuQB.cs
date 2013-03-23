using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    class MainMenuQB : Quarterback
    {
        List<string> levels = new List<string>();

        SpriteFont font;

        int index = 0;

        public override string Name()
        {
            return "MainMenuQB";
        }

        public MainMenuQB()
        {

        }

        public override void LoadContent()
        {
            font = Stage.Content.Load<SpriteFont>("DefaultFont");
#if WINDOWS
            foreach (string world in System.IO.Directory.EnumerateFiles("Content/Worlds"))
            {                
                levels.Add(Path.GetFileNameWithoutExtension(world));
            }
#else
            levels.Add("AlphaLevel");
            levels.Add("HORLevel1");
            levels.Add("RockLevelOne");
#endif
        }

        public override void Update(float dt)
        {
            if (IsPaused) return;
            ControlsQB cqb = Stage.ActiveStage.GetQB<ControlsQB>();
#if WINDOWS
            if (cqb.CurrentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) && cqb.LastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Down))
                index++;
            if (cqb.CurrentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) && cqb.LastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Up))
                index--;
#endif
            if (cqb.CurrentGamePadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadDown) && cqb.LastGamePadState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.DPadDown))
                index++;
            if (cqb.CurrentGamePadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadUp) && cqb.LastGamePadState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.DPadUp))
                index--;

            if (index < 0)
                index = levels.Count - 1;
            else if (index >= levels.Count)
                index = 0;

#if WINDOWS
            if (cqb.CurrentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter) && cqb.LastKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                LoadingQB.LoadLevel = levels[index];
                Stage.LoadStage("LoadingLevel", false);
            }
#endif
            if (cqb.CurrentGamePadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) && cqb.LastGamePadState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.A))
            {
                LoadingQB.LoadLevel = levels[index];
                Stage.LoadStage("LoadingLevel", false);
            }
        }

        public override void DrawUI(float dt)
        {
            Vector2 position = new Vector2(20, 20);
            for (int i = 0; i < levels.Count; i++)
            {
                Renderer.Instance.SpriteBatch.DrawString(font, levels[i], position, i == index ? Color.Yellow : Color.White);
                position.Y += 20.0f;
            }
        }
    }
}
