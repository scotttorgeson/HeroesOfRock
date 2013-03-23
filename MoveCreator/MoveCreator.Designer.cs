namespace MoveCreator {
    partial class MoveCreator {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.exitBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.nameField = new System.Windows.Forms.TextBox();
            this.damageField = new System.Windows.Forms.NumericUpDown();
            this.animationField = new System.Windows.Forms.ComboBox();
            this.audioField = new System.Windows.Forms.ComboBox();
            this.particleField = new System.Windows.Forms.ComboBox();
            this.attackTypeField = new System.Windows.Forms.ComboBox();
            this.aBtn = new System.Windows.Forms.Button();
            this.attackTimeField = new System.Windows.Forms.NumericUpDown();
            this.subMoveCheck = new System.Windows.Forms.CheckBox();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.statusField = new System.Windows.Forms.ComboBox();
            this.bBtn = new System.Windows.Forms.Button();
            this.xBtn = new System.Windows.Forms.Button();
            this.yBtn = new System.Windows.Forms.Button();
            this.multiBtn = new System.Windows.Forms.Button();
            this.buttonSequenceList = new System.Windows.Forms.ListBox();
            this.Name = new System.Windows.Forms.Label();
            this.Damage = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.damageField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.attackTimeField)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Teal;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.exitBtn);
            this.panel1.Controls.Add(this.saveBtn);
            this.panel1.Controls.Add(this.cancelBtn);
            this.panel1.Location = new System.Drawing.Point(1, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(499, 60);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(8, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 19);
            this.label1.TabIndex = 18;
            this.label1.Text = "Create New Move";
            // 
            // exitBtn
            // 
            this.exitBtn.BackColor = System.Drawing.Color.White;
            this.exitBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.exitBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.exitBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.exitBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.exitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitBtn.Location = new System.Drawing.Point(317, 23);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(90, 23);
            this.exitBtn.TabIndex = 16;
            this.exitBtn.Text = "Save and Exit";
            this.exitBtn.UseVisualStyleBackColor = false;
            this.exitBtn.Click += new System.EventHandler(this.exitBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.BackColor = System.Drawing.Color.White;
            this.saveBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.saveBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.saveBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveBtn.Location = new System.Drawing.Point(247, 23);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(58, 23);
            this.saveBtn.TabIndex = 15;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = false;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.BackColor = System.Drawing.Color.White;
            this.cancelBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cancelBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.cancelBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelBtn.Location = new System.Drawing.Point(422, 23);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(58, 23);
            this.cancelBtn.TabIndex = 17;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = false;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // nameField
            // 
            this.nameField.Location = new System.Drawing.Point(12, 92);
            this.nameField.Name = "nameField";
            this.nameField.Size = new System.Drawing.Size(133, 20);
            this.nameField.TabIndex = 1;
            // 
            // damageField
            // 
            this.damageField.Location = new System.Drawing.Point(12, 138);
            this.damageField.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.damageField.Name = "damageField";
            this.damageField.Size = new System.Drawing.Size(38, 20);
            this.damageField.TabIndex = 2;
            // 
            // animationField
            // 
            this.animationField.Enabled = false;
            this.animationField.FormattingEnabled = true;
            this.animationField.Location = new System.Drawing.Point(12, 272);
            this.animationField.Name = "animationField";
            this.animationField.Size = new System.Drawing.Size(132, 21);
            this.animationField.TabIndex = 4;
            // 
            // audioField
            // 
            this.audioField.FormattingEnabled = true;
            this.audioField.Location = new System.Drawing.Point(12, 228);
            this.audioField.Name = "audioField";
            this.audioField.Size = new System.Drawing.Size(132, 21);
            this.audioField.TabIndex = 5;
            // 
            // particleField
            // 
            this.particleField.FormattingEnabled = true;
            this.particleField.Location = new System.Drawing.Point(12, 377);
            this.particleField.Name = "particleField";
            this.particleField.Size = new System.Drawing.Size(132, 21);
            this.particleField.TabIndex = 7;
            // 
            // attackTypeField
            // 
            this.attackTypeField.FormattingEnabled = true;
            this.attackTypeField.Location = new System.Drawing.Point(13, 323);
            this.attackTypeField.Name = "attackTypeField";
            this.attackTypeField.Size = new System.Drawing.Size(132, 21);
            this.attackTypeField.TabIndex = 6;
            // 
            // aBtn
            // 
            this.aBtn.BackColor = System.Drawing.Color.LimeGreen;
            this.aBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.aBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.aBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.aBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.aBtn.Location = new System.Drawing.Point(397, 118);
            this.aBtn.Name = "aBtn";
            this.aBtn.Size = new System.Drawing.Size(75, 23);
            this.aBtn.TabIndex = 9;
            this.aBtn.Text = "A";
            this.aBtn.UseVisualStyleBackColor = false;
            this.aBtn.Click += new System.EventHandler(this.aBtn_Click);
            // 
            // attackTimeField
            // 
            this.attackTimeField.Location = new System.Drawing.Point(13, 183);
            this.attackTimeField.Name = "attackTimeField";
            this.attackTimeField.Size = new System.Drawing.Size(38, 20);
            this.attackTimeField.TabIndex = 3;
            // 
            // subMoveCheck
            // 
            this.subMoveCheck.AutoSize = true;
            this.subMoveCheck.Location = new System.Drawing.Point(397, 394);
            this.subMoveCheck.Name = "subMoveCheck";
            this.subMoveCheck.Size = new System.Drawing.Size(89, 17);
            this.subMoveCheck.TabIndex = 14;
            this.subMoveCheck.Text = "Is SubMove?";
            this.subMoveCheck.UseVisualStyleBackColor = true;
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(484, 462);
            this.shapeContainer1.TabIndex = 15;
            this.shapeContainer1.TabStop = false;
            // 
            // lineShape1
            // 
            this.lineShape1.Name = "lineShape1";
            this.lineShape1.X1 = 184;
            this.lineShape1.X2 = 184;
            this.lineShape1.Y1 = 80;
            this.lineShape1.Y2 = 437;
            // 
            // statusField
            // 
            this.statusField.FormattingEnabled = true;
            this.statusField.Location = new System.Drawing.Point(12, 429);
            this.statusField.Name = "statusField";
            this.statusField.Size = new System.Drawing.Size(132, 21);
            this.statusField.TabIndex = 8;
            // 
            // bBtn
            // 
            this.bBtn.BackColor = System.Drawing.Color.Red;
            this.bBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.bBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.bBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.bBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.bBtn.Location = new System.Drawing.Point(397, 179);
            this.bBtn.Name = "bBtn";
            this.bBtn.Size = new System.Drawing.Size(75, 23);
            this.bBtn.TabIndex = 10;
            this.bBtn.Text = "B";
            this.bBtn.UseVisualStyleBackColor = false;
            this.bBtn.Click += new System.EventHandler(this.bBtn_Click);
            // 
            // xBtn
            // 
            this.xBtn.BackColor = System.Drawing.Color.MediumBlue;
            this.xBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.xBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.xBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.xBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.xBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.xBtn.Location = new System.Drawing.Point(397, 242);
            this.xBtn.Name = "xBtn";
            this.xBtn.Size = new System.Drawing.Size(75, 23);
            this.xBtn.TabIndex = 11;
            this.xBtn.Text = "X";
            this.xBtn.UseVisualStyleBackColor = false;
            this.xBtn.Click += new System.EventHandler(this.xBtn_Click);
            // 
            // yBtn
            // 
            this.yBtn.BackColor = System.Drawing.Color.Gold;
            this.yBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.yBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.yBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.yBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.yBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.yBtn.Location = new System.Drawing.Point(397, 301);
            this.yBtn.Name = "yBtn";
            this.yBtn.Size = new System.Drawing.Size(75, 23);
            this.yBtn.TabIndex = 12;
            this.yBtn.Text = "Y";
            this.yBtn.UseVisualStyleBackColor = false;
            this.yBtn.Click += new System.EventHandler(this.yBtn_Click);
            // 
            // multiBtn
            // 
            this.multiBtn.BackColor = System.Drawing.Color.DimGray;
            this.multiBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.multiBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.multiBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.multiBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.multiBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.multiBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.multiBtn.Location = new System.Drawing.Point(397, 356);
            this.multiBtn.Name = "multiBtn";
            this.multiBtn.Size = new System.Drawing.Size(75, 23);
            this.multiBtn.TabIndex = 13;
            this.multiBtn.Text = "Multi Input";
            this.multiBtn.UseVisualStyleBackColor = false;
            this.multiBtn.Click += new System.EventHandler(this.multiBtn_Click);
            // 
            // buttonSequenceList
            // 
            this.buttonSequenceList.FormattingEnabled = true;
            this.buttonSequenceList.Location = new System.Drawing.Point(211, 106);
            this.buttonSequenceList.Name = "buttonSequenceList";
            this.buttonSequenceList.Size = new System.Drawing.Size(168, 277);
            this.buttonSequenceList.TabIndex = 21;
            this.buttonSequenceList.DoubleClick += new System.EventHandler(this.buttonSequenceList_DblClick);
            // 
            // Name
            // 
            this.Name.AutoSize = true;
            this.Name.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name.Location = new System.Drawing.Point(13, 75);
            this.Name.Name = "Name";
            this.Name.Size = new System.Drawing.Size(40, 15);
            this.Name.TabIndex = 22;
            this.Name.Text = "Name";
            // 
            // Damage
            // 
            this.Damage.AutoSize = true;
            this.Damage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Damage.Location = new System.Drawing.Point(13, 121);
            this.Damage.Name = "Damage";
            this.Damage.Size = new System.Drawing.Size(54, 15);
            this.Damage.TabIndex = 23;
            this.Damage.Text = "Damage";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 15);
            this.label3.TabIndex = 24;
            this.label3.Text = "Attack Time";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 305);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 15);
            this.label4.TabIndex = 25;
            this.label4.Text = "Attack Type";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(13, 256);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 15);
            this.label5.TabIndex = 26;
            this.label5.Text = "Animation";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(13, 212);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 15);
            this.label6.TabIndex = 27;
            this.label6.Text = "Audio Clip";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(13, 359);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 15);
            this.label7.TabIndex = 28;
            this.label7.Text = "Particle Effect";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(13, 411);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 15);
            this.label8.TabIndex = 29;
            this.label8.Text = "Status";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(208, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(104, 15);
            this.label9.TabIndex = 30;
            this.label9.Text = "Button Sequence";
            // 
            // MoveCreator
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(484, 462);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Damage);
            this.Controls.Add(this.Name);
            this.Controls.Add(this.buttonSequenceList);
            this.Controls.Add(this.multiBtn);
            this.Controls.Add(this.yBtn);
            this.Controls.Add(this.xBtn);
            this.Controls.Add(this.bBtn);
            this.Controls.Add(this.statusField);
            this.Controls.Add(this.subMoveCheck);
            this.Controls.Add(this.attackTimeField);
            this.Controls.Add(this.aBtn);
            this.Controls.Add(this.attackTypeField);
            this.Controls.Add(this.particleField);
            this.Controls.Add(this.audioField);
            this.Controls.Add(this.animationField);
            this.Controls.Add(this.damageField);
            this.Controls.Add(this.nameField);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(500, 500);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MoveCreator";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.damageField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.attackTimeField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.TextBox nameField;
        private System.Windows.Forms.NumericUpDown damageField;
        private System.Windows.Forms.ComboBox animationField;
        private System.Windows.Forms.ComboBox audioField;
        private System.Windows.Forms.ComboBox particleField;
        private System.Windows.Forms.ComboBox attackTypeField;
        private System.Windows.Forms.Button aBtn;
        private System.Windows.Forms.NumericUpDown attackTimeField;
        private System.Windows.Forms.CheckBox subMoveCheck;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private System.Windows.Forms.ComboBox statusField;
        private System.Windows.Forms.Button bBtn;
        private System.Windows.Forms.Button xBtn;
        private System.Windows.Forms.Button yBtn;
        private System.Windows.Forms.Button multiBtn;
        private System.Windows.Forms.ListBox buttonSequenceList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Name;
        private System.Windows.Forms.Label Damage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}