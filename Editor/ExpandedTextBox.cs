using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLib;
using System.Windows.Forms;

namespace Editor {
    public class ExpandedTextBox : TextBox
    {
        Actor actor;


        public ExpandedTextBox(Actor actor)
        {
            this.actor = actor;
        }

        public Actor Actor
        {
            get { return actor; }
        }

        public event EventHandler LeaveWithChangedText;

        private bool textChanged;

        protected override void OnEnter(EventArgs e)
        {
            textChanged = false;
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (textChanged)
            {
                OnLeaveWithChangedText(e);
            }
        }

        protected virtual void OnLeaveWithChangedText(EventArgs e)
        {
            if (LeaveWithChangedText != null)
            {
                LeaveWithChangedText(this, e);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            textChanged = true;
            base.OnTextChanged(e);
        }
    }
}
