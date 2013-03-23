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

        public static PlayerIndex PlayerIndex { 
            get { return playerIndex; } 
            set {
                playerIndex = value;
            }
        }

        public static void LoadStage(string file, bool blockingLoad)
        {
            if (!GlobalGameParms.initialized)
            {
                GlobalGameParms.GameParms = Content.Load<ParameterSet>("GameVariables");
                GlobalGameParms.initialized = true;
            }

            if (ActiveStage != null && ActiveStage.Parm.HasParm("AssetName") && !ActiveStage.Parm.GetString("AssetName").Equals(file))
                HasDied = false;

            LoadStage( Content.Load<ParameterSet>("Worlds/" + file), blockingLoad );
        }

        public static bool DoneLoading { get; private set; }
        
        private static Stage loadingStage = null;
        
        public static Stage LoadingStage { get { return loadingStage; } }

        public static bool WaitOnLoadFinish = false;

        public static void LoadStage(ParameterSet parm, bool blockingLoad)
        {
            if (loadingStage != null) // don't load more than one stage at a time...
                return;

            if (blockingLoad)
            {
                if (activeStage != null)
                {
                    foreach (KeyValuePair<Type, Quarterback> qb in activeStage.QBTable)
                        qb.Value.KillInstance();
                    activeStage.QBTable = null;
                    activeStage = null;
                }

                // unload content on blocking loads
                Renderer.Instance.ClearRModelInstances();
                Renderer.Instance.UnloadContent();
                Stage.Content.Unload();
                Renderer.Instance.LoadContent();

                loadingStage = new Stage(parm);
                activeStage = loadingStage;
                loadingStage.Initialize();
                loadingStage.LoadContent();
                
                loadingStage = null;

                foreach (KeyValuePair<Type, Quarterback> qb in activeStage.QBTable)
                    qb.Value.LevelLoaded();

                GC.Collect();

                GameRunning = true;
            }
            else
            {
                System.Threading.Thread thread = new Thread((_) =>
                    {
                        loadingStage = new Stage(parm);
                        loadingStage.Initialize();
                        loadingStage.LoadContent();
                        Stage.DoneLoading = true;

                        //if (activeStage == null)
                        //{
                        //    activeStage = loadingStage;
                        //    loadingStage = null;
                        //    DoneLoading = false;
                        //    GameRunning = true;
                        //}
                    });

                thread.Name = "ResourceThread";
                thread.Start();
            }
        }

        public static void ReloadStage(bool blockingLoad)
        {
            LoadStage(ActiveStage.Parm, blockingLoad);
        }

        private Stage(ParameterSet parm)
        {
            DoneLoading = false;
            this.Parm = parm;
            Time = 0.0f;
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

            foreach (KeyValuePair<Type, Quarterback> qb in QBTable)
                qb.Value.PreLoadInit(Parm);
        }

        public void LoadContent()
        {
            foreach (KeyValuePair<Type, Quarterback> qb in QBTable)
                qb.Value.LoadContent();

            foreach (KeyValuePair<Type, Quarterback> qb in QBTable)
                qb.Value.PostLoadInit(Parm);
            
            //if (Parm.HasParm("Raining") && Parm.GetBool("Raining"))
            //{
                GetQB<Engine.Particles.ParticleQB>().AddParticleEmitter(PlayerAgent.Player, new Vector3(0, 25, 0), false, -1, 40, 48,
                                                1.0f, 1.25f, new Vector2(5.0f,10.0f), new Vector2(10.0f,20.0f), 
                                                new Vector3(25, 2, 25), 50*Vector3.Down, Vector3.Zero, "rain");
            //}
        }

        bool FinishLoad = false;

        public void Update(float dt)
        {
            Time += dt;

            if (FinishLoad)
            {                
                foreach (KeyValuePair<Type, Quarterback> qb in activeStage.QBTable)
                    qb.Value.KillInstance();

                activeStage = loadingStage;
                loadingStage = null;
                DoneLoading = false;
                FinishLoad = false;

                foreach (KeyValuePair<Type, Quarterback> qb in activeStage.QBTable)
                    qb.Value.LevelLoaded();

                GC.Collect();

                return;
            }

            if (!WaitOnLoadFinish && DoneLoading)
            {
                FinishLoad = true;                
            }

            Renderer.Instance.UpdateStart();

            for (int i = 0; i < activeStage.QBTable.Count; i++)
            {
                activeStage.QBTable.Values.ElementAt(i).Update(dt);
            }
            //foreach (KeyValuePair<Type, Quarterback> qb in QBTable)
                //qb.Value.Update(dt);

            Renderer.Instance.UpdateEnd();
        }

        public void PauseGame()
        {
            foreach (KeyValuePair<Type, Quarterback> qb in QBTable)
                qb.Value.PauseQB();
        }

        public void ResumeGame()
        {
            foreach (KeyValuePair<Type, Quarterback> qb in QBTable)
                qb.Value.UnPauseQB();
        }

        public void Draw(float dt)
        {
            // all 3d models handled by renderer, don't try to draw them here
            renderer.SpriteBatch.Begin();

            for (int i = 0; i < activeStage.QBTable.Count; i++)
            {
                activeStage.QBTable.Values.ElementAt(i).DrawUI(dt);
            }
            //foreach (KeyValuePair<Type, Quarterback> qb in QBTable)
                //qb.Value.DrawUI(dt);

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

        public static bool HasDied { get; set; }
    }
}
