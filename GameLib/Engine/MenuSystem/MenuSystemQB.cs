using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameLib.Engine.MenuSystem.Menus;
using GameLib.Engine.MenuSystem.Screens;

namespace GameLib.Engine.MenuSystem {

    public class MenuSystemQB : Quarterback {

        private MenuSystem menuSystem;
        private GameScreen firstScreen;

        public MenuSystemQB () {
            
        }

        private GameScreen GetGameScreenByName (string screenname) {
            switch (screenname) {
                case "CreditsMenu":
                    return new CreditsMenu();
                case "LevelMenu":
                    return new LevelMenu();
                case "MainMenu":
                    menuSystem.AddScreen(new BackgroundScreen());
                    return new MainMenu();
                case "TutorialMenu":
                    return new TutorialMenu();
                case "BackgroundScreen":
                    return new BackgroundScreen();
                case "HudScreen":
                    return new HudScreen();
                case "SplashMenu":
                    menuSystem.AddScreen(new BackgroundScreen());
                    return new SplashMenu();
                case "ElevenTenLogoScreen":
                    menuSystem.AddScreen(new ColoredBackgroundScreen(Color.Black));
                    return new ElevenTenLogoScreen(2);
                case "BepuPhysicsLogoScreen":
                    menuSystem.AddScreen(new ColoredBackgroundScreen(Color.White));
                    return new BepuPhysicsLogoScreen(2);
                default:
                    return null;
            }
        }

        public override void PreLoadInit(ParameterSet Parm)
        {
            if (Parm.HasParm("FirstScreen"))
            {
                Initialize(Parm.GetString("FirstScreen"));
            }
        }

        public void Initialize (string screenName) {
            this.menuSystem = new MenuSystem(Stage.LoadingStage.GetQB<ControlsQB>());
            this.menuSystem.Initialize();

            if (!string.IsNullOrEmpty(screenName))
            {
                this.firstScreenName = screenName;
                this.firstScreen = GetGameScreenByName(screenName);
                menuSystem.AddScreen(firstScreen);
            }
        }

        public override void LoadContent()
        {
            this.menuSystem.LoadContent(Stage.Content);
        }

        public override void PostLoadInit(ParameterSet Parm)
        {
            menuSystem.PostLoadInit(Parm);
            base.PostLoadInit(Parm);
        }

        public void ShowEndLevelScreen()
        {
            menuSystem.AddScreen(new EndLevelMenu("End Level"));
        }

        public void PauseGame()
        {
            if (!Stage.ActiveStage.Paused && menuSystem.CanPause())
            {
                menuSystem.AddScreen(new PauseMenu(""));
            }
        }

        public override void PauseQB()
        {
            //unpausable
        }

        public void GoToLevelSelect()
        {
            //menuSystem.ExitAll();
            menuSystem.AddScreen(new BackgroundScreen());
            menuSystem.AddScreen(new LevelMenu());
        }

        public override void Update(float dt)
        {
            this.menuSystem.Update(dt);
        }
        public override void DrawUI(float dt)
        {
            this.menuSystem.Draw(dt);
        }
        public override string Name () {
            return "MenuSystemQB";
        }
        string firstScreenName;
        public override void Serialize(ParameterSet parm)
        {
            parm.AddParm("FirstScreen", firstScreenName);
        }
    }
}
