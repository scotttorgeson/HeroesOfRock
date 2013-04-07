using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BEPUphysics;
//using BEPUphysicsDrawer;
using BEPUphysics.Collidables;
using GameLib;
using GameLib.Engine.MenuSystem;
using GameLib.Engine.MenuSystem.Menus;
using EasyStorage;

//  svn://lenny.eng.utah.edu/home/torgeson/HeroesOfRock

namespace HeroesOfRock
{
    public class HeroesOfRockGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        public HeroesOfRockGame()
        {
            HORGame = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
#if !EDITOR
            IsMouseVisible = false;
#endif
//#if XBOX360
//            this.Components.Add(new GamerServicesComponent(this));
//#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
//#if WINDOWS
            IsFixedTimeStep = false; //allows a variable frame rate
            graphics.SynchronizeWithVerticalRetrace = false; // turns off vsync
//#endif

            graphics.PreferMultiSampling = true;

            graphics.PreferredBackBufferWidth = Renderer.ScreenWidth;
            graphics.PreferredBackBufferHeight = Renderer.ScreenHeight;

#if !DEBUG && WINDOWS
            graphics.IsFullScreen = true;
#endif
            graphics.ApplyChanges();

            Stage.InitSaveGame(this);

            Stage.renderer = new Renderer(graphics.GraphicsDevice);
            Stage.renderer.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Stage.Content = new ThreadSafeContentManager(Content.ServiceProvider, Content.RootDirectory);

            Stage.LoadStartingStage("StartGame");
            Stage.renderer.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private static HeroesOfRockGame HORGame;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Stage.UpdateSaveGame();

#if XBOX360
            if((!Guide.IsVisible))
            {
#endif
                if (Stage.ActiveStage != null)
                {
                    if (Stage.QuitGame)
                    {
#if DEBUG
                        Stage.ActiveStage.GetQB<ControlsQB>().Exiting();
#endif
                        this.Exit();
                    }

                    if (Stage.GameRunning)
                        Stage.ActiveStage.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
#if XBOX360
            }
#endif
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (Stage.ActiveStage != null)
            {
                //if (Stage.GameRunning)
                    //hud.Draw(gameTime);

                Stage.renderer.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);

                if (Stage.ActiveStage != null)
                    Stage.ActiveStage.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (HeroesOfRockGame game = new HeroesOfRockGame())
            {
                game.Run();
            }
        }
    }
}
