#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameLib.Engine.MenuSystem {

	public class MenuSystem {

        LinkedList<GameScreen> menus = new LinkedList<GameScreen>();

        public bool CanPause()
        {
            foreach (GameScreen screen in menus)
                if (!screen.CanPause)
                    return false;
            return true;
        }

        public int MenuCount
        {
            get { return menus.Count; }
        }
		
		SpriteFont font;
		Texture2D blankTexture;

		public Texture2D BlankTexture {
			get { return blankTexture; }
		}

		MenuInput input;

		public MenuInput Input {
			get { return input; }
			set { input = value; }
		}

		bool isInitialized;

		public SpriteFont Font {
			get { return font; }
		}

		public MenuSystem (ControlsQB controls) {
			Input = new MenuInput(controls);
		}

		public void Initialize()
		{
			isInitialized = true;
		}

		

		/// <summary>
		/// Load your graphics content.
		/// </summary>
		public void LoadContent(ContentManager content)
		{
			//spriteBatch = new SpriteBatch(Renderer.Instance.GraphicsDevice);
			font = content.Load<SpriteFont>("belligerent");
			blankTexture = content.Load<Texture2D>("UI/Menu/blank");

			foreach (GameScreen screen in menus)
			{
				screen.LoadContent();
			}
		}

		public void PostLoadInit(ParameterSet parm)
		{
			input.PostLoadInit(parm);
		}

		public void Update(float dt)
		{
            LinkedListNode<GameScreen> node;

            node = menus.Last;
            if (node == null)
                return;

            bool otherScreenHasFocus = false;//Stage.IsPaused;
            bool coveredByOtherScreen = false;

            do
            {
                if (node.Value.MarkedForRemove)
                {
                    LinkedListNode<GameScreen> temp = node;
                    node = node.Next;
                    menus.Remove(temp);
                }
                else
                {
                    // Update the menu.
                    node.Value.Update(dt, otherScreenHasFocus, coveredByOtherScreen);

                    if (node.Value.MenuState == MenuState.TransitionOn ||
                        node.Value.MenuState == MenuState.Active)
                    {
                        // If this is the first active menu we came across,
                        // give it a chance to handle input.
                        if (!otherScreenHasFocus)
                        {
                            node.Value.HandleInput(input);

                            otherScreenHasFocus = true;
                        }

                        // If this is an active non-popup, inform any subsequent
                        // menus that they are covered by it.
                        if (!node.Value.IsPopup)
                            coveredByOtherScreen = true;
                    }
                }


                if (node != null)
                    node = node.Previous;

            } while (node != null);

		}

		public void Draw(float dt)
		{
			
			foreach (GameScreen screen in menus)
			{
				if (screen.MenuState != MenuState.Hidden)
				    screen.Draw(dt);
			}
		}


		/// <summary>
		/// Adds a new menu to the menu manager.
		/// </summary>
		public void AddScreen(GameScreen screen)
		{
			screen.MenuSystem = this;
			screen.IsExiting = false;

			// If we have a graphics device, tell the menu to load content.
			if (isInitialized)
			{
				screen.LoadContent();
			}

			menus.AddLast(screen);
		}

        public void ExitAll()
        {
            foreach (GameScreen g in menus)
                g.ExitScreen();
        }


		/// <summary>
		/// Helper draws a translucent black fullscreen sprite, used for fading
		/// menus in and out, and for darkening the background behind popups.
		/// </summary>
		public void FadeBackBufferToBlack(float alpha)
		{
			Stage.renderer.SpriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, Renderer.ScreenWidth, Renderer.ScreenHeight),
							 Color.Black * alpha);
		}

	}
}
