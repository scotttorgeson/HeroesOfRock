using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Engine.MenuSystem.Menus {
    public class LevelMenu : GameMenu {
        public LevelMenu ()
            : base("Choose a Level") {
            //LevelMenuEntry TEST = new LevelMenuEntry("TestLevel2", "Test Level", "LevelBlank", false);
            //LevelMenuEntry ONE = new LevelMenuEntry("Level1", "Level 1", "LevelBlank");
            //LevelMenuEntry TWO = new LevelMenuEntry("Level2", "Level 2", "LevelBlank");
            //LevelMenuEntry THREE = new LevelMenuEntry("Level3", "Level 3", "LevelBlank");
            //LevelMenuEntry FOUR = new LevelMenuEntry("Level4", "Level 4", "LevelBlank");

            //LevelMenuEntry TUTORIAL = new LevelMenuEntry("Tutorial", "Tutorial", "LevelAlpha", false);
            LevelMenuEntry LEVEL2 = new LevelMenuEntry("Level2", "Level 1", "LevelAlpha", false);
            LevelMenuEntry LEVEL3 = new LevelMenuEntry("Level3", "Level 2", "LevelAlpha", false);

            //TODO CHANGE ME this will need to be driven from the user profile
            //TUTORIAL.IsLocked = false;
            LEVEL2.IsLocked = false;
            LEVEL3.IsLocked = false;
            //TEST.IsLocked = false;


            // Hook up menu event handlers.
            //TUTORIAL.Selected += LoadLevel;
            LEVEL2.Selected += LoadLevel;
            LEVEL3.Selected += LoadLevel;
            //TEST.Selected += LoadLevel;
            //TEST.Selected += LoadLevel;
            //ONE.Selected += LoadLevel;
            //TWO.Selected += LoadLevel;
            //THREE.Selected += LoadLevel;
            //FOUR.Selected += LoadLevel;

            // Add entries to the menu.
            //MenuEntries.Add(TUTORIAL);
            MenuEntries.Add(LEVEL2);
            MenuEntries.Add(LEVEL3);
            //MenuEntries.Add(TEST);
            //MenuEntries.Add(TEST);
            //MenuEntries.Add(ONE);
            //MenuEntries.Add(TWO);
            //MenuEntries.Add(THREE);
            //MenuEntries.Add(FOUR);
        }

        /// <summary>
        /// Loads the level that is associated with the level entry and raises the loading level.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadLevel (object sender, EventArgs e)
        {
            LevelMenuEntry entry = (LevelMenuEntry)sender;
            if (!entry.IsLocked)
            {
                if (entry.UseLoadingLevel)
                {
                    LoadingQB.LoadLevel = entry.LevelName;
                    Stage.LoadStage("LoadingLevel", true);
                }
                else
                {
                    LoadingScreen.Load(MenuSystem, true, entry.LevelName);
                    //Stage.LoadStage(entry.LevelName, true);
                }
                 this.MarkedForRemove = true;
            }
        }

        /// <summary>
        /// All menu entries are lined up in a vertical list, centered on the menu.
        /// </summary>
        protected override void UpdateMenuEntryLocations () {
            
            Vector2 position = new Vector2(0f, 175f);
            Vector2 selectedPosition = new Vector2();
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // update each menu entry's location in turn
            for (int i = 0; i < MenuEntries.Count; i++) {
                MenuEntry menuEntry = MenuEntries[i];
                int offset = (menuEntry.GetWidth(this) + 50) * (i - selectedEntry);
                // each entry is to be centered vertically
                position.X = Renderer.ScreenWidth - menuEntry.GetWidth(this) / 2 + offset;
                position.Y = Renderer.ScreenHeight - menuEntry.GetHeight(this) / 2;
                selectedPosition.X = Renderer.ScreenRect.Center.X / 2 - (menuEntry.GetWidth(this) / 2);
                selectedPosition.Y = position.Y;

                if (MenuState == MenuState.TransitionOn)
                    position.Y -= transitionOffset * 256;
                else
                    position.Y += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = position;
            }
        }
        
        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel () {
            MenuSystem.AddScreen(new MainMenu());
        }

        public override void HandleInput (MenuInput input) {
            // Move to the previous menu entry?
            if (input.IsMenuLeft() || input.IsMenuUp()) {
                int select = selectedEntry;

                selectedEntry--;

                if (selectedEntry < 0) { 
                    selectedEntry = MenuEntries.Count - 1;
                }
                if (!MenuEntries[selectedEntry].CanSelect) {
                    selectedEntry = select;
                }
            }

            // Move to the next menu entry?
            if (input.IsMenuRight() || input.IsMenuDown()) {
                int select = selectedEntry;

                selectedEntry++;

                if (selectedEntry >= MenuEntries.Count) {
                    selectedEntry = 0;
                }
                if (!MenuEntries[selectedEntry].CanSelect) {
                    selectedEntry = select;
                }
            }

            if (input.IsMenuSelect()) {
                OnSelectEntry(selectedEntry);
            } else if (input.IsMenuCancel()) {
                OnCancel();
            }
        }

    }
}
