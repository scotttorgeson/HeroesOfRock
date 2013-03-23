using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameLib.Engine.AttackSystem;

namespace MoveCreator {
    public partial class MoveCreator : Form {
        Boolean AppExit;
        public List<String> Sequence;
        public MoveCreatorForm parent;
        private Move move;

        public MoveCreator (MoveCreatorForm parent) {
            InitializeComponent();
            AppExit = true;
            Sequence = new List<String>();
            this.FormClosing += MoveCreator_FormClosing;
            this.label1.Text = "Create New Move";
            this.parent = parent;
            SetDefaults();
        }

        public MoveCreator (MoveCreatorForm parent, Move move) {
            InitializeComponent();
            this.FormClosing += MoveCreator_FormClosing;
            this.label1.Text = "Edit New Move";
            this.parent = parent;
            this.move = move;
            move.BoxDims = new string[]{"3", "3"};
            SetDefaults();
            PopulateEditFields();
        }

        private void PopulateEditFields () {
            nameField.Text = move.Name;
            damageField.Value = decimal.Parse(move.Damage.ToString());
            attackTimeField.Value = decimal.Parse(move.AttackTime.ToString());
            attackTypeField.SelectedItem = move.AttackTypeFlag;
            //move.Animation = animationField.SelectedText;
            audioField.SelectedText = move.Audio;
            statusField.SelectedItem = move.Status;
            subMoveCheck.Checked = move.IsSubMove;

            foreach (string b in move.ButtonSequence) {
                buttonSequenceList.Items.Add(b);
            }
        }

        public void AddButton (String button) {
            Sequence.Add(button);
            buttonSequenceList.Items.Add(button);
        }

        private Boolean Save () {
            if (nameField.Text == String.Empty) {
                MessageBox.Show("You must specify a move name", "Cannot Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            Move move = new Move();
            move.Name = nameField.Text;
            move.Damage = float.Parse(damageField.Value.ToString());
            move.AttackTime = float.Parse(attackTimeField.Value.ToString());
            move.AttackTypeFlag = (AttackType) Enum.Parse(typeof(AttackType), attackTypeField.SelectedItem.ToString());    
            //move.Animation = animationField.SelectedText;
            move.Audio = audioField.SelectedText;
            move.Status = (MoveStatus)Enum.Parse(typeof(MoveStatus), statusField.SelectedItem.ToString());
            move.ButtonSequence = getButtonSequence();
            move.IsSubMove = subMoveCheck.Checked;
            move.BoxDims = new string[]{"3", "3"};

            if (parent.movelist.ContainsKey(move.Name))
                parent.movelist.Remove(move.Name);
            
            parent.movelist.Add(move.Name, move);
            return true;
        }

        private string[] getButtonSequence () {
            int count = buttonSequenceList.Items.Count;
            string[] seq = new string[count];
            for(int i = 0; i < count; i++) {
                seq[i] = buttonSequenceList.Items[i].ToString();
            }

            return seq;
        }

        private void SetDefaults () {
            ComboBox[] list = { animationField, audioField, particleField};

            foreach (ComboBox b in list) {
                b.Items.Add("Choose Later");
                b.SelectedIndex = 0;
            }
            foreach(string clip in parent.audioClips){
                int start = clip.LastIndexOf('\\')+1;
                int end = clip.LastIndexOf('.');
                audioField.Items.Add(clip.Substring(start,end-start));
            }
            foreach (string clip in parent.particleFX) {
                int start = clip.LastIndexOf('\\') + 1;
                int end = clip.LastIndexOf('.');
                particleField.Items.Add(clip.Substring(start, end - start));
            }
            foreach (MoveStatus s in Enum.GetValues(typeof(MoveStatus))) {
                statusField.Items.Add(s);
            }

            foreach (AttackType t in Enum.GetValues(typeof(AttackType))) {
                attackTypeField.Items.Add(t);
            }

            attackTypeField.SelectedIndex = 0;
            statusField.SelectedItem = MoveStatus.In_Progress;
        }
        
        /********************************
                    Events
        ********************************/
        private void cancelBtn_Click (object sender, EventArgs e) {
           DialogResult result = MessageBox.Show("Your changes will NOT be saved. Do you still want to exit?", "Cancel and Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

           if (result.Equals(DialogResult.Yes)) {
               AppExit = false; 
               this.Close();
           }
        }

        private void saveBtn_Click (object sender, EventArgs e) {
            Save();
        }

        private void exitBtn_Click (object sender, EventArgs e) {
            if (Save()) {
                AppExit = false;
                this.Close();
            }
        }

        private void MoveCreator_FormClosing (object sender, FormClosingEventArgs e) {
            parent.RefreshList();
            if (AppExit && sender.GetType().Equals(this.GetType())) {
                DialogResult result = MessageBox.Show("Your changes will NOT be saved.", "Cancel and Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result.Equals(DialogResult.No)) {
                    e.Cancel = true;
                }
                this.FormClosing -= MoveCreator_FormClosing;
                
            }

            
        }

        private void aBtn_Click (object sender, EventArgs e) {
            AddButton("A");
        }

        private void bBtn_Click (object sender, EventArgs e) {
            AddButton("B");
        }

        private void xBtn_Click (object sender, EventArgs e) {
            AddButton("X");
        }

        private void yBtn_Click (object sender, EventArgs e) {
            AddButton("Y");
        }

        private void multiBtn_Click (object sender, EventArgs e) {
            MultipleButtonInput m = new MultipleButtonInput(this);
            m.TopMost = true;
            m.ShowDialog(this);
            this.AddOwnedForm(m);
            
        }

        private void buttonSequenceList_DblClick (object sender, EventArgs e) {
            buttonSequenceList.Items.RemoveAt(buttonSequenceList.SelectedIndex);
        }
    }
}
