using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MoveCreator {
    public partial class MultipleButtonInput : Form {
        private List<String> Buttons;
        private MoveCreator parent;

        public MultipleButtonInput (MoveCreator parent) {
            InitializeComponent();
            Buttons = new List<String>();
            this.parent = parent;            
        }

        private void AddButton(String s, Button b){
            b.Enabled = false;
            b.Visible = false;
            Buttons.Add(s);
            buttonList.Items.Add(s);
        }

        //EVENT//

        private void bBtn_Click (object sender, EventArgs e) {
            AddButton("B", (Button)sender);
        }

        private void aBtn_Click (object sender, EventArgs e) {
            AddButton("A", (Button)sender);
        }

        private void xBtn_Click (object sender, EventArgs e) {
            AddButton("X", (Button)sender);
        }

        private void yBtn_Click (object sender, EventArgs e) {
            AddButton("Y", (Button)sender);
        }

        private void doneBtn_Click (object sender, EventArgs e) {
            String Button = "";
            foreach (String s in Buttons) {
                Button +=  s+" | " ;
            }
            parent.AddButton(Button.Trim(" | ".ToCharArray()));
            this.Close();
        }

        private void cancel_btn_Click (object sender, EventArgs e) {
            this.Close();
        }

        private void buttonList_DblClick (object sender, EventArgs e) {
           
            switch (buttonList.SelectedItem.ToString()) {
                case "A":
                    aBtn.Visible = true;
                    aBtn.Enabled = true;
                    break;
                case "B":
                    bBtn.Visible = true;
                    bBtn.Enabled = true;
                    break;
                case "X":
                    xBtn.Visible = true;
                    xBtn.Enabled = true;
                    break;
                case "Y":
                    yBtn.Visible = true;
                    yBtn.Enabled = true;
                    break;

                default:
                    break;
            }

            buttonList.Items.RemoveAt(buttonList.SelectedIndex);
        }
    }
}
