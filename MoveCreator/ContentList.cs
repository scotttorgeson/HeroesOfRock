using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameLib.Engine.AttackSystem;
using WinFormsContentLoading;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;


namespace MoveCreator {
    public partial class MoveCreatorForm : Form {
        public ContentManager content;
        public Dictionary<String, Move> movelist;        
        
        //public List<Animation> animations;
        public List<string> particleFX;
        public List<string> audioClips;

        private Boolean appClose;
        private String directoryHome;

        public MoveCreatorForm () {
            //set up defaults
            this.movelist = new Dictionary<String, Move>();
            this.directoryHome = "../../../HeroesOfRock";
            this.FormClosing += ContentList_FormClosing; ;
            this.appClose = true;

            //set up content manager
            GraphicsDeviceService gds = GraphicsDeviceService.AddRef(this.Handle,
                     this.ClientSize.Width, this.ClientSize.Height);
            ServiceContainer services = new ServiceContainer();
            services.AddService<IGraphicsDeviceService>(gds);
            this.content = new ContentManager(services, String.Concat(directoryHome, "/HeroesOfRock/bin/x86/Debug/Content"));


            //Load and/or parse predefined objects
            LoadMoveList(content.Load<Move[]>("Movelist"));
            this.audioClips = Directory.GetFiles(String.Concat(content.RootDirectory, "/Audio")).ToList<string>();
            this.particleFX = Directory.GetFiles(String.Concat(content.RootDirectory, "/ParticleFX")).ToList<string>();
            //if null, will back up to the content default
            BackUpMoveList(null);
            InitializeComponent();

            RefreshList();
        }

        private void LoadMoveList (Move[] moves) {
            foreach (Move m in moves) {
                movelist.Add(m.Name, m);
            }

        }

        private void BackUpMoveList (String backUpPath) {
            String movelistPath = String.Concat(directoryHome, "/HeroesOfRockContent/Movelist.xml");

            if (backUpPath == null) {
                File.Copy(movelistPath, String.Concat(movelistPath, ".bak"), true);
            }
        }

        public void RefreshList () {
            nameList.Items.Clear();
            statusList.Items.Clear();

            foreach (var m in movelist.OrderBy(i => i.Key)) {
                nameList.Items.Add(m.Key);
                statusList.Items.Add(m.Value.Status);
            }
        }




        private void ExportMoves () {
            Move[] moves = new Move[movelist.Count];

            for(int i = 0; i < movelist.Count; i++){
                moves[i] = movelist.ElementAt(i).Value;
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(String.Concat(directoryHome, "/HeroesOfRockContent/Movelist.xml"), settings)) {
                IntermediateSerializer.Serialize(writer, moves, null);
            }
            
        }

        /**********
         * EVENTS *
         **********/
        private void newMoveBtn_Click (object sender, EventArgs e) {
            MoveCreator mc = new MoveCreator(this);
            this.AddOwnedForm(mc);
            mc.Size = this.Size;
            mc.ShowDialog();
        }

        private void listBox_DoubleClick (object sender, EventArgs e) {
            if (nameList.SelectedItem != null)
                if (nameList.SelectedItem.ToString().Length != 0) {
                    MoveCreator mc = new MoveCreator(this, movelist[nameList.SelectedItem.ToString()]);
                    this.AddOwnedForm(mc);
                    mc.Size = this.Size;
                    mc.ShowDialog();
                }
        }

        private void ContentList_FormClosing (object sender, FormClosingEventArgs e) {
            Boolean cancelClosing = false;

            if (appClose) {
                DialogResult res = MessageBox.Show("Export Moves Before Exiting?", "Program Closing", MessageBoxButtons.YesNoCancel);
                if (res.Equals(DialogResult.Cancel)) {
                    cancelClosing = true;
                    e.Cancel = true;
                } else if(res.Equals(DialogResult.Yes)){
                    ExportMoves();
                }
            }
            appClose = cancelClosing;
        }

        private void saveExit_btn_Click (object sender, EventArgs e) {
            ExportMoves();
            appClose = false;
            this.Close();
        }
    }
}
