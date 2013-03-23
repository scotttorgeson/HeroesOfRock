using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Engine.MenuSystem.Menus {
    public class LevelMenuEntry : MenuEntry {
        public string LevelName { get; private set; }

        public LevelMenuEntry (string levelName, string text)
            : base(text) {
                this.LevelName = levelName;
        }
    }
}
