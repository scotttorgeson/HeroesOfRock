using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameLib;

namespace Editor {
    public class PropertyEventArgs : EventArgs {
        ExpandedTextBox textbox;

        public PropertyEventArgs (ExpandedTextBox textbox) {
            this.textbox = textbox;
        }


        internal ExpandedTextBox Textbox {
            get { return textbox; }
        }

    }
}
