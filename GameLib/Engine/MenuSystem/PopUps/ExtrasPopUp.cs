using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace GameLib.Engine.MenuSystem.Menus {

    public class ExtrasPopUp : PopUpScreen {
        public ExtrasPopUp (string message)
            : this(message, true) { }

        public ExtrasPopUp (string message, bool includeUsageText)
            : base(message) {
            MenuEntry conceptArt = new MenuEntry("Concept Art");
            conceptArt.Selected += ConceptArtSelected;
            MenuEntries.Add(conceptArt);
        }

        void ConceptArtSelected (object sender, EventArgs e) {
            MenuSystem.AddScreen(new ConceptArtPopUp("Art"));
        }
    }
}
