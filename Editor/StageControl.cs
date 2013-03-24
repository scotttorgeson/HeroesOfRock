//#define SUPERSAFEMODE
#region File Description
//-----------------------------------------------------------------------------
// SpriteFontControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameLib;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Entities;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.MathExtensions;
using Editor;
using System;
#endregion

// on stop: kill audio thats being looped

namespace WinFormsGraphicsDevice
{
    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, which allows it to
    /// render using a GraphicsDevice. This control shows how to use ContentManager
    /// inside a WinForms application. It loads a SpriteFont object through the
    /// ContentManager, then uses a SpriteBatch to draw text. The control is not
    /// animated, so it only redraws itself in response to WinForms paint messages.
    /// </summary>
    class StageControl : GraphicsDeviceControl
    {
        ThreadSafeContentManager content;
        SpriteFont font;
        Stopwatch timer;
        double lastTime = 0.0;
        public bool SlowMotion { get; set; }
        public float SlowAmount { get; set; }
        ParameterSet parms;
        public const string worldDirectory = "../../../../HeroesOfRockContent/Worlds/";
        string worldFile = worldDirectory + "AlphaLevel.parm";
        MainForm mf;
        XNAFrameworkDispatcherService dispatcherService;

        public StageControl(MainForm m)
        {
            mf = m;
        }


        public enum PlayState
        {
            Playing, // game running, no editing, no saving
            Stopped, // game play stopped, world in initial state, can edit and save
            Paused, // game paused, no editing, world in w/e state it was paused in
        }

        public PlayState playState { get; private set; }

        public void Start()
        {
            if (playState != PlayState.Playing)
            {
                activeManipulator = null;

                if (playState == PlayState.Stopped)
                {
                    mf.PropertiesPanel.Controls.Clear();
                    PhysicsObject.DisableMass = false;
                    PhysicsObject.ForceStaticMesh = false;

                    // save then load stage
                    //SaveStage(worldFile); //uncomment this line if we want to save to file everytime we start a level in the editor
                    reloadStage();
                    Stage.EditorLoadStage(parms);
                }
                else
                {
                    CameraQB cameraQB = Stage.ActiveStage.GetQB<CameraQB>();
                    cameraQB.EndFreeCam();
                }

                // start game
                playState = PlayState.Playing;

            }
        }

        public void Stop()
        {
            if (playState != PlayState.Stopped)
            {
                // stop game
                playState = PlayState.Stopped;
                PhysicsObject.ForceStaticMesh = true;
                PhysicsObject.DisableMass = true;

                // reload world
                Stage.EditorReloadStage();

                activeManipulator = new Manipulator();
                activeManipulator.Initialize(content, GraphicsDevice);

                //stop the theme song cause that is annoying
                AudioQB aQB = Stage.ActiveStage.GetQB<AudioQB>();
                aQB.PauseTheme();

                // set to freecam
                CameraQB cameraQB = Stage.ActiveStage.GetQB<CameraQB>();
                cameraQB.StartFreeCam();
            }
        }

        public void Pause()
        {
            if (playState == PlayState.Playing)
            {
                // pause game
                playState = PlayState.Paused;

                activeManipulator = new Manipulator();
                activeManipulator.Initialize(content, GraphicsDevice);

                // push the freecam
                CameraQB cameraQB = Stage.ActiveStage.GetQB<CameraQB>();
                cameraQB.StartFreeCam();
            }
        }

        /// <summary>
        /// Initializes the control, creating the ContentManager
        /// and using it to load a SpriteFont.
        /// </summary>
        protected override void Initialize()
        {
            content = new ThreadSafeContentManager(Services, "Content");
            playState = PlayState.Stopped;

            Stage.renderer = new Renderer(GraphicsDevice);
            Stage.renderer.Initialize();
            GameLib.Renderer.DrawTriggers = true;

            timer = Stopwatch.StartNew();

            Stage.Content = content;
            Stage.Editor = true;

            Stage.renderer.LoadContent();

            font = content.Load<SpriteFont>("DefaultFont");

            Mouse.WindowHandle = this.Handle;
            Application.Idle += delegate { Invalidate(); };
            SlowMotion = false;
            SlowAmount = 2.0f;

            dispatcherService = new XNAFrameworkDispatcherService();
        }

        /// <summary>
        /// Disposes the control, unloading the ContentManager.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Unload();
                dispatcherService.Kill();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Calls Update and Draw on the stage.
        /// </summary>
        protected override void Draw()
        {
#if SUPERSAFEMODE
            if (timer == null)
            {
                MessageBox.Show("timer is null");
                return;
            }
            else if (Stage.ActiveStage == null)
            {
                MessageBox.Show("active stage is null");
                return;
            }
            else if (GraphicsDevice == null)
            {
                MessageBox.Show("graphics device is null");
                return;
            }
            else if (Stage.renderer == null)
            {
                Stage.renderer = new BasicRenderer(GraphicsDevice);
                return;
            }

#endif
            try
            {
                if (Stage.ActiveStage != null)
                {
                    //if (Stage.LoadingStage != null) return;
                    float dt = (float)(timer.Elapsed.TotalSeconds - lastTime);
                    lastTime = timer.Elapsed.TotalSeconds;

                    if (playState == PlayState.Playing && SlowMotion)
                        dt /= SlowAmount;

                    if (playState == PlayState.Playing)
                        Stage.ActiveStage.Update(dt);
                    else if (playState == PlayState.Stopped || playState == PlayState.Paused)
                    {
                        Renderer.Instance.EditorUpdate();

                        Stage.ActiveStage.GetQB<ControlsQB>().Update(dt);
                        Stage.ActiveStage.GetQB<CameraQB>().Update(dt);

                        // physics run while stopped so we can update actor positions
                        // doesn't run while paused, so things stay still
                        if (playState == PlayState.Stopped) Stage.ActiveStage.GetQB<PhysicsQB>().Update(dt);
                    }

                    if (Stage.ActiveStage.GetQB<ControlsQB>().CurrentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Delete)
                        && activeManipulator.selectedActor != null)
                    {
                        if (activeManipulator.selectedActor.Name.Contains("Trigger"))
                            Stage.ActiveStage.GetQB<TriggerQB>().triggers.Remove(activeManipulator.selectedActor);
                        Stage.ActiveStage.GetQB<ActorQB>().EditorKillActor(activeManipulator.selectedActor);
                        activeManipulator.selectedActor = null;
                    }

                    Stage.renderer.Draw(dt);
                    Stage.ActiveStage.Draw(dt);

                    if (activeManipulator != null)
                    {
                        activeManipulator.Draw(dt, GraphicsDevice);
                        if (activeManipulator.selectedActor != null)
                        {
                            mf.AddProperties(activeManipulator.selectedActor);
                        }
                    }
                }
                else
                {
                    Renderer.Instance.EditorDrawNoStage();
                }
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Exit();
            }
        }

        public void SaveStage(string filename)
        {
            worldFile = filename;

            //create an empty ParameterSet
            ParameterSet newParms = new ParameterSet();

            // add the asset name (world filename without extension)
            newParms.AddParm("AssetName", System.IO.Path.GetFileNameWithoutExtension(worldFile));

            if (parms.HasParm("SunAngles"))
                newParms.AddParm("SunAngles", parms.GetVector2("SunAngles")); // in need a better solution

            // add the quarterback list to the parm set
            string qbs = "";
            foreach (KeyValuePair<System.Type, Quarterback> qb in Stage.ActiveStage.QBTable)
            {
                //check if this is the first addition to the quarterbacks string
                qbs += qb.Value.Name() + ',';
            }

            qbs = qbs.Remove(qbs.Length - 1); // remove the last comma

            //add the qbs to the Quarterbacks key
            newParms.AddParm("Quarterbacks", qbs);

            foreach (KeyValuePair<System.Type, Quarterback> qb in Stage.ActiveStage.QBTable)
            {
                //serialize the current quarterback
                qb.Value.Serialize(newParms);
            }            

            //set parms to newParms
            parms = newParms;

            //write file
            parms.ToFile(worldFile);
        }

        public void reloadStage()
        {
            //create an empty ParameterSet
            ParameterSet newParms = new ParameterSet();

            // add the asset name (world filename without extension)
            newParms.AddParm("AssetName", System.IO.Path.GetFileNameWithoutExtension(worldFile));

            if (parms.HasParm("SunAngles"))
                newParms.AddParm("SunAngles", parms.GetVector2("SunAngles")); // in need a better solution

            // add the quarterback list to the parm set
            string qbs = "";
            foreach (KeyValuePair<System.Type, Quarterback> qb in Stage.ActiveStage.QBTable)
            {
                //check if this is the first addition to the quarterbacks string
                qbs += qb.Value.Name() + ',';
            }

            qbs = qbs.Remove(qbs.Length - 1); // remove the last comma

            //add the qbs to the Quarterbacks key
            newParms.AddParm("Quarterbacks", qbs);


            //stop the theme song cause that is annoying
            AudioQB aQB = Stage.ActiveStage.GetQB<AudioQB>();
            aQB.ResumeTheme();

            foreach (KeyValuePair<System.Type, Quarterback> qb in Stage.ActiveStage.QBTable)
            {
                //serialize the current quarterback
                qb.Value.Serialize(newParms);
            }

            //set parms to newParms
            parms = newParms;

        }

        public void LoadStage(string filename)
        {
            Stop();
            activeManipulator = null;
            worldFile = filename;
            parms = ParameterSet.FromFile(worldFile);
            GameLib.Stage.EditorLoadStage(parms);
            //stop the theme song cause that is annoying
            AudioQB aQB = Stage.ActiveStage.GetQB<AudioQB>();
            aQB.PauseTheme();

            activeManipulator = new Manipulator();
            activeManipulator.Initialize(content, GraphicsDevice);
            Stage.ActiveStage.GetQB<CameraQB>().StartFreeCam();
        }

        public Manipulator activeManipulator;

        public void StartMove()
        {
            if (playState == PlayState.Stopped)
            {
                Actor selectedActor = null;
                if (activeManipulator != null)
                    selectedActor = activeManipulator.selectedActor;
                activeManipulator = new MoveManipulator();
                ((MoveManipulator)activeManipulator).modelDrawer = new BEPUphysicsDrawer.Models.InstancedModelDrawer(GraphicsDevice, Services);
                activeManipulator.Initialize(content, GraphicsDevice);
                activeManipulator.selectedActor = selectedActor;
            }
        }

        public void StartRotate()
        {
            if (playState == PlayState.Stopped)
            {
                Actor selectedActor = null;
                if (activeManipulator != null)
                    selectedActor = activeManipulator.selectedActor;
                activeManipulator = new RotateManipulator();
                ((RotateManipulator)activeManipulator).modelDrawer = new BEPUphysicsDrawer.Models.InstancedModelDrawer(GraphicsDevice, Services);
                activeManipulator.Initialize(content, GraphicsDevice);
                activeManipulator.selectedActor = selectedActor;
            }
        }

        public void AddActor(string actorname)
        {
            Vector3 pos = CameraQB.WorldMatrix.Translation + CameraQB.WorldMatrix.Forward * 30.0f;
            Vector3 zero = Vector3.Zero;
            
            Stage.ActiveStage.GetQB<ActorQB>().CreateActor(actorname, actorname, ref pos, ref zero, Stage.ActiveStage);
            
            /*
            //try to actually add actors
            int index = Stage.ActiveStage.GetQB<ActorQB>().Actors.Count - 1;
            string key = "Actor" + index;
            Stage.ActiveStage.Parm.AddParm(key, actorname);
            Stage.ActiveStage.Parm.AddParm(key + "Position", pos);
            Stage.ActiveStage.Parm.AddParm(key + "Rotation", zero);
            */

        }

        public void CreateTrigger(string triggerType)
        {
            TriggerQB triggerQB = Stage.ActiveStage.GetQB<TriggerQB>();
            ParameterSet parm = new ParameterSet();
            parm.AddParm("AssetName", "TriggerVolume" + triggerQB.triggers.Count);
            parm.AddParm("ModelName", "TriggerVolume");
            parm.AddParm("Mass", -1.0f);
            parm.AddParm("PhysicsType", "TriggerVolume");
            parm.AddParm("Agents", triggerType);

            switch (triggerType)
            {
                case "PlaySoundTriggerVolume":
                    parm.AddParm("SoundName", "guitarslide");
                    break;
                case "SpawnActorTriggerVolume":
                    parm.AddParm("ActorType", "EnemyWeak");
                    parm.AddParm("SpawnPos", Vector3.Zero);
                    parm.AddParm("Count", 1);
                    parm.AddParm("Freq", 1.0f);
                    parm.AddParm("DelayForFirst", 0.0f);
                    parm.AddParm("SpawnInWaves", false);
                    parm.AddParm("DelayForFirst",0);
                    parm.AddParm("ConstantSpawns",false);
                    parm.AddParm("EnemyList",false);

                    break;
                case "RotateCameraTriggerVolume":
                    parm.AddParm("NewDir", Vector3.Forward);
                    break;
            }

            Vector3 pos = CameraQB.WorldMatrix.Translation + CameraQB.WorldMatrix.Forward * 30.0f;
            triggerQB.AddTrigger(parm, pos);
        }
    }
}