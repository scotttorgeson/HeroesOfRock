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

        IAsyncSaveDevice saveDevice;

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

        //public SaveGame saveGame = new SaveGame();

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
            
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            // create the save device
            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
            Components.Add(sharedSaveDevice);
            saveDevice = sharedSaveDevice;

            // create event hanlders that force the user to choose a new device
            // if they cancel the device selector, or it they disconnect the storage
            // device after selecting it
            sharedSaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            // prompt for a device on the first update we can
            sharedSaveDevice.PromptForDevice();

#if XBOX
            Components.Add(new GamerServicesComponent(this));
#endif

            saveDevice.SaveCompleted += new SaveCompletedEventHandler(saveDevice_SaveCompleted);
            saveDevice.LoadCompleted += new LoadCompletedEventHandler(saveDevice_LoadCompleted);


            Stage.renderer = new Renderer(graphics.GraphicsDevice);
            Stage.renderer.Initialize();

            base.Initialize();
        }

        void saveDevice_LoadCompleted(object sender, FileActionCompletedEventArgs args)
        {
            SaveDataLoaded = true;
            System.Diagnostics.Debug.WriteLine("Load completed!");
        }

        void saveDevice_SaveCompleted(object sender, FileActionCompletedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Save completed!");
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

        bool saveGameRequested = false;
        public static void SaveGame()
        {
            HORGame.saveGameRequested = true;
        }

        public static Dictionary<string, int> highscores = new Dictionary<string, int>();
        public static int levelsUnlocked = 1;

        public static void AddHighScore(string level, int score)
        {
            if (highscores.ContainsKey(level))
            {
                if (highscores[level] < score)
                    highscores[level] = score;
            }
            else
                highscores[level] = score;
        }

        private void CheckSaveGame()
        {
            if (saveGameRequested)
            {
                if (saveDevice.IsReady)
                {
                    saveDevice.SaveAsync("HeroesOfRock", "HeroesOfRockSave.txt", stream =>
                    {
                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream))
                        {
                            writer.WriteLine(levelsUnlocked);
                            foreach (KeyValuePair<string, int> kvp in highscores)
                            {
                                writer.WriteLine(kvp.Key);
                                writer.WriteLine(kvp.Value);
                            }
                        }
                    });
                    saveGameRequested = false;
                }
            }
        }

        public static bool SaveDataLoaded { get; private set; }

        public static void LoadGame()
        {
            if (!SaveDataLoaded)
                loadGameRequested = true;
        }
        private static bool loadGameRequested = false;

        private void CheckLoadGame()
        {
            if (loadGameRequested && !SaveDataLoaded)
            {
                if (saveDevice.IsReady)
                {
                    if (saveDevice.FileExists("HeroesOfRock", "HeroesOfRockSave.txt"))
                    {
                        saveDevice.LoadAsync("HeroesOfRock", "HeroesOfRockSave.txt", stream =>
                            {
                                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                                {
                                    levelsUnlocked = int.Parse(reader.ReadLine());

                                    while (!reader.EndOfStream)
                                    {
                                        string key = reader.ReadLine();
                                        int value = int.Parse(reader.ReadLine());
                                        AddHighScore(key, value);
                                    }
                                }
                            });

                        loadGameRequested = false;
                    }
                    else
                    {
                        SaveDataLoaded = true;
                    }
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            CheckLoadGame();
            CheckSaveGame();

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
