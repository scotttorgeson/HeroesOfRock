#region File Description
//-----------------------------------------------------------------------------
// MainForm.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Windows.Forms;
using System.Collections.Generic;
using GameLib;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Entities;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.MathExtensions;
using Editor;
using System;
#endregion

namespace WinFormsGraphicsDevice
{
    // System.Drawing and the XNA Framework both define Color types.
    // To avoid conflicts, we define shortcut names for them both.
    using GdiColor = System.Drawing.Color;
    using XnaColor = Microsoft.Xna.Framework.Color;

    /// <summary>
    /// Custom form provides the main user interface for the program.
    /// In this sample we used the designer to add a splitter pane to the form,
    /// which contains a SpriteFontControl and a SpinningTriangleControl.
    /// </summary>
    public partial class MainForm : Form
    {
        private ParameterSet agentParms;

        //SB
        Actor currentActor;
        public bool propertiesChanged;
     
        public MainForm()
        {

            InitializeComponent();
            agentParms = ParameterSet.FromFile("../../../../../Editor/listAgents.parm");
          

            openToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            buttonPause.Enabled = false;
            buttonStop.Enabled = false;
            buttonStart.Enabled = true;

            actorEditButton.Enabled = false;
            actorRemoveButton.Enabled = false;
            this.stageControl = new StageControl(this);
            //this.stageControl.Size = new System.Drawing.Size(1280, 720);
            //this.stageControl.Location = new System.Drawing.Point(100, 50);
            this.stageControl.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            this.stageControl.Dock = DockStyle.Fill;
            this.splitContainer1.Panel2.Controls.Add(stageControl);
            //this.Controls.Add(stageControl);

            //this.stageControl.PropertiesPanel = this.PropertiesPanel;

            foreach (string actorFile in System.IO.Directory.EnumerateFiles("../../../../HeroesOfRockContent/Actors/"))
            {
                this.actorListBox.Items.Add(System.IO.Path.GetFileNameWithoutExtension(actorFile));
            }
            propertiesChanged = false;
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = StageControl.worldDirectory;
            openFileDialog.Filter = "Parm files (*.parm)|*.parm|All files (*.*)|*.*";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                stageControl.LoadStage(openFileDialog.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            stageControl.Start();
            UpdateFromPlayState();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            stageControl.Stop();

            UpdateFromPlayState();
        }

        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            stageControl.SlowMotion = checkBoxSlowMotion.Checked;
        }

        private void trackBar1_Scroll(object sender, System.EventArgs e)
        {
            stageControl.SlowAmount = 10 - trackBarSlowMotion.Value;
        }

        private void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = StageControl.worldDirectory;
            saveFileDialog.Filter = "Parm files (*.parm)|*.parm|All files (*.*)|*.*";
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                stageControl.SaveStage(saveFileDialog.FileName);
            }
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            stageControl.Pause();

            UpdateFromPlayState();
        }

        private void UpdateFromPlayState()
        {
            switch (stageControl.playState)
            {
                case StageControl.PlayState.Paused:
                    // change ui to paused state
                    openToolStripMenuItem.Enabled = false;
                    saveToolStripMenuItem.Enabled = false;
                    buttonPause.Enabled = false;
                    buttonStop.Enabled = true;
                    buttonStart.Enabled = true;
                    break;
                case StageControl.PlayState.Stopped:
                    // change ui to stopped state
                    openToolStripMenuItem.Enabled = true;
                    saveToolStripMenuItem.Enabled = true;
                    buttonPause.Enabled = false;
                    buttonStop.Enabled = false;
                    buttonStart.Enabled = true;
                    break;
                case StageControl.PlayState.Playing:
                    // change ui to playing state
                    openToolStripMenuItem.Enabled = false;
                    saveToolStripMenuItem.Enabled = false;
                    buttonPause.Enabled = true;
                    buttonStop.Enabled = true;
                    buttonStart.Enabled = false;
                    break;
            }
        }

        private void buttonMove_Click(object sender, System.EventArgs e)
        {
            stageControl.StartMove();
        }

        private void buttonRotate_Click(object sender, System.EventArgs e)
        {
            stageControl.StartRotate();
        }

        private void actorListBox_DoubleClick(object sender, System.EventArgs e)
        {
            stageControl.AddActor((string)actorListBox.SelectedItem);
        }
        /*
        private void propertyChanged(object sender, System.EventArgs e)
        {
            this.PropertiesPanelLabel.Text = sender.ToString();
        }
        */
        private void actorListBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //enable the edit actor button
            actorRemoveButton.Enabled = true;
            actorEditButton.Enabled = true;
        }

        private void actorAddButton_Click(object sender, System.EventArgs e)
        {
            /*  Forces only one editable parmFrom at a time
            if (OwnedForms.Length == 0)
            {
                AddOwnedForm(new parmForm(pt, actorListBox));
                OwnedForms[0].Show();
            }
             */
            AddOwnedForm(new parmForm(actorListBox));
            OwnedForms[OwnedForms.Length - 1].Show();
        }

        private void actorEditButton_Click(object sender, System.EventArgs e)
        {
            /* Forces only one editable parmFrom at a time
            if (OwnedForms.Length == 0)
            {
                //get the parameters for the parmForm
                string name = (string)actorListBox.SelectedItem;
                string text = getActorText(name);
                AddOwnedForm(new parmForm(pt, name, text));
                OwnedForms[0].Show();
            }
            */
            string name = (string)actorListBox.SelectedItem;
            string text = getActorText(name);
            parmForm pf = new parmForm(name, text);
            AddOwnedForm(pf);
            pf.Show();
        }

        private string getActorText(string name)
        {
            string path = "../../../../HeroesOfRockContent/Actors/" + name + ".parm";
            string text = System.IO.File.ReadAllText(path, System.Text.Encoding.ASCII);
            return text;

        }

        private void actorRemoveButton_Click(object sender, System.EventArgs e)
        {

            string name = (string)actorListBox.SelectedItem;
            string path = "../../../../HeroesOfRockContent/Actors/" + name + ".parm";

            // Confirm user wants to close
            switch (MessageBox.Show(this, "Are you sure you'd like to delete this parm file: " + name + "?", "Delete", MessageBoxButtons.YesNo))
            {
                case DialogResult.No:
                    break;
                default:
                    System.IO.File.Delete(path);
                    actorListBox.Items.Remove((object)name);
                    break;
            }
        }

        private void showTriggersCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            Stage.ActiveStage.GetQB<TriggerQB>().ShowTriggers(showTriggersCheckBox.Checked);
        }

        private void playSoundToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            stageControl.CreateTrigger("PlaySoundTriggerVolume");
        }

        private void spawnActorToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            stageControl.CreateTrigger("SpawnActorTriggerVolume");
        }

        //SB change. This is super hacky and super gross
        public void AddProperties(Actor actor)
        {
            bool agentFlag = false;
            string agents = "";
            //if (!actor.Equals(currentActor) && actor.Parm.GetCount() > 0)

            if (!actor.Equals(currentActor) || propertiesChanged)
            {
                propertiesChanged = false;
                this.currentActor = actor;
                this.PropertiesPanel.Controls.Clear();

                if (currentActor.Parm.HasParm("Agents"))
                    agents = currentActor.Parm.GetString("Agents");
                else
                    agents = "";
                

                var enumerator = actor.Parm.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ExpandedTextBox textbox = new ExpandedTextBox(actor);
                    Label label = new Label();
                    label.Text = enumerator.Current.Key;
                    if (label.Text == "Agents")
                    {
                        agentFlag = true;
                        textbox.DoubleClick += new EventHandler(this.agentTextBox_DoubleClick);
                    }
                    label.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
                    string[] values = isAgentValue(agents, label.Text);
                    /*if (values != null && agents != "")
                    {
                        ComboBox combo = new ComboBox();
                        combo.Name = label.Text;
                        foreach (string value in values)
                        {
                            if (value == enumerator.Current.Value)
                                combo.SelectedIndex = combo.Items.Add(value);
                            else 
                                combo.Items.Add(value);

                        }
                        combo.SelectedIndexChanged += new EventHandler(this.PropertyChangedCombo);

                        PropertiesPanel.Controls.Add(label);
                        PropertiesPanel.Controls.Add(combo);
                    
                    }
                    else
                    {*/
                        textbox.Name = label.Text;
                        textbox.Text = enumerator.Current.Value;
                        textbox.LeaveWithChangedText += new EventHandler(this.PropertyChangedTextBox);
                    
                        PropertiesPanel.Controls.Add(label);
                        PropertiesPanel.Controls.Add(textbox);
                    //}
                }

                //Add agents field if there wasn't one already
                if (!agentFlag)
                {
                    ExpandedTextBox textbox = new ExpandedTextBox(actor);
                    Label label = new Label();
                    label.Text = "Agents";
                    textbox.DoubleClick += new EventHandler(this.agentTextBox_DoubleClick);
                    label.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
                    textbox.Name = label.Text;
                    textbox.TextChanged += new EventHandler(this.PropertyChangedTextBox);
                    PropertiesPanel.Controls.Add(label);
                    PropertiesPanel.Controls.Add(textbox);
                }

                //position
                Vector3 pos = actor.PhysicsObject.Position;

                Label labelPos = new Label();
                labelPos.Text = "Position";
                ExpandedTextBox textBoxPos = new ExpandedTextBox(actor);
                textBoxPos.Name = labelPos.Text;
                textBoxPos.Text = pos.X.ToString() + ' ' + pos.Y.ToString() + ' ' + pos.Z.ToString();
                textBoxPos.TextChanged += new EventHandler(this.PropertyChangedTextBox);
                PropertiesPanel.Controls.Add(labelPos);
                PropertiesPanel.Controls.Add(textBoxPos);
                /*
                labelPos = new Label();
                labelPos.Text = "Position.Y";
                textBoxPos = new ExpandedTextBox(actor);
                textBoxPos.Name = labelPos.Text;
                textBoxPos.Text = pos.Y.ToString();
                textBoxPos.TextChanged += new EventHandler(this.PropertyChangedTextBox);
                PropertiesPanel.Controls.Add(labelPos);
                PropertiesPanel.Controls.Add(textBoxPos);

                labelPos = new Label();
                labelPos.Text = "Position.Z";
                textBoxPos = new ExpandedTextBox(actor);
                textBoxPos.Name = labelPos.Text;
                textBoxPos.Text = pos.Z.ToString();
                textBoxPos.TextChanged += new EventHandler(this.PropertyChangedTextBox);
                PropertiesPanel.Controls.Add(labelPos);
                PropertiesPanel.Controls.Add(textBoxPos);
                */
                //rotation
                Vector3 r;
                Quaternion q = actor.PhysicsObject.Orientation;
                PhysicsHelpers.QuaternionToEuler(ref q, out r);
                Label labelRot = new Label();
                labelRot.Text = "Rotation";
                ExpandedTextBox textBoxRot = new ExpandedTextBox(actor);
                textBoxRot.Name = labelRot.Text;
                textBoxRot.Text = r.X.ToString() + ' ' + r.Y.ToString() + ' ' + r.Z.ToString();
                textBoxRot.TextChanged += new EventHandler(this.PropertyChangedTextBox);
                PropertiesPanel.Controls.Add(labelRot);
                PropertiesPanel.Controls.Add(textBoxRot);

                /*
                labelRot = new Label();
                labelRot.Text = "Rotation.Y";
                textBoxRot = new ExpandedTextBox(actor);
                textBoxRot.Name = labelRot.Text;
                textBoxRot.Text = r.Y.ToString();
                textBoxRot.TextChanged += new EventHandler(this.PropertyChangedTextBox);
                PropertiesPanel.Controls.Add(labelRot);
                PropertiesPanel.Controls.Add(textBoxRot);

                labelRot = new Label();
                labelRot.Text = "Rotation.Z";
                textBoxRot = new ExpandedTextBox(actor);
                textBoxRot.Name = labelRot.Text;
                textBoxRot.Text = r.Z.ToString();
                textBoxRot.TextChanged += new EventHandler(this.PropertyChangedTextBox);
                PropertiesPanel.Controls.Add(labelRot);
                PropertiesPanel.Controls.Add(textBoxRot);
                */
            }
        }

        private void PropertyChangedTextBox(object sender, System.EventArgs e)
        {
            ExpandedTextBox t = (ExpandedTextBox)sender;
            Actor actor = t.Actor;
            //String oldVal;
            if (actor.Name.Contains("Trigger"))
            {
                foreach (Actor trigger in Stage.ActiveStage.GetQB<TriggerQB>().triggers)
                {
                    if (trigger == actor)
                    {
                        if (trigger.Parm.HasParm(t.Name))
                        {
                            trigger.Parm.SetParm(t.Name, t.Text);
                            Stage.ActiveStage.GetQB<TriggerQB>().removeTrigger(trigger);
                            Actor newTrigger = Stage.ActiveStage.GetQB<TriggerQB>().AddTrigger(trigger.Parm, trigger.PhysicsObject.Position);
                            stageControl.activeManipulator.selectedActor = newTrigger;
                        }
                        break;
                    }
                }
            }
            else
            {
                if (actor.Parm.HasParm(t.Name))
                {
                    //oldVal = actor.Parm.GetString(t.Name);
                    actor.Parm.SetParm(t.Name, t.Text);
                    //System.Console.WriteLine("CHANGED " + t.Actor.Name + ": " + oldVal + " to " + actor.Parm.GetString(t.Name));
                }
            }
            if (t.Name.Contains("Position"))
            {
                string[] values = t.Text.Split(' ');
                float x, y, z;

                if (values.Length == 3)
                {
                    if (float.TryParse(values[0], out x) && float.TryParse(values[1], out y) && float.TryParse(values[2], out z))
                    {
                        actor.PhysicsObject.Position = new Vector3(x, y, z);
                        /*
                        Vector3 pos = actor.PhysicsObject.Position;
                        if (t.Name.Contains("X"))
                            actor.PhysicsObject.Position = new Vector3(change, pos.Y, pos.Z);
                        if (t.Name.Contains("Y"))
                            actor.PhysicsObject.Position = new Vector3(pos.X, change, pos.Z);
                        if (t.Name.Contains("Z"))
                            actor.PhysicsObject.Position = new Vector3(pos.X, pos.Y, change);
                         * */
                    }
                }
            }
            else if (t.Name.Contains("Rotation"))
            {
                string[] values = t.Text.Split(' ');
                float x, y, z;
                if (values.Length == 3)
                {
                    if (float.TryParse(values[0], out x) && float.TryParse(values[1], out y) && float.TryParse(values[2], out z))
                    {
                        /*
                    Vector3 rot;
                    Quaternion q = actor.PhysicsObject.Orientation;
                    PhysicsHelpers.QuaternionToEuler(ref q, out rot);
                    if (t.Name.Contains("X"))
                        rot.X = change;
                    if (t.Name.Contains("Y"))
                        rot.Y = change;
                    if (t.Name.Contains("Z"))
                        rot.Z = change;
                        */
                        actor.PhysicsObject = new PhysicsObject(actor, actor.Parm, actor.modelInstance.model.Model, actor.PhysicsObject.Position, new Vector3(x, y, z), Stage.ActiveStage);

                    }
                    /* edits stage.parm 
                     string key = "";
                     Vector3 pos = new Vector3(0);
                     bool flag = false;

                     foreach (KeyValuePair<string, string> kv in Stage.ActiveStage.Parm)
                     {
                         if (kv.Value == actor.Name)
                         {
                             if (Stage.ActiveStage.Parm.HasParm(kv.Key + "Position"))
                             {
                                 if (Stage.ActiveStage.Parm.GetVector3(kv.Key + "Position") == actor.PhysicsObject.Position)
                                 {
                                     key = kv.Key;
                                     pos = Stage.ActiveStage.Parm.GetVector3(key + "Position");
                                     flag = true;
                                     break;
                                 }
                             }
                         }
                     }

                     float change;

                     if (flag)
                     {
                         if (float.TryParse(t.Text, out change))
                         {
                             if (t.Name.Contains("X"))
                                 Stage.ActiveStage.Parm.SetParm(key + "Position", new Vector3(change, pos.Y, pos.Z));
                             if (t.Name.Contains("Y"))
                                 Stage.ActiveStage.Parm.SetParm(key + "Position", new Vector3(pos.X, change, pos.Z));
                             if (t.Name.Contains("Z"))
                                 Stage.ActiveStage.Parm.SetParm(key + "Position", new Vector3(pos.X, pos.Y, change));
                         }
                     }
                      *   */
                }
            }
        }

        private void PropertyChangedCombo(object sender, System.EventArgs e)
        {
            ComboBox t = (ComboBox)sender;
            Actor actor = stageControl.activeManipulator.selectedActor;

            //String oldVal;
            if (actor.Name.Contains("Trigger"))
            {
                foreach (Actor trigger in Stage.ActiveStage.GetQB<TriggerQB>().triggers)
                {
                    if (trigger == actor)
                    {
                        if (trigger.Parm.HasParm(t.Name))
                        {
                            trigger.Parm.SetParm(t.Name, t.SelectedItem);
                            Stage.ActiveStage.GetQB<TriggerQB>().removeTrigger(trigger);
                            Actor newTrigger = Stage.ActiveStage.GetQB<TriggerQB>().AddTrigger(trigger.Parm, trigger.PhysicsObject.Position);
                            stageControl.activeManipulator.selectedActor = newTrigger;
                        }
                        break;
                    }
                }
            }
            else
            {
                if (actor.Parm.HasParm(t.Name))
                {
                    //oldVal = actor.Parm.GetString(t.Name);
                    actor.Parm.SetParm(t.Name, t.SelectedItem);
                    //System.Console.WriteLine("CHANGED " + t.Actor.Name + ": " + oldVal + " to " + actor.Parm.GetString(t.Name));
                }
            }
        }

        private void agentTextBox_DoubleClick(object sender, System.EventArgs e)
        {
            //open a listBOX with all the available agents from a config file
            listBoxForm lbf = new listBoxForm(agentParms, currentActor, stageControl.activeManipulator, this);

            lbf.Show();

        }

        private string[] isAgentValue(string agents, string value)
        {
            foreach (string agent in agents.Split(','))
            {
                if (agentParms.HasParm(agent))
                {
                    foreach (string values in agentParms.GetString(agent).Split(','))
                    {
                        string[] split = values.Split('=');
                        if (value == split[0])
                        {
                            string[] rtn = new string[split.Length - 1];
                            for (int i = 0; i < split.Length - 1; i++)
                                rtn[i] = split[i + 1];

                            return rtn;
                        }
                    }
                }
            }
            return null;
        }

        private void airBurstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stageControl.CreateTrigger("AirBurstTriggerVolume");
        }

        private void buttonPushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stageControl.CreateTrigger("ButtonPushTriggerVolume");
        }

        private void textToScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stageControl.CreateTrigger("TextToScreenTriggerVolume");
        }

        private void healthPackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stageControl.CreateTrigger("HealthTriggerVolume");
        }

        private void rotateCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stageControl.CreateTrigger("RotateCameraTriggerVolume");
        }

        private void endLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stageControl.CreateTrigger("EndLevelTriggerVolume");
        }
    }
}
