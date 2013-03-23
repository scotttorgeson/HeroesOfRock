namespace WinFormsGraphicsDevice
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createTriggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playSoundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spawnActorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.airBurstToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonPushToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.healthPackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.checkBoxSlowMotion = new System.Windows.Forms.CheckBox();
            this.trackBarSlowMotion = new System.Windows.Forms.TrackBar();
            this.buttonPause = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.PropertiesPanelLabel = new System.Windows.Forms.Label();
            this.actorRemoveButton = new System.Windows.Forms.Button();
            this.PropertiesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.actorEditButton = new System.Windows.Forms.Button();
            this.actorAddButton = new System.Windows.Forms.Button();
            this.actorListBox = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.showTriggersCheckBox = new System.Windows.Forms.CheckBox();
            this.buttonRotate = new System.Windows.Forms.Button();
            this.buttonMove = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSlowMotion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.createTriggerToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1362, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // createTriggerToolStripMenuItem
            // 
            this.createTriggerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playSoundToolStripMenuItem,
            this.spawnActorToolStripMenuItem,
            this.airBurstToolStripMenuItem,
            this.buttonPushToolStripMenuItem,
            this.textToScreenToolStripMenuItem,
            this.healthPackToolStripMenuItem,
            this.rotateCameraToolStripMenuItem,
            this.endLevelToolStripMenuItem});
            this.createTriggerToolStripMenuItem.Name = "createTriggerToolStripMenuItem";
            this.createTriggerToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
            this.createTriggerToolStripMenuItem.Text = "Create Trigger";
            // 
            // playSoundToolStripMenuItem
            // 
            this.playSoundToolStripMenuItem.Name = "playSoundToolStripMenuItem";
            this.playSoundToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.playSoundToolStripMenuItem.Text = "Play Sound";
            this.playSoundToolStripMenuItem.Click += new System.EventHandler(this.playSoundToolStripMenuItem_Click);
            // 
            // spawnActorToolStripMenuItem
            // 
            this.spawnActorToolStripMenuItem.Name = "spawnActorToolStripMenuItem";
            this.spawnActorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.spawnActorToolStripMenuItem.Text = "Spawn Actor";
            this.spawnActorToolStripMenuItem.Click += new System.EventHandler(this.spawnActorToolStripMenuItem_Click);
            // 
            // airBurstToolStripMenuItem
            // 
            this.airBurstToolStripMenuItem.Name = "airBurstToolStripMenuItem";
            this.airBurstToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.airBurstToolStripMenuItem.Text = "Air Burst";
            this.airBurstToolStripMenuItem.Click += new System.EventHandler(this.airBurstToolStripMenuItem_Click);
            // 
            // buttonPushToolStripMenuItem
            // 
            this.buttonPushToolStripMenuItem.Name = "buttonPushToolStripMenuItem";
            this.buttonPushToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.buttonPushToolStripMenuItem.Text = "Button Push";
            this.buttonPushToolStripMenuItem.Click += new System.EventHandler(this.buttonPushToolStripMenuItem_Click);
            // 
            // textToScreenToolStripMenuItem
            // 
            this.textToScreenToolStripMenuItem.Name = "textToScreenToolStripMenuItem";
            this.textToScreenToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.textToScreenToolStripMenuItem.Text = "Text To Screen";
            this.textToScreenToolStripMenuItem.Click += new System.EventHandler(this.textToScreenToolStripMenuItem_Click);
            // 
            // healthPackToolStripMenuItem
            // 
            this.healthPackToolStripMenuItem.Name = "healthPackToolStripMenuItem";
            this.healthPackToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.healthPackToolStripMenuItem.Text = "Health Pack";
            this.healthPackToolStripMenuItem.Click += new System.EventHandler(this.healthPackToolStripMenuItem_Click);
            // 
            // rotateCameraToolStripMenuItem
            // 
            this.rotateCameraToolStripMenuItem.Name = "rotateCameraToolStripMenuItem";
            this.rotateCameraToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rotateCameraToolStripMenuItem.Text = "Rotate Camera";
            this.rotateCameraToolStripMenuItem.Click += new System.EventHandler(this.rotateCameraToolStripMenuItem_Click);
            // 
            // endLevelToolStripMenuItem
            // 
            this.endLevelToolStripMenuItem.Name = "endLevelToolStripMenuItem";
            this.endLevelToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.endLevelToolStripMenuItem.Text = "End Level";
            this.endLevelToolStripMenuItem.Click += new System.EventHandler(this.endLevelToolStripMenuItem_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(405, 3);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(567, 3);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 3;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBoxSlowMotion
            // 
            this.checkBoxSlowMotion.AutoSize = true;
            this.checkBoxSlowMotion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxSlowMotion.Location = new System.Drawing.Point(648, 8);
            this.checkBoxSlowMotion.Name = "checkBoxSlowMotion";
            this.checkBoxSlowMotion.Size = new System.Drawing.Size(84, 17);
            this.checkBoxSlowMotion.TabIndex = 4;
            this.checkBoxSlowMotion.Text = "Slow Motion";
            this.checkBoxSlowMotion.UseVisualStyleBackColor = true;
            this.checkBoxSlowMotion.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // trackBarSlowMotion
            // 
            this.trackBarSlowMotion.LargeChange = 1;
            this.trackBarSlowMotion.Location = new System.Drawing.Point(738, 3);
            this.trackBarSlowMotion.Maximum = 9;
            this.trackBarSlowMotion.Name = "trackBarSlowMotion";
            this.trackBarSlowMotion.Size = new System.Drawing.Size(112, 45);
            this.trackBarSlowMotion.TabIndex = 5;
            this.trackBarSlowMotion.Value = 1;
            this.trackBarSlowMotion.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // buttonPause
            // 
            this.buttonPause.Location = new System.Drawing.Point(486, 3);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(75, 23);
            this.buttonPause.TabIndex = 6;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.button3_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitContainer1.Panel1.Controls.Add(this.PropertiesPanelLabel);
            this.splitContainer1.Panel1.Controls.Add(this.actorRemoveButton);
            this.splitContainer1.Panel1.Controls.Add(this.PropertiesPanel);
            this.splitContainer1.Panel1.Controls.Add(this.actorEditButton);
            this.splitContainer1.Panel1.Controls.Add(this.actorAddButton);
            this.splitContainer1.Panel1.Controls.Add(this.actorListBox);
            this.splitContainer1.Size = new System.Drawing.Size(1362, 681);
            this.splitContainer1.SplitterDistance = 98;
            this.splitContainer1.TabIndex = 7;
            // 
            // PropertiesPanelLabel
            // 
            this.PropertiesPanelLabel.BackColor = System.Drawing.SystemColors.Window;
            this.PropertiesPanelLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PropertiesPanelLabel.Enabled = false;
            this.PropertiesPanelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PropertiesPanelLabel.Location = new System.Drawing.Point(0, 0);
            this.PropertiesPanelLabel.Name = "PropertiesPanelLabel";
            this.PropertiesPanelLabel.Size = new System.Drawing.Size(98, 23);
            this.PropertiesPanelLabel.TabIndex = 1;
            this.PropertiesPanelLabel.Text = "Properties Panel";
            this.PropertiesPanelLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // actorRemoveButton
            // 
            this.actorRemoveButton.Location = new System.Drawing.Point(0, 415);
            this.actorRemoveButton.Name = "actorRemoveButton";
            this.actorRemoveButton.Size = new System.Drawing.Size(100, 23);
            this.actorRemoveButton.TabIndex = 4;
            this.actorRemoveButton.Text = "Remove";
            this.actorRemoveButton.UseVisualStyleBackColor = true;
            this.actorRemoveButton.Click += new System.EventHandler(this.actorRemoveButton_Click);
            // 
            // PropertiesPanel
            // 
            this.PropertiesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PropertiesPanel.AutoScroll = true;
            this.PropertiesPanel.BackColor = System.Drawing.SystemColors.Window;
            this.PropertiesPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PropertiesPanel.Location = new System.Drawing.Point(0, 20);
            this.PropertiesPanel.Name = "PropertiesPanel";
            this.PropertiesPanel.Size = new System.Drawing.Size(99, 360);
            this.PropertiesPanel.TabIndex = 0;
            // 
            // actorEditButton
            // 
            this.actorEditButton.Location = new System.Drawing.Point(49, 386);
            this.actorEditButton.Name = "actorEditButton";
            this.actorEditButton.Size = new System.Drawing.Size(50, 23);
            this.actorEditButton.TabIndex = 3;
            this.actorEditButton.Text = "Edit";
            this.actorEditButton.UseVisualStyleBackColor = true;
            this.actorEditButton.Click += new System.EventHandler(this.actorEditButton_Click);
            // 
            // actorAddButton
            // 
            this.actorAddButton.Location = new System.Drawing.Point(0, 386);
            this.actorAddButton.Name = "actorAddButton";
            this.actorAddButton.Size = new System.Drawing.Size(50, 23);
            this.actorAddButton.TabIndex = 2;
            this.actorAddButton.Text = "Add";
            this.actorAddButton.UseVisualStyleBackColor = true;
            this.actorAddButton.Click += new System.EventHandler(this.actorAddButton_Click);
            // 
            // actorListBox
            // 
            this.actorListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.actorListBox.FormattingEnabled = true;
            this.actorListBox.Location = new System.Drawing.Point(0, 434);
            this.actorListBox.Name = "actorListBox";
            this.actorListBox.Size = new System.Drawing.Size(98, 251);
            this.actorListBox.TabIndex = 0;
            this.actorListBox.SelectedIndexChanged += new System.EventHandler(this.actorListBox_SelectedIndexChanged);
            this.actorListBox.DoubleClick += new System.EventHandler(this.actorListBox_DoubleClick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitContainer2.Panel1.Controls.Add(this.showTriggersCheckBox);
            this.splitContainer2.Panel1.Controls.Add(this.buttonRotate);
            this.splitContainer2.Panel1.Controls.Add(this.buttonMove);
            this.splitContainer2.Panel1.Controls.Add(this.buttonStart);
            this.splitContainer2.Panel1.Controls.Add(this.buttonPause);
            this.splitContainer2.Panel1.Controls.Add(this.trackBarSlowMotion);
            this.splitContainer2.Panel1.Controls.Add(this.buttonStop);
            this.splitContainer2.Panel1.Controls.Add(this.checkBoxSlowMotion);
            this.splitContainer2.Panel1MinSize = 33;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(1362, 718);
            this.splitContainer2.SplitterDistance = 33;
            this.splitContainer2.TabIndex = 8;
            // 
            // showTriggersCheckBox
            // 
            this.showTriggersCheckBox.AutoSize = true;
            this.showTriggersCheckBox.Checked = true;
            this.showTriggersCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showTriggersCheckBox.Location = new System.Drawing.Point(943, 8);
            this.showTriggersCheckBox.Name = "showTriggersCheckBox";
            this.showTriggersCheckBox.Size = new System.Drawing.Size(94, 17);
            this.showTriggersCheckBox.TabIndex = 9;
            this.showTriggersCheckBox.Text = "Show Triggers";
            this.showTriggersCheckBox.UseVisualStyleBackColor = true;
            this.showTriggersCheckBox.CheckedChanged += new System.EventHandler(this.showTriggersCheckBox_CheckedChanged);
            // 
            // buttonRotate
            // 
            this.buttonRotate.Location = new System.Drawing.Point(94, 2);
            this.buttonRotate.Name = "buttonRotate";
            this.buttonRotate.Size = new System.Drawing.Size(75, 23);
            this.buttonRotate.TabIndex = 8;
            this.buttonRotate.Text = "Rotate";
            this.buttonRotate.UseVisualStyleBackColor = true;
            this.buttonRotate.Click += new System.EventHandler(this.buttonRotate_Click);
            // 
            // buttonMove
            // 
            this.buttonMove.Location = new System.Drawing.Point(13, 2);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(75, 23);
            this.buttonMove.TabIndex = 7;
            this.buttonMove.Text = "Move";
            this.buttonMove.UseVisualStyleBackColor = true;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(1362, 742);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSlowMotion)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private StageControl stageControl;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.CheckBox checkBoxSlowMotion;
        private System.Windows.Forms.TrackBar trackBarSlowMotion;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button buttonRotate;
        private System.Windows.Forms.Button buttonMove;
        private System.Windows.Forms.ListBox actorListBox;
        public System.Windows.Forms.FlowLayoutPanel PropertiesPanel;
        private System.Windows.Forms.Label PropertiesPanelLabel;
        private System.Windows.Forms.Button actorRemoveButton;
        private System.Windows.Forms.Button actorEditButton;
        private System.Windows.Forms.Button actorAddButton;
        private System.Windows.Forms.CheckBox showTriggersCheckBox;
        private System.Windows.Forms.ToolStripMenuItem createTriggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playSoundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spawnActorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem airBurstToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buttonPushToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem healthPackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem endLevelToolStripMenuItem;
        
    }
}

