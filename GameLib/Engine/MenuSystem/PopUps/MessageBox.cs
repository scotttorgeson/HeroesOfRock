using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace GameLib.Engine.MenuSystem.Menus {
   public  class MessageBoxScreen : PopUpScreen {
        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;

        public MessageBoxScreen (string message)
            : this(message, true) { }

        public MessageBoxScreen (string message, bool includeUsageText) : base(message){
            const string usageText = "\nGreen = ok" +
                                     "\nRed = cancel";

            if (includeUsageText)
                Message = message + usageText;
            else
                Message = message;
        }

        public override void Draw (float dt) {
            base.Draw(dt);
        }

        public override void HandleInput (MenuInput input) {
            if (input.IsMenuSelect()) {
                ExitScreen();
            }
            //} else if (input.IsMenuCancel()) {
            //    // Raise the cancelled event, then exit the message box.
            //    if (Cancelled != null)
            //        Cancelled(this, new EventArgs());

            //    ExitScreen();
            //}
        }

    }
}
