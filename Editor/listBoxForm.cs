using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameLib;
using WinFormsGraphicsDevice;

namespace Editor
{
    public partial class listBoxForm : Form
    {
        Actor selectedActor;
        ParameterSet parms;
        Manipulator man;
        MainForm main;
        bool triggerFlag;

        public listBoxForm()
        {
            InitializeComponent();
            selectedActor = null;
        }

        public listBoxForm(ParameterSet ps, Actor a, Manipulator m, MainForm ma)
        {
            InitializeComponent();
            int index = 0;

            selectedActor = a;
            parms = ps;
            man = m;
            main = ma;
            if (man != null)
                man.enableRayCast = false;

            if (selectedActor.Name.Contains("Trigger"))
                triggerFlag = true;

            string agents = "";
            if (selectedActor.Parm.HasParm("Agents"))
                agents = selectedActor.Parm.GetString("Agents");

            foreach (KeyValuePair<string, string> s in ps)
            {
                if (index++ != 0)
                {
                    this.listBox.Items.Add(s.Key);
                    foreach (string str in agents.Split(','))
                    {
                        if (str == s.Key)
                            listBox.SelectedIndices.Add(index - 2);
                    }
                }
            }
        }

        private void listboxButtonSave_Click(object sender, EventArgs e)
        {
            ParameterSet newParm = defaultParameters(selectedActor.Parm);

            ListBox.SelectedIndexCollection selected = listBox.SelectedIndices;
            string agents = "";
            string prefix = "";
            foreach (int i in selected)
            {
                string key = listBox.Items[i].ToString();
                string values = parms.GetString(key);
                int index = 0;
                foreach (string s in values.Split(','))
                {
                    if (s != "none")
                    {
                        string[] parameter = s.Split('=');
                        if (selectedActor.Parm.HasParm(prefix + parameter[0]))
                            newParm.AddParm(prefix + parameter[0], selectedActor.Parm.GetString(prefix + parameter[0]));
                        else
                            newParm.AddParm(prefix + parameter[0], parameter[1]);
                    }
                }

                if (index == 0)
                    agents = key;
                if (index > 0 && index < selected.Count)
                    agents += ',' + key;
                index++;
            }
          

            string name = selectedActor.Name;
            Vector3 pos = selectedActor.PhysicsObject.Position;
            Vector3 r;
            Quaternion q = selectedActor.PhysicsObject.Orientation;
            PhysicsHelpers.QuaternionToEuler(ref q, out r);

            if (agents != "")
                newParm.AddParm("Agents", agents);

            string path = "../../../../HeroesOfRockContent/Actors/" + selectedActor.Name + ".parm";
            newParm.ToFile(path);
            Stage.ActiveStage.GetQB<ActorQB>().EditorKillActor(selectedActor);
            man.selectedActor = null;
            Actor newActor = Stage.ActiveStage.GetQB<ActorQB>().CreateActor(newParm, name, ref pos, ref r, Stage.ActiveStage);
            main.propertiesChanged = true;
            main.AddProperties(newActor);
        }

        private void listboxButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listBoxForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (man != null)
                man.enableRayCast = true;
        }

        private bool isDefaultActor(string key)
        {
            if (key == "ModelName" || key == "Mass" || key == "PhysicsType" || key == "AssetName")
                return true;
            else
                return false;
        }

        private bool isDefaultTrigger(string key, string assetName)
        {
            if (key.Contains("ModelName") || key.Contains("Position") || key == assetName)
                return true;
            else
                return false;
        }

        private ParameterSet defaultParameters(ParameterSet old)
        {
            ParameterSet newParm = new ParameterSet();

            foreach (KeyValuePair<string, string> kv in old)
            {
                if (kv.Key == "ModelName" || kv.Key == "Mass" || kv.Key == "PhysicsType")
                    newParm.AddParm(kv.Key, kv.Value);
            }

            return newParm;
        }
    }
}
