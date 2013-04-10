using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameLib.Engine.MenuSystem.Menus.MenuComponents;
using System.Collections;
using System.IO;
namespace GameLib.Engine.MenuSystem.Menus {

    public class ConceptArtPopUp : PopUpScreen {
        private int currentIdx;
        //private ArrayList art;

        public ConceptArtPopUp (string message)
            : this(message, true) { }

        public ConceptArtPopUp (string message, bool includeUsageText)
            : base(message) {
                currentIdx = 0;
                //art = new ArrayList();
        }

        public override void HandleInput (MenuInput input) {
            if (input.IsMenuRight()) {
                Next();
            } else if (input.IsMenuLeft()) {
                Previous();
            } else if (input.IsMenuCancel()) {
                ExitScreen();
            }
        }

        public override void LoadContent () {
            base.LoadContent();
            //Load directory info, abort if none
            DirectoryInfo dir = new DirectoryInfo(Stage.Content.RootDirectory + "\\UI\\Extras");
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            //Load all files that matches the file filter
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files) {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                Texture2D texture = Stage.Content.Load<Texture2D>(Stage.Content.RootDirectory + "/UI/Extras/" + key);
                MenuEntries.Add(new MenuGraphic(texture.Name, Vector2.Zero, 1));
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
        private void Next () {
            currentIdx++;
            //if (currentIdx > art.Count)
                //currentIdx = 0;
        }

        private void Previous () {
            currentIdx--;
            //if (currentIdx < 0)
                //currentIdx = art.Count - 1;
        }

    }
}
