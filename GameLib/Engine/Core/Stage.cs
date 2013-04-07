#define DEFERRED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GameLib.Engine.AttackSystem;
using System.Threading;
using GameLib.Engine.MenuSystem;
using Microsoft.Xna.Framework.Input;

namespace GameLib
{
    public class Stage
    {
#if DEFERRED
        public static Renderer renderer;
#else
        public static BasicRenderer renderer;
#endif
        public ParameterSet Parm;
        
        private static Stage activeStage;

        private static PlayerIndex playerIndex;

        public static Stage ActiveStage { get { return activeStage; } }

        public static Rectangle WindowSize { get; set; }

        public static ThreadSafeContentManager Content;

        public static bool GameRunning = false;
        public static bool Editor = false;

        public float Time { get; private set; }
        public static float GlobalTime { get; private set; }

        public static PlayerIndex PlayerIndex
        { 
            get { return playerIndex; } 
            set { playerIndex = value; }
        }

        public static void LoadStage(string file, bool blockingLoad)
        {
            if (!GlobalGameParms.initialized)
            {
                GlobalGameParms.GameParms = Content.Load<ParameterSet>("GameVariables");
                GlobalGameParms.initialized = true;
            }

            LoadStage( Content.Load<ParameterSet>("Worlds/" + file), blockingLoad );
        }

        public static bool DoneLoading { get; private set; }        
        private static Stage loadingStage = null;        
        public static Stage LoadingStage { get { return loadingStage; } }
        public static bool WaitOnLoadFinish = false;

        public static bool DoLoadStage = false;
        public static ParameterSet LoadStageParm = null;

        public static void LoadStage(ParameterSet parm, bool blockingLoad)
        {
            DoLoadStage = true;
            LoadStageParm = parm;
        }

        private static void LoadStage()
        {
            // not supporting async loads right now. commented out all the async load stuff.
            //if (loadingStage != null) // don't load more than one stage at a time...
            //    return;

            //if (blockingLoad)
            //{
                if (activeStage != null)
                {
                    foreach (Quarterback qb in activeStage.QBTable.Values)
                        qb.KillInstance();
                    activeStage.QBTable = null;
                    activeStage = null;
                }

                // unload content on blocking loads
                Renderer.Instance.ClearRModelInstances();
                Renderer.Instance.UnloadContent();
                Stage.Content.Unload();
                Renderer.Instance.LoadContent();

                loadingStage = new Stage(LoadStageParm);
                activeStage = loadingStage;
                loadingStage.Initialize();
                loadingStage.LoadContent();
                
                loadingStage = null;

                foreach (Quarterback qb in activeStage.QBTable.Values)
                    qb.LevelLoaded();

                GC.Collect();

                GameRunning = true;
                DoLoadStage = false;
            //}
            //else
            //{
            //    System.Threading.Thread thread = new Thread((_) =>
            //        {
            //            loadingStage = new Stage(parm);
            //            loadingStage.Initialize();
            //            loadingStage.LoadContent();
            //            Stage.DoneLoading = true;

            //            //if (activeStage == null)
            //            //{
            //            //    activeStage = loadingStage;
            //            //    loadingStage = null;
            //            //    DoneLoading = false;
            //            //    GameRunning = true;
            //            //}
            //        });

            //    thread.Name = "ResourceThread";
            //    thread.Start();
            //}
        }

        public static void ReloadStage(bool blockingLoad)
        {
            LoadStage(ActiveStage.Parm, blockingLoad);
        }

        public static void LoadStartingStage(string stageName)
        {
            LoadStage(stageName, true);
            LoadStage();
        }

        public static void LoadStartingStage(ParameterSet parms)
        {
            LoadStage(parms, true);
            LoadStage();
        }

        public static void EditorLoadStage(ParameterSet parms)
        {
            LoadStage(parms, true);
            LoadStage();
        }

        public static void EditorReloadStage()
        {
            LoadStage(activeStage.Parm, true);
            LoadStage();
        }

        private Stage(ParameterSet parm)
        {
            DoneLoading = false;
            this.Parm = parm;
            Time = 0.0f;
        }

        // called the first time a stage is created
        static Stage()
        {
            GlobalTime = 0.0f;
        }

        public void Initialize()
        {
#if DEBUG
            if ( Renderer.Instance.collisionDebugDrawer != null )
                Renderer.Instance.collisionDebugDrawer.Clear(); // clear out old stuff, just in case
#endif

            Renderer.Instance.sun = new Sun(Parm);
            if (Parm.HasParm("SkyColor"))
                Renderer.Instance.skyColor = new Color(Parm.GetVector4("SkyColor"));

            string[] quarterbackNames = Parm.GetString("Quarterbacks").Split(',');
            foreach (string quarterbackName in quarterbackNames)
            {                
                switch (quarterbackName)
                {
                    case "PhysicsQB":
                        AddQB<PhysicsQB>();
                        break;
                    case "ControlsQB":
                        AddQB<ControlsQB>();
                        break;
                    case "ActorQB":
                        AddQB<ActorQB>();
                        break;
                    case "CameraQB":
                        AddQB<CameraQB>();
                        break;
                    case "DebugQB":
                        AddQB<DebugQB>();
                        break;
                    case "AudioQB":
                        AddQB<AudioQB>();
                        break;
                    case "MainMenuQB":
                        AddQB<MainMenuQB>();
                        break;
                    case "TriggerQB":
                        AddQB<TriggerQB>();
                        break;
                    case "ParticleQB":
                        AddQB<Engine.Particles.ParticleQB>();
                        break;
                    case "PlayerAttackSystemQB":
                        AddQB<PlayerAttackSystemQB>();
                        break;
                    case "LoadingQB":
                        AddQB<LoadingQB>();
                        break;
                    case "MenuSystemQB":
                        AddQB<MenuSystemQB>();
                        break;
                    case "AIQB":
                        AddQB<Engine.AI.AIQB>();
                        break;
                    case "TempoQB":
                        AddQB<TempoQB>();
                        break;
                    case "DecalQB":
                        AddQB<Engine.Decals.DecalQB>();
                        break;
                    case "BloodSplatterQB":
                        AddQB<Gameplay.BloodSplatterQB>();
                        break;
                }
            }

            foreach (Quarterback qb in QBTable.Values)
                qb.PreLoadInit(Parm);
        }

        public void LoadContent()
        {
            foreach (Quarterback qb in QBTable.Values)
                qb.LoadContent();

            foreach (Quarterback qb in QBTable.Values)
                qb.PostLoadInit(Parm);
            
            if (Parm.HasParm("Raining") && Parm.GetBool("Raining"))
            {
                GetQB<Engine.Particles.ParticleQB>().AddParticleEmitter(PlayerAgent.Player, new Vector3(0, 25, 0), false, -1, 40, 48,
                                                1.0f, 1.25f, 0, 0, new Vector2(5.0f,10.0f), new Vector2(10.0f,20.0f), 
                                                new Vector3(25, 2, 25), 50*Vector3.Down, Vector3.Zero, "rain");
            }
        }

        //bool FinishLoad = false;

        public void Update(float dt)
        {
            Time += dt;
            GlobalTime += dt;

            //if (FinishLoad)
            //{
            //    foreach (Quarterback qb in activeStage.QBTable.Values)
            //        qb.KillInstance();

            //    activeStage = loadingStage;
            //    loadingStage = null;
            //    DoneLoading = false;
            //    FinishLoad = false;

            //    foreach (Quarterback qb in activeStage.QBTable.Values)
            //        qb.LevelLoaded();

            //    GC.Collect();

            //    return;
            //}

            //if (!WaitOnLoadFinish && DoneLoading)
            //{
            //    FinishLoad = true;                
            //}

            

            Renderer.Instance.UpdateStart();

            foreach(Quarterback qb in QBTable.Values)
                qb.Update(dt);

            Renderer.Instance.UpdateEnd();

#if DEBUG && DRAW_MODEL_BOUNDING_SPHERES
            if (activeStage.QBTable != null && activeStage.QBTable.ContainsKey(typeof(ControlsQB)))
            {
                if (activeStage.GetQB<ControlsQB>().CurrentKeyboardState.IsKeyDown(Keys.B) && activeStage.GetQB<ControlsQB>().LastKeyboardState.IsKeyUp(Keys.B))
                {
                    foreach (RModelInstance rmodelInstance in Renderer.Instance.RModelInstances)
                    {
                        var e = new BEPUphysics.Entities.Entity(new BEPUphysics.CollisionShapes.ConvexShapes.SphereShape(rmodelInstance.model.boundingSphere.Radius));
                        e.Position = rmodelInstance.physicsObject.Position + rmodelInstance.model.boundingSphere.Center;
                        e.Orientation = rmodelInstance.physicsObject.Orientation;
                        Renderer.Instance.collisionDebugDrawer.Add(e);
                    }
                }
            }
#endif

            if (DoLoadStage)
                LoadStage();
        }

        public void PauseGame()
        {
            foreach (Quarterback qb in QBTable.Values)
                qb.PauseQB();
        }

        public void ResumeGame()
        {
            foreach (Quarterback qb in QBTable.Values)
                qb.UnPauseQB();
        }

        public void Draw(float dt)
        {
            // all 3d models handled by renderer, don't try to draw them here
            renderer.SpriteBatch.Begin();

            foreach (Quarterback qb in QBTable.Values)
                qb.DrawUI(dt);

            renderer.SpriteBatch.End();
        }

        public Dictionary<Type, Quarterback> QBTable = new Dictionary<Type,Quarterback>();

        public T GetQB<T>() where T : Quarterback
        {
            return QBTable[typeof(T)] as T;
        }

        void AddQB<T>() where T : Quarterback, new()
        {
            System.Diagnostics.Debug.Assert(QBTable.ContainsKey(typeof(T)) == false);
            QBTable[typeof(T)] = new T();
        }

        public static bool QuitGame = false;

        public static SaveGame SaveGame;

        public static void InitSaveGame(Game game)
        {
            SaveGame = new SaveGame();
            SaveGame.Initialize(game);
        }

        public static void UpdateSaveGame()
        {
            SaveGame.CheckLoadGame();
            SaveGame.CheckSaveGame();
        }
    }
}
