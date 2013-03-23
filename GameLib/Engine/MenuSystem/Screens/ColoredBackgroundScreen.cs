using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.MenuSystem.Screens {
    public class ColoredBackgroundScreen:GameScreen {
        Color color;

        public ColoredBackgroundScreen (Color color) {
            this.color = color;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw (float dt) {
            Rectangle fullscreen = new Rectangle(0, 0, Renderer.ScreenWidth, Renderer.ScreenHeight);

            Stage.renderer.SpriteBatch.Draw(MenuSystem.BlankTexture, fullscreen,
                             new Color(color.R*TransitionAlpha, color.B*TransitionAlpha, color.G*TransitionAlpha));
        }
    }
}
