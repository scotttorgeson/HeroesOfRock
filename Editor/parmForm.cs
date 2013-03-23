using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameLib;

namespace WinFormsGraphicsDevice
{
    public partial class parmForm : Form
    {
        private ListBox parentListBox;

        //called by add button
        public parmForm(ListBox lb)
        {
            parentListBox = lb;
            InitializeComponent();
            this.textBox1.Enabled = true;
        }
        
        //this contructor is called by the edit button
        public parmForm(string name, string text)
        {
            InitializeComponent();
            this.textBox1.Text = name;
            this.textBox1.Enabled = false;
            this.richTextBox1.Text = text;
            parentListBox = null;
            
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            //save the text with the file name
            string path = "../../../../HeroesOfRockContent/Actors/" + textBox1.Text + ".parm";
            //save the file
            System.IO.File.WriteAllText(path, richTextBox1.Text);
            ParameterSet tempParm = ParameterSet.FromFile(path);
            
            foreach (Actor a in Stage.ActiveStage.GetQB<ActorQB>().Actors)
            {
                if (a.Name == textBox1.Text)
                {
                    foreach (KeyValuePair<string, string> keyValue in tempParm)
                    {
                        a.Parm.SetParm(keyValue.Key, keyValue.Value);
                    }
                }
            }

            if (parentListBox != null)
                parentListBox.Items.Add(textBox1.Text);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            //close this form without saving any changes
            Close();
        }
    }
}

